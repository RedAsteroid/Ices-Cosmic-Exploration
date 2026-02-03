using Dalamud.Game.ClientState.Conditions;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game.WKS;
using ICE.Utilities.Cosmic_Helper;
using ICE.Utilities.GatheringHelper;
using System.Reflection.Metadata;
using TerraFX.Interop.Windows;
using YamlDotNet.Core.Tokens;
using static Dalamud.Interface.Utility.Raii.ImRaii;
using static ECommons.UIHelpers.AddonMasterImplementations.AddonMaster;
using static ICE.Utilities.WKSManagerCustom;

namespace ICE.Scheduler.Tasks
{
    internal static class Task_CheckScore
    {
        public static void Enqueue()
        {
            var Id = CosmicHelper.CurrentLunarMission;
            var mission = CosmicHelper.SheetMissionDict[Id];

            var jobs = mission.Jobs;

            if (CosmicHelper.CrafterJobList.Any(x => jobs.Contains(x)) && CosmicHelper.GatheringJobList.Any(x => jobs.Contains(x)))
            {
                P.TaskManager.Enqueue(() => DualClass(), "Checking dual class score");
                P.TaskManager.Enqueue(() => SchedulerMain.State = IceState.DualClass, "Setting state to dual class");
            }
            else if (CosmicHelper.CrafterJobList.Any(x => jobs.Contains(x)))
            {
                IceLogging.Info("Currently on a crafting job, checking for crafting scoring", "Task: Score Check");
                P.TaskManager.Enqueue(() => SchedulerMain.State = IceState.Craft);
                P.TaskManager.Enqueue(() => Crafts(), "Checking for crafting score mission");
            }
            else if (CosmicHelper.GatheringJobList.Any(x => jobs.Contains(x)))
            {
                var jobId = Player.Job;

                IceLogging.Info($"Currently on a gathering job {jobId}");
                if (jobId == (Job)18)
                {
                    P.TaskManager.Enqueue(() => SchedulerMain.State = IceState.Fish);
                    P.TaskManager.Enqueue(() => Fish(), "Checking fishing missions for score");
                }
                else
                {
                    P.TaskManager.Enqueue(() => SchedulerMain.State = IceState.Gather);
                    P.TaskManager.Enqueue(() => Gather(), "Checking the gathering score");
                }
            }
        }

