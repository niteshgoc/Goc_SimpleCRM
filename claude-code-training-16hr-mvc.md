# Claude Code Training Program
## 16 Hours | ASP.NET Core MVC + Razor + jQuery + MS SQL (Dapper) | Windows

**Objective:** Build a simple CRM application using Claude Code where Claude does the actual development work.

---

# DAY 1 (8 Hours)

---

## Session 1: Installation & First Steps (2 Hours)

### 1.1 Prerequisites Check

Before installing Claude Code, ensure you have:

| Software | Minimum Version | Check Command |
|----------|-----------------|---------------|
| Node.js | 18.0 or higher | `node --version` |
| npm | 8.0 or higher | `npm --version` |
| .NET SDK | 8.0 | `dotnet --version` |

**If Node.js not installed:**
1. Go to https://nodejs.org/
2. Download **LTS version** (Windows Installer .msi)
3. Run installer → Accept defaults → Complete installation
4. Restart your terminal
5. Verify: `node --version`

---

### 1.2 Installing Claude Code on Windows

Open Command Prompt or PowerShell as Administrator:

```cmd
npm install -g @anthropic-ai/claude-code

claude --version
```

**If permission errors occur:**
```cmd
npm config set prefix %USERPROFILE%\npm
```
Restart terminal and try again.

---

### 1.3 First-Time Authentication

```cmd
claude
```

First time will prompt for authentication:
1. Click the link shown
2. Login to Anthropic account
3. Authorize Claude Code
4. Return to terminal - connected!

---

### 1.4 Hands-On: Your First Conversation

**Task 1:** Create project folder and start Claude

```cmd
mkdir C:\Projects\SimpleCRM
cd C:\Projects\SimpleCRM
claude
```

**Task 2:** Let Claude do the work

```
> Create a new ASP.NET Core 8 MVC project called SimpleCRM in the current folder. Set it up with:
- MVC pattern with Razor views
- Bootstrap for styling
- jQuery included
- Folder structure ready for a CRM application
```

Claude will create all the files directly. Watch it work!

**Task 3:** Verify the project

```
> Build the project and tell me if there are any errors.
```

```
> Run the project so I can see it in browser.
```

