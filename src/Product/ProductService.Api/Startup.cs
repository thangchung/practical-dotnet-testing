using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using N8T.Infrastructure;
using N8T.Infrastructure.Dapr;
using N8T.Infrastructure.EfCore;
using N8T.Infrastructure.Tye;
using ProductService.Infrastructure.Data;

namespace ProductService.Api
{
    public class Startup
    {
        public Startup(IConfiguration config, IWebHostEnvironment env)
        {
            Config = config;
            Env = env;
        }

        private IConfiguration Config { get; }
        private IWebHostEnvironment Env { get; }
        private bool IsRunOnTye => Config.IsRunOnTye();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCore(new[] {typeof(Application.Anchor)})
                .AddPostgresDbContext<MainDbContext, Infrastructure.Anchor>(
                    Config.GetConnectionString("postgres"),
                    svc => svc.AddScoped(typeof(IExRepository<>), typeof(ExRepository<>)))
                .AddCustomDaprClient()
                .AddControllers()
                .AddDapr();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}