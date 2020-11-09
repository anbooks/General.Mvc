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
using General.Services.ExportTransportationService;
using General.Services.SysUser;

namespace General.Services.ExportTransportation
{

    public class ExportTransportationService : IExportTransportationService
    {
        private IMemoryCache _memoryCache;  //不用每次去数据库里面筛选

        private const string MODEL_KEY = "General.services.importTrans_main_record";

        private IRepository<Entities.ExportTransportation> _exportTransportationRepository;

        private ISysUserService _sysUserService;
        public ExportTransportationService(ISysUserService sysUserService,IRepository<Entities.ExportTransportation> exportTransportationRepository,
            IMemoryCache memoryCache)
        {
            this._sysUserService = sysUserService;
            this._memoryCache = memoryCache;
            this._exportTransportationRepository = exportTransportationRepository;
        }
        public Entities.ExportTransportation getById(int id)
        {
            return _exportTransportationRepository.getById(id);
        }
        public List<Entities.ExportTransportation> getAll()
        {
            List<Entities.ExportTransportation> list = null;

            if (list != null)
                return list;
            list = _exportTransportationRepository.Table.Where(o => o.IsDeleted != true ).ToList();
            return list;
        }
        public void insertExportTransportation(Entities.ExportTransportation model)
        {
            _exportTransportationRepository.insert(model);
            _memoryCache.Remove(MODEL_KEY);
        }
        public void updateExportTransportation(Entities.ExportTransportation model, string flag)
        {
            var item = _exportTransportationRepository.getById(model.Id);
            if (item == null)
                return;
            if (flag == "出口运输申请")
            { 
            item.DeliverySituation = model.DeliverySituation;
            item.PaymentMethod = model.PaymentMethod;
            item.ImportItem = model.ImportItem;
            item.Applier = model.Applier;
            item.ApplyTime = model.ApplyTime;
            }else if (flag == "创建发运清单")
            {
                item.ItemNo = model.ItemNo;
                item.Project = model.Project;
                item.OfGoods = model.OfGoods;
                item.Creator = model.Creator;
                item.CreationTime= model.CreationTime;
            }
            else if (flag == "综保区填写核注清单")
            {
                item.LicensePlateNo = model.LicensePlateNo;
                item.NuclearNote = model.NuclearNote;
                item.ManufactureDate = model.ManufactureDate;
                item.LicensePlater = model.LicensePlater;
                item.LicensePlateTime = model.LicensePlateTime;
            }
            _exportTransportationRepository.update(item);
            _memoryCache.Remove(MODEL_KEY);
           
        }
        public IPagedList<Entities.ExportTransportation> searchList(SysCustomizedListSearchArg arg, int page, int size, string flag)
        {
               var query = _exportTransportationRepository.Table.Where(o => o.IsDeleted != true);
            if (flag == "出口运输申请")
            {
                query = _exportTransportationRepository.Table.Where(o => o.F_DeliverySituation != true);
            }
            else if (flag == "创建发运清单")
            {
                query = _exportTransportationRepository.Table.Where(o => o.F_DeliverySituation == true&& o.F_Item != true);
            }
            else if (flag == "综保区填写核注清单")
            {
                query = _exportTransportationRepository.Table.Where(o => o.F_Item == true && o.F_LicensePlate != true);
            }
            if (arg != null)
            {
               // if (!String.IsNullOrEmpty(arg.itemno))
                   // query = query.Where(o => o.Itemno.Contains(arg.itemno));
                if (!String.IsNullOrEmpty(arg.shipper))
                    query = query.Where(o => o.DeliverySituation.Contains(arg.shipper));
                if (!String.IsNullOrEmpty(arg.invcurr))
                    query = query.Where(o => o.PaymentMethod.Contains(arg.invcurr));
            }
            // query = query.OrderBy(o => o.CustomizedClassify).ThenBy(o => o.CustomizedValue).ThenByDescending(o => o.CreationTime);
            return new PagedList<Entities.ExportTransportation>(query, page, size);
        }
        public void saveExportTransportation(List<int> categoryIds,string flag)
        {

            foreach (int categoryId in categoryIds)
            {
                var item = _exportTransportationRepository.getById(categoryId);
                if (item == null)
                    return;
                if (flag == "出口运输申请")
                {
                    item.F_DeliverySituation = true;
                }
                else if (flag == "创建发运清单")
                {
                    item.F_Item = true;//F_LicensePlate
                }
                else if (flag == "综保区填写核注清单")
                {
                    item.F_LicensePlate = true;//F_LicensePlate
                }
                _exportTransportationRepository.update(item);
                _memoryCache.Remove(MODEL_KEY);
            }

        }
        public void removeCache()
        {
            _memoryCache.Remove(MODEL_KEY);
        }
    }
}
