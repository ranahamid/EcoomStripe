using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SaasEcom.Core.Infrastructure.PaymentProcessor.Interfaces;
using SaasEcom.Core.Models;
using Stripe;
using Subscription = SaasEcom.Core.Models.Subscription;

namespace SaasEcom.Core.Infrastructure.PaymentProcessor.Stripe
{
    /// <summary>
    /// Implementation for subscription management with Stripe
    /// </summary>
    public class SubscriptionProvider : ISubscriptionProvider
    {
        private readonly SubscriptionService _subscriptionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionProvider"/> class.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        public SubscriptionProvider(string apiKey)
        {
            StripeConfiguration.ApiKey = apiKey;
            this._subscriptionService = new SubscriptionService();
        }

        /// <summary>
        /// Subscribes the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="planId">The plan identifier.</param>
        /// <param name="trialInDays">The trial in days.</param>
        /// <param name="taxPercent">The tax percent.</param>
        public string SubscribeUser(SaasEcomUser user, string planId, int trialInDays = 0, decimal taxPercent = 0)
        {
            var result = this._subscriptionService.Update(user.StripeCustomerId,
                new SubscriptionUpdateOptions
                {
                    Plan = planId,
                    TaxPercent = taxPercent,
                    TrialEnd = DateTime.UtcNow.AddDays(trialInDays)
                });

            return result.Id;
        }

        /// <summary>
        /// Subscribes the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="planId">The plan identifier.</param>
        /// <param name="trialEnds">The trial ends.</param>
        /// <param name="taxPercent">The tax percent.</param>
        public string SubscribeUser(SaasEcomUser user, string planId, DateTime? trialEnds, decimal taxPercent = 0)
        {
            var result = this._subscriptionService.Update(user.StripeCustomerId, 
                new SubscriptionUpdateOptions
                {
                    
                    Plan = planId,
                    TaxPercent = taxPercent,
                    TrialEnd = trialEnds
                });

            return result.Id;
        }

        /// <summary>
        /// Gets the User's subscriptions asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<List<Subscription>> UserSubscriptionsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ends the subscription.
        /// </summary>
        /// <param name="userStripeId">The user stripe identifier.</param>
        /// <param name="subStripeId">The sub stripe identifier.</param>
        /// <param name="cancelAtPeriodEnd">if set to <c>true</c> [cancel at period end].</param>
        /// <returns>
        /// The date when the subscription will be cancelled
        /// </returns>
        public DateTime EndSubscription(string userStripeId, string subStripeId, bool cancelAtPeriodEnd = false)
        {
            var canOp = new SubscriptionCancelOptions
            {
                InvoiceNow = true
            };

            var subscription = this._subscriptionService.Cancel(userStripeId, canOp);

            return cancelAtPeriodEnd ? subscription.EndedAt.Value : DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the subscription. (Change subscription plan)
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="subStripeId">The sub stripe identifier.</param>
        /// <param name="newPlanId">The new plan identifier.</param>
        /// <param name="proRate">if set to <c>true</c> [pro rate].</param>
        /// <returns></returns>
        public bool UpdateSubscription(string customerId, string subStripeId, string newPlanId, bool proRate)
        {
            var result = true;
            try
            {
                var currentSubscription = this._subscriptionService.Get(subStripeId);

                var myUpdatedSubscription = new SubscriptionUpdateOptions
                {
                    Plan = newPlanId,
                    Prorate = proRate
                };

                if (currentSubscription.TrialEnd != null && currentSubscription.TrialEnd > DateTime.UtcNow)
                {
                    myUpdatedSubscription.TrialEnd = currentSubscription.TrialEnd; // Keep the same trial window as initially created.
                }

                _subscriptionService.Update(subStripeId, myUpdatedSubscription);
            }
            catch (Exception ex)
            {
                // TODO: Log
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Updates the subscription tax.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="subStripeId">The sub stripe identifier.</param>
        /// <param name="taxPercent">The tax percent.</param>
        /// <returns></returns>
        public bool UpdateSubscriptionTax(string customerId, string subStripeId, decimal taxPercent = 0)
        {
            var result = true;
            try
            {
                var myUpdatedSubscription = new SubscriptionUpdateOptions
                {
                    TaxPercent = taxPercent
                };
                _subscriptionService.Update(subStripeId, myUpdatedSubscription);
            }
            catch (Exception ex)
            {
                // TODO: log
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Subscribes the user natural month.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="planId">The plan identifier.</param>
        /// <param name="billingAnchorCycle">The billing anchor cycle.</param>
        /// <param name="taxPercent">The tax percent.</param>
        /// <returns></returns>
        public object SubscribeUserNaturalMonth(SaasEcomUser user, string planId, DateTime? billingAnchorCycle, decimal taxPercent)
        {
            var stripeSubscription = _subscriptionService.Create
                (new SubscriptionCreateOptions
                {
                    BillingCycleAnchor = billingAnchorCycle,
                    TaxPercent = taxPercent
                });

            return stripeSubscription;
        }
    }
}
