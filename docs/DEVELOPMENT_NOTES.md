

- [ ] var means C# figures out the type automatically.

- [ ] @User.Identity?.Name

- [ ]  Return View(notes)  - - - -    @model IEnumerable<MyMvcApp.Models.Note>

- [ ] Pin/UnPin,  Delete,  changes content so it is a POST req,   need form instead of <a> tag






1. 
- [ ] dotnet new mvc -o MyMvcApp
- [ ] cd MyMvcApp


2. 
- [ ] dotnet run
- [ ] dotnet watch


3.  for Database CRUD operations via C#
- [ ] dotnet add package Microsoft.EntityFrameworkCore
- [ ] dotnet add package Microsoft.EntityFrameworkCore.Design 
- [ ] dotnet add package Microsoft.EntityFrameworkCore.SqlServer 
- [ ] dotnet add package Microsoft.EntityFrameworkCore.Tools

check .csproj file



4. Docker
- [ ] docker --version
- [ ] docker ps


5. Create SQL Server container:
- [ ] docker run -e 'ACCEPT_EULA=Y' -e 'MSSQL_SA_PASSWORD=YOUR_PASSWORD_HERE' -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2025-latest



6. appsettings.json
add
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=MyMvcDatabase;User Id=sa;Password=MY_PASSWORD;TrustServerCertificate=True;Encrypt=False;"
  },







7. Set up User Secrets :

7.1. From your project directory:
- [ ] dotnet user-secrets init

This adds a UserSecretsId to your .csproj.



7.2.
Then set your actual database password:

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=MyMvcDatabase;User Id=sa;Password=YOUR_PASSWORD_HERE;TrustServerCertificate=True;Encrypt=False;"



7.3. verify that appsettings.json really contains the placeholder and not the real password.
- [ ] grep -n "Password" appsettings.json











8. Create:
Models/
   User.cs


9. Create:
Data/
   AppDbContext.cs






10. Open Program.cs

register:

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);





11. dotnet build
OR… 
ctrl+shift+b 



12.  Install dotnet-ef:
- [ ] dotnet tool install --global dotnet-ef

- [ ] dotnet ef migrations add dbinit
OR….
goto tools => nuget package manager => package manager console 
then command
 add-migration dbinit 




13.  Put all columns in database:
- [ ] dotnet ef database update
OR…
update-database








14. 

14.1. Check Docker :
- [ ] docker ps
You should see: sqlserver
with status: Up ...




14.2. Connect to SQL Server from terminal :
- [ ] docker exec -it sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -C
- [ ] docker exec -it sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'YOUR_PASSWORD_HERE' -C

14.3. Ask SQL Server what databases exist :
- [ ] SELECT name FROM sys.databases;
- [ ] GO

14.4. Switch to your database :
- [ ] USE MyMvcDatabase;
- [ ] GO

14.5. Insert into tables :
- [ ] INSERT INTO Users (Username, Email, PasswordHash)
- [ ] VALUES ('testuser', 'test@example.com', '12345');
- [ ] GO

14.6. 
- [ ] SELECT * FROM Users;
- [ ] GO



14.7. exit










15.  Create :

Controllers/
     AuthController.cs





16. Create MVC Razor Views

Views/
     Auth/
	Login.cshtml     — put a login form.
	Register.cshtml




17. BTW, in Program.cs
*     pattern: "{controller=Dashboard}/{action=Index}/{id?}")






18. Create:

Dto/
    UserDto.cs

DTOs are primarily for transferring data between layers/UI and the application, and they help avoid exposing your database entities directly. They don't replace models for database operations.





19. 
- [ ] Model Binding :
@model UserDto

- [ ] In register & Login.cshtml,… Add inside input field
asp-for="Username"
& so on…

- [ ] input type submit   - - - button




20. The form div of Register page has
method=“post”
asp-action="CreateUser"






21. Goto AuthController.cs

i. Add Primary Constructor .. initialize with _context got from our database.
ii. Add CreateUser Method    with     [HttpPost]
- - - Compare AppDbContext Email with User’s input i.e. Dto Email. 
- - - If User doesnt exist, create by using Actual User from Models


iii. Install BCrypt
- [ ] dotnet add package BCrypt.Net-Next
using MyMvcApp.Models;
using BCrypt.Net;

iii. Codes …
iv. Goto Register.cshtml for error msg display to user.
In email,…  add <span asp-validation-for="Email" class="text-danger"></span>
In password, username too.


v. In Login.cshtml

@if (TempData["SuccessMessage"] != null)
{
    <div class="alert alert-success d-flex align-items-center gap-2 mb-4" role="alert">
        <span>✓</span>
        <span>@TempData["SuccessMessage"]</span>
    </div>
}







22. Rename Password column to PasswordHash.  
* This does NOT delete the existing password data because EF generates a RenameColumn operation. However, always review the migration before applying it, especially on a database containing real user data.

- [ ] dotnet ef migrations add RenamePasswordToPasswordHash
- [ ] dotnet ef database update








23. Create:
- [ ] Dto/
    	   LoginUserDto.cs
- [ ] Login.cshtml   - - form div, email, pw div update. 








24. Configuring Cookie Authentication :

Email + Password
       ↓
Find user
       ↓
BCrypt.Verify()
       ↓
Password correct?
       ↓
Create authentication cookie
       ↓
Redirect to Index (Dashboard)


