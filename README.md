# Task Management API (ASP.NET Core Clean Architecture)

This is a robust Task Management API built with ASP.NET Core following a Clean Architecture approach. It provides functionalities for managing tasks and subtasks, user authentication and authorization (JWT Bearer tokens with ASP.NET Core Identity), role management, soft deletion, task notifications, centralized error handling, and structured logging.

## Features

### User Authentication & Authorization:

- User registration and login using ASP.NET Core Identity.
    
- JWT Bearer token generation for authenticated access.
    
- Role-based authorization (Admin, User, Developer, QA, TeamLead, ProjectManager).
    
- User profile management (view and update).
    
- Password change functionality.
    

### Role Management (Admin Only):

- View all system roles.
    
- Create new roles.
    
- Assign/remove users from roles.
    
- Admin-driven user registration with immediate role assignment.
    

### Task Management:

- Create, retrieve (single, all), update, and soft-delete tasks.
    
- Tasks can be assigned to individual users.
    
- Admins and Team Leads can assign tasks to other users.
    
- Filtering, sorting, and pagination for task retrieval.
    
- Soft deletion: Tasks are marked as deleted rather than permanently removed, allowing for restoration.
    
- Task restoration functionality (Admin only).
    

### Subtask Management:

- Create, retrieve (single, all for a parent task), update, and soft-delete subtasks.
    
- Subtasks are linked to a parent task.
    
- Users can manage their own subtasks.
    

### Task Notifications:

- Tasks can have a `DueDate` and a `NotificationDateTime`.
    
- A background service checks for tasks due for notification and logs reminders.
    
- Tasks are marked as `IsNotified` to prevent duplicate notifications.
    

### Centralized Error Handling:

- Custom middleware (`ErrorHandlingMiddleware`) intercepts all unhandled exceptions.
    
- Provides standardized `ProblemDetails` responses for API errors.
    
- Logs detailed exception information.
    

### Structured Logging:

- Leverages `Microsoft.Extensions.Logging` for structured and contextual logging across the application.
    
- Configurable log levels via `appsettings.json`.
    

## Getting Started

Follow these steps to set up and run the project locally.

### Prerequisites

- .NET 8 SDK
    
- SQL Server (LocalDB is sufficient for local development, typically installed with Visual Studio)
    
- A tool for API testing (e.g., Postman, Swagger UI). Swagger UI is integrated into the project.
    

### Setup Instructions

1. **Clone the Repository (if applicable):**
    
    ```
    git clone <your-repository-url>
    cd TaskManagementSolution
    ```
    
2. Navigate to the Solution Directory:
    
    Ensure you are in the root directory of your solution, e.g., TaskManagementSolution/.
    
3. Install NuGet Packages:
    
    Open your terminal or command prompt and run the following commands for each project:
    
    ```
    # For TaskManagementApi.Presentation
    dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
    
    # For TaskManagementApi.Infrastructure
    dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
    
    # For TaskManagementApi.Core
    dotnet add package Microsoft.AspNetCore.Identity.Core
    dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
    ```
    
4. Update Database Connection String:
    
    Open TaskManagementApi.Presentation/appsettings.json and ensure your DefaultConnection string points to your SQL Server instance.
    
    ```
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskDbCleanArchUoWAuth;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
    }
    ```
    
5. Apply Database Migrations and Seed Data:
    
    From the TaskManagementSolution/ directory, run the following commands to create the database schema and seed initial data (roles, users, tasks, subtasks):
    
    ```
    dotnet ef migrations add InitialSetup -p TaskManagementApi.Infrastructure -s TaskManagementApi.Presentation
    # If you have previous migrations, you might need to run:
    # dotnet ef migrations add AddAssignedByUserIdToTaskItems -p TaskManagementApi.Infrastructure -s TaskManagementApi.Presentation
    # (This migration was added in the previous update)
    
    dotnet ef database update -p TaskManagementApi.Infrastructure -s TaskManagementApi.Presentation
    ```
    
    This will create the `TaskDbCleanArchUoWAuth` database (or update an existing one) and populate it with:
    
    - **Roles:** Admin, User, Developer, QA, TeamLead, ProjectManager.
        
    - **Users:**
        
        - `admin@softwarehouse.com` (Password: `AdminPass123!`, Role: Admin)
            
        - `dev1@softwarehouse.com` (Password: `DevPass123!`, Role: Developer)
            
        - `dev2@softwarehouse.com` (Password: `DevPass123!`, Role: Developer)
            
        - `qa1@softwarehouse.com` (Password: `QAPass123!`, Role: QA)
            
        - `teamlead@softwarehouse.com` (Password: `TLPass123!`, Role: TeamLead)
            
        - `pm@softwarehouse.com` (Password: `PMPass123!`, Role: ProjectManager)
            
    - Sample tasks and subtasks assigned to these users, including tasks assigned by `admin@softwarehouse.com` and `teamlead@softwarehouse.com`.
        
