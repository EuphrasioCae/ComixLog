using ComixLog.Models;
using ComixLog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

// Namespace principal do seu projeto
namespace ComixLog
{
    // Filtro personalizado para adicionar parâmetros no Swagger
    public class CustomParametersFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties == null)
            {
                schema.Properties = new Dictionary<string, OpenApiSchema>();
            }

            // Adicionar as propriedades Secret, Property ID e Scopes
            schema.Properties["Secret"] = new OpenApiSchema { Type = "string", Description = "Chave secreta para autenticação." };
            schema.Properties["PropertyID"] = new OpenApiSchema { Type = "string", Description = "ID da propriedade." };
            schema.Properties["Scopes"] = new OpenApiSchema { Type = "array", Items = new OpenApiSchema { Type = "string" }, Description = "Escopos disponíveis." };
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.Configure<ComixLogDatabaseSettings>(
                builder.Configuration.GetSection("ComixLogDatabase"));
            builder.Services.AddSingleton<ContainersService>();
            builder.Services.AddSingleton<UsersService>();
            builder.Services.AddSingleton<AllocationService>();

            builder.Services.AddControllers();

            // Add Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });

                // Adicionando o filtro CustomParametersFilter
                c.SchemaFilter<CustomParametersFilter>();
            });

            // Configuração da Autenticação JWT
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "yourIssuer", // Defina o issuer do token
                    ValidAudience = "yourAudience", // Defina o audience do token
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperSecureKey1234567890!@#$%^")) // Chave secreta com 32 caracteres
                };
            });

            // Adicionar Autorização baseada em papéis
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", policy =>
                    policy.RequireRole("Admin"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Adicionar Middleware de Autenticação e Autorização
            app.UseAuthentication(); // Certifique-se de que o middleware de autenticação está sendo utilizado
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
