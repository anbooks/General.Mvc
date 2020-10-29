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
        void saveDeliveryStatus(List<int> categoryIds, Guid id);
        void saveShippingMode(List<int> categoryIds); //
        void saveInventoryInput(List<int> categoryIds);
        void saveCustomsBrokerSelect(List<int> categoryIds);
        void savePortCustomerBroker(List<int> categoryIds);
        void saveDeliveryRequired(List<int> categoryIds);
        void saveCheckAndPass(List<int> categoryIds,Guid id);
        void saveDeliveryReceipt(List<int> categoryIds);
        Entities.ImportTrans_main_record getById(int id);
        IPagedList<Entities.ImportTrans_main_record> searchListCheckAndPass(SysCustomizedListSearchArg arg, int page, int size);

        IPagedList<Entities.ImportTrans_main_record> searchListPortCustomerBroker(SysCustomizedListSearchArg arg, int page, int size);

        IPagedList<Entities.ImportTrans_main_record> searchListDeliveryRequired(SysCustomizedListSearchArg arg, int page, int size);

        IPagedList<Entities.ImportTrans_main_record> searchListCustomsBrokerSelect(SysCustomizedListSearchArg arg, int page, int size);
        IPagedList<Entities.ImportTrans_main_record> searchList(SysCustomizedListSearchArg arg, int page, int size);

        IPagedList<Entities.ImportTrans_main_record> searchListInventoryInput(SysCustomizedListSearchArg arg, int page, int size);

        IPagedList<Entities.ImportTrans_main_record> searchListShipModel(SysCustomizedListSearchArg arg, int page, int size);
        IPagedList<Entities.ImportTrans_main_record> searchListDeliveryStatus(SysCustomizedListSearchArg arg, int page, int size);
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
        void updateDeliveryReceipt(Entities.ImportTrans_main_record model);
        void updateInventoryInput(Entities.ImportTrans_main_record model);
        void updateDeliveryRequired(Entities.ImportTrans_main_record model);
        void updatePortCustomerBroker(Entities.ImportTrans_main_record model);
        void updateArrivalTime(Entities.ImportTrans_main_record model);
        void updateCustomsBrokerSelect(Entities.ImportTrans_main_record model);
        void updateImportTransmain(Entities.ImportTrans_main_record model);
        void updateShippingMode(Entities.ImportTrans_main_record model); 
        //void updatePassword(Entities.SysUser model);
         void updateDeliveryStatus(Entities.ImportTrans_main_record model);
        //void updateUsermessage(Entities.SysUser model);
        /// <summary>updateUsermessage
        /// 重置密码。默认重置成账号一样
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifer"></param>
        //void resetPassword(Entities.SysUser modelpass);

        /// <summary>
        /// 验证账号是否已经存在
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        //bool existAccount(string account);


        /// <summary>
        /// 启用禁用账号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enabled"></param>
        /// <param name="modifer"></param>
        //void enabled(Guid id, bool enabled, Guid modifer);

        /// <summary>
        /// 登录锁与解锁
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ulock"></param>
        /// <param name="modifer"></param>
       // void loginLock(Guid id, bool ulock, Guid modifer);

        /// <summary>
        /// 删除用户。无法删除超级用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifer"></param>
        //void deleteUser(Guid id, Guid modifer);

        /// <summary>
        /// 添加用户头像
        /// </summary>
        /// <param name="id"></param>
        /// <param name="avatar"></param>
        //void addAvatar(Guid id, byte[] avatar);

        /// <summary>
        /// 用户自己修改密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        //void changePassword(Guid id, string password);

        /// <summary>
        /// 设置用户最后多动时间
        /// </summary>
        /// <param name="id"></param>
       // void lastActivityTime(Guid id);



        //---------------------------------------------------------------------------
        /// <summary>
        /// 验证登录状态
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <param name="r"></param>
        /// <returns></returns>
       // (bool ,string ,string ,Entities.SysUser ) validateUser(string account, string password, string r);
        //(bool Status,string Message ,string Token ,Entities.SysUser User) validateUser(string account, string password, string r);

    }
}
