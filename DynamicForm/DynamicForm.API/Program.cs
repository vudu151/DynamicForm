using DynamicForm.API.Data;
using DynamicForm.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(localdb)\\mssqllocaldb;Database=DynamicFormDb;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add Services
builder.Services.AddScoped<IFormService, FormService>();
builder.Services.AddScoped<IFormDataService, FormDataService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        // Allow any origin for development (including mixed content HTTPS -> HTTP)
        policy.SetIsOriginAllowed(origin => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// CORS phải được gọi trước UseAuthorization và MapControllers
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DynamicForm API V1");
        c.RoutePrefix = "swagger";
    });
}

// Disable HTTPS redirection for development (API runs on HTTP)
// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
