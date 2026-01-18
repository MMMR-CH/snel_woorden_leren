using System;
using System.Collections.Generic;
using UnityEngine;
using SWL.App.Ports;
using SWL.Core.Domain.Economy;

namespace SWL.Infrastructure.IAP
{
    /// <summary>
    /// Stub IAP service for editor/play mode.
    /// </summary>
    public sealed class StubIapService : IIapService
    {
        private readonly HashSet<string> _owned = new();
        public bool IsInitialized { get; private set; }

        public void Initialize(Action<bool> onReady = null)
        {
            IsInitialized = true;
            onReady?.Invoke(true);
        }

        public bool IsOwned(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId)) return false;
            return _owned.Contains(productId);
        }

        public void Purchase(string productId, Action<PurchaseResult> callback)
        {
            if (!IsInitialized)
            {
                callback?.Invoke(PurchaseResult.NotReady(productId));
                return;
            }

            if (string.IsNullOrWhiteSpace(productId))
            {
                callback?.Invoke(PurchaseResult.Fail(productId, "Invalid product id"));
                return;
            }

            // In stub: always succeed
            if (ProductIds.IsNonConsumable(productId))
                _owned.Add(productId);

            Debug.Log($"[IAP Stub] Purchase success: {productId}");
            callback?.Invoke(PurchaseResult.Ok(productId));
        }

        public void RestorePurchases(Action<bool> callback)
        {
            if (!IsInitialized)
            {
                callback?.Invoke(false);
                return;
            }

            // In stub: nothing else to do
            callback?.Invoke(true);
        }
    }
}
