using FastLane.Dtos.Order;

namespace FastLane.Repository.Order
{
    public interface IOrderRepository
    {
        Task<Models.OrderFinal> GetAllOrders(int? index, int? pageSize, int? user_id);
        Task<List<Models.Order>> GetAllOrdersAsync(DateTime fromDate, DateTime toDate);
        Task<Models.OrderFinal> SearchOrdersAsync(OrderParams request);
        Task<Models.Order_Detail> GetOrderById(int? id);
        Task<Entities.Order> GetOrderByIdAsync(int? id); // Aim to get data from order table  and then update order with customer id
        Task<bool> CreateOrder(Entities.Order order);
        Task<bool> UpdateOrder(Entities.Order order);
        Task<bool> DeleteOrder(int? id);
        Task<Models.Order_Agency_Final> GetOrderDetailsByAgencyIdAsync(Order_Agency_Input request);
    }
}
