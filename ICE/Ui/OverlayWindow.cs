using Dalamud.Game.Text;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using Lumina.Excel.Sheets;
using System.Collections.Generic;
using System.Globalization;
using static ICE.Utilities.CosmicHelper;

namespace ICE.Ui
{
    internal class OverlayWindow : Window
    {
        private uint selectedJob = C.SelectedJob;
        public OverlayWindow() : base("ICE 悬浮窗", ImGuiWindowFlags.AlwaysAutoResize)
        {
            P.windowSystem.AddWindow(this);
        }

        public void Dispose()
        {
            P.windowSystem.RemoveWindow(this);
        }

        public override bool DrawConditions()
        {
            return C.ShowOverlay
                && (PlayerHelper.IsInCosmicZone());
        }
        private bool IsAvailableAtHour(CosmicInfo mission, int hour)
        {
            if (mission.StartTime <= mission.EndTime)
                return hour >= mission.StartTime && hour < mission.EndTime;
            else
                return hour >= mission.StartTime || hour < mission.EndTime;
        }

        private unsafe (List<KeyValuePair<uint, CosmicInfo>> current, List<KeyValuePair<uint, CosmicInfo>> next) GetMissionsForHour()
        {
            var eorzeaTime = DateTimeOffset.FromUnixTimeSeconds(
                Framework.Instance()->ClientTime.EorzeaTime);

            int currentHour = eorzeaTime.Hour;
            int nextHour = (currentHour + 1) % 24;

            var currentHourMissions = CosmicHelper.SheetMissionDict
                .Where(kvp => IsAvailableAtHour(kvp.Value, currentHour))
                .Where(kvp => kvp.Value.TerritoryId == Player.Territory.RowId)
                .OrderBy(kvp => kvp.Value.Jobs.FirstOrDefault())
                .ToList();

            var nextHourMissions = CosmicHelper.SheetMissionDict
                .Where(kvp => IsAvailableAtHour(kvp.Value, nextHour))
                .Where(kvp => kvp.Value.TerritoryId == Player.Territory.RowId)
                .OrderBy(kvp => kvp.Value.Jobs.FirstOrDefault())
                .ToList();

            return (currentHourMissions, nextHourMissions);
        }

        public override void Draw()
        {
            ImGui.Text($"当前状态: " + SchedulerMain.State.ToString());
            if (CosmicHelper.SheetMissionDict.TryGetValue(CosmicHelper.CurrentLunarMission, out var missionName) && SchedulerMain.State != IceState.AbandonMission)
            {
                ImGui.Text($"当前任务: [{CosmicHelper.CurrentLunarMission}] {missionName.Name}");
            }
            else
            {
                ImGui.Text("当前任务: 无");
            }
#if DEBUG
            if (C.ShowDebugGatherInfo)
            {
                ImGui.Text($"当前收藏品状态: {Mission_Settings.CollectableStep}");
                ImGui.Text($"全部节点: {Mission_Settings.nodeTotal}");
                ImGui.Text($"节点计数: {Mission_Settings.nodeCounter}");
            }
#endif

            ImGuiHelpers.ScaledDummy(2);
            ImGui.Separator();
            ImGuiHelpers.ScaledDummy(2);

            (string currentWeather, uint currentWeatherId, string nextWeather, uint nextWeatherId, string nextWeatherTime) = WeatherForecastHandler.GetNextWeather();

            if (currentWeather != null)
            {
                ImGui.AlignTextToFramePadding();
                ImGui.Text("天气预报:");
                Svc.Texture.TryGetFromGameIcon(currentWeatherId, out var currentWeatherIcon);
                ImGui.SameLine(0, 2);
                ImGui.Image(currentWeatherIcon.GetWrapOrEmpty().Handle, new Vector2(23, 23));
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text($"{currentWeather}");
                    ImGui.EndTooltip();
                }
                // 如果 nextWeatherId == 0，天气将持续超过 24 周期(192ET、9小时20分)
                if (nextWeatherId == 0)
                {
                    ImGui.SameLine(0, 2);
                    ImGui.AlignTextToFramePadding();
                    ImGuiEx.Icon(FontAwesomeIcon.LongArrowAltRight);

                    ImGui.SameLine(0, 2);
                    ImGui.AlignTextToFramePadding();
                    ImGui.Text("下一个: 超过9小时20分(24个周期)");
                }
                else
                {
                    // 正常显示下一个天气
                    ImGui.SameLine(0, 2);
                    ImGui.AlignTextToFramePadding();
                    ImGuiEx.Icon(FontAwesomeIcon.LongArrowAltRight);
                    Svc.Texture.TryGetFromGameIcon(nextWeatherId, out var nextWeatherIcon);
                    ImGui.SameLine(0, 2);
                    ImGui.Image(nextWeatherIcon.GetWrapOrEmpty().Handle, new Vector2(23, 23));
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text($"{nextWeather}");
                        ImGui.EndTooltip();
                    }
                    ImGui.SameLine(0, 2);
                    ImGui.AlignTextToFramePadding();
                    ImGui.Text($"下一个: {nextWeatherTime}");
                }
            }

