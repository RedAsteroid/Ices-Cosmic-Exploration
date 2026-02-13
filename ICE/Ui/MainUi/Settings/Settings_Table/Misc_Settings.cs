using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using Pictomancy;
using System.Collections.Generic;
using static ICE.ConfigFiles.Config;

namespace ICE.Ui.MainUi.Settings.Settings_Table
{
    internal class Misc_Settings
    {
        public static void Draw()
        {
            if (ImGui.BeginTable("Misc Columns Stuff", 3, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit))
            {
                ImGui.TableNextRow();

                ImGui.TableSetColumnIndex(0);
                OverlaySettings();

                ImGui.TableNextColumn();
                AutoUse();

                ImGui.TableNextColumn();
                RepairSettings();

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                SafetySettings.Draw();

                ImGui.TableNextColumn();
                MountSelection();

                ImGui.TableNextColumn();
                ShowSystemButtons();

                ImGui.Dummy(new Vector2(0, 5));

                TimeRecords();

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                CraftingLocations();

                ImGui.TableNextColumn();
                ArtisanSettings();

#if DEBUG
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                DebugTab.Draw();
#endif
                ImGui.EndTable();
            }

            PostMissionCommands();
            Separator();
        }

        private static void OverlaySettings()
        {
            ImGuiEx.IconWithText(FontAwesomeIcon.WindowMaximize, "悬浮窗");
            ImGui.Dummy(new (0, 5));

            bool showOverlay = C.ShowOverlay;
            if (ImGui.Checkbox("显示 悬浮窗", ref showOverlay))
            {
                C.ShowOverlay = showOverlay;
                C.Save();
            }

            bool ShowSeconds = C.ShowSeconds;
            if (ImGui.Checkbox("显示 秒数", ref ShowSeconds))
            {
                C.ShowSeconds = ShowSeconds;
                C.Save();
            }

            bool showExpOverlay = C.ShowExpBars;
            if (ImGui.Checkbox("显示 宇宙工具研究数据", ref showExpOverlay))
            {
                C.ShowExpBars = showExpOverlay;
                C.Save();
            }

            bool showTotalScore = C.ShowTotalScore;
            if (ImGui.Checkbox("显示 总技巧点", ref showTotalScore))
            {
                C.ShowTotalScore = showTotalScore;
                C.Save();
            }

            bool disableHudClipping = C.DisableHudClipping;
            if (ImGui.Checkbox("禁用 HUD 裁切", ref disableHudClipping))
            {
                C.DisableHudClipping = disableHudClipping;
                C.Save();
            }
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("启用后, 绘制叠加层将渲染在原生 UI 元素上方");
            }

        }

        private static void AutoUse()
        {
            ImGuiEx.IconWithText(FontAwesomeIcon.PersonRays, "自动使用");
            ImGui.Dummy(new Vector2(0, 5));

            bool AutoMoonSprint = C.MoonSprint;
            if (ImGui.Checkbox("自动使用宇宙冲刺", ref AutoMoonSprint))
            {
                C.MoonSprint = AutoMoonSprint;
                C.Save();
            }

            bool DisableLunarAura = C.RemoveStellarStatus;
            if (ImGui.Checkbox("自动取消贡献之星状态效果", ref DisableLunarAura))
            {
                C.RemoveStellarStatus = DisableLunarAura;
                C.Save();
            }

            bool EnableAutoAntiAFK = C.AutoAntiAFK;
            if (ImGui.Checkbox("启用 自动离开设置为不切换", ref EnableAutoAntiAFK))
            {
                C.AutoAntiAFK = EnableAutoAntiAFK;
                C.Save();
            }

            bool DisableRedAlertPathing = C.DisablePathfindingToRedAlert;
            if (ImGui.Checkbox("紧急探索任务中禁用寻路", ref DisableRedAlertPathing))
            {
                C.DisablePathfindingToRedAlert = DisableRedAlertPathing;
                C.Save();
            }

            bool autoStartOnMoonEnter = C.StartUponEnterMoon;
            if (ImGui.Checkbox("进入宇宙探索地图时自动启动 ICE 运行", ref autoStartOnMoonEnter))
            {
                C.StartUponEnterMoon = autoStartOnMoonEnter;
                C.Save();
            }
            ImGui.SameLine();
            ImGuiEx.IconWithTooltip(FontAwesomeIcon.QuestionCircle,
                                   "首次进入宇宙探索地图时, 若检测到您的职业为能工巧匠或大地使者, \n" +
                                   "将自动启动 ICE 运行, 等同于您手动按下开始按钮。\n" +
                                   "适用于使用自动登录工具/只想进入宇宙探索地图后直接开始的情况。\n" +
                                   "注意: 此功能仅在【首次】进入宇宙探索地图时生效。");
            ImGui.Dummy(Vector2.Zero);
        }

