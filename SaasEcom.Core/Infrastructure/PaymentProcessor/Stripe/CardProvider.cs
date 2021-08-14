﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SaasEcom.Core.DataServices.Interfaces;
using SaasEcom.Core.Infrastructure.PaymentProcessor.Interfaces;
using SaasEcom.Core.Models;
using Stripe;

namespace SaasEcom.Core.Infrastructure.PaymentProcessor.Stripe
{
    /// <summary>
    /// Implementation for CRUD related to credit cards with Stripe and also saves the details in the database. 
    /// </summary>
    public class CardProvider : ICardProvider
    {
        private readonly ICardDataService _cardDataService;
        private readonly CardService _cardService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardProvider"/> class.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <param name="cardDataService">The card data service.</param>
        public CardProvider(string apiKey, ICardDataService cardDataService)
        {
            this._cardDataService = cardDataService;
            StripeConfiguration.ApiKey = apiKey;
            this._cardService = new CardService();
        }

        /// <summary>
        /// Gets all the credit cards asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <returns>
        /// The list of credit cards
        /// </returns>
        public async Task<IList<CreditCard>> GetAllAsync(string customerId)
        {
            return await _cardDataService.GetAllAsync(customerId);
        }

        /// <summary>
        /// Finds the credit card asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="cardId">The card identifier.</param>
        /// <returns>Credit card</returns>
        public async Task<CreditCard> FindAsync(string customerId, int? cardId)
        {
            return await _cardDataService.FindAsync(customerId, cardId);
        }

        /// <summary>
        /// Adds the credit card asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="card">The card.</param>
        /// <returns></returns>
        public async Task AddAsync(SaasEcomUser user, CreditCard card)
        {
            // Save to Stripe
            var stripeCustomerId = user.StripeCustomerId;
            AddCardToStripe(card, stripeCustomerId);

            // Save to storage
            card.SaasEcomUserId = user.Id;
            await _cardDataService.AddAsync(card);
        }

        /// <summary>
        /// Updates the credit card asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="creditcard">The creditcard.</param>
        /// <returns></returns>
        public async Task UpdateAsync(SaasEcomUser user, CreditCard creditcard)
        {
            // Remove current card from stripe
            var currentCard = await _cardDataService.FindAsync(user.Id, creditcard.Id, true);
            var stripeCustomerId = user.StripeCustomerId;
            _cardService.Delete(stripeCustomerId, currentCard.StripeId);

            this.AddCardToStripe(creditcard, stripeCustomerId);

            // Update card in the DB
            creditcard.SaasEcomUserId = user.Id;
            await _cardDataService.UpdateAsync(user.Id, creditcard);
        }

        /// <summary>
        /// Check if the Card belong to user.
        /// </summary>
        /// <param name="cardId">The card identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns>
        /// true or false
        /// </returns>
        public async Task<bool> CardBelongToUser(int cardId, string userId)
        {
            return await this._cardDataService.AnyAsync(cardId, userId);
        }

        /// <summary>
        /// Deletes the credit card asynchronous.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        /// <param name="custStripeId">The customer stripe identifier.</param>
        /// <param name="cardId">The Card identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task DeleteAsync(string customerId, string custStripeId, int cardId)
        {
            var card = await this._cardDataService.FindAsync(customerId, cardId, true);

            this._cardService.Delete(custStripeId, card.StripeId);
            await this._cardDataService.DeleteAsync(customerId, cardId);
        }

        private Card AddCardToStripe(CreditCard card, string stripeCustomerId)
        {
            var cardoptions = new TokenCreateOptions
            {
                Card = new CreditCardOptions
                {
                    Number = card.CardNumber,
                    ExpYear = Convert.ToInt64(card.ExpirationYear),
                    ExpMonth = Convert.ToInt64(card.ExpirationMonth),
                    Cvc = card.Cvc,
                    AddressLine1 = card.AddressLine1,
                    AddressLine2 = card.AddressLine2,
                    AddressCity = card.AddressCity,
                    AddressState = card.AddressState,
                    AddressZip = card.AddressZip,
                    Name = card.Name
                }
            };
            var service = new TokenService();
            Token stripeToken = service.Create(cardoptions);
            
            var options = new CardCreateOptions
            {
                Source = stripeToken.Id,
            };
            return _cardService.Create(stripeCustomerId, options);
        }
    }
}