        public static unsafe bool? Fish()
        {
            string tag = "[Task_Check Score: Fish]";
            var currentMission = CosmicHelper.CurrentLunarMission;

            IceLogging.Verbose($"Score check for fish was initialized. Checking for minimum requirements: [{currentMission}]", tag);
            if (CosmicHelper.SheetMissionDict.TryGetValue(currentMission, out var sheetInfo))
            {
                if (GenericHelpers.TryGetAddonMaster<WKSMissionInfomation>("WKSMissionInfomation", out var missionInfo) && missionInfo.IsAddonReady)
                {
                    if (CosmicHandler.IsMissionTimedOut())
                    {
                        IceLogging.Debug("Mission is timed out, attempting to abandon", tag);
                        SchedulerMain.State = IceState.AbandonMission;
                        P.TaskManager.Tasks.Clear();
                        return true;
                    }

                    IceLogging.Verbose("Mission info was valid, searching what we should be checking for", tag);

                    if (sheetInfo.Gathering_Min.Count > 0)
                    {
                        IceLogging.Verbose($"Fishing mission has a minumum amount of fish needed. Checking the specifics for each", tag);
                        foreach (var fishItem in sheetInfo.Gathering_Min)
                        {
                            if (PlayerHelper.GetItemCount(fishItem.Key, out var amount))
                            {
                                if (amount < fishItem.Value)
                                {
                                    IceLogging.Debug("We've found a fish that we're still missing!\n" +
                                        $"ItemID: {fishItem.Key}. We need: {fishItem.Value}. We have: {amount}", tag);

                                    return true;
                                }
                            }
                        }
                    }
                    else if (sheetInfo.Fish_AmountRequired > 0)
                    {
                        IceLogging.Verbose("We're in a mission where we need a certain amount of fish overall. So checking that", tag);
                        var amount = CurrentCollectedTotal();

                        if (amount < sheetInfo.Fish_AmountRequired)
                        {
                            IceLogging.Debug($"We're not at the total amount needed for the mission. Need: {sheetInfo.Fish_AmountRequired} | Have: {amount}", tag);
                            return true;
                        }
                    }
                    else if (sheetInfo.Fish_VarietyAmount > 0)
                    {
                        var amount = CurrentIndividualTotal();
                        if (amount < sheetInfo.Fish_VarietyAmount)
                        {
                            IceLogging.Debug($"Need a variety of fish, and we're missing some. Need: {sheetInfo.Fish_VarietyAmount} | Have: {amount}", tag);
                            return true;
                        }
                    }
                    else
                    {
                        var currentScore = CurrentScore();
                        var bronzeScore = sheetInfo.BronzeScore;

                        if (currentScore < bronzeScore && sheetInfo.BronzeScore != 0)
                        {
                            IceLogging.Debug("We need to still hit the score threshold for bronze. So we're still gonna fish", tag);
                            return true;
                        }
                    }

                    var rank = CurrentRank();

                    if (rank != MissionRank.None || sheetInfo.Attributes.HasFlag(MissionAttributes.Critical))
                    {
                        if (sheetInfo.Attributes.HasFlag(MissionAttributes.ScoreTimeRemaining))
                        {
                            IceLogging.Debug("We're in a mission where we're just meeting the minimum score. Turning in", tag);
                            SchedulerMain.State = IceState.TurninMission;
                            P.TaskManager.Tasks.Clear();

                            Mission_Settings.TurninState = DetermineTurninState();
                            return true;
                        }
                        else
                        {
                            bool shouldTurnin = false;

                            if (sheetInfo.Attributes.HasFlag(MissionAttributes.Critical))
                            {
                                IceLogging.Verbose("We're in a critical mission, this needs to just be turned in", tag);
                                shouldTurnin = true;
                            }
                            else if (C.XPLeveling_Mode && rank >= MissionRank.Bronze)
                            {
                                IceLogging.Debug("We're in Leveling Mode, and we only need a bronze. So we're setting turnin to true");
                                shouldTurnin = true;
                            }
                            else
                            {
                                var config = C.MissionConfig[currentMission];

                                shouldTurnin = ((config.TurninGold || config.AutoTurnin) && rank >= MissionRank.Gold) ||
                                               (config.TurninSilver && rank >= MissionRank.Silver) ||
                                               (config.TurninBronze && rank >= MissionRank.Bronze);
                            }

                            if (shouldTurnin)
                            {
                                MedalChecker(rank);
                                IceLogging.Info("The threshold for scoring was met. Time to turnin", tag);
                                SchedulerMain.State = IceState.TurninMission;
                                P.TaskManager.Tasks.Clear();

                                return true;
                            }
                            else
                            {
                                var config = C.MissionConfig[currentMission];

                                IceLogging.Debug("We're still going for a score/not met threshold.\n" +
                                    $"Rank: {rank.ToString()}\n" +
                                    $"Any Turnin: {config.AutoTurnin}" +
                                    $"Gold Turnin: {config.TurninGold}\n" +
                                    $"Silver Turnin: {config.TurninSilver}\n" +
                                    $"Bronze Turnin: {config.TurninBronze}", tag);
                                return true;
                            }
                        }
                    }
                    else
                    {
                        IceLogging.Error($"Hey, it seems something slipped through the cracks. If you could report to me this it would be great\n" +
                            $"ID: {currentMission}\n" +
                            $"Name: {sheetInfo.Name}\n" +
                            $"Missed the score ranking but slipped through", tag);

                        return true;

                    }
                }
                else
                {
                    if (GenericHelpers.TryGetAddonMaster<WKSHud>("WKSHud", out var moonHud))
                    {
                        if (EzThrottler.Throttle("Opening the moon hud", 1000))
                        {
                            moonHud.Mission();
                            IceLogging.Info("Hud wasn't visible. Opening it", "[Score Check]");
                        }
                    }
                }
            }

            return false;
        }

