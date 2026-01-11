using UnityEngine;
using SWL.Presentation.UI.Widgets;
using SWL.App;
using SWL.Core.Domain.Player;

namespace SWL.Presentation.Controllers
{
    public sealed class HUDPresenter : MonoBehaviour
    {
        [SerializeField] private HUDView view;

        private PlayerProfileStore _store;

        // DI entry point
        public void Construct(PlayerProfileStore store)
        {
            _store = store;
            UpdateView(_store.Profile);
        }

        private void OnEnable()
        {
            if (_store != null)
                _store.OnChanged += UpdateView;
        }

        private void OnDisable()
        {
            if (_store != null)
                _store.OnChanged -= UpdateView;
        }

        private void UpdateView(PlayerProfile profile)
        {
            view.SetLife(profile.Life);
            view.SetCoins(profile.Coins);
            view.SetGems(profile.Gems);
        }
    }
}
