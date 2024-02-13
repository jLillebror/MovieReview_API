using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MovieReview_API.Data;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Filters;

namespace MovieReview_API.Configuration
{
    public class MovieReview_APICONFIG
    {
        private WebApplicationBuilder _builder;
        private WebApplication _app;
        public MovieReview_APICONFIG(string[] args)
        {
            _builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            ConfigureServices();

            _app = _builder.Build();

            ConfigureApp();
        }
        private void ConfigureDatabase()
        {
            var connectionString = _builder.Configuration.GetConnectionString("MovieReviewAPIContext");
            _builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
        }
        private void ConfigureServices()
        {
            ConfigureDatabase();

            _builder.Services.AddControllers().AddNewtonsoftJson(o =>
                    o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            ConfigureSwagger();

            ConfigureAuth();
        }
        

        private void ConfigureSwagger()
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            _builder.Services.AddEndpointsApiExplorer();
            _builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });
        }

        private void ConfigureAuth()
        {
            _builder.Services.AddAuthorization();

            _builder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
            {
                if (_builder.Environment.IsDevelopment())
                {
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 4;
                }
            }).AddEntityFrameworkStores<ApplicationDbContext>();
        }

        private void ConfigureApp()
        {
            // Configure the HTTP request pipeline.
            if (_app.Environment.IsDevelopment())
            {
                _app.UseSwagger();
                _app.UseSwaggerUI();
            }

            _app.UseHttpsRedirection();

            _app.UseAuthorization();

            _app.MapControllers();

            _app.MapIdentityApi<IdentityUser>();

            ConfigureCors();
        }

        private void ConfigureCors()
        {
            _app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        }

        public void Run()
        {
            _app.Run();
        }
    }
}