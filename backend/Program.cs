using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using backend.Data;

// ========================================================
// Configuração principal da aplicação .NET 8 Web API
// Sistema de Controle de Gastos Residenciais
// ========================================================

var builder = WebApplication.CreateBuilder(args);

// ========================================================
// Configuração da porta do servidor
// A API será executada na porta 5062
// ========================================================
builder.WebHost.UseUrls("http://localhost:5062");

// ========================================================
// Configuração do Entity Framework Core com SQLite
// O banco de dados será criado como um arquivo local "gastos.db"
// SQLite é ideal para aplicações de pequeno/médio porte
// ========================================================
builder.Services.AddDbContext<AppDbContext>(opcoes =>
    opcoes.UseSqlite("Data Source=gastos.db"));

// ========================================================
// Configuração do CORS (Cross-Origin Resource Sharing)
// Permite que o frontend (em qualquer origem) acesse a API
// Em produção, deve-se restringir as origens permitidas
// ========================================================
builder.Services.AddCors(opcoes =>
{
    opcoes.AddDefaultPolicy(politica =>
    {
        politica.AllowAnyOrigin()    // Permite qualquer origem (desenvolvimento)
               .AllowAnyMethod()     // Permite qualquer método HTTP (GET, POST, DELETE, etc.)
               .AllowAnyHeader();    // Permite qualquer cabeçalho na requisição
    });
});

// ========================================================
// Configuração dos Controllers com serialização JSON
// - camelCase: converte propriedades C# (PascalCase) para camelCase no JSON
// - JsonStringEnumConverter: serializa enums como strings ao invés de números
//   Exemplo: "Despesa" ao invés de 0, "Receita" ao invés de 1
// ========================================================
builder.Services.AddControllers()
    .AddJsonOptions(opcoes =>
    {
        // Configura a serialização JSON para usar camelCase
        opcoes.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;

        // Converte enums para strings no JSON (ex: "Despesa" ao invés de 0)
        opcoes.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// ========================================================
// Configuração do Swagger/OpenAPI
// Gera documentação interativa da API automaticamente
// Acessível em: http://localhost:5062/swagger
// ========================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ========================================================
// Criação automática do banco de dados na inicialização
// Utiliza EnsureCreated() para criar o banco e as tabelas
// se eles ainda não existirem.
// Nota: Em produção, é recomendado usar migrações do EF Core.
// ========================================================
using (var escopo = app.Services.CreateScope())
{
    var contexto = escopo.ServiceProvider.GetRequiredService<AppDbContext>();
    contexto.Database.EnsureCreated();
}

// ========================================================
// Configuração do pipeline de middleware
// ========================================================

// Habilita o Swagger apenas em ambiente de desenvolvimento
// (pode ser habilitado em produção se necessário)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilita o CORS para permitir requisições do frontend
app.UseCors();

// Habilita o middleware de autorização (necessário para controllers)
app.UseAuthorization();

// Mapeia os controllers para as rotas definidas
app.MapControllers();

// ========================================================
// Inicia a aplicação
// A API estará disponível em: http://localhost:5062
// Swagger UI: http://localhost:5062/swagger
// ========================================================
app.Run();
