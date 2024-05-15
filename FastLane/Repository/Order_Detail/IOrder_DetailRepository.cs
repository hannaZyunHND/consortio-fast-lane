using DocumentFormat.OpenXml.Bibliography;
using FastLane.Repository.Order;

namespace FastLane.Repository.Order_Detail
{
    public interface IOrder_DetailRepository
    {
        Task<List<Entities.Order_Detail>> GetAllOrder_Details();
        Task<Entities.Order_Detail> GetOrder_DetailById(int? id);
        Task<bool> CreateOrder_Detail(Entities.Order_Detail order);
        Task<bool> UpdateOrder_Detail(Entities.Order_Detail order);
        Task<bool> DeleteOrder_Detail(int? id);
        Task<List<Entities.Order_Detail>> GetOrdersByGroupReference(string groupReference);
        Task<List<Entities.Order_Detail>> GetOrderDetailsByAgencyIdAndMonth(int? agency_Id, int year, int month);
        Task<List<Entities.Order_Detail>> GetOrderDetailsByAgencyIdAndYear(int? agency_Id, int year);

        //Get data for all order in this db
        Task<List<Entities.Order_Detail>> GetMonthOrderDetails(int year, int month);
        Task<List<Entities.Order_Detail>> GetYearOrderDetails(int year);

    }
}
