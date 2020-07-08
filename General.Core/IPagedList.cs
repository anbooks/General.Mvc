using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Core
{
    public interface IPagedList<T>:IList<T>
    {
        /// <summary>
        /// 当前页
        /// </summary>
        int PageIndex { get; }

        /// <summary>
        /// 页条数。最高100条，最低10条
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// 总条数
        /// </summary>
        int TotalCount { get; }

        /// <summary>
        /// 总页数
        /// </summary>
        int TotalPages { get; }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        bool HasPreviousPage { get; }

        /// <summary>
        /// 是否有下一页
        /// </summary>
        bool HasNextPage { get; }
    }
}
