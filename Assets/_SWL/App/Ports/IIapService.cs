using System;
using SWL.Core.Domain.Economy;

namespace SWL.App.Ports
{
    public interface IIapService
    {
        bool IsInitialized { get; }
        void Initialize(Action<bool> onReady = null);

        bool IsOwned(string productId);

        void Purchase(string productId, Action<PurchaseResult> callback);
        void RestorePurchases(Action<bool> callback);
    }
}
