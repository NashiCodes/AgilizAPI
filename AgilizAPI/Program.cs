#region

using AgilizAPI.Data;
using AgilizAPI.Security;

#endregion

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRepositories(builder.Configuration);

builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.ConfigureClaims();

//get user secrets variables
var configuration = builder.Configuration;
// add user secrets variables to environment variables
Environment.SetEnvironmentVariable("RapidKey", configuration.GetValue<string>("RapidApiKey"));
Environment.SetEnvironmentVariable("PRIVATE_KEY", configuration.GetValue<string>("PrivateKey"));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

await app.Services.InitializeDb();

// Configure the HTTP request pipeline.
app.UseSwagger();
if (app.Environment.IsDevelopment()) app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseJWT();

app.MapControllers();

app.Run();