        private static unsafe uint CurrentCollectedTotal()
        {
            var managerPtr = WKSManager.Instance();
            if (managerPtr == null) return 0;

            var manager = (WKSManagerCustom*)managerPtr;
            return manager->CollectedTotal;
        }

        private static unsafe uint CurrentIndividualTotal()
        {
            var managerPtr = WKSManager.Instance();
            if (managerPtr == null) return 0;

            var manager = (WKSManagerCustom*)managerPtr;
            return manager->CollectedIndividual;
        }

        private static unsafe uint CurrentScore()
        {
            var managerPtr = WKSManager.Instance();
            if (managerPtr == null) return 0;

            var manager = (WKSManagerCustom*)managerPtr;
            return manager->CurrentScore;
        }

        public static unsafe MissionRank CurrentRank()
        {
            var managerPtr = WKSManager.Instance();
            if (managerPtr == null) return MissionRank.None;

            var manager = (WKSManagerCustom*)managerPtr;
            return manager->CurrentRank;
        }

        public static unsafe bool? Crafts()
        {
            string tag = "Check Score: Crafts";
            IceLogging.Verbose("Checking score progress with crafts", tag);
            if (GenericHelpers.TryGetAddonMaster<WKSMissionInfomation>("WKSMissionInfomation", out var missionInfo) && missionInfo.IsAddonReady)
            {
                if (CosmicHandler.IsMissionTimedOut())
                {
                    IceLogging.Debug("Mission is timed out, attempting to abandon", tag);
                    SchedulerMain.State = IceState.AbandonMission;
                    P.TaskManager.Tasks.Clear();
                    return true;
                }

                var Id = CosmicHelper.CurrentLunarMission;
                // var mission = CosmicHelper.Dict_CosmicMissions[Id];
                var mission = CosmicHelper.SheetMissionDict[Id];
                bool shouldTurnin = false;

                if (mission.Attributes.HasFlag(MissionAttributes.Critical))
                {
                    if (missionInfo.CriticalScore == null)
                        return false;

                    if (missionInfo.CriticalScore.Value == 1)
                    {
                        IceLogging.Verbose("We've completed the critical!", tag);
                        shouldTurnin = true;
                    }
                }
                else
                {
                    if (missionInfo.CurrentScore == null)
                        return false;

                    // First things first, have to check to see if you have enough of the initial crafts/meet the score threshold
                    foreach (var item in mission.Crafts_Main)
                    {
                        var itemId = item.Value.ItemId;
                        var recipeEntry = item.Value;

                        if (!PlayerHelper.GetItemCount(itemId, out var count) || count < recipeEntry.RequiredAmount)
                        {
                            IceLogging.Debug("Found an item that you didn't have the minumim amount of. Continuing on with our task", "[Task_CheckScore: Craft]");
                            IceLogging.Debug($"RecipeId: {item.Key} | Have: {count} | Expected amount: {recipeEntry.RequiredAmount}");
                            SchedulerMain.State = IceState.Craft;
                            return true;
                        }
                    }

                    // Next, need to check to see if there is a bronze threshold that is required, and make sure we're hitting it (if there is any)

                    var currentScore = missionInfo.CurrentScore.Value;
                    var bronzeScore = mission.BronzeScore;
                    var silverScore = mission.SilverScore;
                    var goldScore = mission.GoldScore;

                    if (bronzeScore != 0 && (currentScore <= bronzeScore))
                    {
                        IceLogging.Info("Bronze score is recorded at not 0. Which means that it needs a minimum score. \n" +
                                        $"Current Score: {currentScore}\n" +
                                        $"Minimum Score: {bronzeScore}\n" +
                                        $"Continuing on with the crafting process");
                        return true;
                    }
                    else
                    {

                        var config = C.MissionConfig[Id];
                        bool AnyTurnin = config.AutoTurnin;
                        bool GoldGoal = goldScore <= currentScore;
                        bool SilverGoal = silverScore <= currentScore;
                        bool TurninBronze = config.TurninBronze;

                        if (C.XPLeveling_Mode)
                        {
                            IceLogging.Info("Leveling mode is enabled, and you met the brone threshold, turning in", tag);
                            shouldTurnin = true;
                        }
                        else if (config.AutoTurnin)
                        {
                            // AutoTurnin enabled, going to check for gold only since we have materials/time still
                            if (GoldGoal)
                            {
                                IceLogging.Info("Auto turnin was enabled, and hit the max score.", tag);
                                shouldTurnin = true;
                            }
                        }
                        else
                        {
                            if (GoldGoal && config.TurninGold)
                            {
                                IceLogging.Info("Gold Turnin was enabled, and hit the max score.", tag);
                                shouldTurnin = true;
                            }
                            else if (SilverGoal && config.TurninSilver)
                            {
                                if (!config.TurninGold) // Check is here, just to make sure we shouldn't still be aiming for gold
                                {
                                    IceLogging.Info("Silver Turnin was enabled, and you didn't have gold enabled.", "[Craft Scoring]");
                                    shouldTurnin = true;
                                }
                            }
                            else if (config.TurninBronze)
                            {
                                if (!config.TurninSilver && !config.TurninGold) // Checking to make sure that silver and gold scores both aren't true
                                {
                                    IceLogging.Info("Bronze Turnin was enabled, and you didn't have gold or silver enabled.", "[Craft Scoring]");
                                    shouldTurnin = true;
                                }
                            }
                        }
                    }
                }

                if (shouldTurnin)
                {
                    IceLogging.Debug("The threshold for scoring was met. Time to turnin", tag);

                    SchedulerMain.State = IceState.TurninMission;
                    P.TaskManager.Tasks.Clear();

                    if (mission.Attributes.HasFlag(MissionAttributes.Critical))
                        Mission_Settings.TurninState = TurninState.Critical;
                    else
                    {
                        var currentScore = missionInfo.CurrentScore.Value;
                        var silverScore = mission.SilverScore;
                        var goldScore = mission.GoldScore;

                        // to check later
                        MedalChecker(currentScore, silverScore, goldScore);
                    }

                    return true;
                }
                else
                {
                    IceLogging.Debug("Minimum scoring isn't met for your current preset. Continuing on", tag);
                    if (mission.Attributes.HasFlag(MissionAttributes.Critical))
                        IceLogging.Debug("Critical score is still 0", tag);
                    else
                    {
                        var currentScore = missionInfo.CurrentScore.Value;
                        var silverScore = mission.SilverScore;
                        var goldScore = mission.GoldScore;

                        IceLogging.Debug("Still missing score/items...", tag);
                        foreach (var item in mission.Crafts_Main)
                        {
                            var itemId = item.Value.ItemId;
                            var recipeEntry = item.Value;

                            PlayerHelper.GetItemCount(itemId, out var count);
                            IceLogging.Debug($"ItemID: {itemId}, current amount: {count}");
                        }
                        IceLogging.Debug("Score board: \n" +
                                         $"Current score: {currentScore}\n" +
                                         $"Bronze goal: {mission.BronzeScore}" +
                                         $"Silver goal: {silverScore}\n" +
                                         $"Gold goal: {goldScore}", tag);
                    }

                    return true;
                }
            }
            else
            {
                // Addon wasn't visiable/ready. Opening it up.
                if (GenericHelpers.TryGetAddonMaster<WKSHud>("WKSHud", out var moonHud))
                {
                    if (EzThrottler.Throttle("Opening the moon hud", 1000))
                    {
                        moonHud.Mission();
                        IceLogging.Info("Hud wasn't visible. Opening it", "[Score Check]");
                    }
                }
            }

            return false;
        }

