using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.Category
{
    public interface ICategoryService
    {
        /// <summary>
        /// 初始化保存方法
        /// </summary>
        /// <param name="list"></param>
        void initCategory(List<Entities.Category> list);

        /// <summary>
        /// 获取所有并缓存
        /// </summary>
        /// <returns></returns>
        List<Entities.Category> getAll();
    }
     
}
