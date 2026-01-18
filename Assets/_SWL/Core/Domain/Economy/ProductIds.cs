namespace SWL.Core.Domain.Economy
{
    /// <summary>
    /// Central product id list used by the IAP layer and shop use-cases.
    /// </summary>
    public static class ProductIds
    {
        // Non-consumables
        public const string RemoveAds = "remove_ads";
        public const string Vip = "vip";

        // Consumables
        public const string GemsSmall = "gems_small";
        public const string GemsMedium = "gems_medium";
        public const string GemsLarge = "gems_large";

        public static bool IsNonConsumable(string productId)
            => productId == RemoveAds || productId == Vip;
    }
}
