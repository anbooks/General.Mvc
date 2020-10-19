using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace General.Entities
{
    [Table("test_JqGrid")]
    public class test_JqGrid
    {
        public int Id { get; set; }//

        public string Name { get; set; }//名字
        public string Stock { get; set; }//是否存货
        public string ShipVia { get; set; }//发运方式
        public string Notes { get; set; }//备注


        [DataType(DataType.Date)]
        public DateTime? LastSales { get; set; } //销售时间  //  ？代表可以是空的，这样处理的时候不会出错
       
    }
}
