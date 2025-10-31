using Microsoft.EntityFrameworkCore;
using Back_end_harjoitustyö.Models;
using Back_end_harjoitustyö.Repositories;
using Back_end_harjoitustyö.Services;
using Back_end_harjoitustyö.Middleware;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>         // <-- Api-avain pakote pois päältä testausta varten
{
    /*
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Anna API-avain tähän muodossa: ABCD",
        Type = SecuritySchemeType.ApiKey,
        Name = "ApiKey",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                Scheme = "ApiKeyScheme",
                Name = "ApiKey",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
    */
});

builder.Services.AddDbContext<MessageAppContext>(options =>
    options.UseInMemoryDatabase("MessageAppTestDB")); // <-- Virtuaalinen tietokanta testausta varten

//builder.Services.AddDbContext<MessageAppContext>(options =>
//options.UseSqlServer(builder.Configuration.GetConnectionString("MessageAppDB"))); 

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();
//builder.Services.AddScoped<ApiKeyMiddleware>();

var app = builder.Build();

//app.UseMiddleware<ApiKeyMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
