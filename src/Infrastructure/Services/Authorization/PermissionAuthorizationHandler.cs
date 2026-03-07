using Infrastructure.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Services.Authorization;

internal sealed class PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Reject unauthenticated users immediately
        if (context.User?.Identity?.IsAuthenticated != true)
        {
            context.Fail();
            return;
        }
        Guid? userId = context.User.GetUserId();

        if (userId is null)
        {
            context.Fail();
            return;
        }

        using IServiceScope scope = serviceScopeFactory.CreateScope();

        PermissionProvider permissionProvider = scope.ServiceProvider.GetRequiredService<PermissionProvider>();

        HashSet<string> permissions = await permissionProvider.GetForUserIdAsync(userId.Value);

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
        Console.WriteLine($"User {userId} permissions: {string.Join(", ", permissions)}");
        Console.WriteLine($"Required permission: {requirement.Permission}");

    }
}
