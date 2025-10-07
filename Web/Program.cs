using Application.Mediator;
using Domain.Statics;
using IOC;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using Web.Configuration;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .CreateBootstrapLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    // SiteTools
    builder.Configuration.GetSection("SiteTools").Get<SiteTools>();

    #region Data protection

    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/KeyDirectory/")))
        .SetApplicationName(SiteTools.SiteName)
        .SetDefaultKeyLifetime(TimeSpan.FromMinutes(int.Parse(builder.Configuration.GetSection("MachineKeyLifetime").Value)));

    #endregion

    builder.ConfigAuthentication();

    var connectionString = builder.Configuration.GetConnectionString("LocalConnectionString");
    DependencyContainer.RegisterDependencies(builder.Services, connectionString);

    //mediatR
    builder.Services.AddMediatR(config =>
    {
        config.RegisterServicesFromAssembly(typeof(IMediatorHandler).Assembly);
    });

    builder.Services.AddEasyCaching(options =>
    {
        options.UseInMemory();
    });

    builder.Logging.ClearProviders();

    #region Logger 
    builder.Host.UseSerilog((ctx, lc) =>
    lc.ReadFrom.Configuration(ctx.Configuration));
    #endregion

    builder.Services.AddResponseCompression();

    var app = builder.Build();

    app.UseResponseCompression();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                if (exceptionHandlerPathFeature?.Error != null)
                {
                    // Log the error
                    Log.Error(exceptionHandlerPathFeature.Error.Message);
                }
                context.Response.Redirect("/500");
            });
        });
        app.UseHsts();
    }

    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000,immutable");
        }
    });

    app.Use(async (context, next) =>
    {
        await next();
        switch (context.Response.StatusCode)
        {
            case 404:
                context.Request.Path = "/404";
                await next();
                break;

            case 403:
                context.Request.Path = "/403";
                await next();
                break;

            default:
                break;
        }
    });

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception? ex)
{
    Log.Error(ex.Message);
}
finally
{
    await Log.CloseAndFlushAsync();
}