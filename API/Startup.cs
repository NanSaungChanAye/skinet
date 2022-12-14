using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using AutoMapper;
using API.Helpers;
using API.Middleware;
using API.Errors;
using Microsoft.OpenApi.Models;
using API.Extensions;
using StackExchange.Redis;
using Infrastructure.Identity;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config; 

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        //public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
           
            services.AddControllers();
            services.AddAutoMapper(typeof(MappingProfiles)); 
            services.AddDbContext<StoreContext>(x=> x.UseSqlite(_config.GetConnectionString("DefaultConnection")));
            services.AddDbContext<AppIdentityDbContext>(x=>
            {
                x.UseSqlite(_config.GetConnectionString("IdentityConnection"));
            });
            services.AddSingleton<IConnectionMultiplexer>(c=>{
               var configuration=ConfigurationOptions.Parse(_config.GetConnectionString("Redis"),true);
               return ConnectionMultiplexer.Connect(configuration); 
            });
            services.AddApplicationServices();
            services.AddIdentityServices(_config);
            services.AddSwaggerDocumentation();
            services.AddCors(opt=>
            {
                opt.AddPolicy("CorsPolicy",policy=>{
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseSwaggerDocumentation();

            if (env.IsDevelopment())
            {
                
                //app.UseDeveloperExceptionPage();
            }
            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseStaticFiles();//To Get Image From wwwroot Folder
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
