using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Entities.SubEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Abstraction;
using Shared;
using Shared.DTOs;

namespace Presentation
{
    [Authorize]
    public class SubscriptionsController(ISubscriptionService _subscriptionService,IUnitOfWork _unitOfWork) : BaseApiController
    {
       
        // GET: api/subscriptions/plans
        [HttpGet("plans")]
        [AllowAnonymous] // or [Authorize] – up to you
        public async Task<IActionResult> GetPlans()
        {
            var plans = await _subscriptionService.GetAllPlansAsync();
            return Ok(plans);
        }

        // POST: api/subscriptions/create-checkout-session
        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionRequest request)
        {
            var userId = User.GetUserId(); // extension method
            var successUrl = request.SuccessUrl;
            var cancelUrl = request.CancelUrl;

            var sessionUrl = await _subscriptionService.CreateCheckoutSessionAsync(userId, request.PlanId, successUrl, cancelUrl);
            return Ok(new { url = sessionUrl });
        }

        // GET: api/subscriptions/status
        [HttpGet("status")]
        public async Task<IActionResult> GetMySubscriptionStatus()
        {
            var userId = User.GetUserId();
            var isActive = await _subscriptionService.CheckUserSubscriptionAsync(userId);
            var subscription = await _unitOfWork.Set<UserSubscription>()
                .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive);
            return Ok(new { isActive, planName = subscription?.SubscriptionPlan?.Name, endDate = subscription?.EndDate });
        }

        // POST: api/subscriptions/cancel (optional)
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelSubscription()
        {
            var userId = User.GetUserId();
            await _subscriptionService.CancelSubscriptionAsync(userId);
            return NoContent();
        }

        // POST: api/subscriptions/
        // (public endpoint for Stripe)
        
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook()
        {
            // Read body with leaveOpen: true and reset position
            string json;
            using (var reader = new StreamReader(HttpContext.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                json = await reader.ReadToEndAsync();
                HttpContext.Request.Body.Position = 0; // ← Important: reset position
            }

            var signature = Request.Headers["Stripe-Signature"].ToString();

            // ✅ Log to see what's coming in
            Console.WriteLine($"📥 Webhook received. Body length: {json?.Length ?? 0}");
            Console.WriteLine($"🔑 Signature: {signature ?? "null"}");

            if (string.IsNullOrEmpty(json))
                return BadRequest("Empty request body.");

            if (string.IsNullOrEmpty(signature))
                return BadRequest("Missing Stripe-Signature header.");

            try
            {
                await _subscriptionService.HandleWebhookAsync(json, signature);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Webhook error: {ex.Message}");
                return BadRequest();
            }
        }

    
    }
}
