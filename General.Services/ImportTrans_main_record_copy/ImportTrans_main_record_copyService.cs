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
using General.Services.ImportTrans_main_record_copyService;
using General.Services.SysUser;

namespace General.Services.ImportTrans_main_record_copy
{

    public class ImportTrans_main_record_copyService : IImportTrans_main_record_copyService
    {
        private IMemoryCache _memoryCache;  //不用每次去数据库里面筛选

        private const string MODEL_KEY = "General.services.ImportTrans_main_record_copy";

        private IRepository<Entities.ImportTrans_main_record_copy> _ImportTrans_main_record_copyRepository;

        private ISysUserService _sysUserService;
        public ImportTrans_main_record_copyService(ISysUserService sysUserService,IRepository<Entities.ImportTrans_main_record_copy> ImportTrans_main_record_copyRepository,
            IMemoryCache memoryCache)
        {
            this._sysUserService = sysUserService;
            this._memoryCache = memoryCache;
            this._ImportTrans_main_record_copyRepository = ImportTrans_main_record_copyRepository;
        }
        public Entities.ImportTrans_main_record_copy getById(int id)
        {
            return _ImportTrans_main_record_copyRepository.getById(id);
        }
        public void insertImportTransmain(Entities.ImportTrans_main_record_copy model)
        {
            _ImportTrans_main_record_copyRepository.insert(model);
            _memoryCache.Remove(MODEL_KEY);
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="role"></param>
        public void updateImportTransmain(Entities.ImportTrans_main_record_copy model)
        {
            var item = _ImportTrans_main_record_copyRepository.getById(model.Id);
            if (item == null)
                return;
            _ImportTrans_main_record_copyRepository.update(model);
            _memoryCache.Remove(MODEL_KEY); 
        }

        
        //CheckAndPass
        
       
       
        public IPagedList<Entities.ImportTrans_main_record_copy> searchList(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _ImportTrans_main_record_copyRepository.Table.Where(o => o.IsDeleted!=true);
            if (arg.invcurr != null||arg.itemno!=null || arg.keyword != null || arg.materiel != null || arg.pono != null || arg.realreceivingdatestrat != null || arg.realreceivingdateend != null || arg.shipper != null)
            {
               
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
            return new PagedList<Entities.ImportTrans_main_record_copy>(query, page, size);
        }
        public IPagedList<Entities.ImportTrans_main_record_copy> searchListBuyer(SysCustomizedListSearchArg arg, int page, int size,string buyer)
        {
            var query = _ImportTrans_main_record_copyRepository.Table.Where(o => o.IsDeleted != true&&o.Buyer==buyer);
            if (arg != null)
            {
                
                if (!String.IsNullOrEmpty(arg.shipper))
                    query = query.Where(o => o.Shipper.Contains(arg.shipper));
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.PoNo.Contains(arg.pono));
                if (!String.IsNullOrEmpty(arg.invcurr))
                    query = query.Where(o => o.Invcurr.Contains(arg.invcurr));
            }
            query = query.OrderByDescending(o => o.Id);
            // query = query.OrderBy(o => o.CustomizedClassify).ThenBy(o => o.CustomizedValue).ThenByDescending(o => o.CreationTime);
            return new PagedList<Entities.ImportTrans_main_record_copy>(query, page, size);
        }
        public IPagedList<Entities.ImportTrans_main_record_copy> searchListPortCustomerBroker(SysCustomizedListSearchArg arg, int page, int size,string forwarder)
        {
            var query = _ImportTrans_main_record_copyRepository.Table.Where(o => o.Forwarder== forwarder&& o.IsDeleted != true);
            if (arg != null)
            {
                
                if (!String.IsNullOrEmpty(arg.shipper))
                    query = query.Where(o => o.Shipper.Contains(arg.shipper));
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.PoNo.Contains(arg.pono));
                if (!String.IsNullOrEmpty(arg.invcurr))
                    query = query.Where(o => o.Invcurr.Contains(arg.invcurr));
            }
            // query = query.OrderBy(o => o.CustomizedClassify).ThenBy(o => o.CustomizedValue).ThenByDescending(o => o.CreationTime);
            return new PagedList<Entities.ImportTrans_main_record_copy>(query, page, size);
        }
        public IPagedList<Entities.ImportTrans_main_record_copy> searchListTransport(SysCustomizedListSearchArg arg, int page, int size, string transport)
        {
            var query = _ImportTrans_main_record_copyRepository.Table.Where(o => o.Transportation == transport && o.IsDeleted != true);
            if (arg != null)
            {
               
                if (!String.IsNullOrEmpty(arg.shipper))
                    query = query.Where(o => o.Shipper.Contains(arg.shipper));
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.PoNo.Contains(arg.pono));
                if (!String.IsNullOrEmpty(arg.invcurr))
                    query = query.Where(o => o.Invcurr.Contains(arg.invcurr));
            }
            // query = query.OrderBy(o => o.CustomizedClassify).ThenBy(o => o.CustomizedValue).ThenByDescending(o => o.CreationTime);
            return new PagedList<Entities.ImportTrans_main_record_copy>(query, page, size);
        }
        public IPagedList<Entities.ImportTrans_main_record_copy> searchListLogistics(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _ImportTrans_main_record_copyRepository.Table.Where(o => o.IsDeleted != true);
            if (arg != null)
            {
               
                    query = query.Where(o => o.Shipper.Contains(arg.shipper));
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.PoNo.Contains(arg.pono));
                if (!String.IsNullOrEmpty(arg.invcurr))
                    query = query.Where(o => o.Invcurr.Contains(arg.invcurr));
            }
            query = query.OrderBy(o => o.CreationTime);
            return new PagedList<Entities.ImportTrans_main_record_copy>(query, page, size);
        }

        public void saveImportTransmain(List<int> categoryIds)
        {
                foreach (int categoryId in categoryIds)
                {
                var item = _ImportTrans_main_record_copyRepository.getById(categoryId);
                if (item == null)
                    return;

                //item.F_ShipmentCreate = true;
                _ImportTrans_main_record_copyRepository.update(item);
                _memoryCache.Remove(MODEL_KEY);
            }
                
        }
        public List<Entities.ImportTrans_main_record_copy> getAll()
        {
            List<Entities.ImportTrans_main_record_copy> list = null;

            if (list != null)
                return list;
            list = _ImportTrans_main_record_copyRepository.Table.Where(o=>o.IsDeleted!=true).ToList();
            return list;
        }
        public List<Entities.ImportTrans_main_record_copy> getCount(string co, string role)
        {
            List<Entities.ImportTrans_main_record_copy> list = null;

            if (list != null)
                return list;
            if (role=="采购员") {
                list = _ImportTrans_main_record_copyRepository.Table.Where(o => o.Buyer == co && o.IsDeleted != true).ToList();
            }
            if (role == "运输代理")
            {
                list = _ImportTrans_main_record_copyRepository.Table.Where(o => o.Transportation == co && o.IsDeleted != true).ToList();
            }
            if (role == "口岸报关行")
            {
                list = _ImportTrans_main_record_copyRepository.Table.Where(o => o.Forwarder == co && o.IsDeleted != true).ToList();
            }
            return list;
        }
        public List<Entities.ImportTrans_main_record_copy> getAllShipModel()
        {
            List<Entities.ImportTrans_main_record_copy> list = null;

            if (list != null)
                return list;
            list = _ImportTrans_main_record_copyRepository.Table.ToList();
            return list;
        }
        public void removeCache()
        {
            _memoryCache.Remove(MODEL_KEY);
        }
    }
}
