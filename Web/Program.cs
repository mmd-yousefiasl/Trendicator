using Application.Mediator;
using Domain.Statics;
using IOC;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using Web.Configuration;
using WebMarkupMin.AspNetCoreLatest;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // MVC
    builder.Services.AddControllersWithViews();

    // SiteTools
    builder.Configuration.GetSection("SiteTools").Get<SiteTools>();

    #region Data protection
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(
            Path.Combine(builder.Environment.ContentRootPath, "KeyDirectory")))
        .SetApplicationName(SiteTools.SiteName)
        .SetDefaultKeyLifetime(TimeSpan.FromMinutes(
            int.Parse(builder.Configuration.GetSection("MachineKeyLifetime").Value)));
    #endregion

    // Authentication
    builder.ConfigAuthentication();

    // Dependencies
    var connectionString = builder.Configuration.GetConnectionString("LocalConnectionString");
    DependencyContainer.RegisterDependencies(builder.Services, connectionString);

    // MediatR
    builder.Services.AddMediatR(config =>
    {
        config.RegisterServicesFromAssembly(typeof(IMediatorHandler).Assembly);
    });

    // Caching
    builder.Services.AddEasyCaching(options =>
    {
        options.UseInMemory();
    });

    // Logging
    builder.Logging.ClearProviders();
    builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

    // Compression
    builder.Services.AddResponseCompression();

    // HTML Minification
    builder.Services.AddWebMarkupMin(options =>
    {
        options.AllowMinificationInDevelopmentEnvironment = true;
        options.DisablePoweredByHttpHeaders = true;
    })
    .AddHtmlMinification(options =>
    {
        options.MinificationSettings.RemoveRedundantAttributes = false; // keep type="text", etc.
        options.MinificationSettings.RemoveOptionalEndTags = false;     // safer for <input>, <textarea>
        options.MinificationSettings.WhitespaceMinificationMode =
            WebMarkupMin.Core.WhitespaceMinificationMode.Safe;          // less aggressive
    });

    var app = builder.Build();

    // Compression + Minification
    app.UseResponseCompression();
    app.UseWebMarkupMin();

    // Error handling
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                if (exceptionHandlerPathFeature?.Error != null)
                {
                    Log.Error(exceptionHandlerPathFeature.Error.Message);
                }
                context.Response.Redirect("/500");
            });
        });
        app.UseHsts();
    }

    // HTTPS first
    app.UseHttpsRedirection();

    // Static files with cache headers
    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000,immutable");
        }
    });

    // Status code handling
    app.UseStatusCodePagesWithReExecute("/{0}");

    // Routing
    app.UseRouting();

    // Auth
    app.UseAuthentication();
    app.UseAuthorization();

    // Routes
    app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Log.Error(ex.Message);
}
finally
{
    await Log.CloseAndFlushAsync();
}