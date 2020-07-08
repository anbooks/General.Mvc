using System;
using System.Collections.Generic;
using System.Text;

namespace General.Core
{
    public interface IEngine
    {
        /// <summary>
        /// 构建一个实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Resolve<T>() where T : class;
    }
}
