using Backend.Repositories;
using Backend.Services;
using Backend.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar serviços e repositórios
builder.Services.AddScoped<AtivoRepository>();
builder.Services.AddScoped<AtivoService>();
builder.Services.AddScoped<DayTradeRepository>();
builder.Services.AddScoped<DayTradeService>();
builder.Services.AddScoped<TradeRepository>();
builder.Services.AddScoped<TradeService>();
builder.Services.AddScoped<BacktestRepository>();
builder.Services.AddScoped<BacktestService>();
builder.Services.AddScoped<CandleRepository>();

// Serviços de análise de backtest
builder.Services.AddScoped<BacktestAnalyzer>();
builder.Services.AddSingleton<ITaxaCalculator, ZeroTaxaCalculator>();

// Serviço em background para processamento de backtests
builder.Services.AddHostedService<BacktestProcessorService>();

// Serviço em background para atualização de IP no DuckDNS
builder.Services.AddHttpClient();
builder.Services.AddHostedService<DuckDnsUpdaterService>();

// Configure CORS
var allowedOrigins = new List<string>
{
    "http://localhost:3000",
    "http://localhost:3001",
    "http://localhost:5173"
};

// Adicionar domínio público se configurado
var publicDomain = builder.Configuration["PUBLIC_DOMAIN"];
var frontendPort = builder.Configuration["FRONTEND_PORT"];
if (!string.IsNullOrEmpty(publicDomain))
{
    allowedOrigins.Add($"http://{publicDomain}:{frontendPort ?? "5173"}");
    allowedOrigins.Add($"https://{publicDomain}:{frontendPort ?? "5173"}");
    // Também permitir sem porta (porta 80/443)
    allowedOrigins.Add($"http://{publicDomain}");
    allowedOrigins.Add($"https://{publicDomain}");
}

Console.WriteLine($"[CORS] Origens permitidas: {string.Join(", ", allowedOrigins)}");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins.ToArray())
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Inicializar banco de dados
using (var scope = app.Services.CreateScope())
{
    var repository = scope.ServiceProvider.GetRequiredService<AtivoRepository>();
    await repository.InicializarBancoDadosAsync();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
