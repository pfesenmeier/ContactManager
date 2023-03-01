using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace ContactManager.Authorization;

public class ContactManagerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Contact>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Contact resource)
    {
        if (context.User == null || resource == null)
        {
            return Task.CompletedTask;
        }

        // If not asking for CRUD permission, return
        if (requirement.Name != Constants.ApproveOperationName &&
            requirement.Name != Constants.ReadOperationName
           )
        {
            // allows other authorization handlers to run
            return Task.CompletedTask;
        }

        if (context.User.IsInRole(Constants.ContactManagersRole))
        {
            context.Succeed(requirement);

        }

        return Task.CompletedTask;



    }


}
