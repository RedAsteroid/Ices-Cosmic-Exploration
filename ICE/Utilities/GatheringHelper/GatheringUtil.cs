using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System.Collections.Generic;

namespace ICE.Utilities.GatheringHelper;

public static unsafe partial class GatheringUtil
{
    public class GatheringActions
    {
        /// <summary>
        /// Internal name for myself to know wtf this is
        /// </summary>
        public string ActionName { get; set; }
        public Dictionary<uint, uint> ClassAction { get; set; } = new();
        /// <summary>
        /// Sheet name
        /// </summary>
        public uint StatusId { get; set; }
        /// <summary>
        /// The status name attached to it (personal use)
        /// </summary>
        public string StatusName { get; set; }
        /// <summary>
        /// The amount of GP required for the skill
        /// </summary>
        public int RequiredGp { get; set; }
        public int RequiredLv { get; set; }
    }

    public static Dictionary<string, GatheringActions> GathActionDict = new()
        {
            { "BoonIncrease1", new GatheringActions
            {
                ActionName = "Pioneer's Gift I",
                ClassAction = new()
                {
                    [16] = 21177,
                    [17] = 21178,
                },
                StatusId = 2666,
                StatusName = "Gift of the Land",
                RequiredGp = 50,
                RequiredLv = 15,
            }},
            { "BoonIncrease2", new GatheringActions
            {
                ActionName = "Pioneer's Gift II",
                ClassAction = new()
                {
                    [16] = 25589,
                    [17] = 25590,
                },
                StatusId = 759,
                StatusName = "Gift of the Land II",
                RequiredGp = 100,
                RequiredLv = 50,
            }},
            { "Tidings", new GatheringActions
            {
                ActionName = "Nophica's Tidings",
                ClassAction = new()
                {
                    [16] = 21203,
                    [17] = 21204,
                },
                StatusId = 2667,
                StatusName = "Gatherer's Bounty",
                RequiredGp = 200,
                RequiredLv = 81,
            }},
            { "YieldI", new GatheringActions
            {
                ActionName = "Blessed Harvest",
                ClassAction = new()
                {
                    [16] = 239,
                    [17] = 222,
                },
                StatusId = 219,
                StatusName = "Gathering Yield Up",
                RequiredGp = 400,
                RequiredLv = 30,
            }},
            { "YieldII", new GatheringActions
            {
                ActionName = "Blessed Harvest II",
                ClassAction = new()
                {
                    [16] = 241,
                    [17] = 224,
                },
                StatusId = 219,
                StatusName = "Gathering Yield Up",
                RequiredGp = 500,
                RequiredLv = 40,
            }},
            { "BonusIntegrity", new GatheringActions
            {
                ActionName = "Ageless Words",
                ClassAction = new()
                {
                    [16] = 232,
                    [17] = 215,
                },
                RequiredGp = 300,
                RequiredLv = 30,
            }},
            { "BonusIntegrityChance", new GatheringActions
            {
                ActionName = "Wise of the World",
                ClassAction = new()
                {
                    [16] = 26521,
                    [17] = 26522,
                },
                StatusId = 2765,
                StatusName = "",
                RequiredGp = 0,
                RequiredLv = 90,
            }},
            { "BountifulYieldII", new GatheringActions
            {
                ActionName = "Bountiful Yield/Harvest II",
                ClassAction = new()
                {
                    [16] = 272,
                    [17] = 273,
                },
                StatusId = 1286,
                StatusName = "",
                RequiredGp = 100,
                RequiredLv = 68,
            }},
            { "FieldMasteryIII", new GatheringActions
            {
                // 50% increase
                ActionName = "Field Mastery III",
                ClassAction = new()
                {
                    [16] = 295,
                    [17] = 294,
                },
                StatusId = 218,
                StatusName = "Gathering Rate Up",
                RequiredGp = 250,
                RequiredLv = 10,
            }},
            { "FieldMasteryII", new GatheringActions
            {
                // 15% increase
                ActionName = "Field Mastery II",
                ClassAction = new()
                {
                    [16] = 237,
                    [17] = 220,
                },
                StatusId = 218,
                StatusName = "Gathering Rate Up",
                RequiredGp = 100,
                RequiredLv = 5,
            }},
            { "FieldMasteryI", new GatheringActions
            {
                // 5% increase
                ActionName = "Field Mastery I",
                ClassAction = new()
                {
                    [16] = 235,
                    [17] = 218,
                },
                StatusId = 218,
                StatusName = "Gathering Rate Up",
                RequiredGp = 50,
                RequiredLv = 4,
            }},
            { "FieldMasteryTemp", new GatheringActions
            {
                // 15% increase [temp]
                ActionName = "Clear Vision | Flora Mastery",
                ClassAction = new()
                {
                    [16] = 4072,
                    [17] = 4086,
                },
                StatusId = 754,
                StatusName = "Gathering Rate Up (Limited)",
                RequiredGp = 50,
                RequiredLv = 23,
            }},
            { "GreaterReach", new GatheringActions
            {
                ActionName = "GreaterReach",
                ClassAction = new()
                {

                }
            }},
        };

    public static Dictionary<string, GatheringActions> GathCollectableBuffs = new()
        {
            { "Scrutiny", new GatheringActions
            {
                ActionName = "Scrutiny",
                ClassAction = new()
                {
                    [16] = 22185,
                    [17] = 22189
                },
                StatusId = 757,
                StatusName = "",
                RequiredGp = 200,
            }},
            { "Focus", new GatheringActions
            {
                ActionName = "Collector's Focus",
                ClassAction = new()
                {
                    [16] = 21205,
                    [17] = 21206
                },
                StatusId = 2668,
                StatusName = "",
                RequiredGp = 100,
            }},
            { "Priming", new GatheringActions
            {
                ActionName = "Priming Touch",
                ClassAction = new()
                {
                    [16] = 21205,
                    [17] = 34872
                },
                StatusId = 2668,
                StatusName = "",
                RequiredGp = 100,
            }},
            { "CollectorsHigh", new GatheringActions
            {
                // Only available in certain missions... *-sighs-*
                ActionName = "Collectors High Standard",
                ClassAction = new()
                {
                    [16] = 27,
                    [17] = 27,
                },
                StatusId = 3911,
                StatusName = "",
                RequiredGp = 0,
            }},
            { "BonusIntegrity", new GatheringActions
            {
                ActionName = "Ageless Words",
                ClassAction = new()
                {
                    [16] = 232,
                    [17] = 215,
                },
                RequiredGp = 300,
            }},
            { "BonusIntegrityChance", new GatheringActions
            {
                ActionName = "Wise of the World",
                ClassAction = new()
                {
                    [16] = 26521,
                    [17] = 26522,
                },
                StatusId = 2765,
                StatusName = "",
                RequiredGp = 0,
            }},
        };

