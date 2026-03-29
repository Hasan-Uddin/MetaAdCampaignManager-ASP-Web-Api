using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.WhatsApp.Conversations.Get;

internal sealed class GetConversationsQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetConversationsQuery, List<ConversationResponse>>
{
    public async Task<Result<List<ConversationResponse>>> Handle(GetConversationsQuery query, CancellationToken cancellationToken)
    {
        return await context.Conversations
            .Where(c => c.UserId == query.UserId)
            .OrderByDescending(c => c.LastMessageAt)
            .Select(c => new ConversationResponse
            {
                Id = c.Id,
                CustomerPhone = c.CustomerPhone,
                CustomerName = c.CustomerName,
                LastMessage = c.LastMessage,
                LastMessageAt = c.LastMessageAt,
                IsRead = c.IsRead
            })
            .ToListAsync(cancellationToken);
    }
}
