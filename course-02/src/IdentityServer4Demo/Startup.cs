// Copyright (c) Jeffcky <see cref="http://www.cnblogs.com/createmyself"/> All rights reserved.
using IdentityServer4Demo.Extentions;
using IdentityServer4Demo.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace IdentityServer4Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = @"data source=WANGPENG;User Id=sa;Pwd=sa123;initial catalog=IdentityServer4Demo;integrated security=True;MultipleActiveResultSets=True;";
           
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddMvc();

            services.AddDbContext<IdentityServerDemoDbContext>(options => options.UseSqlServer(connectionString));

            services.AddIdentityServerDemoIdentity();

            //注入IdentityServer4使用AspNetIdentity
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddAspNetIdentity<IdentityServerDemoIdentityUser>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(@"data source=WANGPENG;User Id=sa;Pwd=sa123;initial catalog=IdentityServer4Demo.OAuth;integrated security=True;",
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(@"data source=WANGPENG;User Id=sa;Pwd=sa123;initial catalog=IdentityServer4Demo.OAuth;integrated security=True;",
                            sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 30;
                });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseStaticFiles();

            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