    public static Dictionary<string, GatheringActions> GathCollectableActions = new()
    {
        { "Scour", new GatheringActions
        {
            // Base general use skill
            ActionName = "Scour",
                ClassAction = new()
                {
                    [16] = 22182,
                    [17] = 22186
                },
                StatusId = 0,
                StatusName = "n/a",
                RequiredGp = 0,
            }},
            { "Brazen", new GatheringActions
            {
                // 50 - 150% buff
                ActionName = "Brazen Woodsman",
                ClassAction = new()
                {
                    [16] = 22183,
                    [17] = 22187
                },
                StatusId = 0,
                StatusName = "n/a",
                RequiredGp = 0,
            }},
            { "Meticulous", new GatheringActions
            {
                // Chance to not use durability/integrity
                ActionName = "Meticulous Woodsman",
                ClassAction = new()
                {
                    [16] = 22184,
                    [17] = 22188
                },
                StatusId = 0,
                StatusName = "n/a",
                RequiredGp = 0,
            }},
            { "BonusIntegrity", new GatheringActions
            {
                ActionName = "Ageless Words",
                ClassAction = new()
                {
                    [16] = 232,
                    [17] = 215
                },
                RequiredGp = 300,
            }},
            { "BonusIntegrityChance", new GatheringActions
            {
                ActionName = "Wise of the World",
                ClassAction = new()
                {
                    [16] = 26521,
                    [17] = 26522
                },
                StatusId = 2765,
                StatusName = "",
                RequiredGp = 0,
            }},
            { "Collect", new GatheringActions
            {
                ActionName = "Collect",
                ClassAction = new()
                {
                    [16] = 240,
                    [17] = 815,
                },
                StatusId = 0,
                StatusName = "",
                RequiredGp = 0,
            } },
        };

    public static uint CollectStandardCharges()
    {
        try
        {
            if (DutyActionManager.GetInstanceIfReady() != null)
                return (uint)(DutyActionManager.GetInstanceIfReady()->CurCharges[1] + DutyActionManager.GetInstanceIfReady()->CurCharges[0]);

            return 0;
        }
        catch (Exception e)
        {
            e.Log();
            return 0;
        }
    }

    /* First things first, there's several types of missions for gathering
     * 1 Quantity Limited(Gather x amount on limited amount of nodes)
     * 2 Quantity(Gather x amount, gather more for increased score)
     * 3 Timed(Gather x amount in the time limit)
     * 4 Chain(Increase score based on chain)
     * 5 Gatherer's Boon (Increase score by hitting boon % chance)
     * 6 Chain + Boon(Get score from chain nodes + boon % chance)
     * 7 Collectables(This is going to be annoying)
     * 8 Time Steller Reduction(???) (Assuming Collectables -> Reducing for score...fuck)
     */

    // Sinus

    private static CriticalInfo AstromagneticStorm1 = new()
    {
        RawLocation = new Vector3(176.24f, 9.40f, 560.07f),
        MapInfo = new Vector2(24.7f, 32.5f)
    };
    private static CriticalInfo AstromagneticStorm2 = new()
    {
        RawLocation = new Vector3(-91.58f, 19.32f, -241.99f),
        MapInfo = new Vector2(19.7f, 16.6f)
    };
    private static CriticalInfo AstromagneticStorm3 = new()
    {
        RawLocation = new Vector3(-72.76f, 51.00f, 768.64f),
        MapInfo = new Vector2(19.9f, 36.9f)
    };
    private static CriticalInfo AstromagneticStorm4 = new()
    {
        RawLocation = new Vector3(-464.50f, 37.89f, -69.89f),
        MapInfo = new Vector2(12.2f, 20.1f)
    };

    private static CriticalInfo MeteorShower1 = new()
    {
        RawLocation = new Vector3(-219.93f, 24.16f, 209.98f),
        MapInfo = new(17.1f, 25.7f)
    };
    private static CriticalInfo MeteorShower2 = new()
    {
        RawLocation = new Vector3(34.86f, 34.38f, -349.75f),
        MapInfo = new(22.2f, 14.5f)
    };
    private static CriticalInfo MeteorShower3 = new()
    {
        RawLocation = new Vector3(845.90f, -58.44f, -390.45f),
        MapInfo = new(38.1f, 13.9f)
    };
    private static CriticalInfo MeteorShower4 = new()
    {
        RawLocation = new Vector3(497.36f, -115.32f, -845.65f),
        MapInfo = new Vector2(31.3f, 4.4f)
    };

    private static CriticalInfo SporingMist1 = new()
    {
        RawLocation = new Vector3(539.43f, 36.38f, 49.89f),
        MapInfo = new Vector2(32.3f, 22.5f)
    };
    private static CriticalInfo SporingMist2 = new()
    {
        RawLocation = new Vector3(654.32f, 52.00f, 100.13f),
        MapInfo = new Vector2(34.6f, 23.5f)
    };
    private static CriticalInfo SporingMist3 = new()
    {
        RawLocation = new Vector3(379.62f, 51.39f, 704.14f),
        MapInfo = new Vector2(29.1f, 35.6f)
    };
    private static CriticalInfo SporingMist4 = new()
    {
        RawLocation = new Vector3(99.66f, 18.11f, -209.80f),
        MapInfo = new Vector2(23.5f, 17.3f)
    };

    // Phaenna
    private static CriticalInfo Thunderstorms1 = new()
    {
        RawLocation = new Vector3(417.57f, 52.00f, -445.41f),
        MapInfo = new Vector2(29.9f, 12.6f),
    };
    private static CriticalInfo Thunderstorms2 = new()
    {
        RawLocation = new Vector3(432.76f, 54.13f, -169.80f),
        MapInfo = new Vector2(30.1f, 18.1f)
    };
    private static CriticalInfo Thunderstorms3 = new()
    {
        RawLocation = new Vector3(169.79f, 41.00f, -210.79f),
        MapInfo = new Vector2(24.9f, 17.3f)
    };
    private static CriticalInfo Thunderstorms4 = new()
    {
        RawLocation = new Vector3(-615.30f, 8.26f, -515.45f),
        MapInfo = new Vector2(9.2f, 11.2f)
    };

