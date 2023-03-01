I left off here: https://learn.microsoft.com/en-us/aspnet/core/security/authorization/secure-data?source=recommendations&view=aspnetcore-7.0#update-the-editmodel

# Things I did
1. Created web app
  - with `-sqlite` flag, `dotnet new` created the `ApplicationContext` for me (though, not sure... maybe this was autocreated because the auth scaffolding)
  - specified `--auth Indididual` 
2. Scaffolded CRUD Pages for a `Contact`
3. Added an owner id to `Contact` model
4. Added Roles Authorization to app builder
5. Added Authorization Handlers to builder
6. Added SeedData.cs to seed database with users and a admin and a manager
7. Added passwords for admin and manager in my dev environment with `dotnet user-secrets`
8. found `dotnet format`
9. Added a base class that inherits from RazorPages's `PageModel` that gives pages access to the user manager and authorization service along with the database context
10. Used the base class in the `Create` and `Index` Contact pages
next: Edit model
