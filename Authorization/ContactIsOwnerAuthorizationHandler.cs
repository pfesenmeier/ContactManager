using ContactManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace ContactManager.Authorization;

public class ContactIsOwnerAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Contact>
{
    UserManager<IdentityUser> _userManager;

    public ContactIsOwnerAuthorizationHandler(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, Contact resource)
    {
        if (context.User == null || resource == null)
        {
            // allows other authorization handlers to run
            return Task.CompletedTask;
        }

        // If not asking for CRUD permission, return
        if (requirement.Name != Constants.CreateOperationName &&
            requirement.Name != Constants.ReadOperationName &&
            requirement.Name != Constants.UpdateOperationName &&
            requirement.Name != Constants.DeleteOperationName
           )
        {
            // allows other authorization handlers to run
            return Task.CompletedTask;

        }

        // if logged-in user created the contact
        if (resource.OwnerId == _userManager.GetUserId(context.User))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
