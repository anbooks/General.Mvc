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
using General.Services.ImportTrans_main_record_copyService;

namespace General.Services.ImportTrans_main_record
{

    public class ImportTrans_main_recordService : IImportTrans_main_recordService
    {
        private IMemoryCache _memoryCache;  //不用每次去数据库里面筛选

        private const string MODEL_KEY = "General.services.importTrans_main_record";

        private IRepository<Entities.ImportTrans_main_record> _importTrans_main_recordRepository;

        private ISysUserService _sysUserService;
        private IImportTrans_main_record_copyService _iImportTrans_main_record_copyService;
        public ImportTrans_main_recordService(IImportTrans_main_record_copyService iImportTrans_main_record_copyService,ISysUserService sysUserService,IRepository<Entities.ImportTrans_main_record> importTrans_main_recordRepository,
            IMemoryCache memoryCache)
        {
            this._sysUserService = sysUserService;
            this._iImportTrans_main_record_copyService = iImportTrans_main_record_copyService;
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
            Entities.ImportTrans_main_record_copy record = new Entities.ImportTrans_main_record_copy();
            record.ActualDeliveryDate=model.ActualDeliveryDate;
            record.Ata = model.Ata;
            record.Atd = model.Atd;
            record.BlDate = model.BlDate;
            record.BrokenRecord = model.BrokenRecord;
            record.Buyer = model.Buyer;
            record.CargoType = model.CargoType;
            record.CheckAndPass = model.CheckAndPass;
            record.ChooseDelivery = model.ChooseDelivery;
            record.CreationTime = DateTime.Now;
            record.Creator = model.Creator;
            record.CustomsDeclarationNo = model.CustomsDeclarationNo;
            record.Declaration = model.Declaration;
            record.DeclarationDate = model.DeclarationDate;
            record.DeliveryReceipt = model.DeliveryReceipt;
            record.DeliveryRequiredDate = model.DeliveryRequiredDate;
            record.Dest = model.Dest;
            record.FlighVessel = model.FlighVessel;
            record.Forwarder = model.Forwarder;
            record.F_ShippingModeGiven = model.F_ShippingModeGiven;
            record.Gw = model.Gw;
            record.Hbl = model.Hbl;
            record.HblAttachment = model.HblAttachment;
            record.Incoterms = model.Incoterms;
            record.InspectionLotNo = model.InspectionLotNo;
            record.Invamou = model.Invamou;
            record.Invcurr = model.Invcurr;
            record.InventoryAttachment = model.InventoryAttachment;
            record.InventoryNo = model.InventoryNo;
            record.IsDeleted = model.IsDeleted;
            record.IsNeedSecondCheck = model.IsNeedSecondCheck;
            record.Itemno = model.Id;
            record.Mbl = model.Mbl;
            record.MblAttachment = model.MblAttachment;
            record.Measurement = model.Measurement;
            record.MeasurementUnit = model.MeasurementUnit;
            record.ModifiedTime = model.ModifiedTime;
            record.Modifier = model.Modifier;
            record.Note = model.Note;
            record.OrderNo = model.OrderNo;
            record.Origin = model.Origin;
            record.Pcs = model.Pcs;
            record.PoNo = model.PoNo;
            record.RealReceivingDate = model.RealReceivingDate;
            record.ReceiptForm = model.ReceiptForm;
            record.ReleaseDate = model.ReleaseDate;
            record.RequestedArrivalTime = model.RequestedArrivalTime;
            record.SecondCheck = model.SecondCheck;
            record.Shipper = model.Shipper;
            record.ShippingMode = model.ShippingMode;
            record.Status = model.Status;
            record.Transportation = model.Transportation;
            _iImportTrans_main_record_copyService.insertImportTransmain(record);
            _memoryCache.Remove(MODEL_KEY); 
        }
        
