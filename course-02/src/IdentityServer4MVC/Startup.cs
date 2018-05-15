// Copyright (c) Jeffcky <see cref="http://www.cnblogs.com/createmyself"/> All rights reserved.
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json.Serialization;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace IdentityServer4MVC
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
            services.AddMvc()
               .AddJsonOptions(opts =>
               {
                   // Force Camel Case to JSON
                   opts.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
               });

            //AspNetCore.Identity
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOpenIdConnect("oidc", options =>
            {
                options.AuthenticationMethod = OpenIdConnectRedirectBehavior.FormPost;
                options.ClientId = "70F3D0E7-1655-4727-827D-36D21C25A955";
                options.Authority = "http://localhost:5000";
                options.ClientSecret = "5CDCCB776008457BBECC7CDE93BE4AA5";
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                options.TokenValidationParameters.NameClaimType = JwtClaimTypes.Name;
                options.TokenValidationParameters.RoleClaimType = JwtClaimTypes.Role;
                options.SaveTokens = true;
                options.Scope.Add("customMVCAPI.read");
                options.Scope.Add("offline_access");
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = false;
                options.Events = new OpenIdConnectEvents()
                {
                    OnAuthenticationFailed = (context) =>
                    {
                        return Task.CompletedTask;
                    },
                    OnRemoteFailure = (context) =>
                    {
                        return Task.CompletedTask;
                    }
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
