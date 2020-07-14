using General.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Framework.Datatable
{
    public class JsonSourceResult<T> where T : class
    {
        public JsonSourceResult(IPagedList<T> pageList)
        {
            this.data = pageList;
            this.counts = pageList.TotalCount;
            this.page = pageList.PageIndex;
            this.prev = pageList.HasPreviousPage;
            this.next = pageList.HasNextPage;
            this.totalpage = pageList.TotalPages;
            this.size = pageList.PageSize;
        }

        /// <summary>
        /// 数据
        /// </summary>
        public IPagedList<T> data { get; }

        /// <summary>
        /// 总条数
        /// </summary>
        public int counts { get; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// 有上一页
        /// </summary>
        public bool prev { get; set; }

        /// <summary>
        /// 有下一页
        /// </summary>
        public bool next { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int totalpage { get; }

        /// <summary>
        /// 页条数
        /// </summary>
        public int size { get; set; }
    }
}
