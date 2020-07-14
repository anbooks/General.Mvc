using General.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Framework.Datatable
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="A"></typeparam>
    public class DataSourceResult<T> : DataSourceResult<T, dynamic> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageList"></param>
        /// <param name="routeName"></param>
        public DataSourceResult(IPagedList<T> pageList, string routeName) :
            base(pageList, routeName, null)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pagedList"></param>
        /// <param name="routeName"></param>
        /// <param name="param"></param>
        public DataSourceResult(IPagedList<T> pagedList,string routeName,object param) : base(pagedList, routeName, param)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="A"></typeparam>
    public class DataSourceResult<T, A> : DataSourceResult<T, A, dynamic> where T : class where A : class
    {
        public DataSourceResult(IPagedList<T> pageList, string routeName, A param) :
            base(pageList, routeName, param,null)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="A"></typeparam>
    public class DataSourceResult<T, A, M1> : DataSourceResult<T, A, M1, dynamic> where T : class where A : class where M1:class
    {
        public DataSourceResult(IPagedList<T> pageList, string routeName, A param,M1 m1) :
            base(pageList, routeName, param,m1,null)
        {

        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="A"></typeparam>
    public class DataSourceResult<T, A, M1, M2> : DataSourceResult<T, A, M1, M2, dynamic> where T : class where A : class where M1:class where M2:class
    {
        public DataSourceResult(IPagedList<T> pageList, string routeName, A param,M1 m1,M2 m2) :
            base(pageList, routeName, param,m1,m2,null)
        {

        }


    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="A"></typeparam>
    public class DataSourceResult<T, A, M1, M2, M3> where T : class where A : class where M1:class where M2:class where M3:class
    {
        public DataSourceResult(IPagedList<T> pageList, string routeName, A param,M1 m1,M2 m2,M3 m3)
        {
            this.Data = pageList;
            this.Paging = new Pagination(pageList.PageIndex, pageList.PageSize, pageList.TotalCount, pageList.TotalPages, pageList.HasPreviousPage, pageList.HasNextPage, routeName, param);
            this.Item1 = m1;
            this.Item2 = m2;
            this.Item3 = m3;
        }

        /// <summary>
        /// 数据
        /// </summary>
        public IPagedList<T> Data { get; }

        /// <summary>
        /// 分页参数
        /// </summary>
        public Pagination Paging { get; }

        /// <summary>
        /// 其它数据M1
        /// </summary>
        public M1 Item1 { get; }

        /// <summary>
        /// 其它数据M2
        /// </summary>
        public M2 Item2 { get; }

        /// <summary>
        /// 其它数据M3
        /// </summary>
        public M3 Item3 { get; }
    }


}