        private static void RepairSettings()
        {
            ImGuiEx.IconWithText(FontAwesomeIcon.Hammer, "修理设置");
            ImGui.Dummy(new Vector2(0, 5));

            bool repairAtVendor = C.RepairAtVendor;
            if (ImGui.Checkbox("NPC 修理工修理", ref repairAtVendor))
            {
                C.RepairAtVendor = repairAtVendor;
                C.Save();
            }

            using (ImRaii.Disabled(repairAtVendor))
            {
                bool selfRepairGather = C.SelfRepairGather;
                if (ImGui.Checkbox("采集时自己修理", ref selfRepairGather))
                {
                    C.SelfRepairGather = selfRepairGather;
                    C.Save();
                }

                bool selfRepairCrafter = C.SelfRepairCrafter;
                if (ImGui.Checkbox("制作时自己修理", ref selfRepairCrafter))
                {
                    C.SelfRepairCrafter= selfRepairCrafter;
                    C.Save();
                }
            }

            float repairAmount = C.RepairPercent;
            ImGui.SetNextItemWidth(150);
            if (ImGui.SliderFloat("###Repair %", ref repairAmount, 0f, 99f, "%.0f%%"))
            {
                if (C.RepairPercent != repairAmount)
                {
                    C.RepairPercent = (int)repairAmount;
                    C.SaveDebounced();
                }
            }
        }

        private static void TimeRecords()
        {
            ImGuiEx.IconWithText(FontAwesomeIcon.Clock, "统计设置");
            ImGui.Dummy(new Vector2(0, 5));

            int TimeHistory = C.TimeHistoryLimit;
            ImGui.SetNextItemWidth(100);
            if (ImGui.InputInt("平均时间历史保留数", ref TimeHistory))
            {
                C.TimeHistoryLimit = TimeHistory;
                C.Save();
            }
            ImGui.SameLine();
            ImGui.TextDisabled("?");
            if (ImGui.IsItemHovered())
            {
                ImGui.SetTooltip("小于 0: 保留全部日志\n" +
                                 "大于 0: 按设定数量保留");
            }
        }

        private static bool visualizeRadius = false;
        private static bool visualizeDismountRadius = false;
        private static Dictionary<uint, string> availableMounts = new();

        private static string mountSearchText = "";
        private static int mountDisplayOffset = 0;
        private static int mountItemsPerPage = 10;

