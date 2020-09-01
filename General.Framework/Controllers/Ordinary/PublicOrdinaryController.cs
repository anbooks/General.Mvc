using General.Core;
using General.Framework.Filters;
using General.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Framework.Controllers.Ordinary
{
    [OrdinaryAuthFilter]
    public class PublicOrdinaryController : OrdinaryAreaController
    {
        private IWorkContext _workContext;

        public PublicOrdinaryController()
        {
            this._workContext = EnginContext.Current.Resolve<IWorkContext>();
        }

        /// <summary>
        /// 
        /// </summary>
        public IWorkContext WorkContext { get { return _workContext; } }

    }
}
