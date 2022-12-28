using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using KnowYourPostTaxes.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var host = Environment.GetEnvironmentVariable("DATABASE_HOST");
var user = Environment.GetEnvironmentVariable("DATABASE_USER");
var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
var database = Environment.GetEnvironmentVariable("DATABASE_NAME");

var connectionString = $"Host={host};Database={database};Username={user};Password={password}";
// Add services to the container.
builder.Services.AddDbContext<TaxContext>(ops => 
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

    var context = services.GetRequiredService<TaxContext>();
    context.Database.EnsureCreated();
}

app.MapPost("/tax/new", (TaxVM tax, TaxContext context) =>
{
    context.Taxes.Add(new Tax(tax.CountryName, tax.TaxRate));
    context.SaveChanges();
    return Results.Ok();
});

app.MapGet("/tax", (TaxContext context) => 
    context.Taxes.Select(t => new { name = t.CountryName, tax = t.TaxRate }).ToList());

app.MapPost("/tax", (TaxVM tax, TaxContext context) =>
{
    var res = context.Taxes.Where(t => t.CountryName.ToLower() == tax.CountryName.ToLower()).FirstOrDefault();
    return res == null ? Results.NotFound() : Results.Ok(res);
});

app.MapPost("/tax/exists", (TaxVM tax, TaxContext context) =>
{
    var exists = context.Taxes.Any(t => t.CountryName == tax.CountryName && t.TaxRate == tax.TaxRate);
    return Results.Ok(exists);
});

app.MapDelete("/tax/{id}", (int id, TaxContext context) =>
{
    var tax = context.Taxes.Find(id);
    if (tax == null) return Results.NotFound();
    context.Taxes.Remove(tax);
    context.SaveChanges();
    return Results.Ok();
});

app.Run();


class TaxVM
{
    [JsonPropertyName("country_name")] public string CountryName { get; set; }
    [JsonPropertyName("taxrate")] public decimal TaxRate { get; set; }
}
