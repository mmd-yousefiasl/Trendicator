using Application.Mediator;
using Domain.Statics;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using Web;
using Web.Configuration;
using Web.Logging;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllersWithViews();

    #region SiteTools
    builder.Configuration.GetSection("SiteTools").Get<SiteTools>();
    #endregion

    #region Data protection
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(
            Path.Combine(builder.Environment.ContentRootPath, "KeyDirectory")))
        .SetApplicationName(SiteTools.SiteName)
        .SetDefaultKeyLifetime(TimeSpan.FromDays(
            int.Parse(builder.Configuration.GetSection("MachineKeyLifetime").Value)));
    #endregion

    #region Authentication
    builder.ConfigAuthentication();
    #endregion

    #region Dependencies
    var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
    DependencyContainer.RegisterDependencies(builder.Services, connectionString);
    #endregion

    #region MediatR
    builder.Services.AddMediatR(config =>
    {
        config.RegisterServicesFromAssembly(typeof(IMediatorHandler).Assembly);
        config.LicenseKey = SiteTools.MediatorLicenceKey;
    });
    #endregion

    #region Caching
    builder.Services.AddEasyCaching(options =>
    {
        options.UseInMemory();
    });
    #endregion

    #region Logging
    builder.Logging.ClearProviders();
    builder.Host.UseSerilog((ctx, lc) => lc
        .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.Sink(new TelegramErrorSink()));
    #endregion

    #region Compression
    builder.Services.AddResponseCompression();
    #endregion

    var app = builder.Build();

    // Compression + Minification
    app.UseResponseCompression();

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