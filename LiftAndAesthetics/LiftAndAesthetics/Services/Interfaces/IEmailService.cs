using System.Threading.Tasks;
using LiftAndAesthetics.Models;

namespace LiftAndAesthetics.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendOrderConfirmationEmailAsync(Order order);
    }
}

