# terra_backend

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
