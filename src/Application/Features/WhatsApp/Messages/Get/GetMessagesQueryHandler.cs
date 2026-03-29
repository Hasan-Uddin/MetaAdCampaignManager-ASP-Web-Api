using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Features.WhatsApp.Messages.Get;

internal sealed class GetMessagesQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetMessagesQuery, List<MessageResponse>>
{
    public async Task<Result<List<MessageResponse>>> Handle(GetMessagesQuery query, CancellationToken cancellationToken)
    {
        return await context.Messages
            .Where(m => m.ConversationId == query.ConversationId)
            .OrderBy(m => m.SentAt)
            .Select(m => new MessageResponse
            {
                Id = m.Id,
                Body = m.Body,
                Direction = m.Direction.ToString(),
                Status = m.Status.ToString(),
                SentAt = m.SentAt
            })
            .ToListAsync(cancellationToken);
    }
}
