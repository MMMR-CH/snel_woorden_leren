using MC.Modules.Keyboard;
using System;
using System.Collections.Generic;


namespace SWL
{
    public class MainBus
    {
        // LevelManager
        public List<WoordData> WoordDatas { get; set; } = null;
        public Action<WoordDataType> SetWoordData { get; set; } = null;
        public Func<WoordDataType> CurrentWoordDataType { get; set; } = null;

        // LocalizationManager
        public Func<LetterSet> GetCurrentLetterSetFunc { get; set; } = null;

        // AudioManager
        public Action OnPlayButtonSound { get; set; } = null;
    }
}
