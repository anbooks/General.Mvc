using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Core
{
    [Serializable]
    public class PagedList<T> : List<T>, IPagedList<T>
    {
        public PagedList()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        public PagedList(IQueryable<T> source, int pageIndex, int pageSize)
        {
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageSize = pageSize < 20 ? 20 : pageSize > 100 ? 100 : pageSize;
            this.TotalCount = source.Count();
            this.TotalPages = TotalCount / pageSize;

            if (TotalCount % pageSize > 0)
                this.TotalPages++;

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.AddRange(source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        public PagedList(IList<T> source, int pageIndex, int pageSize)
        {
            pageSize = pageSize < 10 ? 10 : pageSize > 100 ? 100 : pageSize;
            this.TotalCount = source.Count();
            this.TotalPages = TotalCount / pageSize;

            if (TotalCount % pageSize > 0)
                this.TotalPages++;

            this.PageSize = pageSize;
            this.PageIndex = pageIndex;
            this.AddRange(source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
        }

        public bool HasNextPage
        {
            get { return (PageIndex + 1 <= TotalPages); }
        }

        public bool HasPreviousPage
        {
            get { return (PageIndex > 1); }
        }

        public int PageIndex
        {
            get;
        }

        public int PageSize
        {
            get;
        }

        public int TotalCount
        {
            get;
        }

        public int TotalPages
        {
            get;
        }
    }
}
