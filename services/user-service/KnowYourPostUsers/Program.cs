using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KnowYourPostUsers.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var host = Environment.GetEnvironmentVariable("DATABASE_HOST");
var user = Environment.GetEnvironmentVariable("DATABASE_USER");
var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
var database = Environment.GetEnvironmentVariable("DATABASE_NAME");

var connectionString = $"Host={host};Database={database};Username={user};Password={password}";
// Add services to the container.
builder.Services.AddDbContext<UserContext>(ops => 
    ops.UseNpgsql(connectionString));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<UserContext>();
    context.Database.EnsureCreated();
}

app.MapPost("/user", (UserVM user, UserContext context) =>
{
    context.Users.Add(new User(user.Email, user.Password));
    context.SaveChanges();
    return Results.Ok();
});

app.MapGet("/user", (UserContext context) => 
    context.Users.ToList());

app.MapGet("/user/{id}", (int id, UserContext context) =>
{
    var user = context.Users.Find(id);
    return user == null ? Results.NotFound() : Results.Ok(user);
});

app.MapPost("/user/exists", (UserVM user, UserContext context) =>
{
    var exists = context.Users.Any(u => u.Email == user.Email && u.Password == user.Password);
    return Results.Ok(exists);
});

app.MapDelete("/user/{id}", (int id, UserContext context) =>
{
    var user = context.Users.Find(id);
    if (user == null) return Results.NotFound();
    context.Users.Remove(user);
    context.SaveChanges();
    return Results.Ok();
});

app.Run();


class UserVM
{
    [JsonPropertyName("email")] public string Email { get; set; }
    [JsonPropertyName("password")] public string Password { get; set; }
}