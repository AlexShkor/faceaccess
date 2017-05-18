using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using faceaccess;
using faceaccess.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VueJsAspNetCoreSample.Documents;
using VueJsAspNetCoreSample.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.MongoDB;
using Microsoft.ProjectOxford.Face;
using Microsoft.IdentityModel.Tokens;

namespace VueJsAspNetCoreSample
{
  public class Startup
  {
    public Startup(IHostingEnvironment env)
    {
      var builder = new ConfigurationBuilder()
          .SetBasePath(env.ContentRootPath)
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
          .AddEnvironmentVariables();
      Configuration = builder.Build();
    }
    private readonly Dictionary<Guid, WebSocket> _sockets = new Dictionary<Guid, WebSocket>();

    public IConfigurationRoot Configuration { get; }
    public FileStream file { get; private set; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddIdentityWithMongoStores(Configuration.GetConnectionString("MongoDbConnection"))
          .AddDefaultTokenProviders();
      // Add framework services.
      services.AddMvc();
      // Mongo
      var db = new MongoDatabase(Configuration.GetConnectionString("MongoDbConnection"));
      services.AddSingleton(db);
      services.AddTransient<IEmailSender, AuthMessageSender>();
      services.AddTransient<ISmsSender, AuthMessageSender>();
      services.AddTransient<IPhotoCompressor, ImageServices>();
      services.AddTransient<IJwtGenerator, JwtService>();
      services.AddSingleton<IConfiguration>(Configuration);
      services.AddSingleton<IFaceServiceClient>(new FaceServiceClient(Setting.FaceClientSubscriptionKey));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      loggerFactory.AddConsole(Configuration.GetSection("Logging"));
      loggerFactory.AddDebug();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        //app.UseBrowserLink();
        app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions() { HotModuleReplacement = true });
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }
      app.UseDefaultFiles();
      app.UseStaticFiles();
      app.UseIdentity();
      app.UseWebSockets();
      app.UseJwtBearerAuthentication(new JwtBearerOptions
      {
        AutomaticAuthenticate = true,
        AutomaticChallenge = true,
        TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuer = true,
          ValidIssuer = Setting.ISSUER,
          ValidateAudience = true,
          ValidAudience = Setting.AUDIENCE,
          ValidateLifetime = true,
          IssuerSigningKey = Setting.GetSymmetricSecurityKey(),
          ValidateIssuerSigningKey = true,
        }
      });
      app.Map("/ws", builder =>
            {
              builder.Use(async (context, next) =>
              {
                if (context.WebSockets.IsWebSocketRequest)
                {
                  var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                  _sockets.Add(Guid.NewGuid(), webSocket);
                  await Echo(webSocket);
                  return;
                }
                await next();
              });
            });
      app.UseMvc(routes =>
      {
        routes.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}/{id?}");

        routes.MapSpaFallbackRoute(
            name: "spa-fallback",
            defaults: new { controller = "Home", action = "Index" });
      });
      app.UseCors(cpb => cpb.AllowCredentials().AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
      DatabaseIdentityInitialize(app.ApplicationServices).Wait();
    }

    private async Task Echo(WebSocket webSocket)
    {
      file = File.Create("movie.mov");
      byte[] buffer = new byte[1024 * 4];
      var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
      while (!result.CloseStatus.HasValue)
      {
        await file.WriteAsync(buffer, 0, buffer.Length, CancellationToken.None);
        await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
      }
      await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
      await file.FlushAsync();
      file.Dispose();
    }

    private async Task DatabaseIdentityInitialize(IServiceProvider serviceProvider)
    {
      UserManager<IdentityUser> userManager =
          serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

      string adminEmail = Setting.AdminCredentialsEmail;
      string password = Setting.AdminCredentialsPassword;
      if (await userManager.FindByNameAsync(adminEmail) == null)
      {
        IdentityUser admin = new IdentityUser { Email = adminEmail, UserName = adminEmail };
        IdentityResult result = await userManager.CreateAsync(admin, password);
        if (result.Succeeded)
        {
          await userManager.AddToRoleAsync(admin, "admin");
        }
      }
    }
  }
}
