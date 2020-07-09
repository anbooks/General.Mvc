using General.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace General.Mvc
{
    /// <summary>
    /// 
    /// </summary>
    public class GeneralEngine : IEngine
    {
        private IServiceProvider _serviceProvider;

        

        public GeneralEngine(IServiceProvider serviceProvider)
        {
            this._serviceProvider= serviceProvider;
        }

        /// <summary>
        /// 构建实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>() where T:class
        {
            return _serviceProvider.GetService<T>();
        }
    }
}