    private static CriticalInfo AnnealingWinds1 = new()
    {
        RawLocation = new Vector3(239.77f, 133.83f, -704.44f),
        MapInfo = new Vector2(26.3f, 7.4f)
    };
    private static CriticalInfo AnnealingWinds2 = new()
    {
        RawLocation = new Vector3(-506.27f, -8.42f, -751.29f),
        MapInfo = new Vector2(11.2f, 6.2f)
    };
    private static CriticalInfo AnnealingWinds3 = new()
    {
        RawLocation = new Vector3(410.29f, 18.90f, 25.14f),
        MapInfo = new Vector2(29.6f, 22.2f)
    };
    private static CriticalInfo AnnealingWinds4 = new()
    {
        RawLocation = new Vector3(10.10f, 7.98f, 339.70f),
        MapInfo = new Vector2(21.7f, 28.3f)
    };

    private static CriticalInfo GlassRain1 = new()
    {
        RawLocation = new Vector3(407.15f, -229.45f, 224.76f),
        MapInfo = new Vector2(29.3f, 25.6f)
    };
    private static CriticalInfo GlassRain2 = new()
    {
        RawLocation = new Vector3(544.42f, -251.07f, 634.55f),
        MapInfo = new Vector2(32.5f, 34.2f)
    };
    private static CriticalInfo GlassRain3 = new()
    {
        RawLocation = new Vector3(148.96f, -9.99f, 487.46f),
        MapInfo = new Vector2(24.6f, 31.4f)
    };
    private static CriticalInfo GlassRain4 = new()
    {
        RawLocation = new Vector3(-488.32f, 25.05f, 35.65f),
        MapInfo = new Vector2(11.8f, 22.3f)
    };

    private static CriticalInfo GravitationAnom1 = new()
    {
        RawLocation = new Vector3(77.05f, -58.69f, -475.25f),
        MapInfo = new Vector2(22.9f, 11.9f)
    };
    private static CriticalInfo GravitationAnom2 = new()
    {
        RawLocation = new Vector3(-189.71f, -0.07f, -61.36f),
        MapInfo = new Vector2(17.6f, 20.1f)
    };

    private static CriticalInfo GaleForce1 = new()
    {
        MapInfo = new(33.2f, 12.9f),
        RawLocation = new(584.29f, -60.42f, -429.47f),
    };
    private static CriticalInfo GaleForce2 = new()
    {
        MapInfo = new(24.1f, 20.2f),
        RawLocation = new(125.37f, 0.64f, -68.72f),
    };
    private static CriticalInfo GaleForce3 = new()
    {
        MapInfo = new(8.2f, 12.3f),
        RawLocation = new(-669.31f, -88.50f, -453.18f),
    };
    private static CriticalInfo GaleForce4 = new()
    {
        MapInfo = new(19.0f, 20.5f),
        RawLocation = new(-127.96f, 0.27f, -50.00f)
    };

    private static CriticalInfo BubbleBloom1 = new()
    {
        MapInfo = new(10.1f, 24.6f),
        RawLocation = new(-572.10f, 22.70f, 156.11f),
    };
    private static CriticalInfo BubbleBloom2 = new()
    {
        MapInfo = new(14.1f, 39),
        RawLocation = new(-369.71f, 104.98f, 876.68f),
    };



    public class CriticalInfo
    {
        public Vector3 RawLocation { get; set; }
        public Vector2 MapInfo { get; set; }
    }

    public static Dictionary<uint, CriticalInfo> CriticalLocations = new();

    public static void UpdateCriticalWeather()
    {
        // Sinus - Astromagnetic Storm 1/α
        AddKeys(AstromagneticStorm1, 518, 522, 530);
        AddKeys(AstromagneticStorm2, 537, 543);

        // Astromagnetic Storm 2/β
        AddKeys(AstromagneticStorm3, 512, 521, 527);
        AddKeys(AstromagneticStorm4, 533, 536, 542);

        // Meteor Showers 1/α
        AddKeys(MeteorShower1, 515, 524, 538);
        AddKeys(MeteorShower2, 519, 523);

        // Meteor Showers 2/β
        AddKeys(MeteorShower3, 516, 520, 525);
        AddKeys(MeteorShower4, 531, 534, 539);

        // Sporing Mist 1/α
        AddKeys(SporingMist1, 517, 532);
        AddKeys(SporingMist2, 514, 529, 541);

        // Sporing Mist 2/β
        AddKeys(SporingMist3, 513, 526, 528);
        AddKeys(SporingMist4, 535, 540, 544);

        // Phaenna - Thunderstorm 1/α
        AddKeys(Thunderstorms1, 1007, 1019, 1025);
        AddKeys(Thunderstorms2, 1016, 1022);

        // Thunderstorm 2/β
        AddKeys(Thunderstorms3, 1017, 1023, 1034);
        AddKeys(Thunderstorms4, 1010, 1026, 1031);

        // Annealing Winds 1/α
        AddKeys(AnnealingWinds1, 1028, 1032, 1037);
        AddKeys(AnnealingWinds2, 1011, 1020, 1035);

        // Annealing Winds 2/β
        AddKeys(AnnealingWinds3, 1013, 1029, 1038);
        AddKeys(AnnealingWinds4, 1008, 1036);

        // Glass Rain 1/α
        AddKeys(GlassRain1, 1009, 1014, 1021);
        AddKeys(GlassRain2, 1030, 1039);

        // Glass Rain 2/β
        AddKeys(GlassRain3, 1015, 1018, 1033);
        AddKeys(GlassRain4, 1012, 1024, 1027);

        AddKeys(GravitationAnom1, 1350, 1356);
        AddKeys(GravitationAnom2, 1352, 1354, 1358);

        AddKeys(GaleForce1, 1353, 1357, 1363);
        AddKeys(GaleForce2, 1349, 1360, 1369);
        AddKeys(GaleForce3, 1351, 1365);
        AddKeys(GaleForce4, 1359, 1361, 1367);

        AddKeys(BubbleBloom1, 1348, 1362, 1366);
        AddKeys(BubbleBloom2, 1355, 1364, 1368);
    }

    private static void AddKeys(CriticalInfo location, params uint[] keys)
    {
        foreach (var key in keys)
            CriticalLocations[key] = location;
    }

    public class FisherSpotInfo
    {
        public Vector3 FacePosition { get; set; }
        public Vector3 FishingSpot { get; set; }
        public float RotationTolerance { get; set; } = 0.1f;
    }