6. Run the API:
    
    From the TaskManagementSolution/ directory:
    
    ```
    dotnet run --project TaskManagementApi.Presentation
    ```
    
    The API will start, typically on `https://localhost:7001` (or a similar port). Swagger UI will automatically open in your browser at `/swagger` (e.g., `https://localhost:7001/swagger`).
    

## API Endpoints

All API endpoints are accessible via Swagger UI.

### Authentication

- **`POST /api/Auth/register`**
    
    - Register a new user. Default role `User` is assigned.
        
    - Request Body: `{ "email": "string", "password": "string" }`
        
- **`POST /api/Auth/login`**
    
    - Login and receive a JWT token.
        
    - Request Body: `{ "email": "string", "password": "string" }`
        
    - Response: `{ "token": "jwt_token_string" }`
        

### User Profile (Requires Authentication)

- **`GET /api/UserProfile`**
    
    - Get the current authenticated user's profile.
        
- **`PUT /api/UserProfile`**
    
    - Update the current authenticated user's profile.
        
    - Request Body: `{ "email": "string", "userName": "string" }`
        
- **`POST /api/UserProfile/change-password`**
    
    - Change the current authenticated user's password.
        
    - Request Body: `{ "currentPassword": "string", "newPassword": "string", "confirmNewPassword": "string" }`
        
- **`GET /api/UserProfile/roles`**
    
    - Get the roles of the current authenticated user.
        

### Role Management (Requires Admin Role)

- **`GET /api/RoleManagement/roles`**
    
    - Get all available roles in the system.
        
- **`POST /api/RoleManagement/roles`**
    
    - Create a new role.
        
    - Request Body: `{ "roleName": "string" }`
        
- **`GET /api/RoleManagement/users/{userId}/roles`**
    
    - Get roles for a specific user by their ID.
        
- **`POST /api/RoleManagement/users/{userId}/roles`**
    
    - Add a user to a specific role.
        
    - Request Body: `{ "roleName": "string" }`
        
- **`DELETE /api/RoleManagement/users/{userId}/roles/{roleName}`**
    
    - Remove a user from a specific role.
        
- **`POST /api/RoleManagement/register-with-role`**
    
    - Admin registers a new user and assigns a role simultaneously.
        
    - Request Body: `{ "email": "string", "password": "string", "roleName": "string" }`
        

### Task Management (Requires Authentication)

- **`GET /api/Tasks`**
    
    - Get all tasks. Supports filtering (`search`, `isCompleted`, `dueDateFrom`, `dueDateTo`, `includeDeleted`, `includeNotified`), sorting (`sortBy`, `sortOrder`), and pagination (`pageNumber`, `pageSize`).
        
    - Regular users see tasks assigned to them. Admins see tasks assigned by them OR assigned to them.
        
- **`GET /api/Tasks/MyAssignedTasks`** (Requires Admin Role)
    
    - Specifically for admins to get tasks assigned to them. Supports the same query parameters as `GET /api/Tasks`.
        
- **`GET /api/Tasks/{id}`**
    
    - Get a task by ID.
        
- **`POST /api/Tasks`**
    
    - Create a new task. Admins can assign tasks to other users using `assignedToUserId`.
        
    - Request Body: `{ "title": "string", "description": "string", "dueDate": "datetime?", "isNotificationEnabled": "boolean", "notificationDateTime": "datetime?", "assignedToUserId": "string?" }`
        
