using FastLane.Dtos.Agency;
using FastLane.Dtos.Order;
using FastLane.Dtos.Order.Report;

namespace FastLane.Service.Order
{
    public interface IOrderService
    {
        Task<Models.OrderFinal> GetAllOrders(int? index, int? pageSize, int? user_id);
        Task<Models.Order_Detail> GetOrderByIdAsync(int? id);
        Task<bool> CreateOrderAsync(CreateOrderRequest order);
        Task<bool> UpdateOrderAsync(int? id, EditOrder_Dto order);
        Task<bool> DeleteOrderAsync(int? id);
        Task<Models.OrderFinal> SearchOrderAsync(OrderParams request);
        List<Year_Order_Data> GetTotalBookingByMonth(int year, string airport = null);

        //Get all order in month and year by agency
        Task<AgencyOrderData> GetAgencyOrderDataByIdAndMonth(int? agency_Id, int year);
        Task<Agency_Order_Data_Year> GetAgencyOrderDataByIdAndYear(int? agency_Id, int year);


        //Get all order in month and year in db
        Task<Month_Order_Data> Get_All_Month_Orders_Data(int year);
        Task<Year_Order_Data> Get_Year_Order_Data(int year);

        Task<Models.Order_Agency_Final> GetOrderDetailsByAgencyIdAsync(Order_Agency_Input request);
    }
}
