using NexHire.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add API controller support.
builder.Services.AddControllers();

// Add Swagger / OpenAPI services.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Infrastructure services such as EF Core and SQL Server.
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Swagger is exposed only while developing locally.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Attribute-routed API controllers.
app.MapControllers();

// Simple root endpoint to confirm the API is alive.
app.MapGet("/", () => Results.Ok(new
{
    application = "NexHire API",
    status = "Running",
    environment = app.Environment.EnvironmentName
}));

app.Run();