        public static unsafe bool? Gather()
        {
            if (GenericHelpers.TryGetAddonMaster<WKSMissionInfomation>("WKSMissionInfomation", out var missionInfo) && missionInfo.IsAddonReady)
            {
                if (CosmicHandler.IsMissionTimedOut())
                {
                    SchedulerMain.State = IceState.AbandonMission;
                    P.TaskManager.Tasks.Clear();
                    return true;
                }

                IceLogging.Debug("Checking score for gathering. . .", "[Check Score: Gather]");
                // Hud info should be available. Now time to check the mission status.
                var id = CosmicHelper.CurrentLunarMission;
                var mission = CosmicHelper.SheetMissionDict[id];

                if (mission.Attributes.HasFlag(MissionAttributes.Critical))
                {
                    if (missionInfo.CriticalScore == null)
                        return false;

                    if (missionInfo.CriticalScore == 1)
                    {
                        SchedulerMain.State = IceState.TurninMission;
                        P.TaskManager.Tasks.Clear();

                        Mission_Settings.TurninState = TurninState.Critical;

                        return true;
                    }
                    else
                    {
                        // Still waiting for it to hit 1. So just returning true
                        return true;
                    }
                }
                else if (mission.Attributes.HasFlag(MissionAttributes.ScoreTimeRemaining))
                {
                    // We're just checking to see if we have all the items for missions that have a score time remaining. 
                    // These are typically missions that have 6 gather points, and also require a certain amount of items.
                    foreach (var item in mission.Gathering_Min)
                    {
                        if (PlayerHelper.GetItemCount(item.Key, out var count) && count < item.Value)
                        {
                            // found an item that is less than what we need. Going back to continue on gathering.
                            return true;
                        }
                    }

                    // if we've gotten here, that means that we actually have all the items. Proceeding to turnin item
                    SchedulerMain.State = IceState.TurninMission;
                    P.TaskManager.Tasks.Clear();

                    Mission_Settings.TurninState = DetermineTurninState();

                    return true;
                }
                else
                {
                    if (missionInfo.CurrentScore == null)
                        return false;

                    if (mission.Attributes.HasFlag(MissionAttributes.Limited))
                    {
                        if (Mission_Settings.nodeTotal >= 8 && !Svc.Condition[ConditionFlag.Gathering])
                        {
                            // We've hit the node total, and can't gather anymore. Just going to try and turnin/abandon
                            SchedulerMain.State = IceState.AbandonMission;
                            Mission_Settings.nodeTotal = 0;
                            P.TaskManager.Tasks.Clear();
                            return true;
                        }
                    }

                    var canTurnin = false;

                    // Not retricted by time, but by either score or item's gathered.
                    if (mission.BronzeScore == 0)
                    {
                        // This is a mission that requires a certain amount of each item. Checking that first.
                        foreach (var item in mission.Gathering_Min)
                        {
                            if (PlayerHelper.GetItemCount(item.Key, out var count) && count < item.Value)
                            {
                                // you don't have enough items to meet the turnin here. 
                                return true;
                            }
                        }

                        // If we've gotten this far, than that means we've met the bronze threshold!
                        canTurnin = true;
                    }
                    else
                    {
                        // a minimum threshold of bronze scoring is required. Time to check that.
                        var currentScore = missionInfo.CurrentScore;
                        if (currentScore >= mission.BronzeScore)
                            canTurnin = true;
                    }

                    if (canTurnin)
                    {
                        // Turnin threshold has been met. Time to check to see if we're at the point where we want to turn in minimumly
                        var currentScore = missionInfo.CurrentScore;
                        var bronzeScore = mission.BronzeScore;
                        var silverScore = mission.SilverScore;
                        var goldScore = mission.GoldScore;

                        var config = C.MissionConfig[id];
                        bool AnyTurnin = config.AutoTurnin;
                        bool GoldGoal = goldScore <= currentScore;
                        bool SilverGoal = silverScore <= currentScore;
                        bool TurninBronze = config.TurninBronze;

                        bool shouldTurnin = false;

                        if (C.XPLeveling_Mode)
                        {
                            IceLogging.Debug("Leveling mode is enabled, and we've hit the bronze threshold. So turning in", "[Gathering Score Check]");
                            shouldTurnin = true;
                        }
                        else if (config.AutoTurnin)
                        {
                            // AutoTurnin enabled, going to check for gold only since we have materials/time still
                            if (GoldGoal)
                            {
                                IceLogging.Info("Auto turnin was enabled, and hit the max score.", "[Craft Scoring]");
                                shouldTurnin = true;
                            }
                        }
                        else
                        {
                            if (GoldGoal && config.TurninGold)
                            {
                                IceLogging.Info("Gold Turnin was enabled, and hit the max score.", "[Craft Scoring]");
                                shouldTurnin = true;
                            }
                            else if (SilverGoal && config.TurninSilver)
                            {
                                if (!config.TurninGold) // Check is here, just to make sure we shouldn't still be aiming for gold
                                {
                                    IceLogging.Info("Silver Turnin was enabled, and you didn't have gold enabled.", "[Craft Scoring]");
                                    shouldTurnin = true;
                                }
                            }
                            else if (config.TurninBronze)
                            {
                                if (!config.TurninSilver && !config.TurninGold) // Checking to make sure that silver and gold scores both aren't true
                                {
                                    IceLogging.Info("Silver Turnin was enabled, and you didn't have gold or silver enabled.", "[Craft Scoring]");
                                    shouldTurnin = true;
                                }
                            }
                        }

                        if (shouldTurnin)
                        {
                            IceLogging.Debug("The threshold for scoring was met. Time to turnin", "[Gathering Scoring]");

                            SchedulerMain.State = IceState.TurninMission;
                            P.TaskManager.Tasks.Clear();

                            // to check later
                            MedalChecker(currentScore.Value, silverScore, goldScore);

                            return true;
                        }
                        else
                        {
                            IceLogging.Debug("Minimum scoring isn't met for your current preset. Continuing on", "[Gathering Scoring]");

                            return true;
                        }
                    }
                    else
                    {
                        IceLogging.Debug($"Minimum turnin hasn't been met yet. Continuing onto gathering", "[Score Check: Gather]");
                        return true;
                    }
                }

            }
            else
            {
                // Addon wasn't visiable/ready. Opening it up.
                if (GenericHelpers.TryGetAddonMaster<WKSHud>("WKSHud", out var moonHud))
                {
                    if (EzThrottler.Throttle("Opening the moon hud", 1000))
                    {
                        moonHud.Mission();
                        IceLogging.Info("Hud wasn't visible. Opening it", "[Score Check]");
                    }
                }
            }

            return false;
        }

