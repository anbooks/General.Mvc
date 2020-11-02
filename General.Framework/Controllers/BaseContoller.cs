using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;


namespace General.Framework.Controllers
{
   public abstract class BaseContoller : Controller
    {
        private AjaxResult _ajaxResult;
        private AjaxResultImage _ajaxResultImage;
        public BaseContoller()
        {
            this._ajaxResult = new AjaxResult();
            this._ajaxResultImage = new AjaxResultImage();
        }

        /// <summary>
        /// ajax请求的数据结果
        /// </summary>
        public AjaxResult AjaxData
        {
            get
            {
                return _ajaxResult;
               // return new AjaxResult();   //这样new的化就总是false
            }
        }

        public AjaxResultImage AjaxDataImage
        {
            get
            {
                return _ajaxResultImage;
                // return new AjaxResult();   //这样new的化就总是false
            }
        }
    }
}
