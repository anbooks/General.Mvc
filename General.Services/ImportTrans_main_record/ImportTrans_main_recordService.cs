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
using General.Services.SysUser;

namespace General.Services.ImportTrans_main_record
{

    public class ImportTrans_main_recordService : IImportTrans_main_recordService
    {
        private IMemoryCache _memoryCache;  //不用每次去数据库里面筛选

        private const string MODEL_KEY = "General.services.importTrans_main_record";

        private IRepository<Entities.ImportTrans_main_record> _importTrans_main_recordRepository;

        private ISysUserService _sysUserService;
        public ImportTrans_main_recordService(ISysUserService sysUserService,IRepository<Entities.ImportTrans_main_record> importTrans_main_recordRepository,
            IMemoryCache memoryCache)
        {
            this._sysUserService = sysUserService;
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
            _importTrans_main_recordRepository.update(model);
            _memoryCache.Remove(MODEL_KEY); 
        }

        
        //CheckAndPass
        
       
       
        public IPagedList<Entities.ImportTrans_main_record> searchList(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.IsDeleted!=true);
            if (arg.invcurr != null||arg.itemno!=null || arg.keyword != null || arg.materiel != null || arg.pono != null || arg.realreceivingdatestrat != null || arg.realreceivingdateend != null || arg.shipper != null)
            {
                if (!String.IsNullOrEmpty(arg.itemno))
                    query = query.Where(o => o.Itemno.Contains(arg.itemno));
                if (!String.IsNullOrEmpty(arg.shipper))
                    query = query.Where(o => o.Shipper.Contains(arg.shipper));
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.PoNo.Contains(arg.pono));
                if (!String.IsNullOrEmpty(arg.invcurr))    
                    query = query.Where(o => o.Invcurr.Contains(arg.invcurr));
                if (arg.realreceivingdateend!=null&&arg.realreceivingdatestrat!=null)
                    query = query.Where(o => o.RealReceivingDate>arg.realreceivingdatestrat&&o.RealReceivingDate<arg.realreceivingdateend.Value.AddDays(1));
            }
            else {
                 query = query.Where(o => o.CreationTime.ToString("yyMMdd") == DateTime.Now.ToString("yyMMdd")); 
            }
            query = query.OrderByDescending(o => o.Id);
           // query = query.OrderBy(o => o.CustomizedClassify).ThenBy(o => o.CustomizedValue).ThenByDescending(o => o.CreationTime);
            return new PagedList<Entities.ImportTrans_main_record>(query, page, size);
        }
        public IPagedList<Entities.ImportTrans_main_record> searchListBuyer(SysCustomizedListSearchArg arg, int page, int size,string buyer)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.IsDeleted != true&&o.Buyer==buyer);
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.itemno))
                    query = query.Where(o => o.Itemno.Contains(arg.itemno));
                if (!String.IsNullOrEmpty(arg.shipper))
                    query = query.Where(o => o.Shipper.Contains(arg.shipper));
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.PoNo.Contains(arg.pono));
                if (!String.IsNullOrEmpty(arg.invcurr))
                    query = query.Where(o => o.Invcurr.Contains(arg.invcurr));
            }
            query = query.OrderByDescending(o => o.Id);
            // query = query.OrderBy(o => o.CustomizedClassify).ThenBy(o => o.CustomizedValue).ThenByDescending(o => o.CreationTime);
            return new PagedList<Entities.ImportTrans_main_record>(query, page, size);
        }
        public IPagedList<Entities.ImportTrans_main_record> searchListPortCustomerBroker(SysCustomizedListSearchArg arg, int page, int size,string forwarder)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.Forwarder== forwarder&& o.IsDeleted != true);
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.itemno))
                    query = query.Where(o => o.Itemno.Contains(arg.itemno));
                if (!String.IsNullOrEmpty(arg.shipper))
                    query = query.Where(o => o.Shipper.Contains(arg.shipper));
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.PoNo.Contains(arg.pono));
                if (!String.IsNullOrEmpty(arg.invcurr))
                    query = query.Where(o => o.Invcurr.Contains(arg.invcurr));
            }
            // query = query.OrderBy(o => o.CustomizedClassify).ThenBy(o => o.CustomizedValue).ThenByDescending(o => o.CreationTime);
            return new PagedList<Entities.ImportTrans_main_record>(query, page, size);
        }
        public IPagedList<Entities.ImportTrans_main_record> searchListTransport(SysCustomizedListSearchArg arg, int page, int size, string transport)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.Transportation == transport && o.IsDeleted != true);
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.itemno))
                    query = query.Where(o => o.Itemno.Contains(arg.itemno));
                if (!String.IsNullOrEmpty(arg.shipper))
                    query = query.Where(o => o.Shipper.Contains(arg.shipper));
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.PoNo.Contains(arg.pono));
                if (!String.IsNullOrEmpty(arg.invcurr))
                    query = query.Where(o => o.Invcurr.Contains(arg.invcurr));
            }
            // query = query.OrderBy(o => o.CustomizedClassify).ThenBy(o => o.CustomizedValue).ThenByDescending(o => o.CreationTime);
            return new PagedList<Entities.ImportTrans_main_record>(query, page, size);
        }
        public IPagedList<Entities.ImportTrans_main_record> searchListLogistics(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.IsDeleted != true);
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.itemno))
                    query = query.Where(o => o.Itemno.Contains(arg.itemno));
                if (!String.IsNullOrEmpty(arg.shipper))
                    query = query.Where(o => o.Shipper.Contains(arg.shipper));
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.PoNo.Contains(arg.pono));
                if (!String.IsNullOrEmpty(arg.invcurr))
                    query = query.Where(o => o.Invcurr.Contains(arg.invcurr));
            }
            query = query.OrderBy(o => o.CreationTime);
            return new PagedList<Entities.ImportTrans_main_record>(query, page, size);
        }

        public void saveImportTransmain(List<int> categoryIds)
        {
                foreach (int categoryId in categoryIds)
                {
                var item = _importTrans_main_recordRepository.getById(categoryId);
                if (item == null)
                    return;

                item.F_ShipmentCreate = true;
                _importTrans_main_recordRepository.update(item);
                _memoryCache.Remove(MODEL_KEY);
            }
                
        }
        public List<Entities.ImportTrans_main_record> getAll()
        {
            List<Entities.ImportTrans_main_record> list = null;

            if (list != null)
                return list;
            list = _importTrans_main_recordRepository.Table.Where(o=>o.IsDeleted!=true).ToList();
            return list;
        }
        public List<Entities.ImportTrans_main_record> getAllShipModel()
        {
            List<Entities.ImportTrans_main_record> list = null;

            if (list != null)
                return list;
            list = _importTrans_main_recordRepository.Table.Where(o => o.F_ArrivalTimeRequested == true && o.F_ShippingModeGiven != true).ToList();
            return list;
        }
        public void removeCache()
        {
            _memoryCache.Remove(MODEL_KEY);
        }
    }
}
