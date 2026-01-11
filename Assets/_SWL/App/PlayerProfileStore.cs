using System;
using SWL.App.Ports;
using SWL.Core.Domain.Player;

namespace SWL.App
{
    public sealed class PlayerProfileStore
    {
        public PlayerProfile Profile { get; private set; }

        public event Action<PlayerProfile> OnChanged;

        private readonly IPlayerSave _save;

        public PlayerProfileStore(IPlayerSave save)
        {
            _save = save;
            Profile = _save.Load();
        }

        public void Save()
        {
            _save.Save(Profile);
        }

        public void NotifyChanged()
        {
            _save.Save(Profile);
            OnChanged?.Invoke(Profile);
        }
    }
}
