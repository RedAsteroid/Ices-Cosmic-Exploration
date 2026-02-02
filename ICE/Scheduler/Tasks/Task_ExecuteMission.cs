using ICE.Utilities.Cosmic;
using ICE.Utilities.Cosmic_Helper;
using ICE.Utilities.GatheringHelper;

namespace ICE.Scheduler.Tasks
{
    internal static class Task_ExecuteMission
    {
        public static void Enqueue()
        {
            P.TaskManager.Enqueue(() => ExecuteMission(), "Finding proper mission state");
        }

        private static bool? ExecuteMission()
        {
            if (CosmicHelper.CurrentLunarMission != 0)
            {
                var missionId = CosmicHelper.CurrentLunarMission;
                P.MissionTimer.StartMission(missionId);

                var mission = CosmicHelper.SheetMissionDict[missionId];
                bool fishingMission = mission.Jobs.Contains(18);
                bool gatherMission = mission.Jobs.Contains(16) || mission.Jobs.Contains(17);
                bool craftMission = mission.Jobs.Any(x => CosmicHelper.CrafterJobList.Contains(x));

                C.MissionConfig.TryGetValue(missionId, out var config);
                bool dualClass = (gatherMission && craftMission) || (fishingMission && craftMission);

                if (C.OnlyGrabMission_Debug || (config != null && config.ManualMode) || UnsupportedMissions.Ids.Contains(missionId))
                {
                    SchedulerMain.State = IceState.ManualMode;
                }
                else if (dualClass)
                {
                    IceLogging.Info("We've found a dual class mission! Kicking it off with that.", "[Task: Execute Mission]");
                    SchedulerMain.State = IceState.DualClass;
                    if (fishingMission)
                    {
                        if (config.Use_BuildinPreset)
                        {
                            P.AutoHook.DeleteAllAnonymousPresets();
                            FishingTask(missionId);
                        }
                    }
                }
                else if (fishingMission)
                {
                    // Check exist twice, one here is to actually enable the fishing profile that is selected.
                    var missionConfig = C.MissionConfig[missionId];
                    if (missionConfig.Use_BuildinPreset)
                    {
                        // Using the build in presets that are included in the plugin.
                        P.AutoHook.DeleteAllAnonymousPresets();
                        FishingTask(missionId);
                    }
                    else
                    {
                        string presetName = missionConfig.AutoHookPresetName;
                        P.AutoHook.SetPreset(presetName);
                    }

                    SchedulerMain.State = IceState.Fish;
                    IceLogging.Debug("Mission is a fishing mission, so going to the fishing task");
                }
                else if (gatherMission)
                {
                    SchedulerMain.State = IceState.Gather;
                    IceLogging.Info("Mission is a gathering mission. Need to gather inial resources. But first going to do a check to make sure where we're at.", "[Task_ExecuteMission]");
                }
                else if (craftMission)
                {
                    IceLogging.Debug("Mission is purely a crafting mission (yay), checking current state next", "[Task_ExecuteMission]");
                    SchedulerMain.State = IceState.Craft;
                }
            }
            else if (CosmicHelper.CurrentLunarMission == 0)
            {
                IceLogging.Debug("Hmm... somehow we got in this state. And we shouldn't be? Returning back to the grab mission state");
                SchedulerMain.State = IceState.GrabMission;
            }

            return true;
        }

        public static void FishingTask(uint missionId)
        {
            P.TaskManager.Enqueue(() => ClearFishingPreset(), "Clearing All Fishing Presets");
            P.TaskManager.EnqueueDelay(150);
            P.TaskManager.Enqueue(() => ImportPresetsSequentially(missionId));
        }

        private static bool? ClearFishingPreset()
        {
            if (P.AutoHook.Installed)
            {
                P.AutoHook.DeleteAllAnonymousPresets();
            }
            return true;
        }
        private static void ImportPresetsSequentially(uint missionId)
        {
            bool? ImportOtherPresets(string preset)
            {
                P.AutoHook.CreateAndSelectAnonymousPreset(preset);
                return true;
            }

            var presetList = CosmicHelper.SheetMissionDict[missionId].Fish_Presets;

            if (presetList.Count == 0)
                return;

            IceLogging.Debug($"Current Fish Preset Count for [{missionId}]: {presetList.Count}");

            // Import first preset immediately
            P.AutoHook.CreateAndSelectAnonymousPreset(presetList[0]);

            // Queue remaining presets with delays
            for (int i = 1; i < presetList.Count; i++)
            {
                var preset = presetList[i]; // Capture for closure

                P.TaskManager.EnqueueDelay(100);
                P.TaskManager.Enqueue(() => ImportOtherPresets(preset));
            }
        }

    }
}
