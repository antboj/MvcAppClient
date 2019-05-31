using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MvcApp
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
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookies")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "http://localhost:60087";
                    options.ResponseType = "code id_token";
                    options.RequireHttpsMetadata = false;
                    options.ClientId = "mvcApp";
                    options.ClientSecret = "secret";
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.RequireHttpsMetadata = false;
                    options.Scope.Add("abptenantid");
                    options.Scope.Add("deviceApi");

                    //options.Events.OnTokenValidated += OnTokenValidated;
                });

            services.AddHttpClient();

            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();//

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        /// <summary>
        /// Rucno izvrsimo kod koji se inace izvrsava kad se ukljuci GetClaimsFromUserInfoEndpoint
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        ////private async Task OnTokenValidated(TokenValidatedContext arg)
        ////{
        ////    var code = arg.ProtocolMessage.Code;
        ////    var discoClient = new DiscoveryClient("https://localhost:5001");
        ////    var discoResponse = await discoClient.GetAsync();
        ////    var tokenClient = new TokenClient(discoResponse.TokenEndpoint, "mvcApp", "secret");
        ////    var result = await tokenClient.RequestAuthorizationCodeAsync(code, "http://localhost:58177/signin-oidc");
        ////    var userInfoClient = new UserInfoClient(discoResponse.UserInfoEndpoint);
        ////    var userInfoResponse = await userInfoClient.GetAsync(result.AccessToken);
        ////    var identity = arg.Principal.Identity as ClaimsIdentity;
        ////    identity.AddClaims(userInfoResponse.Claims);
        ////    arg.Success();
        ////}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            //app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
