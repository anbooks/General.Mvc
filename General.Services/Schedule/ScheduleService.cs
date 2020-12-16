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
using General.Services.ScheduleService;
using General.Services.SysUser;

namespace General.Services.ImportTrans_main_record
{

    public class ScheduleService : IScheduleService
    {
        private IMemoryCache _memoryCache;  //不用每次去数据库里面筛选

        private const string MODEL_KEY = "General.services.schedule";

        private IRepository<Entities.Schedule> _scheduleRepository;

        private ISysUserService _sysUserService;
        public ScheduleService(ISysUserService sysUserService,IRepository<Entities.Schedule> scheduleRepository,
            IMemoryCache memoryCache)
        {
            this._sysUserService = sysUserService;
            this._memoryCache = memoryCache;
            this._scheduleRepository = scheduleRepository;
        }
        public Entities.Schedule getById(int id)
        {
            return _scheduleRepository.getById(id);
        }
        public void insertSchedule(Entities.Schedule model)
        {
            _scheduleRepository.insert(model);
            _memoryCache.Remove(MODEL_KEY);
        }
        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="role"></param>
        public void updateSchedule(Entities.Schedule model)
        {
            var item = _scheduleRepository.getById(model.Id);
            if (item == null)
                return;
             item.OrderNo = model.OrderNo;
             item.Buyer = model.Buyer;
             item.OrderLine = model.OrderLine;
             item.ReferenceNo = model.ReferenceNo;
            item.MaterialCode = model.MaterialCode;
            item.Description = model.Description;
            item.Type = model.Type;
            item.Specification = model.Specification;
            item.Thickness = model.Thickness;
            item.Length = model.Length;
            item.Width = model.Width;
            item.PurchaseQuantity = model.PurchaseQuantity;
            item.PurchaseUnit = model.PurchaseUnit;
            item.UnitPrice = model.UnitPrice;
            item.TotalPrice = model.TotalPrice;
            item.ShipmentDate = model.ShipmentDate;
            item.Consignor = model.Consignor;
            item.Manufacturers = model.Manufacturers;
            item.OriginCountry = model.OriginCountry;
            item.BatchNo = model.BatchNo;
            item.Waybill = model.Waybill;
            item.Books = model.Books;
            item.BooksItem = model.BooksItem;
            item.RecordUnit = model.RecordUnit;
            item.RecordUnitReducedPrice = model.RecordUnitReducedPrice;
            item.LegalUnits = model.LegalUnits;
            item.LegalUniteReducedPrice = model.LegalUniteReducedPrice;
            item.Qualification = model.Qualification;
            item.ModifiedTime = model.ModifiedTime;
             item.Modifier = model.Modifier;
            _scheduleRepository.update(item);
            _memoryCache.Remove(MODEL_KEY);
        }
     
        public IPagedList<Entities.Schedule> searchList(SysCustomizedListSearchArg arg, int page, int size,int id)
        {
            var query = _scheduleRepository.Table.Where(o => o.MainId==id);
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.materiel))
                   query = query.Where(o => o.MaterialCode.Contains(arg.materiel));
               // if (!String.IsNullOrEmpty(arg.shipper))
                 //   query = query.Where(o => o.Shipper.Contains(arg.shipper));
               // if (!String.IsNullOrEmpty(arg.pono))
                  //  query = query.Where(o => o.PoNo.Contains(arg.pono));
              //  if (!String.IsNullOrEmpty(arg.invcurr))
                  //  query = query.Where(o => o.Invcurr.Contains(arg.invcurr));
            }
            query = query.OrderBy(o => o.CreationTime).ThenBy(o => o.OrderLine).ThenByDescending(o => o.CreationTime);
            return new PagedList<Entities.Schedule>(query, page, size);
        }
      
        public void saveSchedule( List<int> categoryIds)
        {
 
                foreach (int categoryId in categoryIds)
                {
                var item = _scheduleRepository.getById(categoryId);
                if (item == null)
                    return;

                item.IsDeleted = true;
                _scheduleRepository.update(item);
                _memoryCache.Remove(MODEL_KEY);
            }
                
        }
      
        public List<Entities.Schedule> getAll(int id)
        {
            List<Entities.Schedule> list = null;

            if (list != null)
                return list;
            list = _scheduleRepository.Table.Where(o=>o.MainId==id).ToList();
            return list;
        }
       
        public void removeCache()
        {
            _memoryCache.Remove(MODEL_KEY);
        }
    }
}
