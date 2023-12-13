using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ToDoAPI.Data;
using ToDoAPI.Services;
using ToDoAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default connection")));
builder.Services.AddScoped<ITodoService, TodoService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalHost", builder =>
    {
        builder.WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowLocalHost");

app.UseAuthorization();

// Seed data during application startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    TodoSeeder.Seed(services);
}

app.MapControllers();

app.Run();
