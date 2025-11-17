// Program.cs
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==================== 1. CƠ SỞ DỮ LIỆU ====================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
                         ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

// ==================== 2. IDENTITY ====================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password policy nhẹ để dev nhanh, production có thể siết lại
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ==================== 3. AUTHENTICATION: JWT + GOOGLE ====================
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = builder.Configuration["Jwt:Key"]
             ?? throw new InvalidOperationException("Jwt:Key is missing in configuration.");

// Đảm bảo key đủ dài (>= 256 bits)
if (Encoding.UTF8.GetBytes(jwtKey).Length < 32)
    throw new InvalidOperationException("JWT Key must be at least 32 characters (256 bits) for HMAC-SHA256.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.FromMinutes(1)
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
})
.AddGoogle(options =>
{
    var clientId = builder.Configuration["Google:ClientId"]
                   ?? throw new InvalidOperationException("Google:ClientId is missing.");
    var clientSecret = builder.Configuration["Google:ClientSecret"]; // Có thể null nếu chỉ dùng credential flow

    options.ClientId = clientId;
    if (!string.IsNullOrEmpty(clientSecret))
        options.ClientSecret = clientSecret;

    options.SignInScheme = IdentityConstants.ExternalScheme;
    options.CallbackPath = "/signin-google";

    // Cho phép localhost + production
    options.AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
    options.TokenEndpoint = "https://oauth2.googleapis.com/token";
});

// ==================== 4. DI SERVICES ====================
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<ISmsService, SmsService>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddMemoryCache(); // Cho OTP

// ==================== 5. CONTROLLERS & SWAGGER ====================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Terra API", Version = "v1" });

    // JWT Bearer cho Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header. Example: 'Bearer eyJhbGciOi...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//// ==================== 6. CORS (rất quan trọng cho Vite frontend) ====================
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowTerraFrontend", policy =>
//    {
//        policy.WithOrigins(
//            "http://localhost:5173",     // Vite dev
//            "https://terra.yourdomain.com" // Production (thay domain thật)
//        )
//        .AllowAnyHeader()
//        .AllowAnyMethod()
//        .AllowCredentials();
//    });
//});

// ==================== 7. BUILD APP ====================
var app = builder.Build();

// ==================== 8. MIDDLEWARE PIPELINE ====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Terra API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
//app.UseCors("AllowTerraFrontend");

// Thứ tự cực kỳ quan trọng:
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Tự động migrate DB khi khởi động (chỉ nên bật ở dev)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();