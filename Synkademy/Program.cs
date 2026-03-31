using Microsoft.EntityFrameworkCore;
using Synkademy.Data;
using Synkademy.Services;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);
Env.Load(".env");
// Add services to the container.
var connectionString = $"server={Environment.GetEnvironmentVariable("DB_SERVER")};" +  $"port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                       $"database={Environment.GetEnvironmentVariable("DB_DATABASE")};" +
                       $"user={Environment.GetEnvironmentVariable("DB_USER")};" + $"password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);
Console.WriteLine(connectionString);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<PasswordService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();

app.Run();
