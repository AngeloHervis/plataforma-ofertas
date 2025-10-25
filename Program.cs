using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using plataforma.ofertas.Dto.Agendamentos;
using plataforma.ofertas.Interfaces.Agendamentos;
using plataforma.ofertas.Interfaces.Jobs;
using plataforma.ofertas.Interfaces.Ofertas;
using plataforma.ofertas.Interfaces.Scrapers;
using plataforma.ofertas.Repositories;
using plataforma.ofertas.Services.Agendamentos;
using plataforma.ofertas.Services.Jobs;
using plataforma.ofertas.Services.Ofertas;
using plataforma.ofertas.Services.Scrapers;
using plataforma.ofertas.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("OrigemPermitida", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers(options => { options.ReturnHttpNotAcceptable = false; })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ofertas API",
        Version = "v1"
    });

    const string bearerConstant = "Bearer";
    c.AddSecurityDefinition(bearerConstant, new OpenApiSecurityScheme
    {
        Description = "Autorização via bearer token. Cole apenas o token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = bearerConstant
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = bearerConstant
                },
                Scheme = "oauth2",
                Name = bearerConstant,
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });

    c.CustomSchemaIds(x => x.FullName);
});

builder.Services.Configure<List<CronJobConfig>>(builder.Configuration.GetSection("CronJobs"));

builder.Services.AddSingleton<SupabaseContext>();
builder.Services.AddScoped<IOfertaRepository, OfertaRepository>();

builder.Services.AddScoped<IRunnableService, AtualizarOfertasJob>();

builder.Services.AddSingleton<IJobRegistry, JobRegistry>();
builder.Services.AddHostedService<CronWorker>();

builder.Services.AddScoped<IConsultaOfertasDoBancoService, ConsultaOfertasDoBancoService>();

builder.Services.AddScoped<IOfertaAgendadaRepository, OfertaAgendadaRepository>();

builder.Services.AddScoped<IAgendarOfertaService, AgendarOfertaService>();
builder.Services.AddScoped<IListarOfertasAgendadasService, ListarOfertasAgendadasService>();
builder.Services.AddScoped<IObterOfertaAgendadaDetalheService, ObterOfertaAgendadaDetalheService>();
builder.Services.AddScoped<IConsultaOfertaDetalheService, ConsultaOfertaDetalheService>();
builder.Services.AddScoped<IGerarLinkAfiliadoService, GerarLinkAfiliadoService>();
builder.Services.AddScoped<IAgendarEnvioWhatsappService, AgendarEnvioWhatsappService>();


builder.Services.AddHttpClient<ISendFlowActionsClient, SendFlowActionsClient>((sp, http) =>
    {
        var opts = sp.GetRequiredService<IOptions<SendFlowOptions>>().Value;
        http.BaseAddress = new Uri(opts.BaseUrl.TrimEnd('/') + "/");
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", opts.ApiKey);
    })
    .SetHandlerLifetime(TimeSpan.FromMinutes(10));

builder.Services.AddScoped<IAmazonScraperService, AmazonScraperService>();
builder.Services.AddScoped<IShopeeScraperService, ShopeeScraperService>();
builder.Services.AddScoped<IMercadoLivreScraperService, MercadoLivreScraperService>();
builder.Services.AddHttpClient<IPelandoScraperService, PelandoScraperService>();
builder.Services.AddHttpClient<IPromobitScraperService, PromobitScraperService>();

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

var app = builder.Build();

app.UseCors("OrigemPermitida");

if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ofertas API"); });
}

app.UseAuthorization();
app.MapControllers();

app.Run();