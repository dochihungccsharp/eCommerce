using eCommerce.Shared.Extensions;
using eCommerce.Shared.Serilog;
using eCommerce.WebAPI.Extensions;
using eCommerce.WebAPI.Middlewares;
using Hangfire;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(SeriLogger.Configure);

Log.Information("Start server web api eCommerce");

try
{
    builder.Services.AddCors();
    // Add services to the container.
    builder.Services.AddControllerService();

    builder.Services.AddAuthenticationService(builder.Configuration);
    
    // add swagger service
    builder.Services.AddSwaggerService();
    
    builder.Services.AddSingleton(builder.Configuration);
    
    builder.Services.AddConfigurationSettings(builder.Configuration);
    
    // add  service handler jwt token middleware
    builder.Services.AddTransient<HandleJwtTokenMiddleware>();
    
    // add service handler exception token middleware
    builder.Services.AddTransient<HandleExceptionMiddleware>();
    
    // add all service eCommerce.Infrastructure
    builder.Services.AddInfrastructureService(builder.Configuration);
    
    // add all service eCommerce.Model
    builder.Services.AddModelService(builder.Configuration);
    
    // add all service eCommerce.Service
    builder.Services.AddService(builder.Configuration);
    
    // add service get user from request
    builder.Services.AddUserContextModelService(builder.Configuration);
    
    // add service hangfire
    //builder.Services.AddHangfire(builder);

    var app = builder.Build();
    
    app.UseCors(builder => builder
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
    
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseStaticFiles();
    
    app.UseJwtTokenMiddleware();
    
    app.UseExceptionMiddleware();

    app.UseHttpsRedirection();
    
    app.UseAuthorization();
    
    // app.UseHangfireDashboard();
    //
    // app.UseHangfireServer();
    //
    // var path = Path.Combine(builder.Environment.WebRootPath, "upload");
    // RecurringJob.AddOrUpdate(() => ImageExtensions.DeleteFilesInDirectory(path), "0 0 */12 * *");
    
    
    
    app.MapControllers();
    
    app.Run();
}
catch (Exception exception)
{
    // Unhandled exception: chưa xử lý exception
    Log.Fatal(exception, "Unhandled exception");
}
finally
{
    Log.Information("Shut dow web api eCommerce complete");
    Log.CloseAndFlush();
}