In Program.cs
i. Add authentication service
- [ ] builder.Services.AddAuthentication( ……
LATER,
[Authorize] 
public IActionResult Index() in DashboardController.cs
AND
[Authorize(Roles = "Admin")] 
public IActionResult Admin() 
{ return View(); }




ii. Add middleware in correct order:

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();



iii. Add inside AuthController.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;



iv. Goto step 5 of LoginUser() method
- [ ] Claims 
- [ ] ClaimsIdentity 
- [ ] ClaimsPrincipal 
- [ ] Logged-in user



v. 
- [ ] Create basic DashboardController.cs   - - Index()   { return View()  }
       - - - This is for GET_&_Display_note, POST_orAdd_note, UPDATE, DELETE…
- [ ] Create Views/Dashboard/Index.cshtml
Also add;
@* For Browser's Tab. *@
@{
    ViewData["Title"] = "Notes";
}





25. Done…
/Dashboard/Index         - - - while tryna acces this page w/o loggin in.
       ↓
   [Authorize]          - - - DashboardController.cs file
       ↓
Not authenticated ❌
       ↓
/Auth/Login            - - - Program.cs - -> Cookie Authentication Configuration
       ↓
     Login
       ↓
  Create Cookie 🍪
       ↓
 /Dashboard/Index





26. Logout() inside Authcontroller.cs









27. Index.cshtml page



28. Create Models/Note.cs



29. AppDbContext.cs  - - ->  Add the DbSet<Note>.

- [ ] dotnet build

create the EF Core migration:
- [ ] dotnet ef migrations add AddNotes

Apply the migration to SQL Server
- [ ] dotnet ef database update









Bcz login takes u to Dashboard…
30. Update    Controllers/DashboardController.cs     Index() method    to getting datas from the model.








31. Tell Dashboard/Index.cshtml what Model is
i. add    @model IEnumerable<MyMvcApp.Models.Note>

Now “Model” gets all notes.

ii. Update Pinned-Card & Recent-Notes-Card.





32.  Put aside section in Razor   _Layout.cshtml    - - - since it appears in every pages like Dashboard, All Notes, Trash….  Then bold the currently running page in aside bar.






33. Create:
Views/Dashboard/AllNotes.cshtml

- [ ] Make sure “All Notes Button”  in the aside bar of Dashboard works & will redirect correctly.


- [ ] Make    “+ New Note”    button Work:
i.  Make sure this button <a> tag has      asp-controller=“”  & asp-action=“”


ii.  Put CreateNote()   { return View() }    in DashboardController.cs    as  no. 3.


iii. Create CreateNote Page:    - - -    Clicking “+ New Note” leads here.
Views/Dashboard/CreateNote.cshtml


iv. Create:
Dto/CreateNoteDto.cs 
- - - to fill in Datas like Title & Content after clicking    “Create Note” button. 



v.  Write  CreateNote() method     in  DashboardController.cs 
  - - - This Takes datas (Title + Content) from CreateNote page & put it into Note.cs DB
of the user with userId.
  - - -  Using      DateTime.UtcNow      BTW.











34. Make “Edit” button work :

i. In “Edit” button, Add…  ⚠️
asp-controller="Dashboard" asp-action="EditNote" asp-route-id="@note.Id"



ii.  Add  Displaying EditNote()  Method in DashboardController.cs
- [ ] public async Task<IActionResult> EditNote(int id)    ⚠️
- [ ] return NotFound();     ⚠️



iii. EditNote.cshtml


iv. Add EditNote() method      that actually edit and save changes.
- [ ] public async Task<IActionResult> EditNote(int id, CreateNoteDto noteDto)    ⚠️












35. Implement DeleteNote Feature.

i.   Update Delete button..   Include asp-controller ,….

- [ ] An <a> sends a GET request:

- [ ] Deleting data should be a POST:
POST /Dashboard/DeleteNote/7
So,…

<form 
     asp-controller="Dashboard"
      asp-action="DeleteNote"
      asp-route-id="@note.Id"
      method="post"
      class="d-inline">

    <button type="submit"
            class="btn btn-sm btn-outline-danger">
        Delete
    </button>
</form>



ii. Create DeleteNote Action  in DashboardController.cs










36.  Trash Page..
i. Make sure Trash link  in  <aside>  works…. has asp-controller,…..
ii. Create Trash()   method   in   DashboardController.cs
iii. Create    Trash.cshtml      that displays Page.


iii. Make “Restore” button work:
  - - -   Restore button inside form with asp-controller, action,…..
  - - -   RestoreNote()   method   in   DashboardController.cs

iv. Make “Delete Permanently” button work:
  - - -   Delete Permanently button inside form with asp-controller, action,…..
  - - -   DeletePermanently()   method   in   DashboardController.cs

  - - -    “Are u Sure ?”   Popup







37.   Pin / UnPin :     Same way.

i.         Add     Pin / Unpin     button   in every card.
ii.        Add     asp-route-returnAction="AllNotes"
- - -    Add     asp-route-returnAction=“Index”

iii.       Add     TogglePin   Controller     in DashboardController.cs
 - - -   Check for    return RedirectToAction








38.   Searching    in    AllNotes Page….     AllNotes.cshtml
 - - -  Update  AllNotes()   method   with search parameter & all.


39.      Update Index() method in   DashboardController.cs    to return Count-Statistics.
 - - -   Update   Index.cshtml  to  display Statistics.



40. Edit route Navigation Fix.
Dashboard → Edit was returning to All Notes, so I introduced returnAction.








41. Fix error :

Update to
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();













