using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Core.Librs;
using General.Entities;
using General.Framework.Controllers.Admin;
using General.Framework.Datatable;
using General.Framework.Filters;
using General.Framework.Menu;
using General.Services.SysUser;
using General.Services.SysUserRole;
using General.Services.SysRole;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using General.Services.SysCustomizedList;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/users")]
    // public class UserController :Controller   初始的时候是这个样子的，下面的才是有权限的用户才能对这个表操作
    public class UserController : AdminPermissionController
    {

        private ISysUserService _sysUserService;
        private ISysUserRoleService _sysUserRoleService;
        private ISysRoleService _sysRoleService;
        private ISysCustomizedListService _sysCustomizedListService;
        private IHostingEnvironment _hostingEnvironment;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        public UserController(ISysCustomizedListService sysCustomizedListService, ISysUserService sysUserService,ISysUserRoleService sysUserRoleService, ISysRoleService sysRoleService, IHostingEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            this._sysUserService = sysUserService;
            this._sysUserRoleService = sysUserRoleService;
            this._sysRoleService = sysRoleService;
            this._sysCustomizedListService = sysCustomizedListService;
            this._hostingEnvironment = hostingEnvironment;
            this._httpContextAccessor = httpContextAccessor;
        }
        [Route("roledetail", Name = "roleDetail")]
        [Function("用户角色", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.UserController.UserIndex")]
        [HttpGet]
        public ActionResult RoleDetail(Guid id)
        {
            UserRoleViewModel model = new UserRoleViewModel();
            model.RoleList = _sysRoleService.getAllRoles();
            ViewBag.Userid = id;
            var roleList = _sysUserRoleService.getAll();
            if (roleList != null && roleList.Any())
            {
                model.User = _sysUserService.getById(id); ;
                model.UserRoleList = roleList;
                //model.Permissions = _sysPermissionService.getByRoleId(id);
            }
            return View(model);
        }

        [HttpPost]
        [Route("roledetail")]
        public ActionResult RoleDetail(Guid id,Guid sysResource)
        {
            var model = _sysUserRoleService.getById(id);
            //var model = "";
            ViewBag.ReturnUrl = Url.IsLocalUrl(null) ? null : Url.RouteUrl("userIndex");
            //  _sysPermissionService.saveRolePermission(id, sysResource, WorkContext.CurrentUser.Id);
            if (model == null)
            {
                SysUserRole modela = new SysUserRole();
                var item = _sysRoleService.getRole(sysResource);

                modela.UserId =id;
                modela.RoleId = sysResource;
                modela.RoleName = item.Name;
                _sysUserRoleService.insertSysUserRole(modela);
            }
            else
            {
                var item = _sysRoleService.getRole(sysResource);
                model.RoleName = item.Name;
                model.RoleId = sysResource;

                _sysUserRoleService.updateSysUserRole(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }
      //  [Route("roledetail", Name = "roleDetail")]
      //  [Function("用户角色", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.UserController.UserIndex")]

      //  public IActionResult RoleDetail(Guid? id, string returnUrl = null)
     //   {
    //       ViewBag.Userid = id;
    //        ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("roleDetail");
     //       return View(_sysRoleService.getAllRoles());
     //   }
        [HttpGet]
        [Route("roleUser", Name = "roleUser")]
        [Function("编辑用户角色", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.UserController.UserIndex")]

        public IActionResult RoleUser(Guid id, Guid userid, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("userIndex");
            if (id != null)
            {
                var model = _sysUserRoleService.getById(userid);
                //var model = "";
                var modelab = 0;
                if (model == null) {
                    Guid gv = new Guid("4baaba32-e96b-4a16-8cad-e7bf6d004449");
                    //gv = new Guid();
                    model = _sysUserRoleService.getById(gv);
                     modelab =1;
                }
                //ModelState.Remove("Id");
                // ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("roleDetail");
                // if (!String.IsNullOrEmpty(model.MobilePhone))
                //     model.MobilePhone = StringUitls.toDBC(model.MobilePhone);
                //  model.Name = model.Name.Trim();

                if (modelab == 1)
                {
                    model.Id = Guid.NewGuid();
                    model.UserId = userid;
                    model.RoleId = id;
                    _sysUserRoleService.insertSysUserRole(model);
                }
                else
                {
                    model.UserId = userid;
                    model.RoleId = id;

                    _sysUserRoleService.updateSysUserRole(model);
                }
                return Redirect(ViewBag.ReturnUrl);
            }
            return Redirect(ViewBag.ReturnUrl);
        }
        [HttpPost]
        [Route("roleUser")]
        public ActionResult RoleUser(Entities.SysUserRole model, Guid id,int modelab,string returnUrl = null)
        {
            ModelState.Remove("Id");
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("roleDetail");
            if (!ModelState.IsValid)
                return View(model);
            // if (!String.IsNullOrEmpty(model.MobilePhone))
            //     model.MobilePhone = StringUitls.toDBC(model.MobilePhone);
            //  model.Name = model.Name.Trim();

            if (modelab == 1)
            {
                model.Id = Guid.NewGuid();
                model.UserId = WorkContext.CurrentUser.Id;
                model.RoleId = id;
                _sysUserRoleService.insertSysUserRole(model);
            }
            else
            {
                model.RoleId = id;

                 _sysUserRoleService.updateSysUserRole(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }
        //[Function("系统用户", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.SystemManageController", Sort = 0)]
        //[Route("", Name = "userIndex")]
        //public IActionResult UserIndex()
        //{
        //    return View();
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Function("系统用户", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.SystemManageController", Sort = 0)]
        [Route("", Name = "userIndex")]
        public IActionResult UserIndex(SysUserSearchArg arg, int page = 1, int size = 20)
        {
            var pageList = _sysUserService.searchUser(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.SysUser, SysUserSearchArg>("userIndex", arg);
        
            return View(dataSource);
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [Function("修改密码", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.UserController.UserIndex")]
        [Route("password", Name = "password")]
        public IActionResult EditPassword(Guid? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("mainIndex");
            if (id != null)
            {
                int guid=0;
                var model = _sysUserService.getById(id.Value);
                if (model == null)
                    return Redirect(ViewBag.ReturnUrl);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        [Route("password")]
        public ActionResult EditPassword(Entities.ModifyModel modela, string returnUrl = null)
        {
            ModelState.Remove("Id");
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("mainIndex");
            if (!ModelState.IsValid)
                return View(modela);
            var model = _sysUserService.getById(WorkContext.CurrentUser.Id);

            if (model.Password == EncryptorHelper.GetMD5(modela.OriginalPassword.Trim() + model.Salt))
            {
                model.Password = EncryptorHelper.GetMD5(modela.ConfirmedPassword.Trim() + model.Salt); //model.Name.Trim();;
                //model.Modifier = WorkContext.CurrentUser.Id;
                _sysUserService.updatePassword(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }


        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("message", Name = "usermessages")]
        [Function("个人信息", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.UserController.UserIndex")]
        public IActionResult Usermessages(Guid? id, Entities.SysUserMessage model, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("mainIndex");
            var item= _sysUserService.getById(WorkContext.CurrentUser.Id);
            string host_port = Request.Host.Value;
            model.Email = item.Email;
            model.MobilePhone = item.MobilePhone;
            var AvatarOr= item.Avatar2;
            model.Avatar2 = "http://" + host_port + AvatarOr;
            if (model == null)
                return Redirect(ViewBag.ReturnUrl);
            return View(model);


        }
        [HttpPost]
        [Route("message")]
        public ActionResult Usermessages(Entities.SysUserMessage modela, string returnUrl = null)
        {
            ModelState.Remove("Id");
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("mainIndex");
            var model = _sysUserService.getById(WorkContext.CurrentUser.Id);
            if (!ModelState.IsValid)
                return View(modela);
            if (!String.IsNullOrEmpty(modela.MobilePhone))
                model.MobilePhone = StringUitls.toDBC(modela.MobilePhone);
            if (!String.IsNullOrEmpty(modela.Email))
                model.Email = StringUitls.toDBC(modela.Email);
            //model.Name = model.Name.Trim();

            if (model.Id == Guid.Empty)
            {
                return Redirect(ViewBag.ReturnUrl);
            }
            else
            {
                model.ModifiedTime = DateTime.Now;
                model.Modifier = WorkContext.CurrentUser.Id;
                _sysUserService.updateUsermessage(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }
        [HttpGet]
        [Route("edit", Name = "editUser")]
        [Function("编辑系统用户", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.UserController.UserIndex")]
        public IActionResult EditUser(Guid? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("userIndex");
            var customizedList = _sysCustomizedListService.getByAccount("运输代理");
            ViewData["Colist"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");

            if (id != null)
            {
                var model = _sysUserService.getById(id.Value);
                if (model == null)
                    return Redirect(ViewBag.ReturnUrl);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        [Route("edit")]
        public ActionResult EditUser(Entities.SysUser model, string returnUrl = null)
        {
            ModelState.Remove("Id");
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("userIndex");
            if (!ModelState.IsValid)
                return View(model);
            if (!String.IsNullOrEmpty(model.MobilePhone))
                model.MobilePhone = StringUitls.toDBC(model.MobilePhone);
            if (!String.IsNullOrEmpty(model.Co))
                model.Co = StringUitls.toDBC(model.Co);
            if (!String.IsNullOrEmpty(model.Port))
                model.Port = StringUitls.toDBC(model.Port);
            if (!String.IsNullOrEmpty(model.Transport))
                model.Transport = StringUitls.toDBC(model.Transport);
            model.Name = model.Name.Trim();

            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.Salt = EncryptorHelper.CreateSaltKey();
                model.Account = StringUitls.toDBC(model.Account.Trim());
                model.Enabled = true;
                model.IsAdmin = false;
                model.Password = EncryptorHelper.GetMD5("Sacc2020" + model.Salt);
                model.Creator = WorkContext.CurrentUser.Id;
                _sysUserService.insertSysUser(model);
            }
            else
            {
                model.ModifiedTime = DateTime.Now;
                model.Modifier = WorkContext.CurrentUser.Id;
                _sysUserService.updateSysUser(model);
            }
           // ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("mainIndex");
            return Redirect(Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("userIndex"));
        }
        /// <summary>
        /// 设置启用与禁用账号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        [Route("enabled", Name = "enabled")]
        [Function("设置启用与禁用账号", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.UserController.UserIndex")]
        public JsonResult Enabled(Guid id, bool enabled)
        {
            _sysUserService.enabled(id, enabled, WorkContext.CurrentUser.Id);
            AjaxData.Message = "启用禁用设置完成";
            AjaxData.Status = true;
            return Json(AjaxData);
        }


        /// <summary>
        /// 设置登录锁解锁与锁定
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enabled"></param>
        /// <returns></returns>
        [Route("loginLock", Name = "loginLock")]
        [Function("设置登录锁解锁与锁定", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.UserController.UserIndex")]
        public JsonResult LoginLock(Guid id, bool loginLock)
        {
            _sysUserService.loginLock(id, loginLock, WorkContext.CurrentUser.Id);
            AjaxData.Message = "登录锁状态设置完成";
            AjaxData.Status = true;
            return Json(AjaxData);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("delete/{id}", Name = "deleteUser")]
        [Function("删除用户", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.UserController.UserIndex")]
        public JsonResult DeleteUser(Guid id)
        {
            _sysUserService.deleteUser(id, WorkContext.CurrentUser.Id);
            AjaxData.Status = true;
            AjaxData.Message = "删除完成";
            return Json(AjaxData);
        }

        /// <summary>
        /// 远程验证账号是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        [Route("existAccount", Name = "remoteAccount")]
        //[PermissionActionFilter(true)]   //#Kevin 留存功能-不会使用
        public JsonResult RemoteAccount(string account)
        {
            account = account.Trim();
            return Json(!_sysUserService.existAccount(account));
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("resetPwd/{id}", Name = "resetPassword")]
        [Function("重置密码", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.UserController.UserIndex")]
        public ActionResult ResetPassword(Guid id)
        {
            var modelpass = _sysUserService.getById(id);
            modelpass.Password = EncryptorHelper.GetMD5("Sacc2020" + modelpass.Salt);
            modelpass.Modifier = WorkContext.CurrentUser.Id;
            _sysUserService.resetPassword(modelpass);
            AjaxData.Status = true;
            AjaxData.Message = "用户密码已重置为原始密码";
           // return Json(AjaxData);
            return Redirect(Url.IsLocalUrl(null) ? null : Url.RouteUrl("userIndex"));
        }


        [HttpPost]
        [Route("avatarChange", Name = "avatarChange")]
        public ActionResult AvatarChange([FromForm(Name = "avatar")]List<IFormFile> files)
        {
            string str = (Request.HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString() + ":" + Request.HttpContext.Connection.LocalPort);


            string clientIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();


            //服务器网卡信息
            NetworkInterface[] networks = NetworkInterface.GetAllNetworkInterfaces();
            string serverIpAddresses = string.Empty;
            foreach (var network in networks)
            {
                var ipAddress = network.GetIPProperties().UnicastAddresses.Where(p => p.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(p.Address)).FirstOrDefault()?.Address.ToString();

                serverIpAddresses += network.Name + ":" + ipAddress + "|";
            }

            string test = serverIpAddresses;

            //获取整个url地址：
           string  test_url= Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(Request);
            //访问的方式http
            string test_http = Request.Scheme;   

            //获取域名（不带端口号）
            string test_host = Request.Host.Host;

            //获取域名（带端口号）[Get the host]: localhost:4800
            string host_port = Request.Host.Value;

            //获取端口号（Get port）: 4800 (if a url contains port)
            var test_port = Request.Host.Port;


            //var files = Request.Form.Files;
            //string test= Request.Form["phone"].ToString();
            string show_Url = "";
            files.ForEach(file =>
            {
                var fileName = file.FileName;
                string fileExtension = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1);//获取文件名称后缀 
                //保存文件
                var stream = file.OpenReadStream();
                // 把 Stream 转换成 byte[] 
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                // 设置当前流的位置为流的开始 
                stream.Seek(0, SeekOrigin.Begin);
                // 把 byte[] 写入文件 

                //需要写一个获取文件路径的方法  Kevin？
                // var path = Directory.GetCurrentDirectory();
                var currentDate = DateTime.Now;
                var webRootPath = _hostingEnvironment.WebRootPath;//>>>相当于HttpContext.Current.Server.MapPath("") 

                //F:\Code\ILPT\General.Mvc\General.Mvc\wwwroot
                var fileProfile = webRootPath + "\\Files\\profile\\";

                var fileNameFinal = WorkContext.CurrentUser.Id+ "-"+fileName;//用gid存放图像的照片

                FileStream fs = new FileStream(fileProfile + fileNameFinal, FileMode.Create);  //同名替换
                //FileStream fs = new FileStream("D:\\" + file.FileName, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(bytes);
                bw.Close();
                fs.Close();

             

                show_Url = "/Files/profile/" + fileNameFinal;


                //操作model中的值给数据库赋值 Kevin?
              
                var model = _sysUserService.getById(WorkContext.CurrentUser.Id);
                model.Avatar2 = show_Url;
                _sysUserService.updateUsermessage(model);

            });

            //实现返回值的设置 Kevin？
            AjaxDataImage.Status = "OK";
            AjaxDataImage.Message = "上传成功";

            AjaxDataImage.Url = show_Url;

            return Json(AjaxDataImage);


        }


    }
}