//using Application.Abstractions.Messaging;
//using Application.Features.Meta.MetaSettings.Create;
//using SharedKernel;
//using Web.Api.Extensions;
//using Web.Api.Infrastructure;

//namespace Web.Api.Endpoints.MetaSettings;

//internal sealed class Create : IEndpoint
//{
//    public void MapEndpoint(IEndpointRouteBuilder app)
//    {
//        app.MapPost("meta/settings/set", async (
//            CreateeMetaSettingsCommand command,
//            ICommandHandler<CreateeMetaSettingsCommand> handler,
//            CancellationToken cancellationToken) =>
//        {
//            Result result = await handler.Handle(command, cancellationToken);
//            return result.Match(Results.NoContent, CustomResults.Problem);
//        }).WithTags(Tags.MetaSettings)
//        .Produces<List<CreateeMetaSettingsCommand>>();
//    }
//}


// eg
//{
//  "appId": "123456789",
//  "appSecret": "abc123...",
//  "userToken": "short_lived_token_from_graph_explorer",
//  "webhookVerifyToken": "any-random-string-you-choose"
//}
