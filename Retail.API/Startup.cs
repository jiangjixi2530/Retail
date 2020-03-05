using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Retail.Util;
using Retail.Util.Extend;
using System;
using System.IO;

namespace Retail.API
{
    public class Startup
    {
        ILoggerRepository repository;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            repository = LogManager.CreateRepository("RetailLog");
            BasicConfigurator.Configure(repository);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                //加入返回结果处理
                options.Filters.Add<ApiResultFilterAttribute>();
                //加入企业信息过滤
                options.Filters.Add<CompanyAuthAttribute>();

            }).AddJsonOptions(configure =>
            {
                configure.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
            });
            AppSetting.DbConnectionString = Configuration.GetConnectionString("RetailConnection");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
