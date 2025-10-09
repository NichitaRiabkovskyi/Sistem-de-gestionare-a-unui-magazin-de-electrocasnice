using MyApi.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
var app = builder.Build();

// Daca ai probleme cu HTTPS local, poți comenta UseHttpsRedirection() temporar
app.UseHttpsRedirection();

// Middleware pentru roluri - trebuie înainte de MapControllers
app.UseMiddleware<RoleMiddleware>();

app.MapControllers();
app.MapGet("/", () => "API-ul funcționează!");
app.Run();
