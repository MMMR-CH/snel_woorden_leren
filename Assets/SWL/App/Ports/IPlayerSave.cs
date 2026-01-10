using SWL.Core.Domain.Player;

namespace SWL.App.Ports
{
    public interface IPlayerSave
    {
        PlayerProfile Load();
        void Save(PlayerProfile profile);
    }
}