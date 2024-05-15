using FastLane.Entities;
using FastLane.Models;
using FastLane.Repository.Order;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Globalization;
using System.Net.Http.Headers;

namespace FastLane.Service.Excel
{
    public class ExcelService : IExcelService
    {
        private readonly IOrderRepository _orderRepository;
        public ExcelService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task ExportExcel(List<Models.Order> orders, Stream stream)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.Add("List of booking today!!");

                // Header row
                worksheet.Cells["A1"].Value = "ID";
                worksheet.Cells["B1"].Value = "Name";
                worksheet.Cells["C1"].Value = "Email";
                worksheet.Cells["D1"].Value = "Phone";
                worksheet.Cells["E1"].Value = "Passport Number";
                worksheet.Cells["F1"].Value = "Nationality";
                worksheet.Cells["G1"].Value = "Flight Number";
                worksheet.Cells["H1"].Value = "Airport";
                worksheet.Cells["I1"].Value = "Group Reference";
                worksheet.Cells["J1"].Value = "Service";
                worksheet.Cells["K1"].Value = "Status";
                worksheet.Cells["L1"].Value = "Employee";
                worksheet.Cells["M1"].Value = "Note";
                worksheet.Cells["N1"].Value = "Operator Note";
                worksheet.Cells["O1"].Value = "Created At";
                worksheet.Cells["P1"].Value = "Updated At";
                worksheet.Cells["Q1"].Value = "Service Time";

                // Data rows
                for (var i = 0; i < orders.Count; i++)
                {
                    var row = i + 2; // Start from row 2 (1 for header)
                    var order = orders[i];

                    worksheet.Cells[$"A{row}"].Value = order.Id;
                    worksheet.Cells[$"B{row}"].Value = order.Name;
                    worksheet.Cells[$"C{row}"].Value = order.Email;
                    worksheet.Cells[$"D{row}"].Value = order.Phone;
                    worksheet.Cells[$"E{row}"].Value = order.Passport_Number;
                    worksheet.Cells[$"F{row}"].Value = order.Nationality;
                    worksheet.Cells[$"G{row}"].Value = order.Flight_Number;
                    worksheet.Cells[$"H{row}"].Value = order.AirPort;
                    worksheet.Cells[$"I{row}"].Value = order.GroupReference;
                    worksheet.Cells[$"J{row}"].Value = order.Service;
                    worksheet.Cells[$"K{row}"].Value = order.Status;
                    worksheet.Cells[$"L{row}"].Value = order.Employee;
                    worksheet.Cells[$"M{row}"].Value = order.Note;
                    worksheet.Cells[$"N{row}"].Value = order.Operator_Note;

                    // Convert DateTime to string format for created_at and updated_at
                    var createdAtString = order.Created_at.ToString("yyyy-MM-dd HH:mm:ss");
                    var updatedAtString = order.Updated_at.ToString("yyyy-MM-dd HH:mm:ss");
                    var serviceTimeString = order.Service_Time.ToString("yyyy-MM-dd HH:mm:ss");

                    worksheet.Cells[$"O{row}"].Value = createdAtString;
                    worksheet.Cells[$"P{row}"].Value = updatedAtString;
                    worksheet.Cells[$"Q{row}"].Value = serviceTimeString;
                }

                // Auto-fit columns for better readability
                worksheet.Cells.AutoFitColumns();

                // Style header row
                using (var range = worksheet.Cells["A1:Q1"])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                await package.SaveAsync();
            }
        }

        Task IExcelService.ImportFromExcel(IFormFile file)
        {
            throw new NotImplementedException();
        }
    }
}
