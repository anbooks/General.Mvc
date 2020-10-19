using System;
using System.Collections.Generic;
using System.Text;

namespace General.Entities
{
    /// <summary>
    /// 查询参数
    /// </summary>
    public class SysCustomizedListSearchArg
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public string keyword { get; set; }//下拉列表关键字
                                       /// 冻结、启用
                                       /// </summary>
        public string itemno { get; set; }//发运条目编号
        public string shipper { get; set; }//发运条目发货人
        public string pono { get; set; }//发运条目合同号
        public string invcurr { get; set; }//发运条目货币类型
    }
}
