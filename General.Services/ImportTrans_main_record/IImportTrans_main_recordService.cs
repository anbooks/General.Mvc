using General.Core;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.ImportTrans_main_recordService
{
    public interface IImportTrans_main_recordService
    {
        void saveImportTransmain(List<int> categoryIds);
         
        
        Entities.ImportTrans_main_record getById(int id);
      
       IPagedList<Entities.ImportTrans_main_record> searchListPortCustomerBroker(SysCustomizedListSearchArg arg, int page, int size, string forwarder);
        IPagedList<Entities.ImportTrans_main_record> searchListTransport(SysCustomizedListSearchArg arg, int page, int size, string transport);
        IPagedList<Entities.ImportTrans_main_record> searchList(SysCustomizedListSearchArg arg, int page, int size);
        IPagedList<Entities.ImportTrans_main_record> searchListBuyer(SysCustomizedListSearchArg arg, int page, int size,string buyer);
        IPagedList<Entities.ImportTrans_main_record> searchListLogistics(SysCustomizedListSearchArg arg, int page, int size);
        /// <summary>
        /// 新增，插入DeliveryReceipt
        /// </summary>
        /// <param name="model"></param>
        void insertImportTransmain(Entities.ImportTrans_main_record model);
        List<Entities.ImportTrans_main_record> getAll();
        List<Entities.ImportTrans_main_record> getCount(string co,string role);
        List<Entities.ImportTrans_main_record> getAllShipModel();
        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>DeliveryReceipt
       
        void updateImportTransmain(Entities.ImportTrans_main_record model);
      
    }
}
