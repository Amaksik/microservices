using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Confluent.Kafka;
using KnowYourPostTaxes;
using KnowYourPostTaxes.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using HostedServices = Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

var host = Environment.GetEnvironmentVariable("DATABASE_HOST");
var user = Environment.GetEnvironmentVariable("DATABASE_USER");
var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
var database = Environment.GetEnvironmentVariable("DATABASE_NAME");

var connectionString = $"server={host};database={database};username={user};password={password}";
Console.WriteLine(connectionString);
Console.WriteLine(connectionString);


var kafka_host = Environment.GetEnvironmentVariable("KAFKA_HOST");

var prodConfig = new ProducerConfig()
{
    BootstrapServers = kafka_host
};
builder.Services.AddSingleton<ProducerConfig>(prodConfig);

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

app.MapPost("api/tax-service/tax/new", (TaxVM tax, TaxContext context, ProducerConfig config) =>
{
    //kafka update
    var producer = new ProducerWrapper(config);
    var upd = JsonConvert.SerializeObject(tax.CountryName);
    producer.writeMessage(upd);
    Console.WriteLine(upd);
    context.Taxes.Add(new Tax(tax.CountryName, tax.TaxRate));
    context.SaveChanges();
    return Results.Ok();
});

app.MapGet("api/tax-service/tax", (TaxContext context) =>
{
    var random = new Random();
    if (random.Next(0, 100) < 50)
    {
        Console.WriteLine("!!! RANDOM ERROR OCCURED: TAX-SERVICE WILL RETURN CODE 500 !!!");
        throw new Exception("Something went wrong");
    }
    return Results.Ok(context.Taxes.Select(t => new { name = t.CountryName, tax = t.TaxRate }).ToList());
});

app.MapPost("api/tax-service/tax", (TaxVM tax, TaxContext context) =>
{
    var res = context.Taxes.Where(t => t.CountryName.ToLower() == tax.CountryName.ToLower()).FirstOrDefault();
    return res == null ? Results.NotFound() : Results.Ok(res);
});

app.MapPost("api/tax-service/tax/exists", (TaxVM tax, TaxContext context) =>
{
    var exists = context.Taxes.Any(t => t.CountryName == tax.CountryName && t.TaxRate == tax.TaxRate);
    return Results.Ok(exists);
});

app.MapDelete("api/tax-service/tax/{id}", (int id, TaxContext context) =>
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
