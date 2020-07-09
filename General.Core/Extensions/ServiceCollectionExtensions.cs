using General.Core.Librs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Core.Extensions
{ /// <summary>
  /// IServiceCollection 容器的拓展类
  /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 程序集依赖注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblyName"></param>
        /// <param name="serviceLifetime"></param>
        public static void AddAssembly(this IServiceCollection services, string assemblyName, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services) + "为空");

            if (String.IsNullOrEmpty(assemblyName))
                throw new ArgumentNullException(nameof(assemblyName) + "为空");

            var assembly = RuntimeHelper.GetAssemblyByName(assemblyName);

            if (assembly == null)
                throw new DllNotFoundException(nameof(assembly) + ".dll不存在");

            var types = assembly.GetTypes();
            var list = types.Where(o => o.IsClass && !o.IsAbstract && !o.IsGenericType).ToList();
            if (list == null && !list.Any())
                return;
            foreach (var type in list)
            {
                var interfacesList = type.GetInterfaces();
                if (interfacesList == null || !interfacesList.Any())
                    continue;
                var inter = interfacesList.First();
                switch (serviceLifetime)
                {
                    case ServiceLifetime.Scoped:
                        services.AddScoped(inter, type);
                        break;
                    case ServiceLifetime.Singleton:
                        services.AddScoped(inter, type);
                        break;
                    case ServiceLifetime.Transient:
                        services.AddTransient(inter, type);
                        break;
                }
            }
        }



    }

}