Open the URL Claude shows (usually https://localhost:5001 or similar).

---

## Session 2: Understanding CLAUDE.md (1.5 Hours)

### 2.1 What is CLAUDE.md?

CLAUDE.md tells Claude about YOUR project. Claude reads it automatically when starting.

**Without CLAUDE.md:** Claude makes generic assumptions
**With CLAUDE.md:** Claude follows your specific patterns and conventions

---

### 2.2 How to Write an Effective CLAUDE.md

**Rule 1:** Keep it concise (50-150 lines)
**Rule 2:** Focus on what's unique to YOUR project
**Rule 3:** Include code examples of your patterns

---

### 2.3 CLAUDE.md Template for MVC + Dapper Projects

```
> Create a CLAUDE.md file for this project with the following information:

Project: Simple CRM
Tech Stack: ASP.NET Core 8 MVC, Razor Views, jQuery, Bootstrap, Dapper, MS SQL Server

Project Structure:
- Controllers/ → MVC Controllers
- Models/ → Entity models and ViewModels
- Views/ → Razor views organized by controller
- Repositories/ → Data access with Dapper
- Services/ → Business logic
- wwwroot/ → Static files (CSS, JS)

Coding Conventions:
- Use async/await for all database calls
- ViewModels for passing data to views (suffix: ViewModel)
- Repository pattern with Dapper
- Parameterized SQL queries only
- jQuery for client-side interactions

Database:
- Connection string in appsettings.json
- Table names: plural (Customers, Contacts)
- Use stored procedures for complex queries
```

Claude creates the CLAUDE.md file.

**Test it:**
```
> What do you know about this project based on CLAUDE.md?
```

Claude should respond using information from the file!

---

### 2.4 Updating CLAUDE.md

As project grows, keep CLAUDE.md updated:

```
> Add to CLAUDE.md that we use DataTables jQuery plugin for all grid/table displays.
```

---

## Session 3: Create CRM Application Structure (2 Hours)

### 3.1 Create Project Structure

```
> Set up the folder structure for our CRM:
- Models/Entities/ → Database entity classes
- Models/ViewModels/ → View model classes
- Repositories/Interfaces/ → Repository interfaces
- Repositories/ → Repository implementations
- Services/Interfaces/ → Service interfaces
- Services/ → Service implementations

Create the folders and any base classes we might need.
```

---

### 3.2 Create Customer Entity

```
> Create a Customer entity class in Models/Entities with:
- Id (int, primary key)
- CompanyName (string, required, max 200)
- Email (string, required)
- Phone (string, optional)
- Address (string, optional)
- City (string, optional)
- CreatedAt (DateTime, default now)
- IsActive (bool, default true)
```

---

### 3.3 Create Customer ViewModels

```
> Create ViewModels for Customer in Models/ViewModels:
1. CustomerListViewModel - for displaying in grid (Id, CompanyName, Email, Phone, City, IsActive)
2. CustomerCreateViewModel - for create form (CompanyName, Email, Phone, Address, City) with validation attributes
3. CustomerEditViewModel - for edit form (all fields including Id) with validation attributes
4. CustomerDetailsViewModel - for details page (all fields including CreatedAt)
```

---

### 3.4 Create Repository

```
> Create ICustomerRepository interface in Repositories/Interfaces with methods:
- Task<IEnumerable<Customer>> GetAllAsync()
- Task<Customer?> GetByIdAsync(int id)
- Task<int> CreateAsync(Customer customer)
- Task<bool> UpdateAsync(Customer customer)
- Task<bool> DeleteAsync(int id)
- Task<IEnumerable<Customer>> SearchAsync(string searchTerm)
```

```
> Create CustomerRepository class implementing ICustomerRepository using Dapper.
Use parameterized queries for all operations. Inject IConfiguration to get connection string.
```

---

### 3.5 Create Service Layer

```
> Create ICustomerService interface and CustomerService class.
The service should:
- Use ICustomerRepository
- Have methods matching the repository
- Map between Entity and ViewModels
- Handle any business logic
```

---

### 3.6 Create Customer Controller

```
> Create CustomersController with these actions:
- Index() - display list of customers
- Details(int id) - show customer details
- Create() GET - show create form
- Create(CustomerCreateViewModel) POST - save new customer
- Edit(int id) GET - show edit form
- Edit(CustomerEditViewModel) POST - save changes
- Delete(int id) GET - show delete confirmation
- DeleteConfirmed(int id) POST - actually delete
- Search(string term) - AJAX endpoint for searching

Use constructor injection for ICustomerService.
```

---

### 3.7 Register Services

```
> Update Program.cs to register:
- Database connection (connection string from appsettings)
- ICustomerRepository → CustomerRepository
- ICustomerService → CustomerService
```

---

### 3.8 Build and Check

```
> Build the project. If there are any errors, fix them.
```

---

## Session 4: Connect MS SQL Database with Dapper (1.5 Hours)

### 4.1 Install Dapper

```
> Add Dapper NuGet package to the project and any other packages needed for SQL Server.
```

---

### 4.2 Configure Connection String

```
> Update appsettings.json with SQL Server connection string.
Use: Server=localhost;Database=SimpleCRM;Trusted_Connection=True;TrustServerCertificate=True;

Also create a database connection helper if needed.
```

---

### 4.3 Create Database and Table

```
> Create a SQL script file in a new "Database" folder that:
1. Creates SimpleCRM database if not exists
2. Creates Customers table with all columns matching our Customer entity
3. Includes proper data types, primary key, and constraints
4. Adds some sample data (3-4 customers)

Save it as Database/InitialSetup.sql
```

**Run the script** in SQL Server Management Studio.

---

### 4.4 Test Database Connection

```
> Run the project and navigate to /Customers. Does it connect to database properly?
If there are errors, fix them.
```

---

## Session 5: Essential Commands - /clear and /compact (1 Hour)

### 5.1 Understanding Context

When you chat with Claude Code, it remembers your conversation. This uses tokens (costs money) and can get confusing in long sessions.

---

### 5.2 The /clear Command

**What it does:** Completely resets conversation. Claude forgets everything.

**When to use:**
- Starting new, unrelated task
- Conversation got confused
- Want fresh start

```
> /clear
```

**Example:**
```
> Create Customer views
> [Claude creates views]
> /clear
> What views did we just create?
# Claude doesn't remember - conversation was cleared
```

---

### 5.3 The /compact Command

**What it does:** Summarizes conversation to save tokens but keeps important context.

**When to use:**
- Long conversation, same task
- Want to save tokens
- Keep context without full history

```
> /compact
```

**Example:**
```
> Create Customer entity
> Create Customer repository
> Create Customer service
> /compact
# Claude summarizes what was done
> Now create the controller
# Claude continues knowing the context
```

---

### 5.4 When to Use Which?

| Situation | Command |
|-----------|---------|
| Switching to different task | `/clear` |
| Same task, conversation is long | `/compact` |
| Claude seems confused | `/clear` |
| End of day | `/clear` |

---

### 5.5 Hands-On Practice

**Test /compact:**
```
> Explain what Dapper is
> What are benefits over Entity Framework?
> /compact
> What were we discussing?
```

**Test /clear:**
```
> Create a Product model
> /clear
> What model did we just create?
```

---

# DAY 2 (8 Hours)

---

## Session 6: Git Integration (1.5 Hours)

### 6.1 Initialize Git Repository

```
> Initialize git repository for this project.
Create appropriate .gitignore for ASP.NET Core MVC project.
```

---

### 6.2 First Commit

```
> Stage all files and create initial commit with message "Initial CRM setup with Customer module"
```

---

### 6.3 Connect to Remote Repository

**For GitHub:**
```
> Add remote origin https://github.com/myusername/SimpleCRM.git and push the code.
```

**For Azure DevOps:**
```
> Add remote origin https://dev.azure.com/myorg/project/_git/SimpleCRM and push the code.
```

---

### 6.4 Create Feature Branch

```
> Create new branch called "feature/contact-module" and switch to it.
```

---

## Session 7: Modifying Existing Code + /add Command (2.5 Hours)

### 7.1 The /add Command

**What it does:** Shows Claude your existing code so it can understand and modify it.

| Command | What it does |
|---------|--------------|
| `/add Models/Customer.cs` | Add single file |
| `/add Models/*.cs` | Add all .cs files in folder |
| `/add Models/**/*.cs` | Add files in folder and subfolders |

---

### 7.2 Add Contact Entity to CRM

**Step 1: Look at existing structure**
```
> /add Models/Entities/Customer.cs

> Create a Contact entity that belongs to a Customer (one customer has many contacts).
Fields: Id, CustomerId (FK), FirstName, LastName, Email, Phone, JobTitle, CreatedAt, IsActive
Follow the same pattern as Customer.
```

---

### 7.3 Create Contact Table

```
> Create SQL script Database/AddContactsTable.sql that:
- Creates Contacts table
- Adds foreign key to Customers
- Adds index on CustomerId
- Inserts sample contacts for existing customers
```

Run the script in SSMS.

---

### 7.4 Create Contact ViewModels

```
> /add Models/ViewModels/CustomerCreateViewModel.cs
> /add Models/ViewModels/CustomerListViewModel.cs

> Create ViewModels for Contact following the same pattern:
- ContactListViewModel
- ContactCreateViewModel  
- ContactEditViewModel
- ContactDetailsViewModel

Include CustomerName in list and details view models.
```

---

### 7.5 Create Contact Repository

```
> /add Repositories/Interfaces/ICustomerRepository.cs
> /add Repositories/CustomerRepository.cs

> Create IContactRepository and ContactRepository following the same pattern.
Add method: GetByCustomerIdAsync(int customerId)
```

---

### 7.6 Create Contact Service

```
> /add Services/Interfaces/ICustomerService.cs
> /add Services/CustomerService.cs

> Create IContactService and ContactService following same pattern.
Include GetByCustomerIdAsync method.
```

---

### 7.7 Create Contact Controller

```
> /add Controllers/CustomersController.cs

> Create ContactsController following same pattern.
Add action: ByCustomer(int customerId) - shows contacts for specific customer
```

---

### 7.8 Register Services and Test

```
> /add Program.cs

> Add registration for Contact repository and service.
```

```
> Build and run. Fix any errors.
```

---

### 7.9 Create Razor Views for Contacts

```
> /add Views/Customers/Index.cshtml

> Create Contact views in Views/Contacts/:
1. Index.cshtml - list all contacts with DataTable
2. Create.cshtml - form to add new contact (with customer dropdown)
3. Edit.cshtml - form to edit contact
4. Details.cshtml - show contact details
5. Delete.cshtml - delete confirmation
6. _ContactsByCustomer.cshtml - partial view showing contacts for a customer

Use Bootstrap styling and follow the pattern from Customer views.
```

---

### 7.10 Add Contacts Section to Customer Details

```
> /add Views/Customers/Details.cshtml

> Modify Customer Details view to show list of contacts for this customer.
Add "Add Contact" button that links to Contact Create with CustomerId pre-filled.
Use the _ContactsByCustomer partial view.
```

---

### 7.11 Add jQuery for Interactions

```
> Add jQuery code to the Contact Index page that:
1. Initializes DataTable on the contacts grid
2. Adds search/filter functionality
3. Adds confirm dialog before delete
```

---

### 7.12 Test and Commit

```
> Build and run the project. Test all contact CRUD operations.
```

```
> Stage all changes and commit with message "Add Contact module with CRUD operations"
```

```
> Push to remote and merge to main branch.
```

---

## Session 8: Agents.md Basics (1.5 Hours)

### 8.1 What is Agents.md?

Agents.md defines **specialized modes** for Claude. Call them with `@agentname`.

**Without agents:** Explain requirements every time
**With agents:** Just call the agent, it knows the conventions

---

### 8.2 Create agents.md

```
> Create agents.md file in project root with these agents:

@entity - Creates entity classes following our patterns in Models/Entities

@viewmodel - Creates ViewModels with validation attributes in Models/ViewModels

@repository - Creates repository interface and Dapper implementation

@service - Creates service interface and implementation

@controller - Creates MVC controller with standard CRUD actions

@views - Creates Razor views with Bootstrap styling and jQuery

@sql - Creates SQL Server scripts (tables, procedures, queries)

@crud - Creates complete module: entity, viewmodels, repository, service, controller, views

@fix - Debugging helper that asks for relevant files and fixes errors

Each agent should follow our existing patterns in the project.
```

---

### 8.3 Using Agents

**Test @entity:**
```
> @entity Create Product entity with: Id, Name, Description, Price (decimal), Stock (int), CategoryId, IsActive
```

**Test @sql:**
```
> @sql Create Products table and Categories table with proper relationship and sample data.
```

**Test @viewmodel:**
```
> @viewmodel Create all ViewModels for Product
```

**Test @crud:**
```
> @crud Create complete Order module with:
- Order: Id, CustomerId, OrderDate, TotalAmount, Status, Notes
- Include all layers and views
```

**Test @fix:**
```
> @fix I'm getting "Object reference not set" error on the Contacts Index page
```

---

### 8.4 Benefits of Agents

| Without Agents | With Agents |
|----------------|-------------|
| "Create entity called Product with Name, Price, put in Models/Entities, use our conventions..." | `@entity Create Product with Name, Price` |
| Repeat instructions every time | Agent knows the patterns |
| Inconsistent results | Consistent output |

---

## Session 9: Tips & Best Practices (1 Hour)

### 9.1 Effective Prompts

**Be Specific:**
```
❌ "Create a view"
✅ "@views Create Product Index view with DataTable, search, and export to Excel button"
```

**Reference Existing Files:**
```
> /add Views/Customers/Index.cshtml
> Create Product Index view following this exact pattern
```

---

### 9.2 Quick Reference Card

```
STARTING
========
claude                    Start Claude Code
/exit or Ctrl+C          Exit

CONTEXT COMMANDS  
================
/add <file>              Add file to context
/add folder/*.cs         Add multiple files
/clear                   Reset conversation
/compact                 Summarize to save tokens

AGENTS
======
@entity                  Create entity class
@viewmodel               Create view models
@repository              Create repository
@service                 Create service
@controller              Create controller
@views                   Create Razor views
@sql                     SQL scripts
@crud                    Complete module
@fix                     Debug help
```

---

### 9.3 Common Workflows

**New Module:**
```
1. /clear
2. @entity Create [Entity]
3. @sql Create table script → run in SSMS
4. @viewmodel Create ViewModels
5. @repository Create repository
6. @service Create service  
7. @controller Create controller
8. @views Create all views
9. Register in Program.cs
10. Build and test
11. Commit to Git
```

**Fix Bug:**
```
1. @fix [describe error]
2. /add [files Claude asks for]
3. Let Claude fix it
4. Test
```

**Modify Existing:**
```
1. /add [files to modify]
2. Describe changes needed
3. Review and apply
4. Test
```

---

### 9.4 Do's and Don'ts

| ✅ Do | ❌ Don't |
|-------|---------|
| Review generated code | Blindly accept everything |
| Use /add to show existing code | Expect Claude to guess |
| Be specific in requests | Give vague instructions |
| Use agents for consistency | Repeat full prompts |
| /compact during long sessions | Let context grow huge |
| Test after each change | Make many untested changes |

---

### 9.5 Final Practice Exercise

**Task:** Create "Notes" module

Notes are attached to Customers (one customer has many notes).

**Fields:** Id, CustomerId, Title, Content, CreatedAt

**Steps:**
```
1. git checkout -b feature/notes
2. /clear
3. @entity Create Note entity
4. @sql Create Notes table
5. @viewmodel Create Note ViewModels
6. @repository Create Note repository
7. @service Create Note service
8. @controller Create Notes controller
9. @views Create all Note views
10. Add Notes section to Customer Details page
11. Register services
12. Build and test
13. Commit and push
14. Merge to main
```

---

## Summary

| Session | What You Learned |
|---------|------------------|
| 1 | Install Claude Code, create MVC project |
| 2 | Create effective CLAUDE.md |
| 3 | Build CRM structure - Claude does the work |
| 4 | Connect MS SQL with Dapper |
| 5 | /clear and /compact commands |
| 6 | Git integration |
| 7 | /add command + add Contact module |
| 8 | Create and use agents.md |
| 9 | Best practices and workflows |

---

## What's Next?

**This Week:**
- Use Claude Code for one real task
- Customize CLAUDE.md for your project
- Create agents.md with your patterns

**Future Topics:**
- Advanced Razor components
- AJAX forms and partial views
- Report generation
- Unit testing with Claude

---

**You're ready to use Claude Code!** 🎉
