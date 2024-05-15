namespace FastLane.Service.Excel
{
    public interface IExcelService
    {
        Task ImportFromExcel(IFormFile file);
        Task ExportExcel(List<Models.Order> orders, Stream stream);
    }
}
