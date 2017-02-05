using Hash17.Campaign;
using Hash17.Data;
using Hash17.Game;
using Hash17.MockSystem;
using MockSystem.Term;

namespace Hash17.Utils
{
    public static class Alias
    {
        public static Hash17Game Game { get { return Hash17Game.Instance; } }
        public static DataHolder DataHolder { get { return Game.DataHolder; } }
        public static Terminal Term { get { return Game.Terminal; } }
        public static SystemVariables SysVariables { get { return Game.SystemVariables; } }
        public static GameConfiguration Config { get { return DataHolder.GameConfiguration; } }
        public static CampaignManager Campaign { get { return Game.CampaignManager; } }
        public static DeviceCollection Devices { get { return Game.DeviceCollection; } }
        public static ProgramCollection Programs { get { return Game.ProgramCollection; } }
    }
}