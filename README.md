# terra_backend

## Setup

1. Clone the repository:

```bash
git clone https://github.com/sonlu3612/terra_backend.git
cd terra_backend
```

2. Create `src/.env` file and add the following environment variable to configure

```env
CONNECTION_STRING= "Server=(your_server_name);Database=TerraDb;Integrated Security=True;TrustServerCertificate=True;Connection Timeout=5;ConnectRetryCount=0;"
```

**Note**: you need to replace `(your_server_name)` with your actual SQL Server instance name.

3. Update database schema using Entity Framework Core migrations (2 ways):

```bash
cd src && dotnet ef database update
```

or using npm script:

```bash
npm run db:update
```

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
