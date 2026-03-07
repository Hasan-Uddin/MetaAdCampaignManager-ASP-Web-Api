
using Application.Abstractions.Messaging;

namespace Application.Features.Auth.Get;

public record GetGoogleAuthUrlQuery() : IQuery<GoogleAuthUrlQueryResponse>;
