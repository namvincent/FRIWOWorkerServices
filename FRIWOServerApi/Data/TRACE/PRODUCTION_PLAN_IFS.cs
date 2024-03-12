using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FRIWOServerApi.Data.TRACE
{
    [Table("PRODUCTION_PLAN_IFS", Schema = "TRACE")]
    public class ShopOrdersIFS
    {
        [Column("CONTRACT")]
        public string CONTRACT { get; set; }

        [Column("DEPARTMENT_NO")]
        public string DEPARTMENT_NO { get; set; }

        [Column("DEPARTMENT")]
        public string DEPARTMENT { get; set; }

        //[Column("CREW_SIZE")]
        //public int CREW_SIZE { get; set; }
        [Key]
        [Column("ORDER_NO")]
        public string ORDER_NO { get; set; }

        [Column("RELEASE_NO")]
        public string RELEASE_NO { get; set; }

        [Column("SEQUENCE_NO")]
        public string SEQUENCE_NO { get; set; }

        [Column("PART_NO")]
        public string PART_NO { get; set; }

        [Column("PART_DESCRIPTION")]
        public string PART_DESCRIPTION { get; set; }

        [Column("REVISED_QTY_DUE")]
        public int REVISED_QTY_DUE { get; set; }

        [Column("QTY_TILL_FINISH")]
        public int QTY_TILL_FINISH { get; set; }

        //[Column("NOTE")]
        //public string? NOTE { get; set; }
        [Column("MACH_RUN_FACTOR_MIN_BY_PARTNO")]
        public float? MACH_RUN_FACTOR_MIN_BY_PARTNO { get; set; }

        [Column("MACH_RUN_FACTOR_MIN_BY_SO")]
        public float? MACH_RUN_FACTOR_MIN_BY_SO { get; set; }

        //[Column("OBJSTATE")]
        //public string OBJSTATE { get; set; }
        //[Column("OPERATION_PRIORITY")]
        //public int OPERATION_PRIORITY { get; set; }

        //[Column("LABOR_CLASS_NO")]
        //public string LABOR_CLASS_NO { get; set; }
        //[Column("WORK_CENTER_NO")]
        //public string WORK_CENTER_NO { get; set; }
        //[Column("STATION")]
        //public string STATION { get; set; }
        //[Column("NEED_DATE")]
        //public DateTime? NEED_DATE { get; set; }
    }
}