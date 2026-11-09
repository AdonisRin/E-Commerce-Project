using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LiftAndAesthetics.Repositories
{
    public class DiscountCodeRepository : RepositoryBase<DiscountCode>, IDiscountCodeRepository
    {
        public DiscountCodeRepository(LiftContext context) : base(context) { }

        public async Task<DiscountCode?> GetByCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;
            var normalized = code.Trim().ToUpper();
            return await LiftContext.DiscountCodes
                .FirstOrDefaultAsync(d => d.Code == normalized);
        }
    }
}

