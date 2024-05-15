using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using FastLane.Context;
using FastLane.Repository.Order_Detail;
using Microsoft.EntityFrameworkCore;

namespace FastLane.Repository.Order
{
    public class Order_DetailRepository : IOrder_DetailRepository
    {
        private readonly ApplicationDbContext _context;
        public Order_DetailRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateOrder_Detail(Entities.Order_Detail order)
        {
            try
            {
                _context.Order_Details.Add(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteOrder_Detail(int? id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            var order = await _context.Order_Details.FirstOrDefaultAsync(r => r.Id == id);
            if (order == null)
            {
                return false;
            }

            _context.Order_Details.Remove(order);
            _context.SaveChanges();
            return true;
        }

        public async Task<List<Entities.Order_Detail>> GetAllOrder_Details()
        {
            return await _context.Order_Details.ToListAsync();
        }

        public async Task<Entities.Order_Detail> GetOrder_DetailById(int? id)
        {

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var order = await _context.Order_Details.FirstOrDefaultAsync(r => r.Id == id);
            if (order == null)
            {
                throw new ArgumentException($"Order with Id {id} is not found");
            }

            return order;
        }

        public async Task<bool> UpdateOrder_Detail(Entities.Order_Detail order)
        {
            var id = order.Id;
            var aff = _context.Order_Details.Find(id);
            if (aff != null)
            {
                order.CreateBy = aff.CreateBy;
            }

            _context.Order_Details.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Entities.Order_Detail>> GetOrdersByGroupReference(string groupReference)
        {
            var orders = await _context.Order_Details
                                        .Where(o => o.GroupReference == groupReference)
                                        .ToListAsync();

            return orders;
        }

        public Task<List<Entities.Order_Detail>> GetOrderDetailsByAgencyIdAndMonth(int? agency_Id, int year, int month)
        {
            //Huybc comment
            //Check role of agency
            
            var query = _context.Order_Details
                        
                .Where(od => od.Created_at.Year == year &&
                             od.Created_at.Month == month);

            if(agency_Id != null)
            {
                query = query.Where(od => od.CreateBy == agency_Id);
            }       

            return query.ToListAsync(); 
        }

        public Task<List<Entities.Order_Detail>> GetOrderDetailsByAgencyIdAndYear(int? agency_Id, int year)
        {
            var query = _context.Order_Details
                .Where(od => od.Created_at.Year == year);

            if(agency_Id != null)
            {
                //Lay ra role cua user
                var q = (from r in _context.Roles
                         join ur in _context.UserRole on r.Id equals ur.Role_Id
                         join u in _context.Users on ur.User_Id equals u.Id
                         where u.Customer_ID == agency_Id
                         select r.Name).ToList();


                        
                if (q.Contains("Agency"))
                {
                    query = query.Where(od => od.CreateBy == agency_Id);
                }else
                {
                    var empQuery = (from e in _context.Employee
                                    join u in _context.Users on e.User_Id equals u.Id
                                    join a in _context.Airports on e.Airport_Id equals a.Id
                                    where u.Customer_ID == agency_Id
                                    select a.Name).FirstOrDefault();
                    if (!string.IsNullOrEmpty(empQuery))
                    {
                        query = query.Where(od => od.AirPort.Equals(empQuery));
                    }

                }
                //var empQuery = from e in _context.Employee
                //        join u in _context.Users on e.User_Id equals u.Id
                //        where u.Id
            }

            return query.ToListAsync();
        }

        public Task<List<Entities.Order_Detail>> GetMonthOrderDetails(int year, int month)
        {
            var query = _context.Order_Details
                            .Where(od => od.Created_at.Year == year &&
                                         od.Created_at.Month == month)
                            .ToListAsync();

            return query;
        }

        public Task<List<Entities.Order_Detail>> GetYearOrderDetails(int year)
        {
            var query = _context.Order_Details
                            .Where(od => od.Created_at.Year == year)
                            .ToListAsync();

            return query;
        }
    }
}
