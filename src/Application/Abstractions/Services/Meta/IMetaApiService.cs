using Application.Features.Meta.Ads.Get;
using Application.Features.Meta.AdSets.Get;
using Application.Features.Meta.Campaigns.Get;
using Application.Features.Meta.Forms;
using Application.Features.Meta.Forms.Create;
using Application.Features.Meta.Leads.Get;
using SharedKernel;

namespace Application.Abstractions.Services.Meta;

public interface IMetaApiService
{
    Task<Result<List<CampaignResponse>>> GetCampaignsAsync(string adAccountId, CancellationToken ct = default);
    Task<Result<List<AdSetResponse>>> GetAdSetsAsync(string campaignId, CancellationToken ct = default);
    Task<Result<List<AdResponse>>> GetAdsAsync(string adSetId, CancellationToken ct = default);
    Task<Result<List<LeadResponse>>> GetLeadsAsync(string formId, CancellationToken ct = default);
    Task<Result<List<FormResponse>>> GetFormsAsync(string pageId, CancellationToken ct = default);
    Task<Result<string>> CreateFormAsync(CreateFormCommand command, CancellationToken ct = default);
    Task<Result<FormResponse>> GetFormByIdAsync(string formId, CancellationToken ct = default);
    Task<Result> DeleteFormAsync(string formId, CancellationToken ct = default);
}
