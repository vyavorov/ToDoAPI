using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ToDoAPI.Data;
using ToDoAPI.Services;
using ToDoAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ToDoAPI.Utilities;

var builder = WebApplication.CreateBuilder(args);

var secretKey = JwtTokenSettings.GetSecretKey(builder.Configuration);
var key = Encoding.UTF8.GetBytes(secretKey);

// Add services to the container.

//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("Default connection")));

var connectionString = builder.Configuration.GetConnectionString("Postgre");

// Parse DATABASE_URL environment variable for Heroku
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.LocalPath.TrimStart('/')};User Id={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPasswordHashService, PasswordHashService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    //options.AddPolicy("AllowLocalHost", builder =>
    //{
    //    builder.WithOrigins("http://localhost:5173", "https://todoappbyventsy-579eed981c0e.herokuapp.com")
    //        .AllowAnyMethod()
    //        .AllowAnyHeader();
    //});
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = JwtConfiguration.Issuer,
        ValidAudience = JwtConfiguration.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

var app = builder.Build();

app.UseRouting();

//app.UseCors("CustomCorsPolicy");
app.UseCors();

// Configure the HTTP request pipeline.
if (true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseAuthentication();

app.UseDeveloperExceptionPage();

app.UseAuthorization();

app.UseHttpsRedirection();
app.UseStaticFiles();

// Seed data during application startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    TodoSeeder.Seed(services);
}

app.MapControllers();

app.Run();
