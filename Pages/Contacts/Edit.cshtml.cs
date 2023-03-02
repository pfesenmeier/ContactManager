using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContactManager.Data;
using ContactManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ContactManager.Authorization;

namespace ContactManager.Pages.Contacts
{
    public class EditModel : DI_BasePageModel
    {
        public EditModel(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<IdentityUser> userManager) : base(context, authorizationService, userManager)
        {
        }

        [BindProperty]
        public Contact Contact { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || Context.Contact == null)
            {
                return NotFound();
            }



            var contact = await Context.Contact.FirstOrDefaultAsync(m => m.ContactId == id);
            if (contact == null)
            {
                return NotFound();
            }
            Contact = contact;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, Contact, ContactOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }


            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var contact = await Context.Contact.AsNoTracking().FirstOrDefaultAsync(c => c.ContactId == id);

            if (contact == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, contact, ContactOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Contact.OwnerId = contact.OwnerId;

            Context.Attach(Contact).State = EntityState.Modified;

            if (Contact.Status == ContactStatus.Approved)
            {
                var canApprove = await AuthorizationService.AuthorizeAsync(User, Contact, ContactOperations.Approve);

                if (!canApprove.Succeeded)
                {
                    Contact.Status = ContactStatus.Submitted;
                }
            }

            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private bool ContactExists(int id)
        {
            return (Context.Contact?.Any(e => e.ContactId == id)).GetValueOrDefault();
        }
    }
}
