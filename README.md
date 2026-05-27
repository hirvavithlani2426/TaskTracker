# TaskTracker.Api

A production-ready .NET Web API built as part of the technical assessment for OCAS.

## Architectural Choices & Features
- **Clean Separation of Concerns:** Decoupled architecture using thin Controllers mapping requests to a dedicated Business Logic Service Layer (`TaskService`) to ensure single responsibility.
- **Data Protection:** Separated domain entities entirely from the API surface using dedicated Data Transfer Objects (DTOs) for incoming payloads (`CreateTaskDto`, `UpdateTaskDto`) and outgoing results (`TaskResponseDto`).
- **Hybrid Infrastructure Routing:** Configured a runtime environment switch in `Program.cs`. It automatically provisions a self-healing local **SQLite** file configuration upon startup for seamless development execution, while fully supporting cloud database schemas (**Azure SQL**) for production deployment.
- **Robust Testing:** Implemented an automated unit test suite using **xUnit** and EF Core’s In-Memory tracking database provider. Tests fully cover standard path creations, parameter updates, input validation rejections, and the exact task state transition business constraint rules.

## Local Execution
1. Run the API from the root folder:
   ```powershell
   dotnet run --project src\TaskTracker.Api\TaskTracker.Api.csproj --launch-profile "http"
