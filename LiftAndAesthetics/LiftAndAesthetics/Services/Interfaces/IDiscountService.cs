using System.Threading.Tasks;
using LiftAndAesthetics.Models;

namespace LiftAndAesthetics.Services.Interfaces
{
    public interface IDiscountService
    {
        Task<(bool IsValid, string Message, DiscountCode? Discount)> ValidateCodeAsync(string? code, decimal currentSubtotal);
        decimal CalculateDiscount(DiscountCode discount, decimal subtotal);
        Task RecordUsageAsync(string code);
    }
}