        public static unsafe bool? DualClass()
        {
            if (GenericHelpers.TryGetAddonMaster<WKSMissionInfomation>("WKSMissionInfomation", out var missionInfo) && missionInfo.IsAddonReady)
            {
                if (missionInfo.CurrentScore == null)
                {
                    return false;
                }
                else
                {
                    if (CosmicHandler.IsMissionTimedOut())
                    {
                        SchedulerMain.State = IceState.AbandonMission;
                        P.TaskManager.Tasks.Clear();
                        return true;
                    }

                    var Id = CosmicHelper.CurrentLunarMission;
                    // var mission = CosmicHelper.Dict_CosmicMissions[Id];
                    var mission = CosmicHelper.SheetMissionDict[Id];
                    bool shouldTurnin = false;

                    if (mission.BronzeScore != 0 && (missionInfo.CurrentScore <= mission.BronzeScore))
                    {
                        IceLogging.Info("Bronze score is recorded at not 0. Which means that it needs a minimum score. \n" +
                                        $"Current Score: {missionInfo.CurrentScore}\n" +
                                        $"Minimum Score: {mission.BronzeScore}\n" +
                                        $"Continuing on with the crafting process");
                        return true;
                    }
                    else
                    {
                        var currentScore = missionInfo.CurrentScore;
                        var bronzeScore = mission.BronzeScore;
                        var silverScore = mission.SilverScore;
                        var goldScore = mission.GoldScore;

                        var config = C.MissionConfig[Id];
                        bool AnyTurnin = config.AutoTurnin;
                        bool GoldGoal = goldScore <= currentScore;
                        bool SilverGoal = silverScore <= currentScore;
                        bool TurninBronze = config.TurninBronze;

                        if (config.AutoTurnin)
                        {
                            // AutoTurnin enabled, going to check for gold only since we have materials/time still
                            if (GoldGoal)
                            {
                                IceLogging.Info("Auto turnin was enabled, and hit the max score.", "[Craft Scoring]");
                                shouldTurnin = true;
                            }
                        }
                        else
                        {
                            if (GoldGoal && config.TurninGold)
                            {
                                IceLogging.Info("Gold Turnin was enabled, and hit the max score.", "[Craft Scoring]");
                                shouldTurnin = true;
                            }
                            else if (SilverGoal && config.TurninSilver)
                            {
                                if (!config.TurninGold) // Check is here, just to make sure we shouldn't still be aiming for gold
                                {
                                    IceLogging.Info("Silver Turnin was enabled, and you didn't have gold enabled.", "[Craft Scoring]");
                                    shouldTurnin = true;
                                }
                            }
                            else if (config.TurninBronze)
                            {
                                if (!config.TurninSilver && !config.TurninGold) // Checking to make sure that silver and gold scores both aren't true
                                {
                                    IceLogging.Info("Silver Turnin was enabled, and you didn't have gold or silver enabled.", "[Craft Scoring]");
                                    shouldTurnin = true;
                                }
                            }
                        }
                    }

                    if (shouldTurnin)
                    {
                        IceLogging.Debug("The threshold for scoring was met. Time to turnin", "[Craft Scoring]");

                        if (mission.Attributes.HasFlag(MissionAttributes.Critical))
                            Mission_Settings.TurninState = TurninState.Gold;
                        else
                        {
                            var currentScore = missionInfo.CurrentScore;
                            var silverScore = mission.SilverScore;
                            var goldScore = mission.GoldScore;
                            // to check later
                            MedalChecker(currentScore.Value, silverScore, goldScore);
                        }

                        SchedulerMain.State = IceState.TurninMission;
                        P.TaskManager.Tasks.Clear();

                        return true;
                    }
                    else
                    {
                        var config = C.MissionConfig[Id];
                        var currentScore = missionInfo.CurrentScore;
                        var bronzeScore = mission.BronzeScore;
                        var silverScore = mission.SilverScore;
                        var goldScore = mission.GoldScore;

                        IceLogging.Debug("Minimum scoring isn't met for your current preset. Continuing on", "[Craft Scoring]");
                        IceLogging.Info("Currently Enabled:\n" +
                                        $"Bronze Enable: {config.TurninBronze} | Score: {bronzeScore}" +
                                        $"Silver Enable: {config.TurninSilver} | Score: {silverScore}" +
                                        $"Gold Enabled: {config.TurninGold} | Score: {goldScore}" +
                                        $"Any Turnin Enabled: {config.AutoTurnin}" +
                                        $"Current Score: {missionInfo.CurrentScore}");

                        return true;
                    }
                }
            }
            else
            {
                // Addon wasn't visiable/ready. Opening it up.
                if (GenericHelpers.TryGetAddonMaster<WKSHud>("WKSHud", out var moonHud))
                {
                    if (EzThrottler.Throttle("Opening the moon hud", 1000))
                    {
                        moonHud.Mission();
                        IceLogging.Info("Hud wasn't visible. Opening it", "[Score Check]");
                    }
                }
            }

            return false;
        }

