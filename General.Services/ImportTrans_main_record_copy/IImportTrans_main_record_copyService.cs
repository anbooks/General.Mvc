using General.Core;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.ImportTrans_main_record_copyService
{
    public interface IImportTrans_main_record_copyService
    {
        void saveImportTransmain(List<int> categoryIds);
         
        
        Entities.ImportTrans_main_record_copy getById(int id);
      
       IPagedList<Entities.ImportTrans_main_record_copy> searchListPortCustomerBroker(SysCustomizedListSearchArg arg, int page, int size, string forwarder);
        IPagedList<Entities.ImportTrans_main_record_copy> searchListTransport(SysCustomizedListSearchArg arg, int page, int size, string transport);
        IPagedList<Entities.ImportTrans_main_record_copy> searchList(SysCustomizedListSearchArg arg, int page, int size);
        IPagedList<Entities.ImportTrans_main_record_copy> searchListBuyer(SysCustomizedListSearchArg arg, int page, int size,string buyer);
        IPagedList<Entities.ImportTrans_main_record_copy> searchListLogistics(SysCustomizedListSearchArg arg, int page, int size);
        /// <summary>
        /// 新增，插入DeliveryReceipt
        /// </summary>
        /// <param name="model"></param>
        void insertImportTransmain(Entities.ImportTrans_main_record_copy model);
        List<Entities.ImportTrans_main_record_copy> getAll();
        List<Entities.ImportTrans_main_record_copy> getCount(string co,string role);
        List<Entities.ImportTrans_main_record_copy> getAllShipModel();
        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>DeliveryReceipt
       
        void updateImportTransmain(Entities.ImportTrans_main_record_copy model);
      
    }
}
