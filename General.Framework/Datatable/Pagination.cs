using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace General.Framework.Datatable
{
    /// <summary>
    /// 
    /// </summary>
    public class Pagination : Pagination<dynamic>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="totalPages"></param>
        /// <param name="hasPreviousPage"></param>
        /// <param name="hasNextPage"></param>
        /// <param name="routeName"></param>
        /// <param name="param"></param>
        public Pagination(int pageIndex, int pageSize, int totalCount, int totalPages, bool hasPreviousPage, bool hasNextPage, string routeName, object param = null) :
            base(pageIndex, pageSize, totalCount, totalPages, hasPreviousPage, hasNextPage, routeName, param)
        {

        }
    }

    public class Pagination<A> where A : class
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="totalPages"></param>
        /// <param name="hasPreviousPage"></param>
        /// <param name="hasNextPage"></param>
        /// <param name="routeName"></param>
        /// <param name="param"></param>
        public Pagination(int pageIndex, int pageSize, int totalCount, int totalPages, bool hasPreviousPage, bool hasNextPage, string routeName, A param = null)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.TotalCount = totalCount;
            this.TotalPages = totalPages;
            this.HasPreviousPage = hasPreviousPage;
            this.HasNextPage = hasNextPage;
            this.RouteName = routeName;
            this.RouteArg = param;
        }

        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// 每页数
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// 总记录
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// 分页数
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPreviousPage { get; private set; }

        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNextPage { get; private set; }

        /// <summary>
        /// 路由名称
        /// </summary>
        public string RouteName { get; }

        /// <summary>
        /// 查询路由参数
        /// </summary>
        public A RouteArg { get; }

    }

}
