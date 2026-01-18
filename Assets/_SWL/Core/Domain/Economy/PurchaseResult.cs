namespace SWL.Core.Domain.Economy
{
    public enum PurchaseState
    {
        Success = 0,
        Failed = 1,
        Cancelled = 2,
        NotInitialized = 3
    }

    public readonly struct PurchaseResult
    {
        public readonly PurchaseState State;
        public readonly string ProductId;
        public readonly string Error;

        public PurchaseResult(PurchaseState state, string productId, string error = null)
        {
            State = state;
            ProductId = productId;
            Error = error;
        }

        public bool IsSuccess => State == PurchaseState.Success;

        public static PurchaseResult Ok(string productId) => new(PurchaseState.Success, productId);
        public static PurchaseResult Fail(string productId, string error) => new(PurchaseState.Failed, productId, error);
        public static PurchaseResult Cancel(string productId) => new(PurchaseState.Cancelled, productId);
        public static PurchaseResult NotReady(string productId) => new(PurchaseState.NotInitialized, productId);
    }
}
