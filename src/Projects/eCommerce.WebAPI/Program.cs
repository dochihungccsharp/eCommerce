using eCommerce.WebAPI.Extensions;
using eCommerce.WebAPI.Middlewares;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Configure Swagger to use the JWT bearer authentication scheme
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securityScheme);

    // Make Swagger require a JWT token to access the endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            securityScheme,
            new string[] {}
        }
    });
});


builder.Services.AddTransient<HandleJwtTokenMiddleware>();
builder.Services.AddTransient<HandleExceptionMiddleware>();

builder.Services.AddSingleton(builder.Configuration);
builder.Services.AddConfigurationSettings(builder.Configuration);
builder.Services.AddInfrastructureService(builder.Configuration);
builder.Services.AddModelService(builder.Configuration);
builder.Services.AddUserContextModelService(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseJwtTokenMiddleware();

app.UseExceptionMiddleware();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();



app.Run();
