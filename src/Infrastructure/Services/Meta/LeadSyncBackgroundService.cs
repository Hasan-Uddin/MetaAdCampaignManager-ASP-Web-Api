using Application.Abstractions.Data;
using Application.Abstractions.Services.Meta;
using Application.Features.Meta.Leads.Get;
using Domain.Leads;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Infrastructure.Services.Meta;

// Background service that periodically pulls leads from Meta API
// and stores only the new ones into the database.
internal sealed class LeadSyncBackgroundService(
    IServiceScopeFactory scopeFactory,
    IDateTimeProvider dateTimeProvider,
    ILogger<LeadSyncBackgroundService> logger) : BackgroundService
{
    // Change this value if you want more or less frequent lead syncing.
    private static readonly TimeSpan Interval = TimeSpan.FromHours(4);  // 4 Hours

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // BackgroundService entry point.
        // This loop runs until the application shuts down.
        while (!stoppingToken.IsCancellationRequested)
        {
            // dont run right after application starts
            await Task.Delay(Interval, stoppingToken); 
            await SyncLeadsAsync(stoppingToken);
        }
    }

    private async Task SyncLeadsAsync(CancellationToken ct)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Meta lead sync started at {Time}", dateTimeProvider.UtcNow);
        }

        // Background services are singleton by default, but DbContext is scoped.
        // So we create a new DI scope each cycle to safely resolve scoped services.
        using IServiceScope scope = scopeFactory.CreateScope();

        // Resolve scoped dependencies from the new scope
        IApplicationDbContext context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        IMetaApiService metaApi = scope.ServiceProvider.GetRequiredService<IMetaApiService>();

        try
        {
            // Get all known forms from DB, need Forms ID to find Leads.
            List<string> formIds = await context.Forms
                .Select(f => f.Id)
                .ToListAsync(ct);

            // If no forms exist in DB, there is nothing to sync.
            if (formIds.Count == 0)
            {
                logger.LogInformation("No forms found in DB, skipping lead sync.");
                return;
            }

            int newLeads = 0;

            // Process each form individually
            foreach (string formId in formIds)
            {
                Result<List<LeadResponse>> result = await metaApi.GetLeadsAsync(formId, ct);

                if (result.IsFailure)
                {
                    logger.LogWarning("Failed to fetch leads for form {FormId}: {Error}", formId, result.Error.Description);
                    continue;
                }

                // Collect all lead IDs returned by the API
                // This allows us to check which leads already exist in DB
                var fetchedIds = result.Value.Select(l => l.Id).ToList();

                // Query database for leads that already exist
                // Only select IDs to keep query lightweight
                List<string> existingIds = await context.Leads
                    .Where(l => fetchedIds.Contains(l.Id))
                    .Select(l => l.Id)
                    .ToListAsync(ct);

                // Filter out leads that already exist in DB
                // Only keep the truly new ones
                var newOnes = result.Value
                    .Where(l => !existingIds.Contains(l.Id))
                    .ToList();

                // Convert API responses into Lead entities
                foreach (LeadResponse? item in newOnes)
                {
                    var lead = new Lead
                    {
                        Id = item.Id,
                        FormId = formId,
                        AdId = item.AdId,
                        CampaignId = item.CampaignId,
                        AdSetId = item.AdSetId,
                        FieldData = item.FieldData,
                        CreatedAt = item.CreatedAt,
                        SyncedAt = dateTimeProvider.UtcNow
                    };

                    // Domain event trigger
                    lead.RaiseReceivedEvent();
                    context.Leads.Add(lead);
                    newLeads++;
                }

                // Save only if we actually added something.
                // Avoids unnecessary DB writes.
                if (newOnes.Count > 0)
                {
                    await context.SaveChangesAsync(ct);
                }
            }

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Meta lead sync completed. {NewLeads} new leads across {Forms} forms.",
                    newLeads, formIds.Count);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Meta lead sync failed");
        }
    }
}

//  Every 4 hours:
//    > get all Form IDs from DB
//    > for each form:
//        > call Meta API GET /form/{id}/ leads
//        > compare returned IDs with existing IDs in DB
//        > insert only the new ones
//        > raise LeadReceivedEvent on each new lead  → logger.LogInformation
//    > log summary: "X new leads across Y forms"
