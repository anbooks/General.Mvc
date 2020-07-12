using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Core;
using General.Core.Data;
using General.Core.Extensions;
using General.Core.Librs;
using General.Entities;
using General.Framework;
using General.Framework.Infrastructure;
using General.Framework.Security.Admin;
using General.Services.Category;
using General.Services.Setting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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

            services.AddAuthentication(o=> {
                o.DefaultAuthenticateScheme = CookieAdminAuthInfo.AuthenticationScheme;
                o.DefaultChallengeScheme = CookieAdminAuthInfo.AuthenticationScheme;
                //o.DefaultAuthenticateScheme ="General";
                //o.DefaultChallengeScheme = "General";
               // o.DefaultSignInScheme= "General";
               // o.DefaultSignOutScheme= "General";
            }).AddCookie(CookieAdminAuthInfo.AuthenticationScheme, o =>
            {
               // o.LoginPath = "/Admin/Login/index";
                o.LoginPath = "/admin/login";

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

            services.AddSession();

            //泛型注入到DI里面
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));


            services.AddScoped<IWorkContext, WorkContext>();
            services.AddScoped<IAdminAuthService, AdminAuthService>();
            services.AddSingleton<IMemoryCache, MemoryCache>();

            EnginContext.Initialize(new GeneralEngine(services.BuildServiceProvider()));
            //new GeneralEngine(services.BuildServiceProvider());

            //泛型注入到DI里面
            //services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
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

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");   //默认路由
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
