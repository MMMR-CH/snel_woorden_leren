using System;
using SWL.App.Ports;
using SWL.Core.Domain.Economy;

namespace SWL.App.UseCases.Shop
{
    public sealed class PurchaseProductUseCase
    {
        private readonly PlayerProfileStore _store;
        private readonly IIapService _iap;
        private readonly IRemoteConfig _rc;

        public PurchaseProductUseCase(PlayerProfileStore store, IIapService iap, IRemoteConfig remoteConfig)
        {
            _store = store;
            _iap = iap;
            _rc = remoteConfig;
        }

        public void Purchase(string productId, Action<PurchaseResult> callback)
        {
            if (_iap == null)
            {
                callback?.Invoke(PurchaseResult.NotReady(productId));
                return;
            }

            _iap.Purchase(productId, result =>
            {
                if (!result.IsSuccess)
                {
                    callback?.Invoke(result);
                    return;
                }

                ApplyEntitlement(productId);
                callback?.Invoke(result);
            });
        }

        private void ApplyEntitlement(string productId)
        {
            var p = _store.Profile;

            if (productId == ProductIds.RemoveAds)
            {
                p.RemoveAdsOwned = true;
                _store.NotifyChanged();
                return;
            }

            if (productId == ProductIds.Vip)
            {
                p.VipOwned = true;
                _store.NotifyChanged();
                return;
            }

            if (productId == ProductIds.GemsSmall)
            {
                p.Gems += GemsAmountFor(ProductIds.GemsSmall);
                _store.NotifyChanged();
                return;
            }

            if (productId == ProductIds.GemsMedium)
            {
                p.Gems += GemsAmountFor(ProductIds.GemsMedium);
                _store.NotifyChanged();
                return;
            }

            if (productId == ProductIds.GemsLarge)
            {
                p.Gems += GemsAmountFor(ProductIds.GemsLarge);
                _store.NotifyChanged();
                return;
            }

            // Unknown product: no-op but keep save stable
        }

        private int GemsAmountFor(string productId)
        {
            // Defaults (can be overridden by RemoteConfig)
            if (productId == ProductIds.GemsSmall)
                return _rc?.GetInt(ShopKeys.GemsSmallAmount, 50) ?? 50;
            if (productId == ProductIds.GemsMedium)
                return _rc?.GetInt(ShopKeys.GemsMediumAmount, 120) ?? 120;
            if (productId == ProductIds.GemsLarge)
                return _rc?.GetInt(ShopKeys.GemsLargeAmount, 300) ?? 300;
            return 0;
        }
    }
}
