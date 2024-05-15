using Azure.Core;
using FastLane.Dtos.Order;
using FastLane.Repository.Customer;
using FastLane.Repository.Order;
using FastLane.Repository.Order_Detail;
using FastLane.Service.Passport;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Identity.Client;
using FastLane.Repository.Status;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.RegularExpressions;
using FastLane.Service.Pagination;
using FastLane.Entities;
using FastLane.Service.Email;
using FastLane.Models;
using Npgsql;
using System.Globalization;
using FastLane.Dapper;
using Dapper;
using MailKit.Search;
using Org.BouncyCastle.Tls.Crypto;
using static System.Net.Mime.MediaTypeNames;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Microsoft.EntityFrameworkCore;
using FastLane.Context;
using FastLane.Dtos.Agency;
using DocumentFormat.OpenXml.Bibliography;
using FastLane.Dtos.Order.Report;

namespace FastLane.Service.Order
{
    public class OrderService : IOrderService
    {
        private static Random random = new Random();
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrder_DetailRepository _order_DetailRepository;
        private readonly IImageService _imageService;
        private readonly IPassportDataExtractor _dataExtractor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStatusRepository _statusRepository;
        private readonly IPaginationService _paginationService;
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _context;
       
        public OrderService(IOrderRepository orderRepository,
                            IImageService imageService,
                            IPassportDataExtractor dataExtractor,
                            ICustomerRepository customerRepository,
                            IOrder_DetailRepository order_DetailRepository,
                            IWebHostEnvironment webHostEnvironment,
                            IPaginationService paginationService,
                            IEmailService emailService,
                            IStatusRepository statusRepository)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _imageService = imageService;
            _dataExtractor = dataExtractor;
            _paginationService = paginationService;
            _order_DetailRepository = order_DetailRepository;
            _statusRepository = statusRepository;
            _emailService = emailService;
            _webHostEnvironment = webHostEnvironment;
            _statusRepository = statusRepository;
        }
        public async Task<bool> CreateOrderAsync(CreateOrderRequest order)
        {
            if (order == null)
            {
                Console.WriteLine("Invalid order data");
                return false;
            }  

            if (order.Is_group_order == 1)
            {
                string groupReference = RandomString(10);

                foreach (var orderData in order.Orders)
                {
                    await ProcessOrder(orderData, groupReference);
                }
            }
            else
            {
                foreach (var orderData in order.Orders)
                {
                    await ProcessOrderWithoutGroup(orderData);
                }
            }

            return true;
        }
        private async Task ProcessOrderWithoutGroup(OrderItemDto order)
        {
            string PassportImage_Path = null;
            string VisaImage_Path = null;
            string PortraitImage_Path = null;
            try
            {
                if (!string.IsNullOrEmpty(order.Passport_File))
            {
                PassportImage_Path = await UploadFileFromBase64(order.Passport_File, ".png");
            }
            else
            {
                Console.WriteLine("Passport_File is null");
            }

            if (!string.IsNullOrEmpty(order.Visa_File))
            {
                VisaImage_Path = await UploadFileFromBase64(order.Visa_File, ".png");
            }
            else
            {
                Console.WriteLine("Visa_File is null");
            }

            if (!string.IsNullOrEmpty(order.Portrait_File))
            {
                PortraitImage_Path = await UploadFileFromBase64(order.Portrait_File, ".png");
            }
            else
            {
                Console.WriteLine("Portrait_File is null");
            }

                var newCustomer = new Entities.Customer()
                {
                    Name = order.Name,
                    Email = order.Email,
                };

                //Create a new customer
                await _customerRepository.CreateCustomerAsync(newCustomer);

                var customer = await _customerRepository.GetCustomerByIdAsync(newCustomer.Id);

                Console.WriteLine("Customer ID: " + customer.Id);

                var newOrder = new Entities.Order()
                {
                    Customer_ID = customer.Id,
                };

                //Create a new order
                await _orderRepository.CreateOrder(newOrder);
                var orderId = newOrder.Id;

                var statusId = await _statusRepository.GetStatusByName("Pending");
                var employeeId = 1;
                var newTime = order.Service_Time.AddHours(7);

                var newOrder_Detail = new Entities.Order_Detail
                {
                    OrdersId = orderId,
                    Phone = order.Phone,
                    Passport_Number = order.Passport_Number,
                    Nationality = order.Nationality,
                    Flight_Number = order.Flight_Number,
                    Service_Time = newTime,
                    Employee_Id = employeeId,
                    AirPort = order.AirPort,
                    GroupReference = RandomString(10),
                    Service_ID = order.Service_ID,
                    Status_ID = statusId,
                    Status_Operator_ID = statusId,
                    Status_Sales_Id = statusId,
                    Operator_Note = null,
                    Note = order.Note,
                    CreateBy = order.CreateBy,
                    Passport_Path = PassportImage_Path,
                    Visa_Path = VisaImage_Path,
                    Portrait_Path = PortraitImage_Path,
                };

                await _order_DetailRepository.CreateOrder_Detail(newOrder_Detail);
                var EmailContent = new Dtos.Email.Email()
                {
                    Order_Id = newOrder.Id,
                    To = order.Email,
                    Subject = order.Email,
                    Body = order.Email,
                };

                await _emailService.SendPendingEmail(EmailContent);

                Console.WriteLine("Order created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private async Task ProcessOrder(OrderItemDto order, string groupReference)
        {
            string PassportImage_Path = null;
            string VisaImage_Path = null;
            string PortraitImage_Path = null;
            try { 
            if (!string.IsNullOrEmpty(order.Passport_File))
            {
                PassportImage_Path = await UploadFileFromBase64(order.Passport_File, ".png");
            }
            else
            {
                Console.WriteLine("Passport_File is null");
            }

            if (!string.IsNullOrEmpty(order.Visa_File))
            {
                VisaImage_Path = await UploadFileFromBase64(order.Visa_File, ".png");
            }
            else
            {
                Console.WriteLine("Visa_File is null");
            }

            if (!string.IsNullOrEmpty(order.Portrait_File))
            {
                PortraitImage_Path = await UploadFileFromBase64(order.Portrait_File, ".png");
            }
            else
            {
                Console.WriteLine("Portrait_File is null");
            }


            var newCustomer = new Entities.Customer()
            {
                Name = order.Name,
                Email = order.Email,
            };
                 
            //Create a new customer
            await _customerRepository.CreateCustomerAsync(newCustomer);
            var customer = await _customerRepository.GetCustomerByIdAsync(newCustomer.Id);

            var newOrder = new Entities.Order()
            {
                Customer_ID = customer.Id,
            };

            //Create a new order
            await _orderRepository.CreateOrder(newOrder);
            var orderId = newOrder.Id;
            var statusId = await _statusRepository.GetStatusByName("Pending");
            var newTime = order.Service_Time.AddHours(7);

            var newOrder_Detail = new Entities.Order_Detail
            {
                OrdersId = orderId,
                Phone = order.Phone,
                Passport_Number = order.Passport_Number,
                Nationality = order.Nationality,
                Flight_Number = order.Flight_Number,
                Service_Time = newTime,
                AirPort = order.AirPort,
                GroupReference = groupReference,
                Service_ID = order.Service_ID,
                Status_ID = statusId,
                Status_Operator_ID = statusId,
                Status_Sales_Id = statusId,
                Operator_Note = null,
                Note = order.Note,
                Passport_Path = PassportImage_Path,
                Visa_Path = VisaImage_Path,
                Portrait_Path = PortraitImage_Path,
                Employee_Id = 1,
            };

            await _order_DetailRepository.CreateOrder_Detail(newOrder_Detail);
            var EmailContent = new Dtos.Email.Email()
            {
                Order_Id = newOrder.Id,
                To = order.Email,
                Subject = order.Email,
                Body = order.Email,
            };

            await _emailService.SendPendingEmail(EmailContent);
            Console.WriteLine("Order created successfully.");
        }
          catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public async Task<bool> DeleteOrderAsync(int? id)
        {
            if (id == null)
            {
                return false;
            }

            await _orderRepository.DeleteOrder(id);
            return true;
        }

        public async Task<Models.OrderFinal> GetAllOrders(int? index, int? pageSize, int? user_id)
        {
            return await _orderRepository.GetAllOrders(index,pageSize, user_id);
        }

        public async Task<Models.OrderFinal> SearchOrderAsync(OrderParams request)
        {
            var result = await _orderRepository.SearchOrdersAsync(request);      

            return result;
        }


        public async Task<Models.Order_Detail> GetOrderByIdAsync(int? id)
        {
            var orderDetail = await _orderRepository.GetOrderById(id);
            return orderDetail;
        }
        public async Task<Models.Order_Agency_Final> GetOrderDetailsByAgencyIdAsync(Order_Agency_Input request)
        {
            return await _orderRepository.GetOrderDetailsByAgencyIdAsync(request);
        }

        public async Task<bool> UpdateOrderAsync(int? id, EditOrder_Dto order)
        {
            if (id == null)
            {
                return false;
            }

            //Get all order's information with Id
            var order_DetailDB = await _order_DetailRepository.GetOrder_DetailById(id);
            if (order_DetailDB == null)
            {
                return false;
            }

            //Get all customer's model information
            var order_DetailModels = await _orderRepository.GetOrderById(id);
            var customerDB = await _customerRepository.GetCustomerByIdAsync(order_DetailModels.Customer_Id);
            if (customerDB == null)
            {
                return false;
            }

            //Update all field's value with new data value
            if (customerDB.Name != null || customerDB.Email != null)
            {
                customerDB.Email = order.Email;
                customerDB.Name = order.Name;
                Console.WriteLine(customerDB.Name);
                Console.WriteLine(customerDB.Email);

                await _customerRepository.UpdateCustomerAsync(customerDB);
            }

            string PassportImage_Path = null;
            string VisaImage_Path = null;
            string PortraitImage_Path = null;

            try
            {
                if (!string.IsNullOrEmpty(order.Passport_File))
                {
                    if (IsBase64String(order.Passport_File))
                    {
                        PassportImage_Path = await UploadFileFromBase64(order.Passport_File, ".png");
                    }
                    else
                    {
                        PassportImage_Path = order.Passport_File; 
                    }
                }

                if (order.Visa_File != null)
                {
                    if (IsBase64String(order.Visa_File))
                    {
                        VisaImage_Path = await UploadFileFromBase64(order.Visa_File, ".png");
                    }
                    else
                    {
                        VisaImage_Path = order.Visa_File; 
                    }
                }

                if (order.Portrait_File != null)
                {
                    if (IsBase64String(order.Portrait_File))
                    {
                        PortraitImage_Path = await UploadFileFromBase64(order.Portrait_File, ".png");
                    }
                    else
                    {
                        PortraitImage_Path = order.Portrait_File;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Đã xảy ra lỗi: " + ex.Message);
            }

            int Status_Operator_Id = order.Status_Operator_ID ?? 0;

                //------------Order_Detail-------------------//
                //order_DetailDB.CreateBy = order.CreateBy;
                order_DetailDB.Note = order.Note;
                order_DetailDB.Phone = order.Phone;
                order_DetailDB.AirPort = order.AirPort;
                order_DetailDB.Status_ID = order.Status_ID;
                order_DetailDB.Service_ID = order.Service_ID;
                order_DetailDB.Nationality = order.Nationality;
                order_DetailDB.Employee_Id = order.Employee_Id;
                order_DetailDB.Service_Time = order.Service_Time;
                order_DetailDB.Flight_Number = order.Flight_Number;
                order_DetailDB.Operator_Note = order.Operator_Note;
                order_DetailDB.GroupReference = order.GroupReference;
                order_DetailDB.Passport_Number = order.Passport_Number;
                order_DetailDB.Status_Sales_Id = order.Status_Sales_Id;
                order_DetailDB.Status_Operator_ID = Status_Operator_Id;
                order_DetailDB.UpdatedBy = order.UpdatedBy;
                order_DetailDB.Updated_at = DateTime.Now;
                order_DetailDB.Passport_Path = PassportImage_Path;
                order_DetailDB.Visa_Path = VisaImage_Path;
                order_DetailDB.Portrait_Path = PortraitImage_Path;

                var result = await _order_DetailRepository.UpdateOrder_Detail(order_DetailDB);
                if (result) {
                    Console.WriteLine("ID đây", order_DetailDB.Id);
                await UpdateGroupOrderStatusAsync(order_DetailDB.GroupReference,(int) order.Status_Sales_Id, (int)order.Status_Operator_ID, (int) order.Status_ID);
            }
            return true;
     
        }

        private async Task<string> UploadFileFromBase64(string base64String, string extension)
        {
            if (string.IsNullOrEmpty(base64String))
            {
                return null;
            }

            base64String = base64String.Replace("data:image/png;base64,", "");
            byte[] bytes = Convert.FromBase64String(base64String);

            var wwwRootPath = _webHostEnvironment.WebRootPath;
            var uploadPath = Path.Combine(wwwRootPath, "visa");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var fileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(uploadPath, fileName);

            await File.WriteAllBytesAsync(filePath, bytes);

            var imageUrl = Path.Combine("visa", fileName).Replace("\\", "/");
            return imageUrl;
        }

        public async Task UpdateOrderStatusAsync(int orderId)
       {   
            var orderDetail = await _order_DetailRepository.GetOrder_DetailById(orderId);

            var statusPending_Id = await _statusRepository.GetStatusByName("Pending");
            var statusCancel_Id = await _statusRepository.GetStatusByName("Canceled");
            var statusConfirm_Id = await _statusRepository.GetStatusByName("Confirmed");
            var statusComplete_Id = await _statusRepository.GetStatusByName("Completed");
            var statusUncomplete_Id = await _statusRepository.GetStatusByName("Uncompleted");
            var statusOperator_Cancel_Id = await _statusRepository.GetStatusByName("O.Canceled");
            var statusOperator_Confirm_Id = await _statusRepository.GetStatusByName("O.Confirmed");

            DateTime serviceTime = orderDetail.Service_Time;
            DateTime currentTime = DateTime.Now;
            TimeSpan timeDifference = serviceTime.Subtract(currentTime);

            double totalHoursDifference = timeDifference.TotalHours;

            int daysDifference = timeDifference.Days;
            double remainingHours = totalHoursDifference - (daysDifference * 24); 
       
            if (totalHoursDifference < 6){
                if(orderDetail.Status_ID == statusPending_Id)
                {

                    if (orderDetail.Status_Sales_Id == statusConfirm_Id && orderDetail.Status_Operator_ID == statusOperator_Confirm_Id)
                    {
                        orderDetail.Status_ID = statusConfirm_Id;
                        await _order_DetailRepository.UpdateOrder_Detail(orderDetail);
                        var order = await _orderRepository.GetOrderById(orderDetail.OrdersId);
                        var customer = await _customerRepository.GetCustomerByIdAsync(order.Id);
                        var EmailContent = new Dtos.Email.Email()
                        {
                            Order_Id = orderDetail.Id,
                            To = customer.Email,
                            Subject = customer.Email,
                            Body = customer.Email,
                        };

                        await _emailService.SendConfirmEmail(EmailContent);
                    }
                    else if (orderDetail.Status_Sales_Id == statusCancel_Id || orderDetail.Status_Operator_ID == statusOperator_Cancel_Id)
                    {
                        orderDetail.Status_ID = statusCancel_Id;
                        await _order_DetailRepository.UpdateOrder_Detail(orderDetail);

                        //Send mail cancel to user
                        var order = await _orderRepository.GetOrderById(orderDetail.OrdersId);
                        var customer = await _customerRepository.GetCustomerByIdAsync(order.Id);
                        var EmailContent = new Dtos.Email.Email()
                        {
                            Order_Id = orderDetail.Id,
                            To = order.Email,
                            Subject = order.Email,
                            Body = order.Email,
                        };
                        await _emailService.SendCancelEmail(EmailContent);
                    }
                    else if (orderDetail.Status_Sales_Id == statusConfirm_Id || orderDetail.Status_Operator_ID == statusOperator_Confirm_Id)
                    {
                        orderDetail.Status_ID = statusPending_Id;
                        await _order_DetailRepository.UpdateOrder_Detail(orderDetail);
                    }    
                } 
                else if(orderDetail.Status_ID == statusConfirm_Id)
                {
                    if(orderDetail.Status_Operator_ID == statusComplete_Id)
                    {
                        orderDetail.Status_ID = statusComplete_Id;
                        await _order_DetailRepository.UpdateOrder_Detail(orderDetail);

                        //Send mail   
                        var order = await _orderRepository.GetOrderById(orderDetail.OrdersId);
                        var customer = await _customerRepository.GetCustomerByIdAsync(order.Id);
                        var EmailContent = new Dtos.Email.Email()
                        {
                            Order_Id = orderDetail.Id,
                            To = customer.Email,
                            Subject = customer.Email,
                            Body = customer.Email,
                        };

                        await _emailService.SendCompleteEmail(EmailContent);
                    }
                    else if(orderDetail.Status_ID == statusUncomplete_Id)
                    {
                        orderDetail.Status_ID = statusUncomplete_Id;
                        await _order_DetailRepository.UpdateOrder_Detail(orderDetail);
                    }
                }
            }
            else 
            {
                if(orderDetail.Status_ID == statusPending_Id)
                {
                    var order = await _orderRepository.GetOrderById(orderDetail.OrdersId);

                    if(orderDetail.Status_Sales_Id == statusCancel_Id)
                    {
                        orderDetail.Status_ID = statusCancel_Id;
                        orderDetail.Status_Operator_ID = statusCancel_Id;
                        await _order_DetailRepository.UpdateOrder_Detail(orderDetail);

                        var orderDB = await _orderRepository.GetOrderById(orderDetail.OrdersId);
                        var customer = await _customerRepository.GetCustomerByIdAsync(order.Id);
                        var EmailContent = new Dtos.Email.Email()
                        {
                            Order_Id = orderDetail.Id,
                            To = orderDB.Email,
                            Subject = orderDB.Email,
                            Body = orderDB.Email,
                        };
                        await _emailService.SendCancelEmail(EmailContent);
                    }
                    else if(orderDetail.Status_Sales_Id == statusConfirm_Id)
                    {
                        orderDetail.Status_ID= statusConfirm_Id;
                        orderDetail.Status_Operator_ID = statusConfirm_Id;

                        var EmailContent = new Dtos.Email.Email()
                        {
                            Order_Id = orderDetail.Id,
                            To = order.Email,
                            Subject = order.Email,
                            Body = order.Email,
                        };

                        await _emailService.SendConfirmEmail(EmailContent);
                        await _order_DetailRepository.UpdateOrder_Detail(orderDetail);
                    }
                }
                if(orderDetail.Status_ID == statusConfirm_Id)
                {
                    if (orderDetail.Status_Operator_ID == statusComplete_Id)
                    {
                        orderDetail.Status_ID = statusComplete_Id;
                        await _order_DetailRepository.UpdateOrder_Detail(orderDetail);

                        //Send mail
                        var order = await _orderRepository.GetOrderById(orderDetail.OrdersId);
                        var customer = await _customerRepository.GetCustomerByIdAsync(order.Id);
                        var EmailContent = new Dtos.Email.Email()
                        {
                            Order_Id = orderDetail.Id,
                            To = customer.Email,
                            Subject = customer.Email,
                            Body = customer.Email,
                        };

                        await _emailService.SendCompleteEmail(EmailContent);
                    }
                    else if (orderDetail.Status_Operator_ID == statusUncomplete_Id)
                    {
                        orderDetail.Status_ID = statusUncomplete_Id;
                        await _order_DetailRepository.UpdateOrder_Detail(orderDetail);
                    }
                }
            }
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public List<Year_Order_Data> GetTotalBookingByMonth(int year, string airport = null)
        {
            using (NpgsqlConnection connection = new DatabaseContext().GetConnection())
            {
                connection.Open();

                string airportCondition = "";
                if (!string.IsNullOrEmpty(airport))
                {
                    airportCondition = "AND o.\"AirPort\" = @Airport";
                }

                string sql = @"
                    SELECT 
                        MONTH(o.Created_at) as Label,
                        COUNT(*) as TotalCount
                    FROM 
                        ""Order_Details"" o
                    WHERE 
                        YEAR(o.Created_at) = @Year
                        " + airportCondition + @"
                    GROUP BY 
                        MONTH(o.Created_at)
                    ORDER BY 
                        MONTH(o.Created_at) ASC;
                ";
                Console.WriteLine(sql);
                var queryParams = new
                {
                    Year = year,
                    Airport = airport,
                };

                var orderChartData = connection.Query<Year_Order_Data>(sql, queryParams).AsList();
                return orderChartData;
            }
        }


        bool IsBase64String(string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        public async Task UpdateGroupOrderStatusAsync(string groupReference, int Status_Sales_Id, int Status_Operator_ID, int Status_ID)
        {
            var ordersWithSameGroupReference = await _order_DetailRepository.GetOrdersByGroupReference(groupReference);


            // Lặp qua từng đơn hàng còn lại trong nhóm
            foreach (var orderDetail in ordersWithSameGroupReference)
            {
                // Gán giá trị trạng thái từ đơn hàng đầu tiên cho các đơn hàng còn lại trong nhóm
                orderDetail.Status_ID = Status_ID;
                orderDetail.Status_Operator_ID = Status_Operator_ID;
                orderDetail.Status_Sales_Id = Status_Sales_Id;
                await _order_DetailRepository.UpdateOrder_Detail(orderDetail);

                // Cập nhật thông tin đơn hàng
                await UpdateOrderStatusAsync(orderDetail.Id);
            }

        }


        //report data for agency with the data in each month
        public async Task<AgencyOrderData> GetAgencyOrderDataByIdAndMonth(int? agency_Id, int year)
        {
            AgencyOrderData yearlyData = new AgencyOrderData
            {
                Year = year,
                MonthlyData = new List<MonthOrderData>()
            };



            for (int month = 1; month <= 12; month++)
            {
                int pendingCount = await GetOrderStatusCountForMonth(agency_Id, year, month, "Pending");
                int cancelCount = await GetOrderStatusCountForMonth(agency_Id, year, month, "Canceled");
                int confirmCount = await GetOrderStatusCountForMonth(agency_Id, year, month, "Confirmed");
                int completedCount = await GetOrderStatusCountForMonth(agency_Id, year, month, "Completed");
                int uncompletedCount = await GetOrderStatusCountForMonth(agency_Id, year, month, "Uncompleted");

                MonthOrderData monthlyData = new MonthOrderData
                {
                    Month = month,
                    Total = cancelCount + confirmCount + pendingCount + completedCount + uncompletedCount,
                    PendingCount = pendingCount,
                    CancelCount = cancelCount,
                    ConfirmCount = confirmCount,
                    CompletedCount = completedCount,
                    UncompletedCount = uncompletedCount
                };

                yearlyData.MonthlyData.Add(monthlyData);
            }

            return yearlyData;
        }


        //report data for agency with the data in each year
        public async Task<Agency_Order_Data_Year> GetAgencyOrderDataByIdAndYear(int? agency_Id, int year)
        {
            int pendingCount = await GetOrderStatusCountForYear(agency_Id, year, "Pending");
            int cancelCount = await GetOrderStatusCountForYear(agency_Id, year, "Canceled");
            int confirmCount = await GetOrderStatusCountForYear(agency_Id, year, "Confirmed");
            int completedCount = await GetOrderStatusCountForYear(agency_Id, year, "Completed");
            int uncompletedCount = await GetOrderStatusCountForYear(agency_Id, year, "Uncompleted");

            Agency_Order_Data_Year yearlyData = new Agency_Order_Data_Year
            {
                Year = year,
                PendingCount = pendingCount,
                CancelCount = cancelCount,
                ConfirmCount = confirmCount,
                CompletedCount = completedCount,
                UncompletedCount = uncompletedCount,
                Total = cancelCount + confirmCount + pendingCount + completedCount+ uncompletedCount,
            };

            return yearlyData;
        }

        public async Task<int> GetOrderStatusCountForMonth(int? agency_Id, int year, int month, string statusName)
        {
            var statusId = await _statusRepository.GetStatusByName(statusName);
            var orderDetails = await _order_DetailRepository.GetOrderDetailsByAgencyIdAndMonth(agency_Id, year, month);
            return orderDetails.Count(od => od.Status_ID == statusId);
        }


        public async Task<int> GetOrderStatusCountForYear(int? agency_Id, int year, string statusName)
        {
            var statusId = await _statusRepository.GetStatusByName(statusName);
            var orderDetails = await _order_DetailRepository.GetOrderDetailsByAgencyIdAndYear(agency_Id, year);
            return orderDetails.Count(od => od.Status_ID == statusId);
        }


        //Get all order in month and year in db
        public async Task<Month_Order_Data> Get_All_Month_Orders_Data(int year)
        {
            Month_Order_Data yearlyData = new Month_Order_Data
            {
                Year = year,
                MonthlyData = new List<Each_Month_Order_Data>()
            };

            for (int month = 1; month <= 12; month++)
            {
                int pendingCount = await GetAllOrdersStatusCountForMonth( year, month, "Pending");
                int cancelCount = await GetAllOrdersStatusCountForMonth( year, month, "Canceled");
                int confirmCount = await GetAllOrdersStatusCountForMonth( year, month, "Confirmed");
                int completedCount = await GetAllOrdersStatusCountForMonth( year, month, "Completed");
                int uncompletedCount = await GetAllOrdersStatusCountForMonth( year, month, "Uncompleted");

                Each_Month_Order_Data monthlyData = new Each_Month_Order_Data
                {
                    Month = month,
                    Total = cancelCount + confirmCount + pendingCount + completedCount + uncompletedCount,
                    PendingCount = pendingCount,
                    CancelCount = cancelCount,
                    ConfirmCount = confirmCount,
                    CompletedCount = completedCount,
                    UncompletedCount = uncompletedCount
                };

                yearlyData.MonthlyData.Add(monthlyData);
            }

            return yearlyData;
        }

        public async Task<int> GetAllOrdersStatusCountForMonth( int year, int month, string statusName)
        {
            var statusId = await _statusRepository.GetStatusByName(statusName);
            var orderDetails = await _order_DetailRepository.GetMonthOrderDetails( year, month);
            return orderDetails.Count(od => od.Status_ID == statusId);
        }

        public async Task<Year_Order_Data> Get_Year_Order_Data(int year)
        {
            int pendingCount = await GetAllOrdersStatusCountForYear(year, "Pending");
            int cancelCount = await GetAllOrdersStatusCountForYear(year, "Canceled");
            int confirmCount = await GetAllOrdersStatusCountForYear(year, "Confirmed");
            int completedCount = await GetAllOrdersStatusCountForYear(year, "Completed");
            int uncompletedCount = await GetAllOrdersStatusCountForYear(year, "Uncompleted");

            Year_Order_Data yearlyData = new Year_Order_Data
            {
                Year = year,
                PendingCount = pendingCount,
                CancelCount = cancelCount,
                ConfirmCount = confirmCount,
                CompletedCount = completedCount,
                UncompletedCount = uncompletedCount,
                Total = cancelCount + confirmCount + pendingCount + completedCount + uncompletedCount,
            };

            return yearlyData;
        }

        public async Task<int> GetAllOrdersStatusCountForYear( int year, string statusName)
        {
            var statusId = await _statusRepository.GetStatusByName(statusName);
            var orderDetails = await _order_DetailRepository.GetYearOrderDetails(year);
            return orderDetails.Count(od => od.Status_ID == statusId);
        }
    }
}
