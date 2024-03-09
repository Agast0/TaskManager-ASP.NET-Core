# Task Manager API

This project is a Task Manager API developed using ASP.NET Core. It provides endpoints for managing tasks and user accounts. JWT authentication is implemented for secure access to endpoints, and Swagger UI is integrated for easy testing and documentation.

## Technologies Used

- ASP.NET Core
- Entity Framework Core
- JWT Authentication
- Swagger UI
- MSSQL

## API Endpoints

| Endpoint                            | Authenticated | Description                                        | Returns                   |
|-------------------------------------|---------------|----------------------------------------------------|---------------------------|
| GET /task/get/{id}                  | No            | Get a task by its ID                               | TaskDTO                   |
| GET /task/get                       | No            | Get all tasks                                      | List of TaskDTOs          |
| GET /task/getWithUsername/{username}| No            | Get all tasks associated with a specific user      | List of TaskDTOs          |
| POST /task/create                   | Yes           | Create a new task                                  | Created TaskDTO           |
| PUT /task/update/{id}               | Yes           | Update an existing task                            | Updated TaskDTO           |
| DELETE /task/delete/{id}            | Yes           | Delete a task by its ID                            | -                         |
| GET /user/get/{username}            | No            | Get a user by username                             | GetUserResponseDTO        |
| POST /user/register                 | No            | Register a new user                                | Created UserDTO           |
| POST /user/login                    | No            | User login                                         | JWT Token                 |
| DELETE /user/delete/{username}      | Yes           | Delete a user and all associated tasks             | -                         |

**Note:** Users can only edit/delete their own tasks. To create a task, they must be logged in. Users can only delete their own accounts.

## Database Schema

### Task Table
| Field       | Type       | Description         |
|-------------|------------|---------------------|
| Id          | int        | Primary Key         |
| Title       | varchar(50)| Task title          |
| Description | text       | Task description    |
| DueDate     | datetime   | Due date of the task|
| IsCompleted | bit        | Completion status   |
| Username    | varchar(50)| Foreign key to User |

### User Table
| Field       | Type       | Description         |
|-------------|------------|---------------------|
| Username    | varchar(50)| Primary Key         |
| Password    | varchar(100)| Password hash       |

## Usage Instructions

1. Install and setup Visual Studio for ASP.NET development.
2. Create an MSSQL database and enter the connection string into the `appsettings.json` file.
3. Migrate the database using `Add-Migration [migration name]`.
4. Update the database using `Update-Database`.
5. Run the application and use Swagger UI for testing (ensure JWT token from login is used for authenticated endpoints).