        public static TurninState DetermineTurninState()
        {
            string timerString = ActiveTimerAddon();
            TimeSpan silverRequirement = ParseRequirementTime(SilverTimerAddon());
            TimeSpan goldRequirement = ParseRequirementTime(GoldTimerAddon());

            // Parse the timer string to get remaining time (left side of /)
            var remainingTime = ParseCurrentTime(timerString);

            IceLogging.Info($"Timer Info:\n" +
                $"Current Timer: {remainingTime}\n" +
                $"Silver Requirement: {silverRequirement}\n" +
                $"Gold Requirement: {goldRequirement}");

            if (remainingTime >= goldRequirement)
                return TurninState.Gold;
            else if (remainingTime >= silverRequirement)
                return TurninState.Silver;
            else
                return TurninState.Bronze;
        }

        private static TimeSpan ParseCurrentTime(string timerString)
        {
            IceLogging.Verbose($"Raw timer string: '{timerString}'");

            // Trim to remove the clock icon and any whitespace
            var currentTimeStr = timerString.Trim();

            // Remove any non-numeric characters except ':' (like the clock icon)
            currentTimeStr = new string(currentTimeStr.Where(c => char.IsDigit(c) || c == ':').ToArray());

            IceLogging.Verbose($"Cleaned time string: '{currentTimeStr}'");

            // Parse the time (format: M:SS or MM:SS)
            var timeParts = currentTimeStr.Split(':');
            IceLogging.Verbose($"Time parts count: {timeParts.Length}");

            if (timeParts.Length != 2)
            {
                IceLogging.Verbose($"Time split failed - got {timeParts.Length} parts");
                return new TimeSpan(0, 0, 0);
            }

            if (!int.TryParse(timeParts[0], out var minutes) ||
                !int.TryParse(timeParts[1], out var seconds))
            {
                IceLogging.Verbose($"Failed to parse time values");
                return new TimeSpan(0, 0, 0);
            }

            IceLogging.Verbose($"Successfully parsed - Minutes: {minutes}, Seconds: {seconds}");
            return new TimeSpan(0, minutes, seconds);
        }

