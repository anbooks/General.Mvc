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
              item.RealReceivingDate = model.RealReceivingDate;
              item.Itemno = model.Itemno;
              item.Shipper = model.Shipper;
              item.PoNo = model.PoNo;
              item.Buyer = model.Buyer;
              item.Invcurr = model.Invcurr;
              item.ModifiedTime = model.ModifiedTime;
              item.Modifier = model.Modifier;
            _importTrans_main_recordRepository.update(item);
            _memoryCache.Remove(MODEL_KEY);
        }

        public void updateShippingMode(Entities.ImportTrans_main_record model)
        {
            var item = _importTrans_main_recordRepository.getById(model.Id);
            if (item == null)
                return;
            item.ShippingMode = model.ShippingMode;
            item.ShippingModeGivenTime = model.ShippingModeGivenTime;
            item.ShippingModeGiver = model.ShippingModeGiver;
            
            _importTrans_main_recordRepository.update(item);
            _memoryCache.Remove(MODEL_KEY);
        }
        public void updatePortCustomerBroker(Entities.ImportTrans_main_record model)
        {
            var item = _importTrans_main_recordRepository.getById(model.Id);
            if (item == null)
                return;
            item.PortCustomerBrokerInputer = model.PortCustomerBrokerInputer;
            item.PortCustomerBrokerInputTime = model.PortCustomerBrokerInputTime;
            item.BlDate = model.BlDate;
            item.DeclarationDate = model.DeclarationDate;
            item.ReleaseDate = model.ReleaseDate;
            item.CustomsDeclarationNo = model.CustomsDeclarationNo;
            item.InspectionLotNo = model.InspectionLotNo;
            item.IsNeedSecondCheck = model.IsNeedSecondCheck;

            _importTrans_main_recordRepository.update(item);
            _memoryCache.Remove(MODEL_KEY);
        }
        public void updateCheckAndPass(List<int> categoryIds, Guid id)
        {

            foreach (int categoryId in categoryIds)
            {
                var item = _importTrans_main_recordRepository.getById(categoryId);
                if (item == null)
                    return;
                item.CheckAndPassor = id;
                item.CheckAndPassTime = DateTime.Now;
                item.CheckAndPass = true;
                _importTrans_main_recordRepository.update(item);
                _memoryCache.Remove(MODEL_KEY);
            }

        }
        public void updateDeliveryRequired(Entities.ImportTrans_main_record model)
        {
            var item = _importTrans_main_recordRepository.getById(model.Id);
            if (item == null)
                return;
            item.DeliveryDateRequirer = model.DeliveryDateRequirer;
            item.DeliveryRequiredDate = model.DeliveryRequiredDate;
            item.DeliveryDateRequiredTime = model.DeliveryDateRequiredTime;
    
            _importTrans_main_recordRepository.update(item);
            _memoryCache.Remove(MODEL_KEY);
        }
        public void updateDeliveryStatus(Entities.ImportTrans_main_record model)
        {
            var item = _importTrans_main_recordRepository.getById(model.Id);
            if (item == null)
                return;
            item.Status = model.Status;
            item.DeliveryStatusInputTime = model.DeliveryStatusInputTime;
            item.DeliveryStatusInputer = model.ShippingModeGiver;

            _importTrans_main_recordRepository.update(item);
            _memoryCache.Remove(MODEL_KEY);
        }
        public void updateDeliveryReceipt(Entities.ImportTrans_main_record model)
        {
            var item = _importTrans_main_recordRepository.getById(model.Id);
            if (item == null)
                return;
            item.DeliveryReceipt = model.DeliveryReceipt;
            item.ActualDeliveryDate = model.ActualDeliveryDate;
            item.ChooseDelivery = model.ChooseDelivery;

            _importTrans_main_recordRepository.update(item);
            _memoryCache.Remove(MODEL_KEY);
        }
         public void updateCustomsBrokerSelect(Entities.ImportTrans_main_record model)
        {
            var item = _importTrans_main_recordRepository.getById(model.Id);
            if (item == null)
                return;
            item.Forwarder = model.Forwarder;
            item.CustomsBrokerSelectTime = model.CustomsBrokerSelectTime;
            item.CustomsBrokerSelecter = model.CustomsBrokerSelecter;

            _importTrans_main_recordRepository.update(item);
            _memoryCache.Remove(MODEL_KEY);
        }
        public void updateReceiptInput(Entities.ImportTrans_main_record model)
        {
            var item = _importTrans_main_recordRepository.getById(model.Id);
            if (item == null)
                return;
            item.Note = model.Note;
            item.ReceiptForm = model.ReceiptForm;
            item.ReceiptFormer = model.ReceiptFormer;
            item.ReceiptFormTime = model.ReceiptFormTime;

            _importTrans_main_recordRepository.update(item);
            _memoryCache.Remove(MODEL_KEY);
        }
        public void updateSecondCheck(List<int> categoryIds,Guid id)
        {

            foreach (int categoryId in categoryIds)
            {
                var item = _importTrans_main_recordRepository.getById(categoryId);
                if (item == null)
                    return;//CustomsBrokerSelect


                item.SecondCheckor = id;
                item.SecondCheckTime = DateTime.Now;
                item.SecondCheck = true;
                _importTrans_main_recordRepository.update(item);
                _memoryCache.Remove(MODEL_KEY);
            }

        }
        public void updateDeclaration(List<int> categoryIds, Guid id)
        {

            foreach (int categoryId in categoryIds)
            {
                var item = _importTrans_main_recordRepository.getById(categoryId);
                if (item == null)
                    return;//CustomsBrokerSelect


                item.Declarationer = id;
                item.DeclarationTime = DateTime.Now;
                item.Declaration = true;
                _importTrans_main_recordRepository.update(item);
                _memoryCache.Remove(MODEL_KEY);
            }

        }
        public void updateArrivalTime(Entities.ImportTrans_main_record model)
        {
            var item = _importTrans_main_recordRepository.getById(model.Id);
            if (item == null)
                return;
            item.RequestedArrivalTime = model.RequestedArrivalTime;
            item.Requester = model.Requester;
            item.RequestTime = model.RequestTime;

            _importTrans_main_recordRepository.update(item);
            _memoryCache.Remove(MODEL_KEY);
        }
        public void updateInventoryInput(Entities.ImportTrans_main_record model)
        {
            var item = _importTrans_main_recordRepository.getById(model.Id);
            if (item == null)
                return;
            item.InventoryInputTime = model.InventoryInputTime;
            item.InventoryInputer = model.InventoryInputer;
            item.InventoryNo = model.InventoryNo;

            _importTrans_main_recordRepository.update(item);
            _memoryCache.Remove(MODEL_KEY);
        }
        //CheckAndPass
        public IPagedList<Entities.ImportTrans_main_record> searchListCheckAndPass(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.F_DeliveryReceipt == true && o.F_CheckAndPass != true);
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
        public IPagedList<Entities.ImportTrans_main_record> searchListReceiptInput(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.F_CheckAndPass == true && o.F_ReceiptForm != true);
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
        public IPagedList<Entities.ImportTrans_main_record> searchListSecondCheck(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.F_ReceiptForm == true && o.F_SecondCheck != true);
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
        public IPagedList<Entities.ImportTrans_main_record> searchListDeclaration(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.F_SecondCheck == true && o.F_Declaration != true);
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
        public IPagedList<Entities.ImportTrans_main_record> searchList(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.IsDeleted!=true);
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
        public IPagedList<Entities.ImportTrans_main_record> searchListPortCustomerBroker(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.F_InventoryInput == true && o.F_PortCustomerBrokerInput != true);
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
        public IPagedList<Entities.ImportTrans_main_record> searchListDeliveryRequired(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.F_PortCustomerBrokerInput == true && o.F_DeliveryDateRequired != true);
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
        public IPagedList<Entities.ImportTrans_main_record> searchListShipModel(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.F_ArrivalTimeRequested == true && o.F_ShippingModeGiven != true);
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
        public IPagedList<Entities.ImportTrans_main_record> searchListDeliveryStatus(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.F_ShippingModeGiven == true && o.F_DeliveryStatusInput != true);
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
        public IPagedList<Entities.ImportTrans_main_record> searchListDeliveryReceipt(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.F_PortCustomerBrokerInput == true && o.F_DeliveryReceipt != true);
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
        public IPagedList<Entities.ImportTrans_main_record> searchListCustomsBrokerSelect(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.F_DeliveryStatusInput == true && o.F_CustomsBrokerSelect != true);
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
        public IPagedList<Entities.ImportTrans_main_record> searchListInventoryInput(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _importTrans_main_recordRepository.Table.Where(o => o.F_CustomsBrokerSelect == true && o.F_InventoryInput!= true);
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
        public void saveCheckAndPass(List<int> categoryIds)
        {

            foreach (int categoryId in categoryIds)
            {
                var item = _importTrans_main_recordRepository.getById(categoryId);
                if (item == null)
                    return;
            
                item.F_CheckAndPass = true;
                _importTrans_main_recordRepository.update(item);
                _memoryCache.Remove(MODEL_KEY);
            }

        }
        public void savePortCustomerBroker(List<int> categoryIds)
        {

            foreach (int categoryId in categoryIds)
            {
                var item = _importTrans_main_recordRepository.getById(categoryId);
                if (item == null)
                    return;

                item.F_PortCustomerBrokerInput = true;
                _importTrans_main_recordRepository.update(item);
                _memoryCache.Remove(MODEL_KEY);
            }

        }
        public void saveSecondCheck(List<int> categoryIds)
        {

            foreach (int categoryId in categoryIds)
            {
                var item = _importTrans_main_recordRepository.getById(categoryId);
                if (item == null)
                    return;

                item.F_SecondCheck= true;
                _importTrans_main_recordRepository.update(item);
                _memoryCache.Remove(MODEL_KEY);
            }

        }
        public void saveDeclaration(List<int> categoryIds)
        {

            foreach (int categoryId in categoryIds)
            {
                var item = _importTrans_main_recordRepository.getById(categoryId);
                if (item == null)
                    return;

                item.F_Declaration = true;
                _importTrans_main_recordRepository.update(item);
                _memoryCache.Remove(MODEL_KEY);
            }

        }
        public void saveReceiptInput(List<int> categoryIds)
        {

            foreach (int categoryId in categoryIds)
            {
                var item = _importTrans_main_recordRepository.getById(categoryId);
                if (item == null)
                    return;

                item.F_ReceiptForm = true;
                _importTrans_main_recordRepository.update(item);
                _memoryCache.Remove(MODEL_KEY);
            }

        }
        public void saveDeliveryReceipt(List<int> categoryIds)
        {

            foreach (int categoryId in categoryIds)
            {
                var item = _importTrans_main_recordRepository.getById(categoryId);
                if (item == null)
                    return;

                item.F_DeliveryReceipt = true;
                _importTrans_main_recordRepository.update(item);
                _memoryCache.Remove(MODEL_KEY);
            }

        }
        public void saveDeliveryRequired(List<int> categoryIds)
        {

            foreach (int categoryId in categoryIds)
            {
                var item = _importTrans_main_recordRepository.getById(categoryId);
                if (item == null)
                    return;

                item.F_DeliveryDateRequired = true;
                _importTrans_main_recordRepository.update(item);
                _memoryCache.Remove(MODEL_KEY);
            }

        }
        public void saveInventoryInput(List<int> categoryIds)
        {

            foreach (int categoryId in categoryIds)
            {
                var item = _importTrans_main_recordRepository.getById(categoryId);
                if (item == null)
                    return;
                item.InventoryNo = "具体报关信息体现在明细表中";
                item.F_InventoryInput = true;
                _importTrans_main_recordRepository.update(item);
                _memoryCache.Remove(MODEL_KEY);
            }

        }
        public void saveDeliveryStatus(List<int> categoryIds,Guid id)
           {

            foreach (int categoryId in categoryIds)
            {
                var item = _importTrans_main_recordRepository.getById(categoryId);
                if (item == null)
                    return;//CustomsBrokerSelect
                var model=_sysUserService.getById(id);
                if(model.Co== "北京捷诚"|| model.Co == "辽宁北方")
                {
                    if (item.Dest== "PEK")
                    {
                        item.Forwarder ="北京捷诚";
                    }
                    else if (item.Dest == "SHE")
                    {
                        item.Forwarder = "辽宁北方";
                    }
                    else if (item.Dest == "DLC")
                    {
                        item.Forwarder = "大连环球";
                    }
                    else
                    {
                        item.Forwarder = "辽宁北方";
                    }
                }
                if (model.Co == "北京和合" || model.Co == "大连环球")
                {
                    if (item.Dest == "PEK")
                    {
                        item.Forwarder = "北京捷诚";
                    }
                    else if (item.Dest == "SHE")
                    {
                        item.Forwarder = "辽宁北方";
                    }
                    else if (item.Dest == "DLG")
                    {
                        item.Forwarder = "大连环球";
                    }
                    else
                    {
                        item.Forwarder = "辽宁北方";
                    }
                }
                //item.Forwarder = model.Co;
                if (item.Forwarder != null)
                {
                    Guid gv = new Guid("42F5B183-52B6-44C7-9F48-89576F888DEB");
                    item.CustomsBrokerSelecter = gv;
                }
 
                item.F_DeliveryStatusInput = true;
                _importTrans_main_recordRepository.update(item);
                _memoryCache.Remove(MODEL_KEY);
            }

        }
        public void saveCustomsBrokerSelect(List<int> categoryIds)
        {

            foreach (int categoryId in categoryIds)
            {
                var item = _importTrans_main_recordRepository.getById(categoryId);
                if (item == null)
                    return;//CustomsBrokerSelect
               
                
                //item.Forwarder = model.Co;
              
                item.F_CustomsBrokerSelect = true;
                _importTrans_main_recordRepository.update(item);
                _memoryCache.Remove(MODEL_KEY);
            }

        }
        public void saveShippingMode(List<int> categoryIds)
        {

            foreach (int categoryId in categoryIds)
            {
                var item = _importTrans_main_recordRepository.getById(categoryId);
                if (item == null)
                    return;

                item.F_ShippingModeGiven = true;
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
