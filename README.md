# Task Management API

-----

This is a robust and scalable Task Management API built with **.NET Core**, following **Clean Architecture** principles. It provides a secure backend for managing user-specific tasks, incorporating modern development practices such as DTOs, AutoMapper, and comprehensive authentication/authorization.

## Features Implemented So Far

-----

The project is under active development, and the following core functionalities have been implemented:

  * **User Authentication & Authorization**:
      * User registration and login using **JWT (JSON Web Tokens)**.
      * Secure endpoints requiring authenticated access.
      * Password hashing and security managed by **ASP.NET Core Identity**.
  * **Task Management**:
      * **CRUD** (Create, Read, Update, Delete) operations for tasks.
      * Tasks are associated with individual users, ensuring data isolation.
  * **Data Transfer Objects (DTOs)**:
      * Dedicated DTOs for `TaskPost` (creating tasks), `TaskGetDto` (retrieving tasks), and `TaskPutDto` (updating tasks).
      * `TaskGetDto` includes the `UserName` of the task owner for enhanced visibility.
  * **AutoMapper Integration**:
      * Seamless mapping between entities and DTOs using **AutoMapper**, reducing boilerplate code in the service layer.
      * Custom mapping profiles for tasks and user profiles.
  * **Sorting, Filtering, and Pagination for Tasks**:
      * Users can sort tasks by various fields (e.g., `CreatedAt`, `Title`, `DueDate`) in ascending or descending order.
      * Tasks can be filtered by completion status (`isCompleted`), date range (`dueDateFrom`, `dueDateTo`), and keywords (`search`) in title or description.
      * Pagination is implemented to retrieve tasks in manageable chunks (`pageNumber`, `pageSize`).
  * **User Profile Management**:
      * Authenticated users can retrieve their own profile information (`GET /api/UserProfile`).
      * Users can update their `UserName` and `Email` (`PUT /api/UserProfile`).
      * Dedicated functionality for changing a user's password (`POST /api/UserProfile/change-password`).
  * **Clean Architecture**:
      * The project adheres to **Clean Architecture** principles, separating concerns into Core, Infrastructure, and Presentation layers for better maintainability, testability, and scalability.

## Technologies Used

-----

  * **.NET Core**: Backend framework.
  * **ASP.NET Core Web API**: For building RESTful APIs.
  * **Entity Framework Core**: ORM for database interaction.
  * **SQL Server (LocalDB)**: Database for persistence.
  * **ASP.NET Core Identity**: For user management, authentication, and authorization.
  * **JWT (JSON Web Tokens)**: For securing API endpoints.
  * **AutoMapper**: For object-to-object mapping (Entities to DTOs and vice-versa).
  * **Swagger/OpenAPI**: For API documentation and testing.

## Setup and Running the Project

-----

To set up and run this project locally, follow these steps:

1.  **Clone the Repository**:

    ```bash
    git clone <your-repository-url>
    cd TaskManagmentApi # Or the root directory of your solution
    ```

2.  **Restore NuGet Packages**:

    Navigate to each project directory (`TaskManagementApi.Core`, `TaskManagementApi.Infrastructure`, `TaskManagementApi.Presentation`) and run:

    ```bash
    dotnet restore
    ```

    Alternatively, run `dotnet restore` from the solution root.

3.  **Update `appsettings.json`**:

    Open `TaskManagementApi.Presentation/appsettings.json` and ensure your `DefaultConnection` string points to your SQL Server instance. You can use the provided LocalDB string for quick setup.

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskDbCleanArchUoWAuth;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
    },
    "Jwt": {
      "Key": "ThisIsASecretKeyThatIsAtLeast32CharactersLongForHS256", // !!! IMPORTANT: Change this to a strong, long, random key in production !!!
      "Issuer": "TaskManagementApi",
      "Audience": "TaskManagementUsers"
    }
    ```

    **Remember to change the `Jwt:Key` to a strong, long, random key for production environments.**

4.  **Apply Database Migrations**:

    Navigate to the solution root directory (`TaskManagmentApi`) in your terminal and run the following commands to create/update the database schema:

    ```bash
    dotnet ef migrations add InitialCreate -p TaskManagementApi.Infrastructure -s TaskManagementApi.Presentation
    dotnet ef database update -p TaskManagementApi.Infrastructure -s TaskManagementApi.Presentation
    ```

    (Note: If you've already run `AddUsersAndTaskUserRelationship` migration previously, you might skip the `migrations add` step and just run `database update`.)

5.  **Run the API**:

    Navigate to the `TaskManagementApi.Presentation` project directory and run:

    ```bash
    dotnet run
    ```

    The API will typically run on `https://localhost:7001` (or a similar port).

6.  **Access Swagger UI**:

    Open your web browser and navigate to `https://localhost:7001/swagger` (replace the port if different). This will provide an interactive UI to explore and test all API endpoints.

## API Endpoints Overview

-----

The API exposes endpoints for:

  * **Authentication**:
      * `POST /api/Auth/register`: Register a new user.
      * `POST /api/Auth/login`: Log in and receive a JWT.
  * **Tasks** (Requires Authentication):
      * `GET /api/Tasks`: Retrieve all tasks for the authenticated user (supports filtering, sorting, pagination).
      * `GET /api/Tasks/{id}`: Retrieve a specific task by ID.
      * `POST /api/Tasks`: Create a new task.
      * `PUT /api/Tasks/{id}`: Update an existing task.
      * `DELETE /api/Tasks/{id}`: Delete a task.
  * **User Profile** (Requires Authentication):
      * `GET /api/UserProfile`: Get the authenticated user's profile.
      * `PUT /api/UserProfile`: Update the authenticated user's profile.
      * `POST /api/UserProfile/change-password`: Change the authenticated user's password.

Detailed request/response schemas and examples can be found in the Swagger UI.

## Future Enhancements

-----

This project is continuously evolving. Planned future enhancements include:

  * Task Categories/Tags
  * Soft Deletion for Tasks
  * Task Reminders/Notifications
  * Role-Based Authorization (RBAC)
  * File Attachments to Tasks
  * Advanced Search Functionality
  * Global Error Handling
  * Logging and Monitoring
  * Health Checks

As these features are implemented, this README will be updated accordingly.

## Contributing

-----

Contributions are welcome\! If you find a bug or have a feature request, please open an issue or submit a pull request.

## License

-----

This project is licensed under the MIT License. See the `LICENSE` file for details.
