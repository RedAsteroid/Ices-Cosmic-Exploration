using System.Collections.Generic;

namespace ICE.Utilities.Cosmic
{
    /// <summary>
    /// IDs of missions that should be disabled and shown as unsupported.
    /// </summary>
    public static class UnsupportedMissions
    {
        public static readonly HashSet<uint> Ids = new HashSet<uint>
        {
            0,

            479, 
            481, 482, 
            484, 
            486, 487,
            489, 490, 491, 492, 493, 494, 495,
            // 508,509, // Dual Craft (A Rank)
            511, 510, // Dual Craft (Ex+ Rank)
            543,

            1003, 
            // 1004, 1005, 
            1006, 
            // blacklisted mission ID

            // Oizyr Missions

            1281,

            // Fisher, the cursed lands
            1341, 1345, 1346, 1347, 


        };
    }
}