            var (currentList, nextList) = GetMissionsForHour();
            ImGui.AlignTextToFramePadding();
            ImGui.Text("限时任务: ");

            foreach (var mission in currentList)
            {
                var jobId = mission.Value.Jobs[0];
                if (CosmicHelper.JobIconDict.TryGetValue(jobId, out var jobIcon))
                {
                    ImGui.SameLine(0, 2);
                    ImGui.Image(jobIcon.GetWrapOrEmpty().Handle, new Vector2(23, 23));

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text($"[{mission.Key}] {mission.Value.Name}");
                        ImGui.EndTooltip();
                    }
                }
            }

            ImGui.SameLine(0, 2);
            ImGuiEx.Icon(FontAwesomeIcon.LongArrowAltRight);

            foreach (var mission in nextList)
            {
                var jobId = mission.Value.Jobs[0];
                if (CosmicHelper.JobIconDict.TryGetValue(jobId, out var jobIcon))
                {
                    ImGui.SameLine(0, 2);
                    ImGui.Image(jobIcon.GetWrapOrEmpty().Handle, new Vector2(23, 23));

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text($"[{mission.Key}] {mission.Value.Name}");
                        ImGui.EndTooltip();
                    }
                }
            }
            if (PlayerHelper.UsingSupportedJob())
            {
                if (CosmicHelper.CurrentLunarMission != 0)
                {
                    var missionId = CosmicHelper.CurrentLunarMission;
                    if (CosmicHelper.SheetMissionDict.TryGetValue(missionId, out var missionInfo))
                    {
                        foreach (var jobId in missionInfo.Jobs)
                        {
                            if (CosmicHelper.JobIconDict.TryGetValue(jobId, out var jobIcon))
                            {
                                var imageSize = new Vector2(23, 23);
                                ImGui.Image(jobIcon.GetWrapOrEmpty().Handle, imageSize);
                                ImGui.SameLine();
                                ImGui.AlignTextToFramePadding();
                                Relic_XP.DrawScoreBar(new Vector2(340, 10), false, jobId);
                            }
                        }
                    }
                }
                else
                {
                    var jobId = (uint)Player.Job;
                    if (CosmicHelper.JobIconDict.TryGetValue(jobId, out var jobIcon))
                    {
                        var imageSize = new Vector2(23, 23);
                        ImGui.Image(jobIcon.GetWrapOrEmpty().Handle, imageSize);
                        ImGui.SameLine();
                        ImGui.AlignTextToFramePadding();
                        Relic_XP.DrawScoreBar(new Vector2(340, 10), false);
                    }
                }
            }
            if (C.ShowTotalScore)
            {
                (uint TotalScore, uint TotalComplete, uint MaxScore, Dictionary<uint, uint> ClassInfo) = Relic_XP.GetTotalScores();
                var ScoreBarSize = new Vector2(340, 10);
                Relic_XP.DrawXPBar($"总技巧点 | 已完成: [{TotalComplete} / 11]", TotalScore, MaxScore, ScoreBarSize);
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    foreach (var job in ClassInfo)
                    {
                        var jobIdInfo = job.Key;
                        var jobScore = job.Value;
                        var jobImage = CosmicHelper.JobIconDict[jobIdInfo];
                        ImGui.Image(jobImage.GetWrapOrEmpty().Handle, new Vector2(23, 23));
                        ImGui.SameLine();
                        ImGui.AlignTextToFramePadding();
                        ImGui.Text($"技巧点: {jobScore:N0}");
                    }
                    ImGui.EndTooltip();
                }
            }

            ImGuiHelpers.ScaledDummy(2);
            ImGui.Separator();
            ImGuiHelpers.ScaledDummy(2);

            if (ImGuiEx.IconButton(FontAwesomeIcon.Home, "打开 ICE"))
            {
                P.mainWindow.IsOpen = true;
            }
            ImGui.SameLine();

            // Start button (disabled while already ticking).
            bool xpLeveling = C.XPLeveling_Mode;
            bool unsupportedArtisan = xpLeveling && !P.Artisan.UpdatedArtisan() && CosmicHelper.CrafterJobList.Contains((uint)Player.Job);
            bool unsupportedClass = !PlayerHelper.UsingSupportedJob();

            bool unsupported = SchedulerMain.State != IceState.Idle || !PlayerHelper.UsingSupportedJob() || unsupportedArtisan;

            using (ImRaii.Disabled(unsupported))
            {
                var defaultButtonColor = ImGui.GetStyle().Colors[(int)ImGuiCol.Button];
                var color = unsupported ? EColor.Red : defaultButtonColor;

                using var tempButton = ImRaii.PushColor(ImGuiCol.Button, color);
                if (ImGui.Button("开始")) // Start
                {
                    SchedulerMain.EnablePlugin();
                }
            }

            ImGui.SameLine();

            // Stop button (disabled while not ticking).
            using (ImRaii.Disabled(SchedulerMain.State == IceState.Idle))
            {
                if (ImGui.Button("停止")) // Stop
                {
                    SchedulerMain.DisablePlugin();
                }
            }
            ImGui.SameLine();
            ImGui.Checkbox("当前任务结束后停止", ref Mission_Settings.StopAfterCurrent);

            ImGuiHelpers.ScaledDummy(2);
            ImGui.Separator();
            ImGuiHelpers.ScaledDummy(2);

            if (C.ShowExpBars)
            {
                var currentJobId = (uint)Player.Job;

                bool showExp = (CosmicHelper.CrafterJobList.Contains(currentJobId) || CosmicHelper.GatheringJobList.Contains(currentJobId));

                if (CosmicHelper.CrafterJobList.Contains(currentJobId) || CosmicHelper.GatheringJobList.Contains(currentJobId))
                {
                    if (ImGui.CollapsingHeader("宇宙工具研究数据"))
                    {
                        Relic_XP.DrawRelicXP((uint)currentJobId);
                    }
                }
            }
        }

        void DrawScore()
        {
            try
            {
                var (classScore, cappedClassScore, totalScores, classId) = CosmicHelper.GetCosmicClassScores();

                ImGui.TextUnformatted(string.Create(CultureInfo.InvariantCulture,
                    $"{Svc.Data.GetExcelSheet<ClassJob>().GetRow(classId).Name}: {(float)cappedClassScore / 500_000:P} ({classScore:N0})")); // 原始: Abbreviation
                ImGui.SameLine();
                using (ImRaii.Disabled())
                {
                    ImGui.TextUnformatted("--");
                    ImGui.SameLine();
                    ImGui.TextUnformatted(string.Create(CultureInfo.InvariantCulture,
                        $"总技巧点: {(float)totalScores / 11 / 500_000:P} ({SeIconChar.CrossWorld.ToIconChar()} {11 * 500_000 - totalScores:N0})")); // All:
                }
            }
            catch
            {
                // meh
            }
        }
    }
}