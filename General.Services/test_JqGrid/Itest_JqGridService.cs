using General.Core;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.test_JqGrid
{
   public interface Itest_JqGridService
    {
        List<Entities.test_JqGrid> getAlltest_JqGrid();



        /// <summary>
        /// 获得所有的列表展示
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        IPagedList<Entities.test_JqGrid> searchList(test_JqGridSearchArg arg, int page, int size);

        void updatetest_JqGrid(Entities.test_JqGrid model);


        void deletetest_JqGrid(int test_JqGrid);



    }
}
