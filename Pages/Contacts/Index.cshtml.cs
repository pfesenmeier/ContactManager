using Microsoft.AspNetCore.Mvc;
using ContactManager.Models;
using ContactManager.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ContactManager.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.Pages.Contacts
{
    public class IndexModel : DI_BasePageModel
    {
        public IndexModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<IdentityUser> userManager): base(context, authorizationService, userManager)
        {
        } 

        public IList<Contact> Contact { get; set; } = default!;

        public async Task OnGetAsync()
        {
            var contacts = from c in Context.Contact select c;

            var isAuthorized = User.IsInRole(Constants.ContactManagersRole) || User.IsInRole(Constants.ContactAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);

            if (!isAuthorized)
            {
              contacts = contacts.Where(c => c.Status == ContactStatus.Approved || c.OwnerId == currentUserId);
            }

            Contact = await contacts.ToListAsync();
        }
    }
}
