using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using General.Core;
using General.Framework.Datatable;

namespace General.Framework.Datatable
{
    /// <summary>
    /// 
    /// </summary>
    public static class IPageListExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageList"></param>
        /// <param name="routeName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataSourceResult<T> toDataSourceResult<T>(this IPagedList<T> pageList, dynamic param = null) where T : class
        {
            return new DataSourceResult<T>(pageList, null, param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageList"></param>
        /// <param name="routeName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataSourceResult<T> toDataSourceResult<T>(this IPagedList<T> pageList, string routeName, dynamic param = null) where T : class  
        {
            return new DataSourceResult<T>(pageList, routeName, param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageList"></param>
        /// <param name="routeName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataSourceResult<T,A> toDataSourceResult<T,A>(this IPagedList<T> pageList, string routeName, A param) where T : class where A:class
        {
            return new DataSourceResult<T,A>(pageList, routeName, param);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageList"></param>
        /// <param name="routeName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataSourceResult<T, A,M1> toDataSourceResult<T, A,M1>(this IPagedList<T> pageList, string routeName, A param,M1 m1) where T : class where A : class where M1:class
        {
            return new DataSourceResult<T, A,M1>(pageList, routeName, param,m1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageList"></param>
        /// <param name="routeName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataSourceResult<T, A, M1,M2> toDataSourceResult<T, A, M1,M2>(this IPagedList<T> pageList, string routeName, A param, M1 m1,M2 m2) 
            where T : class where A : class where M1 : class where M2:class
        {
            return new DataSourceResult<T, A, M1,M2>(pageList, routeName, param, m1,m2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageList"></param>
        /// <param name="routeName"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataSourceResult<T, A, M1, M2,M3> toDataSourceResult<T, A, M1, M2,M3>(this IPagedList<T> pageList, string routeName, A param, M1 m1, M2 m2,M3 m3)
            where T : class where A : class where M1 : class where M2 : class where M3:class
        {
            return new DataSourceResult<T, A, M1, M2,M3>(pageList, routeName, param, m1, m2,m3);
        }


        /// <summary>
        /// app json格式数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageList"></param>
        /// <returns></returns>
        public static JsonSourceResult<T> toJsonSourceResult<T>(this IPagedList<T> pageList) where T : class
        {
            return new JsonSourceResult<T>(pageList);
        }
    }
}