        public IPagedList<Entities.ImportTrans_main_record> searchList(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.IsDeleted!=true);
            if (arg.buy != null || arg.invcurr != null||arg.itemno!=null || arg.keyword != null || arg.materiel != null || arg.pono != null || arg.realreceivingdatestrat != null || arg.realreceivingdateend != null || arg.shipper != null)
            {
                if (!String.IsNullOrEmpty(arg.keyword))
                    query = query.Where(o => o.Transportation.Contains(arg.keyword));
                if (!String.IsNullOrEmpty(arg.buy))
                    query = query.Where(o => o.Buyer.Contains(arg.buy));
                if (!String.IsNullOrEmpty(arg.itemno))
                    query = query.Where(o => o.Id == Convert.ToInt32(arg.itemno));
                if (!String.IsNullOrEmpty(arg.shipper))
                    query = query.Where(o => o.Shipper.Contains(arg.shipper));
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.PoNo.Contains(arg.pono));
                if (!String.IsNullOrEmpty(arg.invcurr))    
                    query = query.Where(o => o.Invcurr.Contains(arg.invcurr));
                if (arg.realreceivingdateend!=null)
                    query = query.Where(o => o.RealReceivingDate<arg.realreceivingdateend.Value.AddDays(1));
                if ( arg.realreceivingdatestrat != null)
                    query = query.Where(o => o.RealReceivingDate > arg.realreceivingdatestrat);
            }
            else {
                query = query.Where(o => o.Ata > DateTime.Today.AddMonths(-2)||o.CheckAndPass!="是");
            }
            query = query.OrderByDescending(o => o.Id);
           // query = query.OrderBy(o => o.CustomizedClassify).ThenBy(o => o.CustomizedValue).ThenByDescending(o => o.CreationTime);
            return new PagedList<Entities.ImportTrans_main_record>(query, page, size);
        }
        public IPagedList<Entities.ImportTrans_main_record> searchListYd(SysCustomizedListSearchArg arg, int page, int size,string port,string tran)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.IsDeleted != true);

            query = query.Where(o => (o.Forwarder == port && o.Forwarder != null) || (o.Transportation == tran && o.Transportation != null));
          
            if ( arg.invcurr != null || arg.itemno != null || arg.keyword != null || arg.materiel != null || arg.pono != null || arg.realreceivingdatestrat != null || arg.realreceivingdateend != null || arg.shipper != null)
            {
                if (port == null && tran == null )
                {
                        query = query.Where(o => o.Id == 0);
                }
                else
                {
                    if (!String.IsNullOrEmpty(arg.itemno))
                        query = query.Where(o => o.Id == Convert.ToInt32(arg.itemno));
                    if (!String.IsNullOrEmpty(arg.shipper))
                        query = query.Where(o => o.Shipper.Contains(arg.shipper));
                    if (!String.IsNullOrEmpty(arg.pono))
                        query = query.Where(o => o.PoNo.Contains(arg.pono));
                    if (!String.IsNullOrEmpty(arg.invcurr))
                        query = query.Where(o => o.Invcurr.Contains(arg.invcurr));
                    if (arg.realreceivingdateend != null)
                        query = query.Where(o => o.RealReceivingDate < arg.realreceivingdateend.Value.AddDays(1));
                    if (arg.realreceivingdatestrat != null)
                        query = query.Where(o => o.RealReceivingDate > arg.realreceivingdatestrat);
                }
            }
            else
            {
                if (port == null && tran == null)
                {

                    query = query.Where(o => o.Id == 0);

                }
                else
                {
                    query = query.Where(o => o.CheckAndPass != "是");
                }
            }
            query = query.OrderByDescending(o => o.Id);
            // query = query.OrderBy(o => o.CustomizedClassify).ThenBy(o => o.CustomizedValue).ThenByDescending(o => o.CreationTime);
            return new PagedList<Entities.ImportTrans_main_record>(query, page, size);
        }
        public IPagedList<Entities.ImportTrans_main_record> searchListBuyer(SysCustomizedListSearchArg arg, int page, int size,string buyer)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.IsDeleted != true&&o.Buyer==buyer&& o.CheckAndPass != "是");
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
            var query = _importTrans_main_recordRepository.Table.Where(o => o.Forwarder== forwarder&& o.Forwarder != null&& o.IsDeleted != true && o.CheckAndPass != "是");
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
            var query = _importTrans_main_recordRepository.Table.Where(o => o.Transportation == transport && o.IsDeleted != true && o.CheckAndPass != "是");
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
        public IPagedList<Entities.ImportTrans_main_record> searchListTransportW(SysCustomizedListSearchArg arg, int page, int size, string transport)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.Transportation == transport && o.IsDeleted != true && o.CheckAndPass != "是");
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
            var query = _importTrans_main_recordRepository.Table.Where(o => o.IsDeleted != true && o.CheckAndPass != "是");
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.itemno))
                    query = query.Where(o => o.Id==Convert.ToInt32(arg.itemno));
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

                //item.F_ShipmentCreate = true;
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
        public List<Entities.ImportTrans_main_record> getCount(string co, string role)
        {
            List<Entities.ImportTrans_main_record> list = null;

            if (list != null)
                return list;
            if (role=="采购员") {
                list = _importTrans_main_recordRepository.Table.Where(o => o.Buyer == co && o.IsDeleted != true&& o.CheckAndPass!="是").ToList();
            }else
            {
                list = _importTrans_main_recordRepository.Table.Where(o => (o.Transportation == role || o.Forwarder== role) && o.IsDeleted != true && o.CheckAndPass != "是").ToList();
            }
           
            return list;
        }
        public List<Entities.ImportTrans_main_record> getAllShipModel()
        {
            List<Entities.ImportTrans_main_record> list = null;

            if (list != null)
                return list;
            list = _importTrans_main_recordRepository.Table.ToList();
            return list;
        }
        public void removeCache()
        {
            _memoryCache.Remove(MODEL_KEY);
        }
    }
}
