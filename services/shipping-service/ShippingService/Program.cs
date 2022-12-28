using Microsoft.EntityFrameworkCore;
using ShippingService.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var host = Environment.GetEnvironmentVariable("DATABASE_HOST");
var user = Environment.GetEnvironmentVariable("DATABASE_USER");
var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
var database = Environment.GetEnvironmentVariable("DATABASE_NAME");

var connectionString = $"Host={host};Database={database};Username={user};Password={password}";

builder.Services.AddControllers();
builder.Services.AddDbContext<shippingdbContext>(options => {
    options.UseNpgsql(connectionString);
});
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

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<shippingdbContext>();
    
    context.Database.Migrate();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
