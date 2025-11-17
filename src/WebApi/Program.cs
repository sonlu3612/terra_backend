var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Always enable Swagger and Swagger UI so users are redirected to API docs
app.UseSwagger();
app.UseSwaggerUI();

// Redirect root URL to Swagger UI (so visiting / opens the docs)
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        // Redirect to the Swagger UI landing page
        context.Response.Redirect("/swagger");
        return;
    }

    await next();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
