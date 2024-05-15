using DocumentFormat.OpenXml.Office2010.Excel;
using FastLane.Context;
using FastLane.Dtos.Order;
using FastLane.Dtos.Status;
using FastLane.Entities;
using FastLane.Models;
using FastLane.Repository.Airport;
using FastLane.Repository.Employee;
using FastLane.Repository.Order_Detail;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FastLane.Repository.Order
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrder_DetailRepository _order_DetailRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAirportRepository _airportRepository;
        public OrderRepository(ApplicationDbContext context, IOrder_DetailRepository order_DetailRepository, 
                            IEmployeeRepository employeeRepository, IAirportRepository airportRepository)
        {
            _context = context;
            _employeeRepository = employeeRepository;
            _airportRepository = airportRepository;
            _order_DetailRepository = order_DetailRepository;
        }
        public async Task<bool> CreateOrder(Entities.Order order)
        {
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteOrder(int? id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            var order = await _context.Orders.FirstOrDefaultAsync(r => r.Id == id);
            if (order == null)
            {
                return false;
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();
            return true;
        }


        public async Task<List<Models.Order>> GetAllOrdersAsync(DateTime fromDate, DateTime toDate)
        {
            var orderDetailsQuery = await _context.Order_Details
                                    .Include(od => od.Employee)
                                    .Include(od => od.Orders)
                                    .ThenInclude(o => o.Customer)
                                    .Where(od => od.Service_Time >= fromDate && od.Service_Time <= toDate)
                                    .OrderByDescending(od => od.Created_at)
                                    .ToListAsync();

            var mappedOrderDetails = new List<Models.Order>();

            foreach (var od in orderDetailsQuery)
            {
                var statusName = await _context.Statuses
                    .Where(r => r.Id == od.Status_ID)
                    .Select(r => r.Name)
                    .FirstOrDefaultAsync();

                var serviceName = await _context.Services
                    .Where(r => r.Id == int.Parse(od.Service_ID))
                    .Select(r => r.Name)
                    .FirstOrDefaultAsync();

                var customer = od.Orders.Customer;
                var employee = od.Employee != null ? od.Employee.User.Customer.Name : "Unknown";

                var mappedOrderDetail = new Models.Order
                {
                    Id = od.Id,
                    Status = statusName,
                    Name = customer.Name,
                    Email = customer.Email,
                    Employee = employee,
                    Phone = od.Phone,
                    Passport_Number = od.Passport_Number,
                    Nationality = od.Nationality,
                    Flight_Number = od.Flight_Number,
                    Passport_File = od.Passport_Path,
                    Visa_File = od.Visa_Path,
                    AirPort = od.AirPort,
                    GroupReference = od.GroupReference,
                    Service = serviceName,
                    Note = od.Note,
                    Created_at = od.Created_at,
                    Updated_at = od.Updated_at,
                    Service_Time = od.Service_Time,
                    Operator_Note = od.Operator_Note,
                };

                mappedOrderDetails.Add(mappedOrderDetail);
            }

            return mappedOrderDetails;
        }


        public async Task<Models.OrderFinal> GetAllOrders(int? index, int? pageSize, int? customer_id)
        {
            int pageIndex = index ?? 1;
            int size = pageSize ?? 20;
            int skip = (pageIndex - 1) * size;

            if (customer_id == null)
            {
                throw new KeyNotFoundException($"User with ID {customer_id} not found");
            }

            IQueryable<Entities.Order_Detail> orderDetailsQuery = null;

            var userDB = await _context.Users.FirstOrDefaultAsync(r => r.Customer_ID == customer_id);
            
            var employeeDB = await _context.Employee.FirstOrDefaultAsync(r => r.User_Id == userDB.Id);

            if (employeeDB != null)
            {
                var aiportDB = await _airportRepository.GetAirportByIdAsync(employeeDB.Airport_Id);

                orderDetailsQuery = _context.Order_Details
                                   .Include(od => od.Employee)
                                       .ThenInclude(e => e.User)
                                   .Include(od => od.Employee)
                                       .ThenInclude(e => e.Airport)
                                   .Include(od => od.Orders)
                                       .ThenInclude(o => o.Customer)
                                   .Where(od => od.AirPort == aiportDB.Name);
            }
            else
            {
                orderDetailsQuery = _context.Order_Details
                                       .Include(od => od.Employee)
                                           .ThenInclude(e => e.User)
                                       .Include(od => od.Employee)
                                           .ThenInclude(e => e.Airport)
                                       .Include(od => od.Orders)
                                           .ThenInclude(o => o.Customer);
            }


            int totalCount = await orderDetailsQuery.CountAsync();

            var orderDetails = await orderDetailsQuery
                                    .OrderByDescending(r => r.Created_at)
                                    .Skip(skip)
                                    .Take(size)
                                    .ToListAsync();

            var mappedOrderDetails = new List<Models.Order>();

            foreach (var od in orderDetails)
            {
                var statusName = await _context.Statuses
                    .Where(r => r.Id == od.Status_ID)
                    .Select(r => r.Name)
                    .FirstOrDefaultAsync();

                var status_operator = await _context.Statuses
                    .Where(r => r.Id == od.Status_Operator_ID)
                    .Select(r => r.Name)
                    .FirstOrDefaultAsync();

                var status_sale = await _context.Statuses
                   .Where(r => r.Id == od.Status_Sales_Id)
                   .Select(r => r.Name)
                   .FirstOrDefaultAsync();

                var updateBy = await _context.Customers
                   .Where(r => r.Id == od.UpdatedBy)
                   .Select(r => r.Name)
                   .FirstOrDefaultAsync();
                var createdName = await _context.Customers
                    .Where(r => r.Id == od.CreateBy)
                    .Select(r => r.Name)
                    .FirstOrDefaultAsync() ?? "";

                var customer = od.Orders.Customer;
                var employeeName = od.Employee != null ? od.Employee.Id.ToString() : "Unknown";
                var airportName = od.Employee?.Airport?.Name ?? "Unknown";

                Console.WriteLine("day là employee", employeeName);

                var mappedOrderDetail = new Models.Order
                {
                    Id = od.Id,
                    Status = statusName,
                    Status_Sales = status_sale,
                    Status_Operator = status_operator,
                    Name = customer.Name,
                    Email = customer.Email,
                    Employee = employeeName,
                    Phone = od.Phone,
                    Passport_Number = od.Passport_Number,
                    Nationality = od.Nationality,
                    Flight_Number = od.Flight_Number,
                    Passport_File = od.Passport_Path,
                    Visa_File = od.Visa_Path,
                    Portrait_File = od.Portrait_Path,
                    AirPort = od.AirPort,
                    GroupReference = od.GroupReference,
                    Service = od.Service_ID,
                    Note = od.Note,
                    UpdatedBy = updateBy,
                    Created_at = od.Created_at,
                    Updated_at = od.Updated_at,
                    Service_Time = od.Service_Time,
                    Operator_Note = od.Operator_Note,
                    CreatedName = createdName,
                };

                mappedOrderDetails.Add(mappedOrderDetail);
            }

            var orderFinal = new Models.OrderFinal
            {
                Orders = mappedOrderDetails,
                TotalCount = totalCount
            };

            return orderFinal;
        }

        public async Task<Models.OrderFinal> SearchOrdersAsync(OrderParams request)
        {
            int pageIndex = request.Index;
            int size = request.PageSize;
            int skip = (pageIndex - 1) * size;

            
            var orderQuery = _context.Order_Details
                .Include(od => od.Status)
                .Include(od => od.Orders)
                .ThenInclude(o => o.Customer)
                .Select(od => new
                {
                    OrderDetail = od,
                    StatusName = _context.Statuses.FirstOrDefault(s => s.Id == od.Status_ID).Name,
                    OperatorStatusName = _context.Statuses.FirstOrDefault(s => s.Id == od.Status_Operator_ID).Name,
                    SalesStatusName = _context.Statuses.FirstOrDefault(s => s.Id == od.Status_Sales_Id).Name,
                    EmployeeName = od.Employee.User.Customer.Name,
                    CustomerName = od.Orders.Customer.Name,
                    od.Orders.Customer.Email,
                    od.Phone,
                    od.AirPort,
                    od.GroupReference
                });

            if(request.Agency_Id != null)
            {
                orderQuery = orderQuery.Where(o => o.OrderDetail.CreateBy == request.Agency_Id);
            }

            if (request != null)
            {
                if (request.Status != null)
                {
                    orderQuery = orderQuery.Where(o => o.OrderDetail.Status.Id == request.Status);
                }

                if (request.toDate != null)
                {
                    orderQuery = orderQuery.Where(o => o.OrderDetail.Service_Time.Date <= request.toDate.Value.Date);
                }

                if (request.fromDate != null)
                {
                    orderQuery = orderQuery.Where(o => o.OrderDetail.Service_Time.Date >= request.fromDate.Value.Date);
                }

                if (!string.IsNullOrEmpty(request.AirPort))
                {
                    orderQuery = orderQuery.Where(o => o.OrderDetail.AirPort == request.AirPort);
                }

                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    string keyword = request.Keyword.ToLower();
                    orderQuery = orderQuery.Where(o =>
                        o.CustomerName.ToLower().Contains(keyword) ||
                        o.Email.ToLower().Contains(keyword) ||
                        o.Phone.ToLower().Contains(keyword) ||
                        o.GroupReference.ToLower().Contains(keyword) ||
                        o.OrderDetail.Flight_Number.ToLower().Contains(keyword)
                    );
                }
            }

            var totalCount = await orderQuery.CountAsync();
            var orderList = orderQuery
                .OrderBy(o => o.OrderDetail.Service_Time)
                .Skip(skip)
                .Take(size)
                .Select(o => new Models.Order
                {
                    Id = o.OrderDetail.Id,
                    Status = o.StatusName,
                    Status_Sales = o.SalesStatusName,
                    Status_Operator = o.OperatorStatusName,
                    Name = o.CustomerName,
                    Email = o.Email,
                    Phone = o.Phone,
                    Employee = o.EmployeeName,
                    Passport_Number = o.OrderDetail.Passport_Number,
                    Nationality = o.OrderDetail.Nationality,
                    Flight_Number = o.OrderDetail.Flight_Number,
                    Passport_File = o.OrderDetail.Passport_Path,
                    Visa_File = o.OrderDetail.Visa_Path,
                    AirPort = o.AirPort,
                    GroupReference = o.OrderDetail.GroupReference,
                    Service = o.OrderDetail.Service_ID,
                    Note = o.OrderDetail.Note,
                    Created_at = o.OrderDetail.Created_at,
                    Updated_at = o.OrderDetail.Updated_at,
                    Service_Time = o.OrderDetail.Service_Time,
                    Operator_Note = o.OrderDetail.Operator_Note
                }).ToList();

            var orderFinal = new Models.OrderFinal
            {
                Orders = orderList,
                TotalCount = totalCount
            };

            return orderFinal;
        }


        public async Task<Models.Order_Detail> GetOrderById(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var order_detail = await _order_DetailRepository.GetOrder_DetailById(id);

            if (order_detail == null)
            {
                Console.WriteLine("Order not found");
                return null;
            }

            var Order_ID = order_detail.OrdersId;

            if (Order_ID == null)
            {
                Console.WriteLine("Order ID is null");
                return null;
            }
            var order = await _context.Orders.FirstOrDefaultAsync(r => r.Id == Order_ID);

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == order.Customer_ID);
            var Status_OpeartorId = order_detail.Status_Operator_ID;
            var employeeId = order_detail.Employee_Id != null ? (int)order_detail.Employee_Id : 1;

            var mappedOrderDetail = new Models.Order_Detail
            {
                Id = order_detail.Id,
                Name = customer.Name,
                Email = customer.Email,
                Customer_Id = customer.Id,
                Phone = order_detail.Phone,
                Employee = employeeId,
                Passport_Number = order_detail.Passport_Number,
                Nationality = order_detail.Nationality,
                Flight_Number = order_detail.Flight_Number,
                Passport_File = order_detail.Passport_Path,
                Visa_File = order_detail.Visa_Path,
                Portrait_File = order_detail.Portrait_Path,
                AirPort = order_detail.AirPort,
                GroupReference = order_detail.GroupReference,
                Service = order_detail.Service_ID,
                Status = order_detail.Status_ID.ToString(),
                Status_Sales = order_detail.Status_Sales_Id.ToString(),
                Status_Opeartor = order_detail.Status_Operator_ID.ToString(),
                Note = order_detail.Note,
                Operator_Note = order_detail.Operator_Note,
                Created_at = order_detail.Created_at,
                Updated_at = order_detail.Updated_at,
                Service_Time = order_detail.Service_Time,
            };

            Console.WriteLine($"Mapped Order Detail: {mappedOrderDetail}");

            return mappedOrderDetail;
        }


        public async Task<bool> UpdateOrder(Entities.Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Models.Order_Agency_Final> GetOrderDetailsByAgencyIdAsync(Order_Agency_Input request)
        {
            try
            {
                if (request.agencyId == null)
                {
                    throw new ArgumentNullException(nameof(request.agencyId), "AgencyId cannot be null.");
                }

                int pageIndex = request.Index;
                int size = request.PageSize;
                int skip = (pageIndex - 1) * size;

                var orderQuery = await _context.Order_Details
                                    .Include(od => od.Status)
                                    .Include(od => od.Orders)
                                        .ThenInclude(o => o.Customer)
                                    .Select(od => new
                                    {
                                        OrderDetail = od,
                                        StatusName = od.Status.Name,
                                        CustomerName = od.Orders.Customer.Name,
                                        od.Orders.Customer.Email,
                                        od.CreateBy,
                                        od.Phone,
                                        od.AirPort,
                                    }).ToListAsync();

                var orderResult = orderQuery.Where(r => r.CreateBy == request.agencyId);

                var totalOrder = orderResult.Count();

                var orderList = orderResult
                                    .Skip(skip)
                                    .Take(size)
                                    .Select(o => new Order_By_Agency
                                    {
                                        Id = o.OrderDetail.Id,
                                        Status = o.StatusName,
                                        Name = o.CustomerName,
                                        Email = o.Email,
                                        Phone = o.Phone,
                                        Passport_Number = o.OrderDetail.Passport_Number,
                                        Nationality = o.OrderDetail.Nationality,
                                        Flight_Number = o.OrderDetail.Flight_Number,
                                        Passport_File = o.OrderDetail.Passport_Path,
                                        Visa_File = o.OrderDetail.Visa_Path,
                                        AirPort = o.AirPort,
                                        GroupReference = o.OrderDetail.GroupReference,
                                        Service = o.OrderDetail.Service_ID,
                                        Note = o.OrderDetail.Note,
                                        Created_at = o.OrderDetail.Created_at,
                                        Updated_at = o.OrderDetail.Updated_at,
                                        Service_Time = o.OrderDetail.Service_Time,
                                    }).ToList();

                var orderFinal = new Order_Agency_Final()
                {
                    TotalCount = totalOrder,
                    order_By_Agencies = orderList
                };

                return orderFinal;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Entities.Order> GetOrderByIdAsync(int? id)
        {
            return await _context.Orders.FirstAsync(o => o.Id == id);
        }
    }
}
    
