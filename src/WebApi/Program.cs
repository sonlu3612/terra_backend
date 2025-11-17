using Core.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ============ Entity Framework & Identity Setup ============
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Infrastructure")
    )
);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ============ Application Services ============
// TODO: Uncomment and implement these services later
// builder.Services.AddScoped<Infrastructure.Services.ITokenService, Infrastructure.Services.TokenService>();
// builder.Services.AddScoped<Core.Interfaces.IOtpService, Infrastructure.Services.OtpService>();
// builder.Services.AddScoped<Core.Interfaces.ISmsService, Infrastructure.Services.SmsService>();
// builder.Services.AddScoped<Core.Interfaces.IEmailSender, Infrastructure.Services.EmailSender>();

// ============ MediatR ============
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(Application.Features.Queries.Users.GetCurrentUserQuery).Assembly);
});

// ============ API Documentation ============
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Terra Backend API",
        Version = "v1",
        Description = "API for Terra social media application"
    });
});

// ============ CORS ============
// TODO: Restrict CORS origins when frontend is deployed
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ============ Middleware Pipeline ============
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Redirect root URL to Swagger UI
    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/")
        {
            context.Response.Redirect("/swagger");
            return;
        }

        await next();
    });
}

// Global exception handler middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

// TODO: Change policyname when restricting CORS
app.UseCors("AllowAll");

// TODO: Authentication middleware will be added later

app.MapControllers();

// ============ Database Initialization ============
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    try
    {
        // Apply pending migrations
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.Run();
