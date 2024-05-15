using FastLane.Dtos.Agency;
using FastLane.Dtos.Order;
using FastLane.Entities;
using FastLane.Repository.Order;
using FastLane.Service.Excel;
using FastLane.Service.Order;
using FastLane.Service.Passport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Globalization;

namespace FastLane.Controllers
{
    //[Authorize(Roles ="sale")]
    [Route("order")]
    [ApiController]
    public class OrderController : Controller
    {

        private readonly IOrderService _orderService;
        private readonly IExcelService _excelService;
        private readonly IOrderRepository _orderRepository;
        public OrderController(IOrderService orderService, 
                               IExcelService excelService,
                               IOrderRepository orderRepository)
        {
            _excelService = excelService;
            _orderService = orderService;
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders(int? index, int? pageSize, int? user_id)
        {
            var orders = await _orderService.GetAllOrders(index, pageSize, user_id);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int? id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return Ok(order);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                    var result = await _orderService.CreateOrderAsync(request);
                    if (!result)
                    {
                        return StatusCode(500, new { Message = "Failed to create order" });
                    }

                return Ok(new { Message = "Orders created successfully" });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Failed to create order", Error = ex.Message });
            }
        }

        [HttpPost("update/{id}")]
        public async Task<IActionResult> UpdateOrder(int? id, EditOrder_Dto order)
        {
            var result = await _orderService.UpdateOrderAsync(id, order);
            if (result)
            {
                return Ok(new { Message = "Order updated successfully" });
            }
            else
            {
                return NotFound(new { Message = "Order not found" });
            }
        }

        [HttpDelete("delete/{Id}")]
        public async Task<IActionResult> DeleteOrder(int? Id)
        {
            var result = await _orderService.DeleteOrderAsync(Id);
            if (result)
            {
                return Ok(new { Message = "Order deleted successfully" });
            }
            else
            {
                return NotFound(new { Message = "Order not found" });
            }
        }

        [HttpPost("search")]
        public async Task<IActionResult> OrderSearch(OrderParams request)
        {
            var orders = await _orderService.SearchOrderAsync(request);
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> GetOrderData(int year, string airport = null)
        {
            try
            {
                var orderData =  _orderService.GetTotalBookingByMonth(year,airport);
                return Ok(orderData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("report/agency/month")]
        public async Task<ActionResult<List<AgencyOrderData>>> GetAgencyOrderDatatById(int? agency_Id, int year)

        {
            try
            {
                var orderData = await _orderService.GetAgencyOrderDataByIdAndMonth(agency_Id, year);
                return Ok(orderData);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("report/agency")]
        public async Task<ActionResult<List<AgencyOrderData>>> GetAgencyOrderDatatByIdAndYear(int? agency_Id, int year)

        {
            try
            {
                var orderData = await _orderService.GetAgencyOrderDataByIdAndYear(agency_Id, year);
                return Ok(orderData);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("agency")]
        public async Task<IActionResult> GetOrderDetailsByAgencyId(Order_Agency_Input request)
        {
            try
            {
                var orderFinal = await _orderService.GetOrderDetailsByAgencyIdAsync(request);

                if (orderFinal == null)
                {
                    return NotFound("AgencyId is null. Please provide a valid agencyId.");
                }

                return Ok(orderFinal);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }



        [HttpPost("export")]
        public async Task<IActionResult> ExportOrderToExcel([FromBody] Order_Excel_Input request)
        {
            try
            {
                var orders = await _orderRepository.GetAllOrdersAsync(request.fromDate, request.toDate);

                using (var stream = new MemoryStream())
                {
                    await _excelService.ExportExcel(orders, stream);
                    var fileName = $"Orders_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

    }
}