        private static unsafe void MountSelection()
        {
            bool mountOutsideMission = C.UseMountOutsideMission;
            bool mountInMission = C.UseMountInMission;
            float minMountRange = C.MountRadius;
            float dismountRange = C.DismountRadius;

            ImGuiEx.IconWithText(FontAwesomeIcon.Feather, "坐骑设置");
            ImGui.Dummy(new Vector2(0, 5));

            if (ImGui.Button("选择坐骑"))
            {
                availableMounts.Clear();
                availableMounts[0] = "随机坐骑";

                var mountSheet = Svc.Data.GetExcelSheet<Mount>();

                foreach (var mountItem in mountSheet)
                {
                    //Checking to see if the current mount is unlocked
                    if (!PlayerState.Instance()->IsMountUnlocked(mountItem.RowId)) continue;

                    string mountName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(mountItem.Singular.ToString().ToLower());
                    uint id = mountItem.RowId;

                    availableMounts[id] = mountName;
                }

                mountSearchText = "";
                mountDisplayOffset = 0;

                ImGui.OpenPopup("Mount Options");
            }
            ImGui.SameLine();
            ImGui.AlignTextToFramePadding();
            ImGui.Text($"坐骑: {C.MountName}");

            if (ImGui.BeginPopup("Mount Options"))
            {
                // Search box
                ImGui.InputText("搜索", ref mountSearchText, 100);

                // Filter mounts based on search
                var filteredMounts = availableMounts
                    .Where(kvp => string.IsNullOrEmpty(mountSearchText) ||
                                  kvp.Value.Contains(mountSearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Calculate page count here, just to peeps know how many pages there are
                int totalItems = filteredMounts.Count;
                int maxOffset = Math.Max(0, totalItems - mountItemsPerPage);
                mountDisplayOffset = Math.Min(mountDisplayOffset, maxOffset);

                // Display current page of mounts
                var displayMounts = filteredMounts
                    .Skip(mountDisplayOffset)
                    .Take(mountItemsPerPage);

                foreach (var mount in displayMounts)
                {
                    if (ImGui.Selectable($"{mount.Value}##{mount.Key}"))
                    {
                        C.MountId = mount.Key;
                        C.MountName = mount.Value;
                        C.Save();
                        ImGui.CloseCurrentPopup();
                    }
                }

                // Navigation buttons
                ImGui.Separator();

                if (ImGui.Button("上页") && mountDisplayOffset > 0)
                {
                    mountDisplayOffset = Math.Max(0, mountDisplayOffset - mountItemsPerPage);
                }

                ImGui.SameLine();
                ImGui.Text($"{mountDisplayOffset + 1}-{Math.Min(mountDisplayOffset + mountItemsPerPage, totalItems)} of {totalItems}");

                ImGui.SameLine();
                if (ImGui.Button("下页") && mountDisplayOffset < maxOffset)
                {
                    mountDisplayOffset = Math.Min(maxOffset, mountDisplayOffset + mountItemsPerPage);
                }

                ImGui.EndPopup();
            }

            if (ImGui.Checkbox("任务外使用坐骑", ref mountOutsideMission))
            {
                C.UseMountOutsideMission = mountOutsideMission;
                C.Save();
            }

            if (ImGui.Checkbox("任务内使用坐骑", ref mountInMission))
            {
                C.UseMountInMission = mountInMission;
                C.Save();
            }

            ImGui.SetNextItemWidth(100);
            if (ImGui.DragFloat("使用坐骑的最小范围", ref minMountRange, 1))
            {
                C.MountRadius = minMountRange;
                C.Save();
            }
            ImGui.Checkbox("可视化使用坐骑半径范围", ref visualizeRadius);
            ImGui.SetNextItemWidth(100);
            if (ImGui.DragFloat("下坐骑目标范围", ref dismountRange, 1))
            {
                C.DismountRadius = dismountRange;
                C.Save();
            }
            ImGui.Checkbox("可视化下坐骑半径范围", ref visualizeDismountRadius);

            using (var drawList = PictoService.Draw(hints: Utils.GetPictoHints()))
            {
                if (drawList == null)
                    return;

                var playerPos = Player.Position;

                if (visualizeRadius)
                    PictoService.VfxRenderer.AddCircle("Mount_Radius Circle", playerPos, C.MountRadius, Utils.FromUintABGR(2616716297));
                if (visualizeDismountRadius)
                    PictoService.VfxRenderer.AddCircle("Dismount_Radius Circle", playerPos, C.DismountRadius, Utils.FromUintABGR(2601121571));
            }
        }

        private static void ShowSystemButtons()
        {
            ImGuiEx.IconWithText(FontAwesomeIcon.WindowRestore, "显示/隐藏 标签栏");
            ImGui.Dummy(new(0, 5));

            bool showStopWhen = C.Show_StopWhen;
            if (ImGui.Checkbox("显示 停止条件 标签栏", ref showStopWhen))
            {
                C.Show_StopWhen = showStopWhen;
                C.Save();
            }

            bool showGProfile = C.Show_GatheringProfile;
            if (ImGui.Checkbox("显示 采集配置 标签栏", ref showGProfile))
            {
                C.Show_GatheringProfile = showGProfile;
                C.Save();
            }

            bool showMissionPrio = C.Show_MissionPriority;
            if (ImGui.Checkbox("显示 任务优先级 标签栏", ref showMissionPrio))
            {
                C.Show_MissionPriority = showMissionPrio;
                C.Save();
            }

            bool showMisc = C.Show_MiscSettings;
            if (ImGui.Checkbox("显示 杂项设置 标签栏", ref showMisc))
            {
                C.Show_MiscSettings = showMisc;
                C.Save();
            }

            bool showHubActivities = C.Show_HubActivities;
            if (ImGui.Checkbox("显示 基地活动 选项", ref showHubActivities))
            {
                C.Show_HubActivities = showHubActivities;
                C.Save();
            }
        }
        private static void PostMissionCommands()
        {
            ImGuiEx.IconWithText(FontAwesomeIcon.Play, "任务后命令");
            ImGui.Dummy(new Vector2(0, 5));

            ImGui.TextWrapped("请在下方输入希望在运行完成后自动执行的一系列命令。\n" +
                              "这算是我提供的一种方式, 让您能够在一定程度上编写脚本/设置一系列您想做但插件本身可能未包含的其他操作。\n" +
                              "如果您需要更复杂的功能, 直接编写一个 SND 脚本即可, 然后在任务结束后自动执行脚本。");

            if (ImGui.Button("添加新命令"))
            {
                C.PostMissionCommands.Add(new MissionCommand 
                { 
                    command = "", 
                    Delay = 0,
                });
                C.Save();
            }

            MissionCommand? toRemove = null;
            int entryCounter = 0;

            if (ImGui.BeginTable("Mission Commands", 3, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Borders))
            {
                ImGui.TableSetupColumn("命令");
                ImGui.TableSetupColumn("延迟");
                ImGui.TableSetupColumn("移除");

                ImGui.TableHeadersRow();

                foreach (var entry in C.PostMissionCommands)
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.SetNextItemWidth(200);

                    ImGui.PushID($"{entryCounter}_MissionCommand");
                    string command = entry.command;
                    if (ImGui.InputText("##Command", ref command))
                    {
                        entry.command = command;
                        C.SaveDebounced();
                    }

                    ImGui.TableNextColumn();
                    ImGui.SetNextItemWidth(100);
                    int delay = entry.Delay;
                    if (ImGui.InputInt("###Delay", ref delay))
                    {
                        entry.Delay = delay;
                        C.SaveDebounced();
                    }

                    ImGui.TableNextColumn();
                    if (ImGuiEx.IconButton(FontAwesomeIcon.Trash, $"remove{C.PostMissionCommands.IndexOf(entry)}"))
                    {
                        toRemove = entry;
                    }
                    ImGui.PopID();
                    entryCounter += 1;
                }

                if (toRemove != null)
                {
                    C.PostMissionCommands.Remove(toRemove);
                    C.Save();
                }

                ImGui.EndTable();
            }
        }

        private static void CraftingLocations()
        {
            ImGuiEx.IconWithText(FontAwesomeIcon.MapPin, "制作返回点");
            ImGui.Dummy(new Vector2(0, 5));

            bool usePersonalLocations = C.PersonalReturnSpot;
            if (ImGui.Checkbox("启用 制作返回点", ref usePersonalLocations))
            {
                C.PersonalReturnSpot = usePersonalLocations;
                C.Save();
            }

            if (usePersonalLocations)
            {
                var territory = Player.Territory.RowId;
                var location = Player.Position;
                if (C.CrafterLocations.TryGetValue(territory, out var moonLoc))
                {
                    ImGui.Text($"地图: {territory} \n" +
                               $"位置: {moonLoc:N2}");
                    if (ImGui.Button("设置为当前位置"))
                    {
                        C.CrafterLocations[territory] = location;
                        C.Save();
                    }
                }
                else
                {
                    ImGui.Text("当前未设置位置");
                    if (ImGui.Button("添加位置"))
                    {
                        C.CrafterLocations[territory] = Player.Position;
                        C.Save();
                    }
                }
            }
        }

        private static void ArtisanSettings()
        {
            ImGuiEx.IconWithText(FontAwesomeIcon.Wrench, "Artisan 设置");
            ImGui.SameLine();
            ImGuiEx.HelpMarker("对 MeowZWR 的在线仓库版本进行了基于RepoUrl特征的额外兼容\n如果您使用的汉化版本为本地插件, 此功能可能会失效");
            ImGui.Dummy(new Vector2(0, 5));

            bool force_Raphael = C.Artisan_RaphaelForce;
            bool expertRaphael = C.Artisan_RaphaelMaster;

            if (ImGui.Checkbox("强制使用 Raphael 求解器", ref force_Raphael))
            {
                C.Artisan_RaphaelForce = force_Raphael;
                C.Save();
            }
            ImGui.SameLine();
            ImGuiEx.Icon(FontAwesomeIcon.QuestionCircle);
            if (ImGui.IsItemHovered())
            {
                ImGui.BeginTooltip();
                ImGui.Text($"在插件运行期间, 强制所有配方使用 Raphael 求解器。");
                ImGui.Text($"不包含高难度配方, 因为它们的机制不太适合。");
                ImGui.EndTooltip();
            }
            ImGui.Dummy(Vector2.Zero);
            if (force_Raphael)
            {
                if (ImGui.Checkbox("应用于高难度配方", ref expertRaphael))
                {
                    C.Artisan_RaphaelMaster = expertRaphael;
                    C.Save();
                }
                ImGui.SameLine();
                ImGuiEx.Icon(FontAwesomeIcon.QuestionCircle);
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text($"强制那些原本应使用 专家配方求解器 的高难度配方改为使用 Raphael 求解器。"); // 如果按国服本地化来翻译, Artisan 的 Expert Recipe Solver 应该叫 高难度配方求解器
                    ImGuiEx.Icon(new Vector4(1.0f, 0.4f, 0.0f, 1.0f), FontAwesomeIcon.Diamond);
                    ImGui.SameLine();
                    ImGui.Text($"对应配方详细信息里的那个图标, 顺带一提。");
                    ImGui.Text($"不建议在俄匊斯行星上使用, 它并不完美, 而且已经给不少人带来了问题。");
                    ImGui.EndTooltip();
                }
            }
        }

        private static void Separator()
        {
            ImGui.Dummy(new Vector2(0, 5));
            ImGui.Separator();
            ImGui.Dummy(new Vector2(0, 5));
        }
    }
}
