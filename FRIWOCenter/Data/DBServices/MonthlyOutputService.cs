using FRIWOCenter.Data.TRACE;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;

namespace FRIWOCenter.DBServices
{
    public class MonthlyOutputService : ControllerBase
    {
        private readonly IDbContextFactory<TraceDbContext> _contextFactory;
        public MonthlyOutputService(IDbContextFactory<TraceDbContext> context)
        {
            _contextFactory = context;
        }
        List<Monthly_Output> KPI_output;
        public async Task<List<Monthly_Output>> getMonthly()
        {
            KPI_output = await CallPackageTest("", "");
            return KPI_output;
        }
        public async Task<List<Monthly_Output>> CallPackageTest(String fromtime, String totime)
        {
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                List<Monthly_Output> datas = new List<Monthly_Output>();

                var parameters = new OracleParameter[]
                {
                new OracleParameter("p_from", fromtime),
                new OracleParameter("p_to",totime),
                new OracleParameter("p_refcursor", OracleDbType.RefCursor, ParameterDirection.ReturnValue)


                };
                string query = "BEGIN TRS_REPORT_PKG.GET_KPI_MONTHLY_PRC(:p_from, :p_to, :p_refcursor); END;";

                var result = await _context.KPI_Monthly
                    .FromSqlRaw(query, parameters)
                    .AsNoTracking()
                    .ToListAsync();

                foreach (var item in result)
                {
                    var outputs = new Monthly_Output();
                    outputs.MONTHH = item.MONTHH;
                    outputs.PART_NO = item.PART_NO;
                    outputs.QTY = item.QTY;
                    outputs.PROD_TIME = item.PROD_TIME;
                    datas.Add(outputs);

                }
                return datas;
            }
            
        }


        public async Task<MemoryStream> CreateExcel(List<Monthly_Output> datas)
        {
            ExcelPackage excel = new ExcelPackage();
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                //Create an instance of ExcelEngine.
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                
                var worksheet = excel.Workbook.Worksheets.Add("Sheet 1");
                List<Monthly_Output> data = new List<Monthly_Output>();
                int row = 1;
                worksheet.Cells[row, 1].Value = "Month";
                worksheet.Cells[row, 2].Value = "Part NO";
                worksheet.Cells[row, 3].Value = "Qty";
                worksheet.Cells[row, 4].Value = "PRO Time";
                row++;
                foreach (Monthly_Output value in datas)
                {
                    worksheet.Cells[row, 1].Value = value.MONTHH;
                    worksheet.Cells[row, 2].Value = value.PART_NO;
                    worksheet.Cells[row, 3].Value = value.QTY;
                    worksheet.Cells[row, 4].Value = value.PROD_TIME;
                    row++;

                }
            }
                //Save the document as a stream and return the stream.
                using (MemoryStream stream = new MemoryStream())
                {
                    //Save the created Excel document to MemoryStream.
                    excel.SaveAs(stream);
                    return stream;
                }
            
        }
        public async Task<DataTable> SampleDataTable()
        {
            using (var _context = await _contextFactory.CreateDbContextAsync())
            {
                DataTable reports = new DataTable();

                reports.Columns.Add("MONTH");
                reports.Columns.Add("PART");
                reports.Columns.Add("PROD TIME");
                reports.Columns.Add("QTY");

                List<Monthly_Output> data = new List<Monthly_Output>();
                data = await CallPackageTest("", "");
                foreach (Monthly_Output value in data)
                {
                    reports.Rows.Add(value.MONTHH, value.PART_NO, value.PROD_TIME, value.QTY);
                }


                return reports;
            }
        }

    }
}
