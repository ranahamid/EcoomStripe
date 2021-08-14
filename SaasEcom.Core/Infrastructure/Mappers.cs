using System.Collections.Generic;
using System.Linq;
using SaasEcom.Core.Models;
using Stripe;
using Invoice = SaasEcom.Core.Models.Invoice;

namespace SaasEcom.Core.Infrastructure
{
    /// <summary>
    /// Mapper for Stripe classes to SaasEcom classes
    /// </summary>
    public class Mappers
    {
        #region Stripe to SaasEcom Mapper
        public static Invoice Map(Stripe.Invoice stripeInvoice)
        {
            var invoice = new Invoice
            {
                AmountDue = (int)stripeInvoice.AmountDue,
                ApplicationFee = (int)stripeInvoice.ApplicationFeeAmount,
                AttemptCount = (int)stripeInvoice.AttemptCount,
                Attempted = stripeInvoice.Attempted,
                //Closed = stripeInvoice.Closed,
                Currency = stripeInvoice.Currency,
                Date = stripeInvoice.DueDate,
                Description = stripeInvoice.Description,
                //Discount = Map(stripeInvoice.Discount),
                EndingBalance = (int)stripeInvoice.EndingBalance,
                //Forgiven = stripeInvoice.Forgiven,
                NextPaymentAttempt = stripeInvoice.NextPaymentAttempt,
                Paid = stripeInvoice.Paid,
                PeriodStart = stripeInvoice.PeriodStart,
                PeriodEnd = stripeInvoice.PeriodEnd,
                ReceiptNumber = stripeInvoice.ReceiptNumber,
                StartingBalance = (int)stripeInvoice.StartingBalance,
                StripeCustomerId = stripeInvoice.CustomerId,
                StatementDescriptor = stripeInvoice.StatementDescriptor,
                Tax = (int)stripeInvoice.Tax,
                TaxPercent = stripeInvoice.TaxPercent,
                StripeId = stripeInvoice.Id,
                Subtotal = (int)stripeInvoice.Subtotal,
                Total = (int)stripeInvoice.Total,
                LineItems = Map(stripeInvoice.Lines.Data)
            };

            return invoice;
        }

        private static ICollection<Invoice.LineItem> Map(IEnumerable<Stripe.InvoiceLineItem> list)
        {
            if (list == null)
                return null;

            return list.Select(i => new Invoice.LineItem
            {
                Amount = (int)i.Amount,
                Currency = i.Currency,
                Period = Map(i.Period),
                Plan = Map(i.Plan),
                Proration = i.Proration,
                Quantity = (int)i.Quantity,
                StripeLineItemId = i.Id,
                Type = i.Type
            }).ToList();
        }

        private static Invoice.Plan Map(Stripe.Plan stripePlan)
        {
            return new Invoice.Plan
            {
                AmountInCents = (int)stripePlan.Amount,
                Created = stripePlan.Created,
                Currency = stripePlan.Currency,
                //StatementDescriptor = stripePlan.StripeResponse.,
                Interval = stripePlan.Interval,
                IntervalCount = (int)stripePlan.IntervalCount,
                Name = stripePlan.Nickname,
                StripePlanId = stripePlan.Id,
                TrialPeriodDays = (int)stripePlan.TrialPeriodDays
            };
        }

        private static Invoice.Period Map(Stripe.Period stripePeriod)
        {
            if (stripePeriod == null)
                return null;

            return new Invoice.Period
            {
                Start = stripePeriod.Start,
                End = stripePeriod.End
            };
        }
        #endregion
    }
}
