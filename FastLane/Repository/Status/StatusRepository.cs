using FastLane.Context;
using FastLane.Dtos.Status;
using FastLane.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace FastLane.Repository.Status
{
    public class StatusRepository : IStatusRepository
    {
        private readonly ApplicationDbContext _context;

        public StatusRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<StatusDTO>> GetAllStatusesAsync()
        {
            var statuses = await _context.Statuses
                                          .Include(s => s.Role)
                                          .ToListAsync();

            var statusDTOs = statuses.Select(s => new StatusDTO
            {
                Id = s.Id,
                Name = s.Name,
                Role = s.Role != null ? s.Role.Name : string.Empty,
                Created_at = s.Created_at,
                Updated_at = s.Updated_at
            }).ToList();

            return statusDTOs;
        }


        public async Task<bool> CreateStatusAsync(Entities.Status status)
        {
            try
            {
                _context.Statuses.Add(status);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> UpdateStatusAsync(Entities.Status status)
        {
            _context.Statuses.Update(status);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteStatusAsync(int? statusId)
        {
            if (statusId == null)
            {
                throw new ArgumentNullException(nameof(statusId), "StatusId is required");
            }

            var status = await _context.Statuses.FirstOrDefaultAsync(r => r.Id == statusId);
            if (status == null)
            {
                return false;
            }

            _context.Statuses.Remove(status);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Entities.Status> GetStatusByIdAsync(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Status ID is required");
            }

            var status = await _context.Statuses.FirstOrDefaultAsync(r => r.Id == id);
            if (status == null)
            {
                throw new KeyNotFoundException($"Status with ID {id} not found");
            }

            return status;
        }

        public async Task<List<Entities.Status>> GetStatusByRole_IdAsync( int? roleId)
        {
            if(roleId == null)
            {
                throw new ArgumentNullException(nameof(roleId), "Role ID is required");
            }

            var status = await _context.Statuses.Where(r => r.RoleId == roleId)
                                                .ToListAsync();
            if (status.Count == 0)
            {
                throw new KeyNotFoundException($"No statuses found for Role ID {roleId}");
            }

            return status;
        }

        public async Task<int> GetStatusByName(string name)
        {
            var status = await _context.Statuses.FirstOrDefaultAsync(r => r.Name == name);
            if (status == null)
            {
                throw new ArgumentNullException("Status Name isn't found");
            }

            return status.Id;
        }

    }
}
