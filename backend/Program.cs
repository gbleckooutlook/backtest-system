using Backend.Repositories;
using Backend.Services;

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

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
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
