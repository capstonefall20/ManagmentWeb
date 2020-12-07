using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagmentWeb.Models.Entity;
using ManagmentWeb.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.Extensions.Logging;
using ManagmentWeb.Repository.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.CookiePolicy;
using ManagmentWeb.Models;
using Microsoft.Owin;


namespace ManagmentWeb
{
    public class Startup
    {
        public IHostingEnvironment _hostingEnvironment { get; }
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
               //.AddServiceFabricConfiguration(serviceContext)
               .AddEnvironmentVariables();
            Configuration = builder.Build();
            _hostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
         
            // Add framework services.      
            services.AddOptions();
            AppSetting.SecureAppUrl = Configuration["profiles:" + _hostingEnvironment.EnvironmentName.ToLower() + ":SecureAppUrl"];
            AppSetting.ConnectionString = Configuration["profiles:" + _hostingEnvironment.EnvironmentName.ToLower() + ":ConnectionStrings:DefaultConnection"];
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(AppSetting.ConnectionString);
            });

            services.AddScoped<IPasswordHasher<ApplicationUser>, SQLPasswordHasher>();
            //services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.AllowedUserNameCharacters = "()#abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ $%^*!`~^&=[]{}\"';:,<.>?";
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddScoped<IRepository, RepositoryService>();
            services.AddHttpContextAccessor();
            //ConfigureJwtAuth(services);
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie();
            //.AddCookie(options =>
            //{
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.SecurePolicy = _hostingEnvironment.IsDevelopment()
            //      ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
            //    options.Cookie.SameSite = SameSiteMode.Lax;
            //});
            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    options.MinimumSameSitePolicy = SameSiteMode.Strict;
            //    options.HttpOnly = HttpOnlyPolicy.None;
            //    options.Secure = _hostingEnvironment.IsDevelopment()
            //      ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
            //});
            services.AddControllersWithViews();
            //  services.AddMvc(options => options.EnableEndpointRouting = false);

            #region "JWT Token For Authentication Login"    
            SiteKeys.Configure(Configuration.GetSection("AppSettings"));
            var key = Encoding.ASCII.GetBytes(SiteKeys.Token);

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
            });


            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(token =>
             {
                 token.RequireHttpsMetadata = false;
                 token.SaveToken = true;
                 token.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(key),
                     ValidateIssuer = true,
                     ValidIssuer = SiteKeys.WebSiteDomain,
                     ValidateAudience = true,
                     ValidAudience = SiteKeys.WebSiteDomain,
                     RequireExpirationTime = true,
                     ValidateLifetime = true,
                     ClockSkew = TimeSpan.Zero
                 };
             });

            #endregion
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
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
           // loggerFactory.AddConsole(Configuration.GetSection("Logging"));
          //  loggerFactory.AddDebug();
            app.UseHttpsRedirection();

           
            
            app.UseCookiePolicy();
            
            app.UseAuthentication();
            app.UseRouting();
            #region "JWT Token For Authentication Login"    

            app.UseCookiePolicy();
            app.UseSession();
            app.Use(async (context, next) =>
            {
                var JWToken = context.Session.GetString("JWToken");
                if (!string.IsNullOrEmpty(JWToken))
                {
                    context.Request.Headers.Add("Authorization", "Bearer " + JWToken);
                }
                await next();
            });
            app.UseAuthentication();
            app.UseAuthorization();


            #endregion
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<SignalRChatHub>("/signalRChatHub");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=login}");
            });
        }
        private void ConfigureJwtAuth(IServiceCollection services)
        {
            //services.AddAuthorization(o =>
            //{
            //    o.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
            //        .RequireAuthenticatedUser()
            //        .Build();
            //    o.AddPolicy("TrialUserRestriction", policy => policy.Requirements.Add(new TrialUserRestriction("Trial User")));
            //});
            services.AddAuthorization(o =>
            {
                o.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                   .RequireAuthenticatedUser()
                   .Build();
                //o.AddPolicy("TrialUserRestriction", policy =>
                //{
                //    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                //    policy.RequireAuthenticatedUser();
                //    policy.Requirements.Add(new RoleRequirement("Trial User"));
                //});
                //o.AddPolicy("StandardUserRestriction", policy =>
                //{
                //    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                //    policy.RequireAuthenticatedUser();
                //    policy.Requirements.Add(new RoleRequirement("User"));
                //});
                //o.AddPolicy("Over21Only",
                //                policy => policy.Requirements.Add(new MinimumAgeRequirement(21)));
            });

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    // The signing key must match!
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AuthOptions:Secret").Value)),

                    // Validate the JWT Issuer (iss) claim
                    ValidateIssuer = true,
                    ValidIssuer = Configuration.GetSection("AuthOptions:Issuer").Value,

                    // Validate the JWT Audience (aud) claim
                    ValidateAudience = true,
                    ValidAudience = Configuration.GetSection("AuthOptions:Audience").Value,

                    // Validate the token expiry
                    ValidateLifetime = true,
                    // If you want to allow a certain amount of clock drift, set that here:
                    ClockSkew = TimeSpan.Zero
                };

               
            });

          //  services.AddSingleton<IAuthorizationHandler, UserAccessHandler>();
        }
    }
}
