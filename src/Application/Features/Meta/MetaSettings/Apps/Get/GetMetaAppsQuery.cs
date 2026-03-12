using Application.Abstractions.Authentication.MetaAuth;
using Application.Abstractions.Messaging;

namespace Application.Features.Meta.MetaSettings.Apps.Get;

public sealed record GetMetaAppsQuery : IQuery<List<MetaAppInfo>>;
