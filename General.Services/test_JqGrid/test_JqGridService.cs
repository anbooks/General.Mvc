using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using General.Core;
using General.Core.Data;
using General.Entities;
namespace General.Services.test_JqGrid
{
    public class test_JqGridService : Itest_JqGridService
    {
        private IRepository<Entities.test_JqGrid> _test_JqGrid_recordRepository;

        public test_JqGridService(IRepository<Entities.test_JqGrid> test_JqGrid_recordRepository  )
        {
          
            this._test_JqGrid_recordRepository = test_JqGrid_recordRepository;
        }

        public List<Entities.test_JqGrid> getAlltest_JqGrid()
        {
            List<Entities.test_JqGrid> list = null;
          
            if (list != null)
                return list;
            list = _test_JqGrid_recordRepository.Table.ToList();
            return list;
        }


        IPagedList<Entities.test_JqGrid> Itest_JqGridService.searchList(test_JqGridSearchArg arg, int page, int size)
        {
            var query = _test_JqGrid_recordRepository.Table.Where(o => o.Name != null);
            if (arg != null)
            {
               
            }
            // query = query.OrderBy(o => o.CustomizedClassify).ThenBy(o => o.CustomizedValue).ThenByDescending(o => o.CreationTime);
            return new PagedList<Entities.test_JqGrid>(query, page, size);
        }
    }
}