        private static TimeSpan ParseRequirementTime(string requirementString)
        {
            if (string.IsNullOrWhiteSpace(requirementString))
                return TimeSpan.Zero;

            // Split on whitespace and take the first part (the time)
            var parts = requirementString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return TimeSpan.Zero;

            var timeStr = parts[0];

            // Parse the time (format: M:SS or MM:SS)
            var timeParts = timeStr.Split(':');
            if (timeParts.Length != 2)
                return TimeSpan.Zero;

            if (!int.TryParse(timeParts[0], out var minutes) ||
                !int.TryParse(timeParts[1], out var seconds))
                return TimeSpan.Zero;

            return new TimeSpan(0, minutes, seconds);
        }

        private static unsafe string ActiveTimerAddon()
        {
            return AddonHelper.GetNodeText("WKSMissionInfomation", 24);
        }

        private static string SilverTimerAddon()
        {
            return AddonHelper.GetNodeText("WKSMissionInfomation", 15);
        }

        private static string GoldTimerAddon()
        {
            return AddonHelper.GetNodeText("WKSMissionInfomation", 11);
        }

        private static void MedalChecker(uint current, uint silver, uint gold)
        {
            if (current >= gold)
                Mission_Settings.TurninState = TurninState.Gold;
            else if (current >= silver)
                Mission_Settings.TurninState = TurninState.Silver;
            else
                Mission_Settings.TurninState = TurninState.Bronze;
        }

        private static void MedalChecker(MissionRank rank)
        {
            if (rank == MissionRank.Gold)
                Mission_Settings.TurninState = TurninState.Gold;
            else if (rank == MissionRank.Silver)
                Mission_Settings.TurninState = TurninState.Silver;
            else if (rank == MissionRank.Bronze)
                Mission_Settings.TurninState = TurninState.Bronze;
        }
    }
}