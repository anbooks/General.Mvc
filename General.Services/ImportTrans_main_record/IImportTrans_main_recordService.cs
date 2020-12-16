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
         
        void saveCustomsBrokerSelect(List<int> categoryIds);
        void savePortCustomerBroker(List<int> categoryIds);
        void saveDeliveryRequired(List<int> categoryIds);
        void saveCheckAndPass(List<int> categoryIds);
        void saveDeliveryReceipt(List<int> categoryIds);
        void saveReceiptInput(List<int> categoryIds);
        void saveSecondCheck(List<int> categoryIds);
        void saveDeclaration(List<int> categoryIds);
        Entities.ImportTrans_main_record getById(int id);
        IPagedList<Entities.ImportTrans_main_record> searchListCheckAndPass(SysCustomizedListSearchArg arg, int page, int size);
        IPagedList<Entities.ImportTrans_main_record> searchListReceiptInput(SysCustomizedListSearchArg arg, int page, int size);

        IPagedList<Entities.ImportTrans_main_record> searchListSecondCheck(SysCustomizedListSearchArg arg, int page, int size, string role);

        IPagedList<Entities.ImportTrans_main_record> searchListDeclaration(SysCustomizedListSearchArg arg, int page, int size);

        IPagedList<Entities.ImportTrans_main_record> searchListPortCustomerBroker(SysCustomizedListSearchArg arg, int page, int size);

        IPagedList<Entities.ImportTrans_main_record> searchListDeliveryRequired(SysCustomizedListSearchArg arg, int page, int size);

        IPagedList<Entities.ImportTrans_main_record> searchListCustomsBrokerSelect(SysCustomizedListSearchArg arg, int page, int size);
        IPagedList<Entities.ImportTrans_main_record> searchList(SysCustomizedListSearchArg arg, int page, int size);
        IPagedList<Entities.ImportTrans_main_record> searchListBuyer(SysCustomizedListSearchArg arg, int page, int size,string buyer);
        
         IPagedList<Entities.ImportTrans_main_record> searchListDeliveryReceipt(SysCustomizedListSearchArg arg, int page, int size);

        /// <summary>
        /// 新增，插入DeliveryReceipt
        /// </summary>
        /// <param name="model"></param>
        void insertImportTransmain(Entities.ImportTrans_main_record model);
        List<Entities.ImportTrans_main_record> getAll(); 
        List<Entities.ImportTrans_main_record> getAllShipModel();
        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>DeliveryReceipt
       
        void updateImportTransmain(Entities.ImportTrans_main_record model);
      
    }
}
