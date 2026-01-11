namespace SWL.App.UseCases
{
    public sealed class GrantLevelRewardUseCase
    {
        private readonly PlayerProfileStore _store;

        public GrantLevelRewardUseCase(PlayerProfileStore store)
        {
            _store = store;
        }

        public void Grant(int coinReward, int gemReward = 0)
        {
            _store.Profile.Coins += coinReward;
            _store.Profile.Gems += gemReward;

            _store.NotifyChanged();
        }
    }
}
