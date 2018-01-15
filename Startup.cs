using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VCAPI.Repository.Interfaces;
using VCAPI.Repository.MySQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using VCAPI.Options;

namespace VCAPI
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
            services.AddMvc();
            services.AddOptions();
            services.AddSingleton<DatabaseConnector>(new DatabaseConnector(Configuration["ConnectionStrings:Connection"]));
            services.AddCors();
            
#region Database injections
            services.AddTransient<IUserRepository, MySQLUserRepository>();
            services.AddTransient<IProjectRepository, MySQLProjectRepository>();
            services.AddTransient<IResourceAccess, MySQLResourceAccess>();
            services.AddTransient<ICategoryRepository, MySQLCategoryRepository>();
            services.AddTransient<IDocumentRepository, MySQLDocumentRepository>();
            services.AddTransient<IComponentRepository, MySQLComponentRepository>();
            services.AddTransient<IComponentTypeRepository, MySQLComponentTypeRepository>();

            services.Configure<JWTOptions>(Configuration.GetSection("TokenSettings"));
#endregion        
    
#region JWT

            JWTOptions jwt = Configuration.GetSection("TokenSettings").Get<JWTOptions>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwt.secretKey)),
                    ValidateIssuer = false
                };
                options.Audience = jwt.audience;
            });
#endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();

            app.UseMvc();
            app.UseCors(builder => builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod());
        }
    }
}
