using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace FRIWOServerApi.Data.TRACE
{
    [Table("PRODUCTION_STAFF")]
    public class ProductionStaff
    {
        [Column("CONTRACT")]
        public string Contract { get; set; }

        [Key]
        [Column("IDX")]
        public int ID { get; set; }

        [Column("EMPLOYEE_NO")]
        public string EmployeeNo { get; set; }

        [Column("EMPLOYEE_NAME")]
        public string EmployeeName { get; set; }

        [Column("POSITION")]
        public string EmployeePosition { get; set; }

        [Column("SIT")]
        public int Sit { get; set; }

        [Column("FLAG")]
        public int Flag { get; set; }
    }
}