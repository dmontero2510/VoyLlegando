using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

using VoyLlegando.Application.Interfaces;
using VoyLlegando.Application.Services;

using VoyLlegando.Infrastructure.Database;
using VoyLlegando.Infrastructure.Repositories;
using VoyLlegando.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------------------
// BASE DE DATOS
// -------------------------------------------------------

builder.Services.AddSingleton(sp =>
{
    var connectionString =
        builder.Configuration.GetConnectionString("DefaultConnection");

    return new DbConnectionFactory(connectionString!);
});

// -------------------------------------------------------
// CONTROLLERS
// -------------------------------------------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// -------------------------------------------------------
// SWAGGER
// -------------------------------------------------------

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "VoyLlegando API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegue únicamente el token JWT."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// -------------------------------------------------------
// DEPENDENCIAS
// -------------------------------------------------------

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IViajeRepository, ViajeRepository>();

builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ViajeService>();
builder.Services.AddScoped<IViajeEventoRepository, ViajeEventoRepository>();

builder.Services.AddScoped<IProductorRepository, ProductorRepository>();
builder.Services.AddScoped<ICampoRepository, CampoRepository>();

builder.Services.AddScoped<ITipoIvaRepository, TipoIvaRepository>();

builder.Services.AddScoped<IPlantaRepository,PlantaRepository>();

builder.Services.AddScoped<IDestinoRepository,DestinoRepository>();

builder.Services.AddScoped<ICerealRepository,CerealRepository>();

builder.Services.AddScoped<ILogisticaRepository,LogisticaRepository>();

// -------------------------------------------------------
// MAPAS
// -------------------------------------------------------

builder.Services.AddHttpClient<RutaService>(client =>
{
client.BaseAddress =
new Uri("https://router.project-osrm.org/");
});

// -------------------------------------------------------
// JWT
// -------------------------------------------------------

var jwt = builder.Configuration.GetSection("Jwt");

var jwtKey = jwt["Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException(
        "No está configurada Jwt:Key.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwt["Issuer"],
                ValidAudience = jwt["Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),

                ClockSkew = TimeSpan.Zero
            };
    });

// -------------------------------------------------------
// AUTORIZACIÓN
// -------------------------------------------------------

builder.Services.AddAuthorization();

// -------------------------------------------------------
// APP
// -------------------------------------------------------

var app = builder.Build();

// -------------------------------------------------------
// SWAGGER
// -------------------------------------------------------

app.UseSwagger();
app.UseSwaggerUI();

// -------------------------------------------------------
// HTTPS
// -------------------------------------------------------

app.UseHttpsRedirection();

// -------------------------------------------------------
// ARCHIVOS ESTATICOS
// -------------------------------------------------------

app.UseDefaultFiles();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.CacheControl =
            "no-store, no-cache, must-revalidate, max-age=0";

        ctx.Context.Response.Headers.Pragma =
            "no-cache";

        ctx.Context.Response.Headers.Expires =
            "0";
    }
});
// -------------------------------------------------------
// AUTENTICACIÓN / AUTORIZACIÓN
// -------------------------------------------------------

app.UseAuthentication();
app.UseAuthorization();

// -------------------------------------------------------
// CONTROLLERS
// -------------------------------------------------------

app.MapControllers();

app.Run();