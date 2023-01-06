using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using HostedServices = Microsoft.Extensions.Hosting;
using Confluent.Kafka;
using ShippingService;
using ShippingService.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var host = Environment.GetEnvironmentVariable("DATABASE_HOST");
var user = Environment.GetEnvironmentVariable("DATABASE_USER");
var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
var database = Environment.GetEnvironmentVariable("DATABASE_NAME");

var connectionString = $"Host={host};Database={database};Username={user};Password={password}";


var kafka_host = Environment.GetEnvironmentVariable("KAFKA_HOST");
//"localhost:9092, localhost:9093, localhost:9094"
builder.Services.AddSingleton<HostedServices.IHostedService, UpdateDatabaseService>();
var consumerConfig = new ConsumerConfig()
{
    GroupId="update-service-group",
    EnableAutoCommit=false,
    BootstrapServers= kafka_host,
    AutoOffsetReset= AutoOffsetReset.Earliest
};
builder.Services.AddSingleton<ConsumerConfig>(consumerConfig);


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
