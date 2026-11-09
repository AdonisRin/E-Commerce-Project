using System;
using System.Threading.Tasks;
using LiftAndAesthetics.Models;
using LiftAndAesthetics.Repositories.Interfaces;
using LiftAndAesthetics.Services.Interfaces;

namespace LiftAndAesthetics.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IRepositoryWrapper _repository;

        public DiscountService(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        public async Task<(bool IsValid, string Message, DiscountCode? Discount)> ValidateCodeAsync(string? code, decimal currentSubtotal)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return (false, "Please enter a promo code.", null);
            }

            var discount = await _repository.DiscountCode.GetByCodeAsync(code);
            if (discount == null)
            {
                return (false, $"Discount code '{code.ToUpper()}' does not exist.", null);
            }

            if (!discount.IsActive)
            {
                return (false, "This discount code has been deactivated.", null);
            }

            if (DateTime.Now > discount.ExpiryDate)
            {
                return (false, $"Discount code expired on {discount.ExpiryDate:dd MMM yyyy}.", null);
            }

            if (discount.TimesUsed >= discount.MaxUses)
            {
                return (false, "This discount code has reached its maximum usage limit.", null);
            }

            if (currentSubtotal <= 0)
            {
                return (false, "Cart subtotal must be greater than zero to apply a discount.", null);
            }

            string benefit = discount.DiscountType == "Percentage" 
                ? $"{discount.Value}% OFF" 
                : $"{discount.Value:0.00} LEI OFF";

            return (true, $"Promo code '{discount.Code}' applied successfully ({benefit})!", discount);
        }

        public decimal CalculateDiscount(DiscountCode discount, decimal subtotal)
        {
            if (discount == null || subtotal <= 0) return 0m;

            if (discount.DiscountType == "Percentage")
            {
                decimal amount = Math.Round(subtotal * (discount.Value / 100m), 2);
                return Math.Min(amount, subtotal);
            }
            else // Fixed Amount
            {
                return Math.Min(discount.Value, subtotal);
            }
        }

        public async Task RecordUsageAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return;

            var discount = await _repository.DiscountCode.GetByCodeAsync(code);
            if (discount != null)
            {
                discount.TimesUsed++;
                _repository.DiscountCode.Update(discount);
                await _repository.SaveAsync();
            }
        }
    }
}

