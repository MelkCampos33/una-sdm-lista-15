using Microsoft.EntityFrameworkCore;
using GreenDriveApi20260101.Data;

var builder = WebApplication.CreateBuilder(args);

// =============================================
// Configuração de Serviços
// =============================================

// Controllers
builder.Services.AddControllers();

// SQLite - Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
                     ?? "Data Source=GreenDrive.db"));

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "GreenDrive Smart Grid & Lifecycle API",
        Version = "v1",
        Description = "Ecossistema distribuído para gerenciamento do ciclo de vida de baterias de veículos elétricos. " +
                      "Controla telemetria, conformidade ambiental e reciclagem sustentável.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Centro Universitário UNA - Sistemas Distribuídos e Mobile",
            Email = "contato@una.edu.br"
        }
    });
    // Incluir comentários XML para documentação no Swagger
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// =============================================
// Pipeline de Middleware
// =============================================

// Garantir criação automática do banco de dados SQLite
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    Console.WriteLine("[GreenDrive] Banco de dados SQLite inicializado com sucesso.");
}

// Swagger sempre ativo (útil para desenvolvimento e demonstração)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GreenDrive API v1");
    c.RoutePrefix = string.Empty; // Swagger na raiz "/"
    c.DocumentTitle = "GreenDrive Smart Grid API";
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("[GreenDrive] API iniciada. Acesse: http://localhost:5000");
Console.WriteLine("[GreenDrive] Swagger UI disponível em: http://localhost:5000/index.html");

app.Run();
