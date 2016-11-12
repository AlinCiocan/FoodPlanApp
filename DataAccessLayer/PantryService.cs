using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlanEatSave.Models;
using PlanEatSave.Utils;

namespace PlanEatSave.DataAceessLayer
{
    public class PantryService
    {
        private ApplicationDbContext _context;
        private IPlanEatSaveLogger _logger;

        public PantryService(ApplicationDbContext context, IPlanEatSaveLogger logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<Pantry> GetPantryByUserIdAsync(string userId)
        {
            return await _context.Pantries
                                .Where(pantry => pantry.UserId == userId)
                                .Include(pantry => pantry.PantryItems).FirstOrDefaultAsync();
        }

        public async Task<bool> AddItem(string userId, long pantryId, PantryItem item)
        {
            var pantryDb = await _context.Pantries.Where(pantry => pantry.Id == pantryId).FirstOrDefaultAsync();
            if (pantryDb == null || pantryDb.UserId != userId)
            {
                return false;
            }

            _context.PantryItems.Add(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}