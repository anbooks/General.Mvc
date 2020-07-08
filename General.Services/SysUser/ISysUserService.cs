using General.Core;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.SysUser
{
    public interface ISysUserService
    {
        /// <summary>
        /// 验证登录状态
        /// </summary>
        /// <param name="account">登录账号</param>
        /// <param name="password">登录密码</param>
        /// <param name="r">登录随机数</param>
        /// <returns></returns>
        (bool Status,string Message ,string Token ,Entities.SysUser User) validateUser(string account, string password, string r);

        /// <summary>
        /// 通过账号获取用户
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        Entities.SysUser getByAccount(string account);

        /// <summary>
        /// 通过当前登录用户的token 获取用户信息，并缓存
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Entities.SysUser getLogged(string token);

        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        IPagedList<Entities.SysUser> searchUser(SysUserSearchArg arg, int page, int size);

        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Entities.SysUser getById(Guid id);

        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        void insertSysUser(Entities.SysUser model);

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
        void updateSysUser(Entities.SysUser model);

        /// <summary>
        /// 重置密码。默认重置成账号一样
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifer"></param>
        void resetPassword(Guid id, Guid modifer);

        /// <summary>
        /// 验证账号是否已经存在
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        bool existAccount(string account);


        /// <summary>
        /// 启用禁用账号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enabled"></param>
        /// <param name="modifer"></param>
        void enabled(Guid id, bool enabled, Guid modifer);

        /// <summary>
        /// 登录锁与解锁
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ulock"></param>
        /// <param name="modifer"></param>
        void loginLock(Guid id, bool ulock, Guid modifer);

        /// <summary>
        /// 删除用户。无法删除超级用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifer"></param>
        void deleteUser(Guid id, Guid modifer);

        /// <summary>
        /// 添加用户头像
        /// </summary>
        /// <param name="id"></param>
        /// <param name="avatar"></param>
        void addAvatar(Guid id, byte[] avatar);

        /// <summary>
        /// 用户自己修改密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        void changePassword(Guid id, string password);

        /// <summary>
        /// 设置用户最后多动时间
        /// </summary>
        /// <param name="id"></param>
        void lastActivityTime(Guid id);
    }
}
