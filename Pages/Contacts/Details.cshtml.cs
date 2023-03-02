using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ContactManager.Authorization;

namespace ContactManager.Pages.Contacts
{
    public class DetailsModel : DI_BasePageModel
    {

        public DetailsModel(ContactManager.Data.ApplicationDbContext context, IAuthorizationService authenticationService, UserManager<IdentityUser> userManager) : base(context, authenticationService, userManager)
        {
        }

        public Contact Contact { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || Context.Contact == null)
                return NotFound();

            var contact = await Context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);
            if (contact == null)
                return NotFound();
            else
                Contact = contact;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id, ContactStatus status)
        {
            if (id is null || Context.Contact is null)
                return NotFound();

            var contact = await Context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);

            if (contact is null)
                return NotFound();

            IAuthorizationRequirement? operation;
            operation = status switch
            {
                ContactStatus.Approved => ContactOperations.Approve,
                ContactStatus.Rejected => ContactOperations.Reject,
                _ => null
            };

            if (operation is null)
                return BadRequest();

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Contact, operation);

            if (!isAuthorized.Succeeded)
                return Forbid();

            contact.Status = status;
            Context.Contact.Update(contact);
            await Context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
