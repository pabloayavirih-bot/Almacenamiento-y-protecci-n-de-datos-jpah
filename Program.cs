using FacciStats;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Configura ConnectionStrings:DefaultConnection con Secret Manager.");
builder.Services.AddDbContext<AppDbContext>(o => o.UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 36))));
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddScoped<ApiService>();
builder.Services.AddSingleton<CifradoService>();
var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.MapOpenApi(); app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "FacciStats v1")); }
app.UseHttpsRedirection(); app.MapControllers(); app.Run();