- **`PUT /api/Tasks/{id}`**
    
    - Update an existing task. Admins can reassign tasks using `assignedToUserId`.
        
    - Request Body: `{ "id": "int", "title": "string", "description": "string", "isCompleted": "boolean", "dueDate": "datetime?", "isNotificationEnabled": "boolean", "notificationDateTime": "datetime?", "assignedToUserId": "string?" }`
        
- **`DELETE /api/Tasks/{id}`** (Requires Admin Role)
    
    - Soft delete a task. The task will be marked as `IsDeleted = true`.
        
- **`POST /api/Tasks/{id}/restore`** (Requires Admin Role)
    
    - Restore a soft-deleted task.
        

### Subtask Management (Requires Authentication)

- **`GET /api/Tasks/{parentTaskId}/SubTasks`**
    
    - Get all subtasks for a specific parent task. Supports filtering, sorting, and pagination.
        
- **`GET /api/Tasks/{parentTaskId}/SubTasks/{id}`**
    
    - Get a specific subtask by its ID.
        
- **`POST /api/Tasks/{parentTaskId}/SubTasks`**
    
    - Create a new subtask for a given parent task.
        
    - Request Body: `{ "title": "string", "description": "string", "dueDate": "datetime?" }`
        
- **`PUT /api/Tasks/{parentTaskId}/SubTasks/{id}`**
    
    - Update an existing subtask.
        
    - Request Body: `{ "id": "int", "title": "string", "description": "string", "isCompleted": "boolean", "dueDate": "datetime?" }`
        
- **`DELETE /api/Tasks/{parentTaskId}/SubTasks/{id}`**
    
    - Soft delete a subtask.
        
- **`POST /api/Tasks/{parentTaskId}/SubTasks/{id}/restore`**
    
    - Restore a soft-deleted subtask.
        

## Authentication

To interact with authenticated endpoints:

1. Use the `POST /api/Auth/login` endpoint with seeded credentials (e.g., `admin@softwarehouse.com` / `AdminPass123!`).
    
2. Copy the token from the response.
    
3. In Swagger UI, click the "Authorize" button (top right), select "Bearer", and paste your JWT token in the format Bearer YOUR_JWT_TOKEN_HERE.
    
    (e.g., Bearer eyJhbGciOiJIUzI1Ni...)
    
4. Click "Authorize" to apply the token. You can now access protected endpoints.
    

## Roles and Permissions

The system defines the following roles:

- **Admin:** Full access to all functionalities, including user and role management, task deletion/restoration, and assigning tasks to any user.
    
- **ProjectManager:** Can create and manage tasks, view all tasks, and potentially manage teams (if team functionality were implemented).
    
- **TeamLead:** Can create and manage tasks, assign tasks to Developer and QA users, and manage tasks within their team.
    
- **Developer:** Can manage tasks assigned to them, create personal tasks, and manage their own subtasks.
    
- **QA:** Can manage tasks assigned to them, create personal tasks, and manage their own subtasks.
    
- **User:** The default role for new registrations, with basic task management capabilities (manage their own tasks and subtasks).
    

## Error Handling

The API uses a centralized error handling middleware (`ErrorHandlingMiddleware`) to provide consistent error responses. In a development environment, detailed exception information (message, stack trace) is included in the `ProblemDetails` response. In production, this information would be suppressed for security.

**Example Error Response (Development):**

```
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "An error occurred while processing your request.",
  "status": 500,
  "detail": "Detailed error message from exception.",
  "instance": "/api/Tasks",
  "extensions": {
    "stackTrace": "...",
    "innerExceptionMessage": "..."
  }
}
```

## Logging

The application uses `Microsoft.Extensions.Logging` for structured logging. Log levels can be configured in `appsettings.json`. For example, setting `"TaskManagementApi": "Information"` will log informational messages from your application's namespaces.

```
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning",
    "TaskManagementApi": "Information"
  }
}
```

## Background Services

A `TaskNotificationBackgroundService` runs periodically to check for tasks that are due for notification. It logs reminders to the console based on the `NotificationDateTime` set for tasks. The check interval and lead time for notifications are configurable in `appsettings.json`.

```
"NotificationService": {
  "CheckIntervalMinutes": 1,
  "NotificationLeadTimeMinutes": 5
}
```
