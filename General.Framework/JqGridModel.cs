using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Framework
{
    public class JqGridModel
    {
        /// <summary>
        /// Grids the data.
        /// </summary>
        /// <param name="page1">当前页数.</param>
        /// <param name="rows">每页显示数目.</param>
        /// <param name="total"></param>
        /// <param name="objects">集合对象</param>
        /// <returns>System.Object.</returns>
        public static object GridData(int page1, int rows, int total, IEnumerable<object> objects)
        {
            int pageSize = rows;
            var totalPages = (int)Math.Ceiling((float)total / pageSize);

            //可根据具体情况，实现排序。
            var jsonData = new
            {
                total = totalPages,
                page = page1,
                records = total,
                rows = objects.ToArray()
            };

            return jsonData;
        }

    }
}
