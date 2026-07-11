using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;

namespace Services.Abstraction
{
    public interface ISubscriptionService
    {
        Task<string> CreateCheckoutSessionAsync(int userId, int planId, string successUrl, string cancelUrl);
        Task HandleWebhookAsync(string json, string signatureHeader);
        Task<bool> CheckUserSubscriptionAsync(int userId);
        Task<IEnumerable<SubscriptionPlanDto>> GetAllPlansAsync();

        Task CancelSubscriptionAsync(int userId);
    }
}
