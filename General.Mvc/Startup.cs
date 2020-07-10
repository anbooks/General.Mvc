using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Core;
using General.Core.Data;
using General.Core.Extensions;
using General.Core.Librs;
using General.Entities;
using General.Services.Category;
using General.Services.Setting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace General.Mvc
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

            //services.AddDbContextPool<GeneralDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDbContext<GeneralDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            //services.AddDbContext<GeneralDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")),ServiceLifetime.Transient); //两个对象
            services.AddDbContextPool<GeneralDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            //services.AddAuthentication();   //权限过滤

            services.AddAuthentication("General").AddCookie(o =>
            {
                o.LoginPath = "/Admin/Login/index";

            });

            //程序集依赖注入
            //var assembly =RuntimeHelper.GetAssemblyByName("General.Services");

            //var types= assembly.GetTypes();


            //services.AddScoped<ICategoryService, CategoryService>();
            //services.AddScoped<ISettingService, SettingService>();  //也不能写100多个吧？

            //services.BuildServiceProvider().GetService<ICategoryService>();

            // var assembly= RuntimeHelper.GetAssemblyByName("General.Services");

            //  var types = assembly.GetTypes();
            //var list =  types.Where(o => o.IsClass && !o.IsAbstract && !o.IsGenericType).ToList();
            //  foreach (var type in list)
            //  {
            //    var interfacesList=  type.GetInterfaces();
            //      if (interfacesList.Any())
            //      {
            //          var inter = interfacesList.First();
            //          services.AddScoped(inter,type);
            //      }
            //  }


            //程序集依赖注入
            services.AddAssembly("General.Services");
            //services.AddAssembly("abc");

            //泛型注入到DI里面
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));


            EnginContext.Initialize(new GeneralEngine(services.BuildServiceProvider()));
            //new GeneralEngine(services.BuildServiceProvider());

            //泛型注入到DI里面
            //services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
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

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //      name: "areas",
            //      template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            //    );
            //});

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "areas",
                  template: "{area:exists}/{controller=Login}/{action=Index}/{id?}"
                );
            });

        }
    }
}
