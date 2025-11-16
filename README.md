# terra_backend

## Run the solution

There are two ways to run the solution:

1. Using Visual Studio: Open the `SocialMedia.sln` solution file and click on the `Run` button.

2. Using Command Line (Require npm installed):

  ```bash
  npm run dev
  ```

## Project Structure

This project followed clean architecture principles. The directory structure is organized as follows:

```bash
SocialMedia.sln
├── src
│   ├── Application
│   │   ├── Features/ (CQRS pattern: Commands, Queries)
│   │   ├── DTOs/
│   │   └── Mappers/
│   ├── Core ( Domain)
│   │   ├── Entities/
│   │   ├── Interfaces/ (Repositories, Services)
│   │   └── Common/
│   ├── Infrastructure
│   │   ├── Persistence/ (DbContext, Migrations, Repositories Impl)
│   │   └── Services/ (EmailService, FileStorageService Impl)
│   └── WebApi
│       ├── Controllers/
│       ├── Hubs/ (SignalR Hubs)
│       ├── Middleware/
│       └── Program.cs
└── tests (Maybe later)
    ├── Application.UnitTests/
    └── Infrastructure.IntegrationTests/
```
