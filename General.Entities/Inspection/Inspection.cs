using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("Inspection")]
    public partial class Inspection
    {
        public int Id { get; set; }

        public string ContractNo { get; set; }//订单号
        public string Supplier { get; set; }//供应商
        public string Manufacturer { get; set; }//制造商
        public string CofC { get; set; }//合格证号
        public string Description { get; set; }//材料名称
        public string MaterialCode { get; set; }//物料代码
        public string Type { get; set; }//牌号/图号
        public string Size { get; set; }//规格
        public string Batch { get; set; }//质量编号
        public DateTime? ReceivedDate { get; set; }//入厂日期
        public string Specification { get; set; }//材料规范
        public double? Qty { get; set; }//采购数量
        public string Remark { get; set; }//下达数量
        public double? UnPlaceQty { get; set; }//可下达数量
        public double? AcceptQty { get; set; }//接收数量
        public string Creator { get; set; }//
        public DateTime? CreationTime { get; set; }//AcceptTime
        public DateTime? AcceptTime { get; set; }//AcceptTime
        public string Modifier { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public string Status { get; set; }
        public string DateId { get; set; }
        public string Item { get; set; }
        public string Project { get; set; }
        public string Keeper { get; set; }//保管
        public string Inspector { get; set; }//检验
        public bool?IsDeleted { get; set; }
        public int?flag { get; set; }
        public int?Serial { get; set; }
        public int MainId { get; set; }
        [ForeignKey("MainId")]
        public virtual InspecationMain Main { get; set; }
    }
}
