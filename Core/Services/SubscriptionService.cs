using System.Text.Json;
using Domain.Contracts;
using Domain.Entities;
using Domain.Entities.SubEntity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Services.Abstraction;
using Shared.DTOs;
using Stripe;

namespace Services
{
    public class SubscriptionService(IUnitOfWork _unitOfWork, IConfiguration _configuration, UserManager<Users> _userManager) :
        ISubscriptionService
    {

        public async Task<string> CreateCheckoutSessionAsync(int userId, int planId, string successUrl, string cancelUrl)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            var plan = await _unitOfWork.GetRepository<SubscriptionPlan>().GetByIdAsync(planId);
            if (plan == null) throw new KeyNotFoundException("Plan not found");

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) throw new KeyNotFoundException("User not found");

            // Create or get Stripe customer
            if (string.IsNullOrEmpty(user.StripeCustomerId))
            {
                var customerOptions = new CustomerCreateOptions
                {
                    Email = user.Email,
                    Name = user.UserName
                };
                var customerService = new CustomerService();
                var customer = await customerService.CreateAsync(customerOptions);
                user.StripeCustomerId = customer.Id;
                await _userManager.UpdateAsync(user);
            }
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                Customer = string.IsNullOrEmpty(user.StripeCustomerId) ? null : user.StripeCustomerId,
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>
            {
                new Stripe.Checkout.SessionLineItemOptions
                {
                    Price = plan.StripePriceId,
                    Quantity = 1,
                }
            },
                Mode = "subscription",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                Metadata = new Dictionary<string, string>
            {
                { "userId", userId.ToString() },
                { "planId", planId.ToString() }
            }
            };

            var service = new Stripe.Checkout.SessionService();
            var session = await service.CreateAsync(options);
            return session.Url;
        }
        public async Task HandleWebhookAsync(string json, string signatureHeader)
        {
            if (string.IsNullOrEmpty(json))
            {
                Console.WriteLine("Webhook error: Empty JSON body.");
                throw new Exception("Empty JSON body");
            }

            if (string.IsNullOrEmpty(signatureHeader))
            {
                Console.WriteLine("Webhook error: Missing Stripe-Signature header.");
                throw new Exception("Missing Stripe-Signature header");
            }

            var webhookSecret = _configuration["StripeSettings:EndPointSecret"];

            if (string.IsNullOrEmpty(webhookSecret))
            {
                Console.WriteLine("Webhook error: Webhook secret not configured.");
                throw new Exception("Webhook secret not configured.");
            }

            var stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, webhookSecret, throwOnApiVersionMismatch: false);

            switch (stripeEvent.Type)
            {
                case "checkout.session.completed":
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    var userId = int.Parse(session.Metadata["userId"]);
                    var planId = int.Parse(session.Metadata["planId"]);

                    // Check if already exists
                    var existing = await _unitOfWork.Set<UserSubscription>()
                        .FirstOrDefaultAsync(us => us.UserId == userId && us.StripeSubscriptionId == session.SubscriptionId);
                    if (existing != null) return;

                    // Get subscription details from Stripe
                    var stripeSubscriptionService = new Stripe.SubscriptionService();
                    var stripeSubscription = await stripeSubscriptionService.GetAsync(session.SubscriptionId);
                    var subscriptionItem = stripeSubscription.Items.Data.FirstOrDefault();

                    var userSubscription = new UserSubscription
                    {
                        UserId = userId,
                        SubscriptionPlanId = planId,
                        StripeSubscriptionId = session.SubscriptionId,
                        StartDate = subscriptionItem?.CurrentPeriodStart.ToUniversalTime() ?? DateTime.UtcNow,
                        EndDate = subscriptionItem?.CurrentPeriodEnd.ToUniversalTime() ?? DateTime.UtcNow.AddMonths(1),
                        IsActive = true
                    };
                    await _unitOfWork.Set<UserSubscription>().AddAsync(userSubscription);
                    await _unitOfWork.SaveChangesAsync();
                    break;

                case "customer.subscription.updated":
                case "customer.subscription.deleted":
                    var subscription = stripeEvent.Data.Object as Stripe.Subscription;
                    var localSub = await _unitOfWork.Set<UserSubscription>()
                        .FirstOrDefaultAsync(us => us.StripeSubscriptionId == subscription.Id);
                    if (localSub != null)
                    {
                        var subItem = subscription.Items.Data.FirstOrDefault();
                        if (subItem != null)
                        {
                            localSub.EndDate = subItem.CurrentPeriodEnd.ToUniversalTime();
                            localSub.IsActive = subscription.Status == "active";
                            _unitOfWork.Set<UserSubscription>().Update(localSub);
                            await _unitOfWork.SaveChangesAsync();
                        }
                    }
                    break;
            }
        }

        public async Task<IEnumerable<SubscriptionPlanDto>> GetAllPlansAsync()
        {
            var plans = await _unitOfWork.GetRepository<SubscriptionPlan>().GetAllAsync();

            return plans.Select(p => new SubscriptionPlanDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                DurationDisplay = GetDurationDisplay(p.DurationWeeks), // ✅ Convert
                Features = string.IsNullOrEmpty(p.FeaturesJson)? new List<string>():JsonSerializer.Deserialize<List<string>>(p.FeaturesJson),
                StripePriceId = p.StripePriceId
            });
        }

        // ✅ Helper method
        private string GetDurationDisplay(int weeks)
        {
            return weeks switch
            {
                1 => "per week",
                2 => "per 2 weeks",
                4 => "per month",
                8 => "per 2 months",
                12 => "per 3 months",
                26 => "per 6 months",
                52 => "per year",
                _ => $"per {weeks} weeks"
            };
        }


        public async Task<bool> CheckUserSubscriptionAsync(int userId)
        {
            var activeSub = await _unitOfWork.Set<UserSubscription>()
                .AnyAsync(us => us.UserId == userId && us.IsActive && us.EndDate > DateTime.UtcNow);
            return activeSub;
        }

        // 4. Cancel a user's subscription (call Stripe and update local DB)
        public async Task CancelSubscriptionAsync(int userId)
        {
            var userSubscription = await _unitOfWork.Set<UserSubscription>()
                .Include(us => us.SubscriptionPlan)
                .FirstOrDefaultAsync(us => us.UserId == userId && us.IsActive);
            if (userSubscription == null)
                throw new InvalidOperationException("No active subscription found");

            var stripeSubscriptionService = new Stripe.SubscriptionService();
            await stripeSubscriptionService.CancelAsync(userSubscription.StripeSubscriptionId, null);

            userSubscription.IsActive = false;
            _unitOfWork.Set<UserSubscription>().Update(userSubscription);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
