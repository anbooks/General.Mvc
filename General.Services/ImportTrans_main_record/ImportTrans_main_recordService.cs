using General.Core.Data;
using System;
using System.Collections.Generic;
using System.Text;
using General.Entities;
using System.Linq;
using General.Core.Librs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using General.Core;
using General.Services.ImportTrans_main_recordService;

namespace General.Services.ImportTrans_main_record
{

    public class ImportTrans_main_recordService : IImportTrans_main_recordService
    {
        private IMemoryCache _memoryCache;  //不用每次去数据库里面筛选

        private const string MODEL_KEY = "General.services.importTrans_main_record";

        private IRepository<Entities.ImportTrans_main_record> _importTrans_main_recordRepository;


        public ImportTrans_main_recordService(IRepository<Entities.ImportTrans_main_record> importTrans_main_recordRepository,
            IMemoryCache memoryCache)
        {
            this._memoryCache = memoryCache;
            this._importTrans_main_recordRepository = importTrans_main_recordRepository;
        }
        public Entities.ImportTrans_main_record getById(int id)
        {
            return _importTrans_main_recordRepository.getById(id);
        }
        public void insertImportTransmain(Entities.ImportTrans_main_record model)
        {
            _importTrans_main_recordRepository.insert(model);
            _memoryCache.Remove(MODEL_KEY);
        }
        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="role"></param>
        public void updateImportTransmain(Entities.ImportTrans_main_record model)
        {
            var item = _importTrans_main_recordRepository.getById(model.Id);
            if (item == null)
                return;
             // item.Name = model.Name;
            //  item.CustomizedValue = model.CustomizedValue;
            //  item.Description = model.Description;
            //  item.ModifiedTime = model.ModifiedTime;
            //  item.Modifier = model.Modifier;
            _importTrans_main_recordRepository.update(item);
            _memoryCache.Remove(MODEL_KEY);
        }
        public IPagedList<Entities.ImportTrans_main_record> searchList(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.Incoterms!=null);
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.gjz))
                    query = query.Where(o => o.Incoterms.Contains(arg.gjz) || o.Invamou.Contains(arg.gjz));
          
            }
           // query = query.OrderBy(o => o.CustomizedClassify).ThenBy(o => o.CustomizedValue).ThenByDescending(o => o.CreationTime);
            return new PagedList<Entities.ImportTrans_main_record>(query, page, size);
        }
    }
}
