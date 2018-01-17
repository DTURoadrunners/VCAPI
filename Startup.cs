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
            
            // Enables the CORS middleware
            services.AddCors();
            
#region Database injections
            services.AddSingleton<DatabaseConnector>(new DatabaseConnector(Configuration["ConnectionStrings:Connection"]));
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
            // Gets the configuration for out JWT tokens.
            JWTOptions jwt = Configuration.GetSection("TokenSettings").Get<JWTOptions>();

            //Enables JWT authentication
            services.AddAuthentication(options =>
            {
                // Makes the default scheme to be checked JWT
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                // Should be set to true once the server is deployed with SSL certificates
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    // The key to use for signing tokens
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(jwt.secretKey)),

                    // No need to validate who issued the token when developing
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
