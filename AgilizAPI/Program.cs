#region

using AgilizAPI.Data;
using AgilizAPI.Security;

#endregion

var builder = WebApplication.CreateBuilder(args);

//Adiciona os repositorios ao container e linka o banco de dados
builder.Services.AddRepositories(builder.Configuration);

//configura o JWT e as claims
builder.Services.ConfigureJWT(builder.Configuration);
builder.Services.ConfigureClaims();

//Pega as configurações do appsettings.json e user secrets
var configuration = builder.Configuration;

// Adiciona as variaveis de configuração as variaveis de ambiente
Environment.SetEnvironmentVariable("RapidKey", configuration.GetValue<string>("RapidApiKey"));
Environment.SetEnvironmentVariable("PRIVATE_KEY", configuration.GetValue<string>("PrivateKey"));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Inicia o banco de dados e as migrations
await app.Services.InitializeDb().ConfigureAwait(false);

// Configure the HTTP request pipeline.
app.UseSwagger();
if (app.Environment.IsDevelopment()) app.UseSwaggerUI();

app.UseHttpsRedirection();

// Configura o CORS
app.UseJWT();

app.MapControllers();

app.Run();