using System;
using SWL.App.Ports;
using SWL.Core.Domain.Economy;

namespace SWL.App.UseCases.Shop
{
    public sealed class RestorePurchasesUseCase
    {
        private readonly PlayerProfileStore _store;
        private readonly IIapService _iap;

        public RestorePurchasesUseCase(PlayerProfileStore store, IIapService iap)
        {
            _store = store;
            _iap = iap;
        }

        public void Restore(Action<bool> callback)
        {
            if (_iap == null)
            {
                callback?.Invoke(false);
                return;
            }

            _iap.RestorePurchases(success =>
            {
                if (success)
                    ApplyOwnedFlags();

                callback?.Invoke(success);
            });
        }

        private void ApplyOwnedFlags()
        {
            var p = _store.Profile;
            p.RemoveAdsOwned = _iap.IsOwned(ProductIds.RemoveAds) || p.RemoveAdsOwned;
            p.VipOwned = _iap.IsOwned(ProductIds.Vip) || p.VipOwned;
            _store.NotifyChanged();
        }
    }
}