    public static Dictionary<uint, Dictionary<Vector2, List<FisherSpotInfo>>> MoonFishingLocations = new()
    {
        [1237] = new()
        {
            [new Vector2(-673f, 497f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-685.48f, 57.15f, 448.15f),
                    FishingSpot = new Vector3(-684.29f, 57.14f, 449.44f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-683.17f, 56.18f, 410.53f),
                    FishingSpot = new Vector3(-681.86f, 56.15f, 409.36f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-714.26f, 57.07f, 400.39f),
                    FishingSpot = new Vector3(-715.62f, 57.07f, 398.90f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-718.85f, 57.17f, 429.20f),
                    FishingSpot = new Vector3(-720.61f, 57.15f, 429.68f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-710.78f, 57.15f, 450.46f),
                    FishingSpot = new Vector3(-711.97f, 57.13f, 451.87f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-744.14f, 69.99f, 511.90f),
                    FishingSpot = new Vector3(-742.36f, 69.99f, 511.54f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-752.28f, 70.25f, 497.35f),
                    FishingSpot = new Vector3(-750.49f, 70.21f, 496.88f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-766.31f, 72.83f, 495.21f),
                    FishingSpot = new Vector3(-766.88f, 72.81f, 493.54f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-785.76f, 75.98f, 511.21f),
                    FishingSpot = new Vector3(-787.50f, 75.94f, 511.36f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-779.33f, 73.05f, 531.23f),
                    FishingSpot = new Vector3(-780.99f, 73.10f, 532.39f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-754.05f, 70.47f, 534.56f),
                    FishingSpot = new Vector3(-754.94f, 70.47f, 536.35f),
                },
            },
            [new Vector2(-642f, -631f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-704.47f, 78.86f, -624.86f),
                    FishingSpot = new Vector3(-702.74f, 78.86f, -625.20f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-706.16f, 80.15f, -655.53f),
                    FishingSpot = new Vector3(-704.44f, 80.15f, -654.71f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-719.40f, 81.29f, -674.84f),
                    FishingSpot = new Vector3(-719.62f, 81.27f, -676.77f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-748.08f, 82.86f, -653.60f),
                    FishingSpot = new Vector3(-749.59f, 82.81f, -654.64f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-734.22f, 78.50f, -622.21f),
                    FishingSpot = new Vector3(-735.90f, 78.47f, -621.42f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-756.54f, 79.55f, -589.94f),
                    FishingSpot = new Vector3(-757.96f, 79.48f, -590.99f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-740.87f, 78.75f, -550.59f),
                    FishingSpot = new Vector3(-742.23f, 78.70f, -549.42f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-718.82f, 78.06f, -533.16f),
                    FishingSpot = new Vector3(-719.09f, 78.02f, -531.37f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-693.19f, 76.95f, -546.06f),
                    FishingSpot = new Vector3(-691.86f, 76.91f, -544.77f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-683.75f, 76.02f, -565.56f),
                    FishingSpot = new Vector3(-682.70f, 75.97f, -564.08f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-587.65f, 69.48f, -650.96f),
                    FishingSpot = new Vector3(-589.30f, 69.46f, -650.02f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-588.52f, 69.70f, -672.03f),
                    FishingSpot = new Vector3(-590.26f, 69.65f, -672.61f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-580.89f, 69.61f, -696.34f),
                    FishingSpot = new Vector3(-582.38f, 69.56f, -697.39f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-551.81f, 69.04f, -697.00f),
                    FishingSpot = new Vector3(-551.48f, 68.99f, -698.79f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-536.81f, 68.99f, -673.98f),
                    FishingSpot = new Vector3(-534.98f, 68.99f, -673.93f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-548.28f, 69.07f, -654.38f),
                    FishingSpot = new Vector3(-547.26f, 69.03f, -652.89f),
                },
            },
            [new Vector2(-348f, 604f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-331.19f, 47.71f, 605.12f),
                    FishingSpot = new Vector3(-329.34f, 47.71f, 605.86f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-324.45f, 47.63f, 581.66f),
                    FishingSpot = new Vector3(-322.90f, 47.62f, 580.78f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-347.86f, 47.65f, 566.97f),
                    FishingSpot = new Vector3(-347.55f, 47.62f, 565.18f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-370.45f, 47.68f, 579.07f),
                    FishingSpot = new Vector3(-371.96f, 47.67f, 577.85f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-368.28f, 46.99f, 610.29f),
                    FishingSpot = new Vector3(-370.08f, 46.95f, 610.97f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-357.57f, 47.82f, 635.41f),
                    FishingSpot = new Vector3(-358.79f, 47.78f, 636.85f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-336.68f, 47.69f, 642.65f),
                    FishingSpot = new Vector3(-336.22f, 47.66f, 644.39f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-324.19f, 47.63f, 630.32f),
                    FishingSpot = new Vector3(-322.40f, 47.62f, 629.60f),
                },
            },
            [new Vector2(-281f, -104f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-296.67f, 23.60f, -99.04f),
                    FishingSpot = new Vector3(-298.62f, 23.61f, -98.77f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-288.99f, 22.55f, -82.12f),
                    FishingSpot = new Vector3(-290.41f, 22.56f, -80.72f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-277.21f, 22.42f, -71.16f),
                    FishingSpot = new Vector3(-276.94f, 22.39f, -69.24f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-259.12f, 21.82f, -84.84f),
                    FishingSpot = new Vector3(-257.21f, 21.82f, -85.43f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-265.77f, 22.11f, -93.86f),
                    FishingSpot = new Vector3(-263.89f, 22.11f, -94.51f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-267.32f, 25.05f, -119.22f),
                    FishingSpot = new Vector3(-265.36f, 25.07f, -119.82f),
                },
            },
            [new Vector2(-139f, -283f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-127.01f, 19.50f, -282.83f),
                    FishingSpot = new Vector3(-125.02f, 19.49f, -282.55f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-122.23f, 21.01f, -297.40f),
                    FishingSpot = new Vector3(-120.34f, 21.01f, -296.72f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-134.13f, 21.89f, -305.78f),
                    FishingSpot = new Vector3(-133.89f, 21.89f, -307.83f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-145.22f, 21.36f, -299.16f),
                    FishingSpot = new Vector3(-147.11f, 21.38f, -300.01f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-151.91f, 20.48f, -276.35f),
                    FishingSpot = new Vector3(-153.78f, 20.48f, -275.65f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-140.59f, 20.73f, -265.49f),
                    FishingSpot = new Vector3(-140.35f, 20.74f, -263.50f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-128.37f, 19.81f, -268.13f),
                    FishingSpot = new Vector3(-126.56f, 19.80f, -267.38f),
                },
            },
            [new Vector2(104f, -269f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(81.29f, 18.29f, -250.49f),
                    FishingSpot = new Vector3(79.81f, 18.34f, -249.27f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(109.53f, 18.80f, -231.42f),
                    FishingSpot = new Vector3(110.04f, 18.81f, -229.60f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(145.64f, 24.61f, -258.82f),
                    FishingSpot = new Vector3(147.53f, 24.62f, -258.54f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(142.12f, 28.26f, -277.96f),
                    FishingSpot = new Vector3(143.39f, 28.23f, -279.23f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(96.34f, 29.02f, -295.20f),
                    FishingSpot = new Vector3(95.97f, 29.03f, -297.06f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(67.54f, 25.26f, -291.20f),
                    FishingSpot = new Vector3(65.88f, 25.24f, -292.10f),
                },
            },
            [new Vector2(193f, 196f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(191.93f, 17.58f, 188.29f),
                    FishingSpot = new Vector3(191.46f, 17.58f, 186.35f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(184.14f, 17.50f, 193.09f),
                    FishingSpot = new Vector3(182.83f, 17.50f, 191.59f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(182.53f, 17.60f, 200.46f),
                    FishingSpot = new Vector3(181.67f, 17.55f, 201.99f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(196.90f, 19.35f, 206.23f),
                    FishingSpot = new Vector3(196.96f, 19.35f, 208.23f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(202.64f, 19.47f, 202.95f),
                    FishingSpot = new Vector3(203.70f, 19.47f, 204.47f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(208.40f, 19.45f, 196.68f),
                    FishingSpot = new Vector3(210.37f, 19.45f, 197.03f),
                },
            },
            [new Vector2(573f, 573f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(609.12f, 54.33f, 580.25f),
                    FishingSpot = new Vector3(610.75f, 54.33f, 581.42f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(622.91f, 53.68f, 534.43f),
                    FishingSpot = new Vector3(624.69f, 53.68f, 533.99f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(617.22f, 54.78f, 513.61f),
                    FishingSpot = new Vector3(619.07f, 54.78f, 512.96f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(616.89f, 54.38f, 495.02f),
                    FishingSpot = new Vector3(618.41f, 54.36f, 496.12f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(615.84f, 50.98f, 479.81f),
                    FishingSpot = new Vector3(616.91f, 50.92f, 478.42f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(603.52f, 50.28f, 480.78f),
                    FishingSpot = new Vector3(603.09f, 50.27f, 479.07f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(593.25f, 50.03f, 491.32f),
                    FishingSpot = new Vector3(591.76f, 50.03f, 490.30f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(586.13f, 50.49f, 502.14f),
                    FishingSpot = new Vector3(584.81f, 50.48f, 500.88f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(576.58f, 50.34f, 514.46f),
                    FishingSpot = new Vector3(575.03f, 50.29f, 513.57f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(562.37f, 51.04f, 534.96f),
                    FishingSpot = new Vector3(561.92f, 51.03f, 533.17f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(529.04f, 51.24f, 538.05f),
                    FishingSpot = new Vector3(528.37f, 51.21f, 536.42f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(514.62f, 52.87f, 559.11f),
                    FishingSpot = new Vector3(512.91f, 52.87f, 558.70f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(518.79f, 51.11f, 580.81f),
                    FishingSpot = new Vector3(516.99f, 51.06f, 581.01f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(527.99f, 53.66f, 597.43f),
                    FishingSpot = new Vector3(526.16f, 53.66f, 597.76f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(521.59f, 53.16f, 619.16f),
                    FishingSpot = new Vector3(519.88f, 53.14f, 618.53f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(520.15f, 51.23f, 638.33f),
                    FishingSpot = new Vector3(518.43f, 51.19f, 638.93f),
                },
            },
            [new Vector2(909f, -336f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(916.68f, -58.34f, -328.82f),
                    FishingSpot = new Vector3(914.87f, -58.35f, -328.79f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(920.33f, -58.86f, -336.27f),
                    FishingSpot = new Vector3(918.65f, -58.90f, -337.07f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(935.54f, -57.68f, -355.58f),
                    FishingSpot = new Vector3(934.37f, -57.65f, -356.92f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(938.19f, -56.75f, -360.04f),
                    FishingSpot = new Vector3(936.45f, -56.75f, -360.90f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(941.39f, -55.70f, -364.66f),
                    FishingSpot = new Vector3(940.42f, -55.71f, -366.29f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(944.47f, -54.79f, -367.15f),
                    FishingSpot = new Vector3(943.26f, -54.79f, -368.71f),
                },
            },
        },
        [1291] = new()
        {
            [new Vector2(-700f, -652f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-682.75f, -3.97f, -653.59f),
                    FishingSpot = new Vector3(-680.97f, -4.06f, -653.59f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-686.68f, -3.82f, -661.86f),
                    FishingSpot = new Vector3(-685.18f, -3.94f, -662.78f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-695.94f, -3.85f, -666.93f),
                    FishingSpot = new Vector3(-695.36f, -3.98f, -668.60f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-709.43f, -3.91f, -664.23f),
                    FishingSpot = new Vector3(-710.34f, -3.99f, -665.82f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-715.24f, -3.87f, -651.65f),
                    FishingSpot = new Vector3(-717.04f, -3.96f, -651.28f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-710.10f, -3.98f, -638.81f),
                    FishingSpot = new Vector3(-711.18f, -4.07f, -637.41f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-695.71f, -4.07f, -635.30f),
                    FishingSpot = new Vector3(-695.14f, -4.13f, -633.54f),
                },
            },
            // Export for Fishing Zone 1291, Flag (-522, 462)
            [new Vector2(-522f, 462f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-441.19f, 10.00f, 487.85f),
                    FishingSpot = new Vector3(-439.22f, 10.00f, 487.49f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-445.83f, 10.00f, 468.06f),
                    FishingSpot = new Vector3(-443.86f, 10.00f, 467.75f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-452.36f, 10.00f, 455.65f),
                    FishingSpot = new Vector3(-450.51f, 10.00f, 454.90f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-459.28f, 10.00f, 438.29f),
                    FishingSpot = new Vector3(-457.47f, 10.00f, 439.13f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-461.28f, 10.00f, 428.23f),
                    FishingSpot = new Vector3(-460.19f, 10.00f, 426.55f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-471.23f, 10.00f, 417.92f),
                    FishingSpot = new Vector3(-469.41f, 10.00f, 417.09f),
                },
            },
            [new Vector2(-450f, -673f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-453.06f, -9.17f, -673.84f),
                    FishingSpot = new Vector3(-454.69f, -9.23f, -673.17f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-450.95f, -9.19f, -663.79f),
                    FishingSpot = new Vector3(-452.35f, -9.21f, -662.55f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-445.48f, -9.10f, -661.14f),
                    FishingSpot = new Vector3(-444.48f, -9.18f, -659.68f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-442.20f, -9.78f, -667.60f),
                    FishingSpot = new Vector3(-440.41f, -9.82f, -666.91f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-441.65f, -8.79f, -674.87f),
                    FishingSpot = new Vector3(-439.83f, -8.77f, -674.59f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-447.86f, -9.56f, -684.91f),
                    FishingSpot = new Vector3(-446.96f, -9.69f, -686.45f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-457.66f, -8.34f, -689.08f),
                    FishingSpot = new Vector3(-457.68f, -8.38f, -690.98f),
                },
            },
            // Export for Fishing Zone 1291, Flag (-252, -74)
            [new Vector2(-252f, -74f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-250.39f, -10.20f, -76.12f),
                    FishingSpot = new Vector3(-249.49f, -10.25f, -77.68f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-240.21f, -11.59f, -76.41f),
                    FishingSpot = new Vector3(-240.33f, -11.62f, -78.31f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-231.32f, -11.42f, -75.66f),
                    FishingSpot = new Vector3(-230.43f, -11.45f, -77.20f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-232.67f, -11.04f, -73.56f),
                    FishingSpot = new Vector3(-231.23f, -11.10f, -72.50f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-238.98f, -11.15f, -70.22f),
                    FishingSpot = new Vector3(-238.98f, -11.25f, -68.43f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-245.00f, -11.12f, -68.31f),
                    FishingSpot = new Vector3(-243.54f, -11.18f, -67.34f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-252.52f, -10.07f, -65.02f),
                    FishingSpot = new Vector3(-252.52f, -10.07f, -63.13f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-259.07f, -9.85f, -66.64f),
                    FishingSpot = new Vector3(-260.30f, -9.90f, -65.25f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-269.25f, -10.80f, -75.05f),
                    FishingSpot = new Vector3(-270.98f, -10.83f, -74.62f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-265.35f, -10.26f, -77.56f),
                    FishingSpot = new Vector3(-266.49f, -10.29f, -78.99f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-257.34f, -11.07f, -81.12f),
                    FishingSpot = new Vector3(-257.52f, -11.08f, -82.96f),
                },
            },
            // Export for Fishing Zone 1291, Flag (-239, -352)
            [new Vector2(-239f, -352f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-239.01f, -6.54f, -353.85f),
                    FishingSpot = new Vector3(-239.26f, -6.63f, -355.69f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-232.77f, -7.04f, -357.39f),
                    FishingSpot = new Vector3(-234.13f, -7.05f, -358.69f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-219.62f, -4.49f, -360.75f),
                    FishingSpot = new Vector3(-218.18f, -4.50f, -361.86f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-222.11f, -7.16f, -353.34f),
                    FishingSpot = new Vector3(-220.26f, -7.21f, -353.64f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-219.93f, -6.95f, -345.15f),
                    FishingSpot = new Vector3(-218.17f, -6.95f, -345.71f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-215.17f, -7.55f, -335.11f),
                    FishingSpot = new Vector3(-213.56f, -7.57f, -334.14f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-223.81f, -9.02f, -332.62f),
                    FishingSpot = new Vector3(-224.87f, -9.06f, -331.05f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-227.75f, -6.76f, -345.32f),
                    FishingSpot = new Vector3(-228.63f, -6.71f, -343.76f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-239.19f, -8.74f, -342.49f),
                    FishingSpot = new Vector3(-239.12f, -8.75f, -340.62f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-245.71f, -9.11f, -347.48f),
                    FishingSpot = new Vector3(-246.44f, -9.14f, -345.84f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-253.36f, -9.66f, -350.12f),
                    FishingSpot = new Vector3(-255.05f, -9.66f, -349.50f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-259.28f, -8.49f, -358.00f),
                    FishingSpot = new Vector3(-260.39f, -8.70f, -359.37f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-250.44f, -6.84f, -355.39f),
                    FishingSpot = new Vector3(-250.22f, -6.85f, -357.14f),
                },
            },
            // Export for Fishing Zone 1291, Flag (28, 99)
            [new Vector2(28f, 99f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(4.38f, -12.41f, 65.35f),
                    FishingSpot = new Vector3(2.85f, -12.51f, 64.48f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(9.18f, -14.01f, 66.33f),
                    FishingSpot = new Vector3(10.25f, -14.11f, 64.94f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(16.86f, -12.19f, 75.01f),
                    FishingSpot = new Vector3(18.04f, -12.19f, 73.60f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(26.57f, -12.28f, 78.14f),
                    FishingSpot = new Vector3(27.43f, -12.32f, 76.54f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(30.68f, -12.17f, 83.49f),
                    FishingSpot = new Vector3(32.59f, -12.18f, 83.39f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(38.61f, -12.68f, 99.73f),
                    FishingSpot = new Vector3(40.23f, -12.72f, 98.95f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(42.75f, -11.42f, 110.16f),
                    FishingSpot = new Vector3(43.90f, -11.44f, 108.78f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(48.54f, -12.61f, 120.29f),
                    FishingSpot = new Vector3(50.35f, -12.65f, 120.35f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(59.60f, -12.07f, 132.49f),
                    FishingSpot = new Vector3(60.76f, -12.09f, 131.02f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(62.03f, -12.27f, 138.09f),
                    FishingSpot = new Vector3(63.89f, -12.32f, 138.41f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(49.03f, -13.73f, 137.06f),
                    FishingSpot = new Vector3(48.68f, -13.80f, 138.88f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(32.23f, -13.47f, 127.90f),
                    FishingSpot = new Vector3(32.15f, -13.68f, 129.67f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(22.37f, -12.88f, 121.08f),
                    FishingSpot = new Vector3(20.85f, -13.00f, 121.99f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(16.11f, -12.12f, 109.93f),
                    FishingSpot = new Vector3(14.83f, -12.12f, 111.30f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(11.28f, -12.08f, 101.84f),
                    FishingSpot = new Vector3(9.58f, -12.16f, 101.33f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(20.85f, -10.47f, 89.04f),
                    FishingSpot = new Vector3(19.14f, -10.52f, 89.53f),
                },
            },
            // Export for Fishing Zone 1291, Flag (46, -344)
            [new Vector2(46f, -344f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(41.02f, 9.81f, -341.12f),
                    FishingSpot = new Vector3(41.51f, 9.79f, -339.41f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(32.40f, 7.93f, -331.17f),
                    FishingSpot = new Vector3(31.79f, 7.84f, -329.47f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(35.20f, 8.40f, -337.68f),
                    FishingSpot = new Vector3(33.50f, 8.40f, -338.15f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(39.32f, 8.84f, -345.02f),
                    FishingSpot = new Vector3(38.50f, 8.76f, -346.58f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(52.08f, 12.01f, -353.86f),
                    FishingSpot = new Vector3(50.84f, 11.86f, -355.20f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(58.34f, 11.73f, -349.74f),
                    FishingSpot = new Vector3(60.05f, 11.59f, -350.42f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(54.53f, 10.39f, -342.30f),
                    FishingSpot = new Vector3(55.84f, 10.34f, -341.09f),
                },
            },
            // Export for Fishing Zone 1291, Flag (214, -742)
            [new Vector2(214f, -742f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(217.65f, 133.63f, -748.86f),
                    FishingSpot = new Vector3(217.51f, 133.57f, -750.65f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(211.36f, 133.82f, -743.07f),
                    FishingSpot = new Vector3(209.77f, 133.82f, -744.00f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(205.07f, 133.68f, -736.70f),
                    FishingSpot = new Vector3(205.18f, 133.59f, -738.54f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(205.86f, 133.71f, -731.38f),
                    FishingSpot = new Vector3(206.03f, 133.68f, -729.59f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(217.06f, 133.43f, -733.05f),
                    FishingSpot = new Vector3(218.78f, 133.40f, -732.39f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(216.22f, 133.74f, -740.91f),
                    FishingSpot = new Vector3(217.94f, 133.71f, -740.18f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(222.46f, 133.68f, -748.29f),
                    FishingSpot = new Vector3(224.13f, 133.67f, -747.62f),
                },
            },
            // Export for Fishing Zone 1291, Flag (462, -47)
            [new Vector2(462f, -47f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(528.20f, 1.61f, 7.02f),
                    FishingSpot = new Vector3(526.37f, 1.46f, 6.78f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(519.13f, 3.82f, -8.26f),
                    FishingSpot = new Vector3(517.66f, 3.72f, -7.25f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(531.90f, 2.14f, 15.45f),
                    FishingSpot = new Vector3(532.72f, 2.09f, 17.03f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(530.45f, 3.97f, -3.05f),
                    FishingSpot = new Vector3(530.28f, 3.94f, -4.81f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(543.67f, 4.18f, -10.15f),
                    FishingSpot = new Vector3(545.12f, 4.14f, -11.22f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(538.89f, 2.93f, 2.09f),
                    FishingSpot = new Vector3(540.22f, 2.90f, 3.38f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(390.65f, 28.77f, -81.59f),
                    FishingSpot = new Vector3(392.52f, 28.75f, -82.02f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(390.79f, 26.54f, -71.23f),
                    FishingSpot = new Vector3(392.21f, 26.54f, -70.02f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(380.76f, 25.43f, -67.89f),
                    FishingSpot = new Vector3(380.79f, 25.43f, -65.96f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(377.05f, 26.12f, -72.07f),
                    FishingSpot = new Vector3(375.33f, 26.09f, -72.63f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(382.83f, 27.37f, -76.90f),
                    FishingSpot = new Vector3(381.41f, 27.31f, -78.12f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(387.62f, 29.30f, -84.06f),
                    FishingSpot = new Vector3(386.65f, 29.25f, -85.66f),
                },
            },
            // Export for Fishing Zone 1291, Flag (526, 448)
            [new Vector2(526f, 448f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(522.33f, -255.60f, 453.09f),
                    FishingSpot = new Vector3(521.07f, -255.60f, 454.64f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(511.24f, -255.60f, 429.31f),
                    FishingSpot = new Vector3(509.94f, -255.60f, 430.82f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(528.40f, -255.60f, 415.81f),
                    FishingSpot = new Vector3(526.90f, -255.60f, 417.13f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(518.11f, -255.60f, 405.44f),
                    FishingSpot = new Vector3(519.26f, -255.60f, 407.08f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(534.20f, -255.60f, 397.19f),
                    FishingSpot = new Vector3(534.44f, -255.60f, 395.21f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(546.96f, -255.60f, 405.64f),
                    FishingSpot = new Vector3(548.01f, -255.60f, 403.94f),
                },
            },
            // Export for Fishing Zone 1291, Flag (562, 580)
            [new Vector2(562f, 580f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(585.75f, -250.81f, 616.36f),
                    FishingSpot = new Vector3(585.30f, -250.81f, 618.31f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(580.21f, -250.11f, 614.54f),
                    FishingSpot = new Vector3(579.87f, -250.11f, 616.51f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(571.92f, -250.85f, 613.36f),
                    FishingSpot = new Vector3(571.70f, -250.85f, 615.35f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(565.48f, -251.45f, 612.56f),
                    FishingSpot = new Vector3(565.34f, -251.45f, 614.56f),
                },
            },
        },
        [1310] = new()
        {
            // Export for Fishing Zone 1310, Flag (-511, 230)
            [new Vector2(-511f, 230f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-512.13f, 15.40f, 227.57f),
                    FishingSpot = new Vector3(-510.37f, 15.40f, 226.63f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-516.50f, 15.40f, 223.56f),
                    FishingSpot = new Vector3(-515.30f, 15.40f, 221.96f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-504.38f, 15.41f, 218.23f),
                    FishingSpot = new Vector3(-503.29f, 15.41f, 216.55f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-491.18f, 15.40f, 225.17f),
                    FishingSpot = new Vector3(-492.53f, 15.40f, 223.69f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-497.89f, 15.40f, 232.71f),
                    FishingSpot = new Vector3(-497.09f, 15.40f, 230.88f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-493.26f, 15.40f, 238.30f),
                    FishingSpot = new Vector3(-493.36f, 15.40f, 236.30f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-502.69f, 15.40f, 238.92f),
                    FishingSpot = new Vector3(-500.73f, 15.40f, 238.50f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-519.05f, 15.40f, 236.67f),
                    FishingSpot = new Vector3(-517.06f, 15.40f, 236.84f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-526.60f, 15.41f, 240.59f),
                    FishingSpot = new Vector3(-524.95f, 15.41f, 239.47f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-531.07f, 15.40f, 234.91f),
                    FishingSpot = new Vector3(-529.07f, 15.40f, 235.08f),
                },
            },
            // Export for Fishing Zone 1310, Flag (-334, 898)
            [new Vector2(-334f, 898f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-380.18f, 102.58f, 922.58f),
                    FishingSpot = new Vector3(-380.26f, 102.58f, 920.58f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-373.28f, 102.58f, 922.31f),
                    FishingSpot = new Vector3(-373.36f, 102.58f, 920.31f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-355.99f, 102.61f, 920.24f),
                    FishingSpot = new Vector3(-356.47f, 102.61f, 918.30f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-350.54f, 102.62f, 919.65f),
                    FishingSpot = new Vector3(-351.03f, 102.62f, 917.71f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-347.62f, 102.61f, 919.36f),
                    FishingSpot = new Vector3(-347.14f, 102.61f, 917.42f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-376.59f, 102.60f, 922.05f),
                    FishingSpot = new Vector3(-376.57f, 102.60f, 920.05f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-384.27f, 102.56f, 923.24f),
                    FishingSpot = new Vector3(-384.64f, 102.56f, 921.27f),
                },
            },
            // Export for Fishing Zone 1310, Flag (-51, -519)
            [new Vector2(-51f, -519f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-55.38f, -66.96f, -527.45f),
                    FishingSpot = new Vector3(-56.40f, -66.96f, -529.18f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-40.73f, -65.77f, -539.43f),
                    FishingSpot = new Vector3(-42.53f, -65.77f, -540.29f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-31.08f, -68.54f, -537.84f),
                    FishingSpot = new Vector3(-29.12f, -68.54f, -538.27f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-26.15f, -66.13f, -526.53f),
                    FishingSpot = new Vector3(-24.63f, -66.13f, -527.83f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-31.74f, -62.66f, -518.68f),
                    FishingSpot = new Vector3(-30.55f, -62.66f, -517.07f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-45.90f, -66.10f, -509.32f),
                    FishingSpot = new Vector3(-44.59f, -66.10f, -507.82f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-63.94f, -69.19f, -495.79f),
                    FishingSpot = new Vector3(-62.40f, -69.19f, -494.51f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-71.87f, -71.46f, -502.63f),
                    FishingSpot = new Vector3(-73.84f, -71.46f, -502.98f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(-68.20f, -71.81f, -513.47f),
                    FishingSpot = new Vector3(-70.20f, -71.81f, -513.42f),
                },
            },
            // Export for Fishing Zone 1310, Flag (106, -39)
            [new Vector2(106f, -39f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(96.64f, -1.22f, -30.78f),
                    FishingSpot = new Vector3(94.65f, -1.22f, -30.54f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(103.03f, -1.22f, -26.30f),
                    FishingSpot = new Vector3(101.46f, -1.22f, -27.54f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(112.07f, -1.22f, -27.00f),
                    FishingSpot = new Vector3(110.08f, -1.22f, -26.77f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(113.49f, -1.22f, -33.72f),
                    FishingSpot = new Vector3(115.48f, -1.22f, -33.96f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(108.45f, -1.22f, -39.52f),
                    FishingSpot = new Vector3(110.01f, -1.22f, -38.27f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(119.13f, -1.22f, -43.91f),
                    FishingSpot = new Vector3(117.15f, -1.22f, -43.66f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(119.70f, -1.22f, -48.81f),
                    FishingSpot = new Vector3(120.94f, -1.22f, -50.38f),
                },
            },
            // Export for Fishing Zone 1310, Flag (241, 430)
            [new Vector2(241f, 430f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(449.80f, 94.63f, 404.88f),
                    FishingSpot = new Vector3(447.92f, 94.63f, 405.54f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(446.51f, 91.86f, 393.92f),
                    FishingSpot = new Vector3(444.81f, 91.86f, 394.98f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(401.28f, 82.40f, 268.95f),
                    FishingSpot = new Vector3(399.84f, 82.40f, 270.34f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(383.58f, 78.66f, 248.24f),
                    FishingSpot = new Vector3(382.80f, 78.66f, 250.09f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(338.36f, 76.28f, 254.76f),
                    FishingSpot = new Vector3(339.02f, 76.28f, 256.65f),
                },
            },
            // Export for Fishing Zone 1310, Flag (429, -736)
            [new Vector2(429f, -736f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(410.80f, -144.58f, -636.60f),
                    FishingSpot = new Vector3(410.24f, -144.58f, -634.68f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(393.89f, -146.83f, -638.76f),
                    FishingSpot = new Vector3(393.74f, -146.83f, -636.77f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(373.38f, -150.69f, -648.75f),
                    FishingSpot = new Vector3(372.75f, -150.69f, -646.85f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(347.03f, -154.54f, -655.43f),
                    FishingSpot = new Vector3(346.45f, -154.54f, -653.52f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(341.52f, -154.55f, -656.59f),
                    FishingSpot = new Vector3(341.03f, -154.55f, -654.65f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(332.83f, -154.56f, -660.02f),
                    FishingSpot = new Vector3(332.34f, -154.56f, -658.08f),
                },
            },
            // Export for Fishing Zone 1310, Flag (639, 48)
            [new Vector2(639f, 48f)] = new()
            {
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(614.55f, 160.22f, 63.39f),
                    FishingSpot = new Vector3(615.35f, 160.22f, 65.23f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(611.16f, 158.98f, 64.77f),
                    FishingSpot = new Vector3(611.72f, 158.98f, 66.69f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(605.80f, 156.92f, 65.59f),
                    FishingSpot = new Vector3(605.54f, 156.92f, 67.57f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(600.11f, 155.26f, 64.24f),
                    FishingSpot = new Vector3(599.85f, 155.26f, 66.23f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(595.27f, 152.97f, 60.86f),
                    FishingSpot = new Vector3(594.09f, 152.97f, 62.47f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(588.11f, 151.23f, 59.41f),
                    FishingSpot = new Vector3(588.16f, 151.23f, 61.40f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(584.33f, 150.32f, 59.80f),
                    FishingSpot = new Vector3(584.58f, 150.32f, 61.79f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(580.29f, 148.96f, 61.39f),
                    FishingSpot = new Vector3(580.93f, 148.96f, 63.28f),
                },
                new FisherSpotInfo()
                {
                    FacePosition = new Vector3(576.68f, 147.61f, 63.01f),
                    FishingSpot = new Vector3(577.33f, 147.61f, 64.90f),
                },
            },
        }
    };

    public static Dictionary<string, List<uint>> MoonBaits = new();
    public static Dictionary<string, List<uint>> MoonFish = new();
}
