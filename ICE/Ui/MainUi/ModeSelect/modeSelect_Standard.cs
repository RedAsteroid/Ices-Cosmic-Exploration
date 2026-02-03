using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.LayoutEngine;
using ICE.Ui.MainUi.Settings.Settings_Table;
using ICE.Utilities.ImGuiTools;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace ICE.Ui.MainUi.ModeSelect
{
    internal class modeSelect_Standard
    {
        private static readonly Dictionary<string, uint> BattleJobs = new()
        {
            // Tanks
            { "骑士", 19 },          // Paladin
            { "战士", 21 },          // Warrior
            { "暗黑骑士", 32 },      // Dark Knight
            { "绝枪战士", 37 },      // Gunbreaker

            // Healers
            { "白魔法师", 24 },      // White Mage
            { "学者", 28 },          // Scholar
            { "占星术士", 33 },      // Astrologian
            { "贤者", 40 },          // Sage

            // Melee DPS
            { "武僧", 20 },          // Monk
            { "龙骑士", 22 },        // Dragoon
            { "忍者", 30 },          // Ninja
            { "武士", 34 },          // Samurai
            { "钐镰客", 39 },        // Reaper
            { "蝰蛇剑士", 41 },      // Viper

            // Physical Ranged DPS
            { "吟游诗人", 23 },      // Bard
            { "机工士", 31 },        // Machinist
            { "舞者", 38 },          // Dancer

            // Magical Ranged DPS
            { "黑魔法师", 25 },      // Black Mage
            { "召唤师", 27 },        // Summoner
            { "赤魔法师", 35 },      // Red Mage
            { "绘灵法师", 42 }       // Pictomancer
        };
        private static string newListName = "";

        public static void Draw()
        {
            using var style = ImRaii.PushStyle(ImGuiStyleVar.ChildRounding, 10).Push(ImGuiStyleVar.ChildBorderSize, 1);

            // Header at the top
            float scale = ImGuiHelpers.GlobalScale;

            bool autoSelectMoon = C.AutoSelectMoon;
            if (autoSelectMoon)
            {
                if (PlayerHelper.IsInSinusArdorum() && (!C.ShowSinusMissions || C.ShowPhaennaMissions || C.ShowOizysMissions))
                {
                    C.ShowSinusMissions = true;
                    C.ShowPhaennaMissions = false;
                    C.ShowOizysMissions = false;
                    C.Save();
                }
                else if (PlayerHelper.IsInPhaenna() && (C.ShowSinusMissions || !C.ShowPhaennaMissions || C.ShowOizysMissions))
                {
                    C.ShowSinusMissions = false;
                    C.ShowPhaennaMissions = true;
                    C.ShowOizysMissions = false;
                    C.Save();
                }
                else if (PlayerHelper.IsInOizys() && (C.ShowSinusMissions || C.ShowPhaennaMissions || !C.ShowOizysMissions))
                {
                    C.ShowSinusMissions = false;
                    C.ShowPhaennaMissions = false;
                    C.ShowOizysMissions = true;
                    C.Save();
                }
            }

            using (var headerChild = ImRaii.Child("##modeSelect_StandardHeader", new Vector2(0, 45 * scale), true, ImGuiWindowFlags.NoScrollbar))
            {
                if (!headerChild.Success) return;

                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10 * scale);
                ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 5 * scale);

                string modeType = string.Empty;
                FontAwesomeIcon modeIcon = FontAwesomeIcon.List;

                bool relicMode = C.XPRelicGrind;
                bool xpLeveling = C.XPLeveling_Mode;
                bool standard = (!relicMode && !xpLeveling);


                if (standard)
                    modeType = "标准模式"; // Standard
                else if (relicMode)
                {
                    modeType = "宇宙工具研究数据刷取模式"; // Relic Grind
                    modeIcon = FontAwesomeIcon.ArrowUpRightDots;
                }
                else if (xpLeveling)
                {
                    modeType = "练级模式"; // Leveling Grind
                    modeIcon = FontAwesomeIcon.Leaf;
                }

                ImGuiEx.IconWithText(modeIcon, $"{modeType}"); // Mode | 这里去掉直接使用原始文本

                ImGui.SameLine(0, 10 * scale);

                // Adjust the Y position to center the button vertically with the text
                float textHeight = ImGui.GetTextLineHeight();
                float buttonHeight = ImGui.GetFrameHeight();
                float yOffset = (textHeight - buttonHeight) / 2f;
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + yOffset);

                if (ImGuiEx.IconButtonWithText(FontAwesomeIcon.Play, "模式选择")) // Mode Selection
                {
                    ImGui.OpenPopup("Mode Select | Select Mode Window");
                }
                if (ImGui.BeginPopup("Mode Select | Select Mode Window"))
                {
                    ImGui.Text("选择模式"); // Select Mode
                    ImGui.Separator();

                    if (ImGui.RadioButton("标准模式", standard)) // Standard
                    {
                        C.XPRelicGrind = false;
                        C.XPLeveling_Mode = false;
                        C.Save();
                    }
                    ImGuiEx.HelpMarker("标准模式 \n" + // Stand Mode (typo!)
                                       "-> 用于选择您想要刷取的任务。此模式按以下优先级顺序执行:\n" + // Used to select which missions you want to grind. It'll priortize in the following order:\n
                                       "-> 紧急探索任务 -> 临时性任务 [连续/时间限定/天气限定任务] -> 标准任务 [A->D]\n" + // Critical -> Provisional [Sequence/Timed/Weather] -> Standard [A->D]\n
                                       "-> 选择你想要做的任务, 然后开始吧。"); // Select which missions you want to do, and go at it.
                    if (ImGui.RadioButton("宇宙工具研究数据刷取模式", relicMode)) // Relic Grind
                    {
                        C.XPRelicGrind = true;
                        C.XPLeveling_Mode = false;
                        C.Save();
                    }
                    ImGuiEx.HelpMarker("宇宙工具研究数据刷取模式\n" + // Relic Grind\n
                                       "-> 自动选择最适合完成宇宙工具的任务\n" +
                                       "-> 任务选择的权重依据为完成宇宙工具到下一阶段所需的研究数据\n" +
                                       "-> 如果您只想做特定任务, 可以启用此选项并选择您要做的任务");

                    if (ImGui.RadioButton("练级模式", xpLeveling)) // Leveling Grind
                    {
                        C.XPRelicGrind = false;
                        C.XPLeveling_Mode = true;
                        C.Save();
                    }
                    ImGuiEx.HelpMarker("练级模式\n" +
                                       "-> 根据您当前职业所处的等级区间, 自动选择最适合升级的任务\n" + // Will automatically select which mission is the best for leveling your current class based on what level bracket you're in
                                       "-> 这些任务由我手动挑选, 依据完成所需时间决定\n" + // These are hand picked by me, and determined by the time it takes to complete it
                                       "-> 能工巧匠职业会优先选择制作进展需求最低的任务\n" + // For crafters it's whatever missions take the least amount of progress
                                       "-> 大地使者职业会优先选择在最低技能需求下最不折磨的任务\n" + // For gathering, it's whatever is the least pain to do w/ the minimum amount of skills
                                       "**启用这些模式时会自动临时调整相关设置**"); // These will automatically set settings for using these modes temporarily

                    ImGui.EndPopup();
                }

                uint currentJobId = (uint)Player.Job;
                bool usingSupportedJob = CosmicHelper.CrafterJobList.Contains(currentJobId) || CosmicHelper.GatheringJobList.Contains(currentJobId);

                bool AnyStop = C.StopOnceHitCosmicScore
                             | C.StopWhenLevel
                            || C.StopOnceHitCosmoCredits
                            || C.StopOnceHitLunarCredits
                            || C.StopOnceRelicFinished;
                if (AnyStop)
                {
                    ImGui.SameLine(0, 10 * scale);
                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + yOffset);
                    ImGuiEx.Icon(FontAwesomeIcon.ExclamationTriangle);
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();

                        ImGui.Text("看起来你启用了以下停止条件");
                        if (C.StopOnceHitCosmicScore)
                            ImGui.BulletText($"技巧点达到阈值时停止: [{C.CosmicScoreCap:N0}]");
                        if (C.StopWhenLevel)
                            ImGui.BulletText($"等级达到阈值时停止: [{C.TargetLevel:N0}]");
                        if (C.StopOnceHitCosmoCredits)
                            ImGui.BulletText($"宇宙信用点达到阈值时停止: [{C.CosmoCreditsCap:N0}]");
                        if (C.StopOnceHitLunarCredits)
                            ImGui.BulletText($"行星信用点达到阈值时停止: [{C.LunarCreditsCap:N0}]");
                        if (C.StopOnceRelicFinished)
                            ImGui.BulletText($"宇宙工具可报告时停止");

                        ImGui.Text("所以如果停了你又不确定原因... 可能就是它们导致的。");

                        ImGui.EndTooltip();
                    }
                }

                ImGui.SameLine(0, 10 * scale);
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + yOffset);

                bool unsupportedArtisan = xpLeveling && !P.Artisan.UpdatedArtisan() && CosmicHelper.CrafterJobList.Contains((uint)Player.Job);
                bool unsupportedMoon = PlayerHelper.IsInOizys() && xpLeveling;

                using (ImRaii.Disabled(SchedulerMain.State != IceState.Idle || !usingSupportedJob || unsupportedArtisan || unsupportedMoon))
                {
                    if (ImGui.Button("开始", new Vector2(150 * scale, 0)))
                    {
                        SchedulerMain.EnablePlugin();
                    }
                }

                if (unsupportedArtisan)
                {
                    ImGui.SameLine(0, 10 * scale);
                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + yOffset);
                    ImGuiEx.Icon(EColor.Red, FontAwesomeIcon.ExclamationTriangle);
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("嘿! 你需要更新 Artisan 才能使用这个模式, 请至少更新到以下版本:");
                        ImGui.Text("4.0.4.29");
                        ImGui.EndTooltip();
                    }
                }
                else if (unsupportedMoon)
                {
                    ImGui.SameLine(0, 10 * scale);
                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + yOffset);
                    ImGuiEx.Icon(EColor.Red, FontAwesomeIcon.ExclamationTriangle);
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("嘿! 这个地图目前还不支持练级。(而且比憧憬湾、法恩娜行星更烂)");
                        ImGui.Text("请等我有时间再来处理这个问题。");
                        ImGui.EndTooltip();
                    }
                }

                ImGui.SameLine(0, 10 * scale);
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + yOffset);

                using (ImRaii.Disabled(SchedulerMain.State == IceState.Idle))
                {
                    using (ImRaii.PushColor(ImGuiCol.Button, new Vector4(0.8f, 0.2f, 0.2f, 1.0f)))
                    using (ImRaii.PushColor(ImGuiCol.ButtonHovered, new Vector4(0.9f, 0.3f, 0.3f, 1.0f)))
                    using (ImRaii.PushColor(ImGuiCol.ButtonActive, new Vector4(0.7f, 0.1f, 0.1f, 1.0f)))
                    {
                        if (ImGui.Button("停止", new Vector2(150 * scale, 0)))
                        {
                            SchedulerMain.DisablePlugin();
                        }
                    }
                }
            }

            if (ImGui.BeginTable("modeSelect_TableHeader", 5, ImGuiTableFlags.SizingFixedFit, Vector2.Zero))
            {
                ImGui.TableSetupColumn("Class Selector");
                ImGui.TableSetupColumn("Other Settings");

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);

                bool tableSettingExpanded = modeSelect_Tools.DrawCompactCategoryHeader("表格设置", FontAwesomeIcon.Table); // Table Settings

                ImGui.TableNextColumn();
                bool missionSettingExpanded = modeSelect_Tools.DrawCompactCategoryHeader("任务设置", FontAwesomeIcon.UserCog); // Mission Settings

                bool relicGrindExpanded = false;
                if (C.XPRelicGrind)
                {
                    ImGui.TableNextColumn();
                    relicGrindExpanded = modeSelect_Tools.DrawCompactCategoryHeader("宇宙工具研究数据刷取设置", FontAwesomeIcon.ArrowUpRightDots); // Relic Grind Settings
                }

                bool completionExpanded = false;
                if (C.ShowCompletionWindow)
                {
                    ImGui.TableNextColumn();
                    completionExpanded = modeSelect_Tools.DrawCompactCategoryHeader("完成情况表格设置", FontAwesomeIcon.Trophy); // Completion Table Settings
                }

                bool showPlaylistExpanded = false;
                bool standard = !(C.XPRelicGrind || C.XPLeveling_Mode || C.ShowCompletionWindow);
                if (standard)
                {
                    ImGui.TableNextColumn();
                    showPlaylistExpanded = modeSelect_Tools.DrawCompactCategoryHeader("任务预设", FontAwesomeIcon.PlayCircle);
                }

                bool showJobSwapExpanded = false;
                bool relicJobSwap = C.TurninRelic;

                if (relicJobSwap)
                {
                    ImGui.TableNextColumn();
                    showJobSwapExpanded = modeSelect_Tools.DrawCompactCategoryHeader("宇宙工具职业切换", FontAwesomeIcon.Hammer);
                }

                bool showNextColumn = tableSettingExpanded || missionSettingExpanded || (relicGrindExpanded && C.XPRelicGrind) || (completionExpanded && C.ShowCompletionWindow) || showPlaylistExpanded || showJobSwapExpanded;

                if (showNextColumn)
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    if (tableSettingExpanded)
                    {
                        Settings_TableColumns.ColumnSettings();
                    }

                    ImGui.TableNextColumn();
                    if (missionSettingExpanded)
                    {
                        Settings_TableColumns.GeneralMissionSettings();
                    }

                    if (C.XPRelicGrind)
                    {
                        ImGui.TableNextColumn();
                        if (relicGrindExpanded)
                        {
                            bool relicTurnin = C.TurninRelic;
                            if (ImGui.Checkbox($"宇宙工具可报告时提交##RelicTurnin_RelicGrind", ref relicTurnin))
                            {
                                C.TurninRelic = relicTurnin;
                                C.Save();
                            }
                            ImGui.SameLine();
                            ImGui.TextDisabled("?");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.SetTooltip("这是此功能的工作方式说明。如果我未来修改了这个功能, 这个提示也会随之改变。\n" +
                                                 "1: 此功能将检查你的当前职业 [不是菜单中选择的职业, 是实际当前职业] 提交宇宙工具。\n" +
                                                 "2: 此功能的优先级高于 \"宇宙工具可报告时停止\" 选项，如果两个都启用, 它会选择提交宇宙工具而不是停止, 并继续执行任务。\n" +
                                                 "3: 如果你当前是能工巧匠职业，报告后会自动返回你之前正在制作的位置。\n" +
                                                 "\t- 这是可选的, 你可以自由关闭。我个人喜欢开着, 方便我回到自己选定的安静区域。");
                            }

                            ImGui.Separator();

                            bool EnableRelicXp = C.XPRelicGrind;
                            if (ImGui.Checkbox("自动根据研究数据挑选任务", ref EnableRelicXp))
                            {
                                C.XPRelicGrind = EnableRelicXp;
                                C.Save();
                            }
                            ImGui.SameLine();
                            ImGui.TextDisabled("?");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.SetTooltip("请注意: 此功能仅会执行基础任务标签下的任务刷取研究数据。\n" +
                                                   "即使您选择启用了连续/时间限定/天气限定/紧急探索任务, 也不会执行这些任务。");
                            }
                            if (EnableRelicXp)
                            {
                                bool OnlySelected = C.XPRelicOnlyEnabled;
                                if (ImGui.Checkbox("仅限启用的任务", ref OnlySelected))
                                {
                                    C.XPRelicOnlyEnabled = OnlySelected;
                                    C.Save();
                                }
                                if (C.ShowManualMode)
                                {
                                    bool IgnoreManual = C.XPRelicIgnoreManual;
                                    if (ImGui.Checkbox("忽略手动模式任务", ref IgnoreManual))
                                    {
                                        C.XPRelicIgnoreManual = IgnoreManual;
                                        C.Save();
                                    }
                                }
                            }
                        }
                    }

                    if (C.ShowCompletionWindow)
                    {
                        ImGui.TableNextColumn();
                        if (completionExpanded)
                        {
                            bool showSelectedJobOnly = C.ShowSelectedJobOnly;
                            if (ImGui.Checkbox("只显示选择的职业", ref showSelectedJobOnly))
                            {
                                C.ShowSelectedJobOnly = showSelectedJobOnly;
                                if (showSelectedJobOnly)
                                    C.ShowCompletionOnlyJob = false;
                                C.Save();
                            }

                            bool nonGold = C.ShowCompletion_MissingGold;
                            if (ImGui.Checkbox("只显示非金星评价任务", ref nonGold))
                            {
                                C.ShowCompletion_MissingGold = nonGold;
                                C.Save();
                            }
                        }
                    }

                    if (standard)
                    {
                        ImGui.TableNextColumn();

                        if (showPlaylistExpanded)
                        {
                            if (ImGui.Button("保存当前任务为预设"))
                            {
                                ImGui.OpenPopup("Preset Save Editor");
                            }

                            if (ImGui.BeginPopup("Preset Save Editor"))
                            {
                                ImGui.InputText($"预设名称", ref newListName);
                                using (ImRaii.Disabled(string.IsNullOrEmpty(newListName)))
                                {
                                    if (ImGui.Button("保存"))
                                    {
                                        List<uint> new_Playlist = new();
                                        foreach (var mission in C.MissionConfig.Where(x => x.Value.Enabled))
                                        {
                                            new_Playlist.Add(mission.Key);
                                        }
                                        if (C.Mission_Playlist.ContainsKey(newListName))
                                        {
                                            C.Mission_Playlist[newListName] = new_Playlist;
                                        }
                                        else
                                        {
                                            C.Mission_Playlist.Add(newListName, new_Playlist);
                                        }
                                        C.Save();
                                        ImGui.CloseCurrentPopup();
                                    }
                                }

                                ImGui.EndPopup();
                            }

                            if (C.Mission_Playlist.Count > 0)
                            {
                                if (ImGui.Button("查看所有预设"))
                                {
                                    ImGui.OpenPopup("Preset: List Viewer");
                                }

                                if (ImGui.BeginPopup("Preset: List Viewer"))
                                {
                                    ImGui.Text($"加载任务预设");

                                    if (ImGui.BeginTable($"Preset: TableViewer", 3, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders))
                                    {
                                        ImGui.TableSetupColumn("名称");
                                        ImGui.TableSetupColumn("启用任务数量");

                                        ImGui.TableHeadersRow();

                                        ImGui.TableNextRow();
                                        ImGui.TableSetColumnIndex(0);
                                        ImGui.AlignTextToFramePadding();
                                        ImGui.Text($"全部清空");
                                        ImGui.SameLine();
                                        if (ImGuiEx.IconButton(FontAwesomeIcon.ArrowUpRightFromSquare, $"FreshPreset_Button"))
                                        {
                                            foreach (var mission in C.MissionConfig)
                                            {
                                                mission.Value.Enabled = false;
                                            }
                                            C.Save();
                                            ImGui.CloseCurrentPopup();
                                        }

                                        foreach (var item in C.Mission_Playlist)
                                        {
                                            ImGui.TableNextRow();
                                            ImGui.TableSetColumnIndex(0);
                                            ImGui.AlignTextToFramePadding();
                                            ImGui.Text($"{item.Key}");
                                            ImGui.SameLine();
                                            if (ImGuiEx.IconButton(FontAwesomeIcon.ArrowUpRightFromSquare, $"{item.Key}_Button"))
                                            {
                                                foreach (var mission in C.MissionConfig)
                                                {
                                                    if (item.Value.Contains(mission.Key))
                                                        mission.Value.Enabled = true;
                                                    else
                                                        mission.Value.Enabled = false;
                                                }
                                                C.Save();
                                                ImGui.CloseCurrentPopup();
                                            }
                                            if (ImGui.IsItemHovered())
                                            {
                                                ImGui.SetTooltip("导入任务");
                                            }

                                            ImGui.TableNextColumn();
                                            ImGui.AlignTextToFramePadding();
                                            ImGui.Text($"{item.Value.Count}");

                                            ImGui.TableNextColumn();
                                            if (ImGuiEx.IconButton(FontAwesomeIcon.Trash, $"{item.Key}_Remove"))
                                            {
                                                C.Mission_Playlist.Remove(item);
                                                C.Save();
                                            }
                                            if (ImGui.IsItemHovered())
                                            {
                                                ImGui.SetTooltip("移除");
                                            }
                                        }

                                        ImGui.EndTable();
                                    }

                                    ImGui.EndPopup();
                                }
                            }
                        }
                    }

                    if (C.TurninRelic)
                    {
                        ImGui.TableNextColumn();
                        if (showJobSwapExpanded)
                        {
                            bool swapJobs = C.Relic_SwapJob;
                            if (ImGui.Checkbox("提交宇宙工具时切换职业", ref swapJobs)) // Swap jobs when turning in relic
                            {
                                C.Relic_SwapJob = swapJobs;
                                C.Save();
                            }
                            ImGuiEx.HelpMarker("如果您使用宇宙工具作为主手, 请务必启用此选项, 并设置好战斗职业以保证提交宇宙工具正常运行。\n" +
                                               "因为宇宙工具作为主手装备时无法提交的, 必须通过切换职业来绕过。");


                            string currentJobName = BattleJobs.FirstOrDefault(x => x.Value == C.Relic_BattleJob).Key ?? "无";

                            if (ImGui.BeginCombo("战斗职业", currentJobName))
                            {
                                foreach (var job in BattleJobs)
                                {
                                    bool isSelected = C.Relic_BattleJob == job.Value;
                                    if (ImGui.Selectable(job.Key, isSelected))
                                    {
                                        C.Relic_BattleJob = job.Value;
                                        C.Save();
                                    }
                                    if (isSelected)
                                        ImGui.SetItemDefaultFocus();
                                }
                                ImGui.EndCombo();
                            }

                            bool useStylist = C.Relic_Stylist;
                            if (ImGui.Checkbox($"使用 Stylist 插件重新装备工具", ref useStylist)) // Use Stylist to re-equip tools
                            {
                                C.Relic_Stylist = useStylist;
                                C.Save();
                            }
                        }
                    }
                }

                ImGui.EndTable();
            }

            using (var bodyChild = ImRaii.Child("##modeSelect_Body", new Vector2(0, -1), true, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                if (!bodyChild.Success) return;

                foreach (var missionType in modeSelect_TableInfo.missionList)
                {
                    missionType.Value.Clear();
                }

                foreach (var mission in CosmicHelper.SheetMissionDict)
                {
                    var Jobs = mission.Value.Jobs;
                    var territoryId = mission.Value.TerritoryId;
                    uint selectedJob = C.SelectedJob;
                    bool sinusEnabled = C.ShowSinusMissions;
                    bool phaennaEnabled = C.ShowPhaennaMissions;
                    bool oizysEnabled = C.ShowOizysMissions;

                    if (C.ShowCompletionWindow)
                    {
                        if (C.ShowCompletionOnlyJob)
                        {
                            if (!Jobs.Contains(selectedJob))
                                continue;
                        }
                    }

                    if (!sinusEnabled && territoryId == 1237)
                        continue;

                    if (!phaennaEnabled && territoryId == 1291)
                        continue;

                    if (!oizysEnabled && territoryId == 1310)
                        continue;

                    bool provisional = mission.Value.Attributes.HasFlag(MissionAttributes.ProvisionalWeather)
                                    || mission.Value.Attributes.HasFlag(MissionAttributes.ProvisionalTimed)
                                    || mission.Value.Attributes.HasFlag(MissionAttributes.ProvisionalSequential);

                    if (provisional)
                    {
                        if (!C.GrindAllProvisionals)
                        {
                            if (!Jobs.Contains(selectedJob))
                                continue;
                        }

                        if (mission.Value.Attributes.HasFlag(MissionAttributes.ProvisionalWeather))
                            modeSelect_TableInfo.missionList["Weather"].Add(new modeSelect_TableInfo.Mission { id = mission.Key, enabled = C.MissionConfig[mission.Key].Enabled });
                        else if (mission.Value.Attributes.HasFlag(MissionAttributes.ProvisionalTimed))
                            modeSelect_TableInfo.missionList["Timed"].Add(new modeSelect_TableInfo.Mission { id = mission.Key, enabled = C.MissionConfig[mission.Key].Enabled });
                        else if (mission.Value.Attributes.HasFlag(MissionAttributes.ProvisionalSequential))
                            modeSelect_TableInfo.missionList["Sequence"].Add(new modeSelect_TableInfo.Mission { id = mission.Key, enabled = C.MissionConfig[mission.Key].Enabled });

                        if (C.MissionConfig.ContainsKey(mission.Key) && C.MissionConfig[mission.Key].Enabled)
                        {
                            modeSelect_TableInfo.missionList["All Enabled"].Add(new modeSelect_TableInfo.Mission { id = mission.Key, enabled = C.MissionConfig[mission.Key].Enabled });
                        }
                    }
                    else
                    {
                        if (!Jobs.Contains(selectedJob))
                            continue;

                        if (mission.Value.Attributes.HasFlag(MissionAttributes.Critical))
                            modeSelect_TableInfo.missionList["Critical"].Add(new modeSelect_TableInfo.Mission { id = mission.Key, enabled = C.MissionConfig[mission.Key].Enabled });
                        else if (mission.Value.Rank > 3)
                            modeSelect_TableInfo.missionList["ARank"].Add(new modeSelect_TableInfo.Mission { id = mission.Key, enabled = C.MissionConfig[mission.Key].Enabled });
                        else if (mission.Value.Rank == 3)
                            modeSelect_TableInfo.missionList["BRank"].Add(new modeSelect_TableInfo.Mission { id = mission.Key, enabled = C.MissionConfig[mission.Key].Enabled });
                        else if (mission.Value.Rank == 2)
                            modeSelect_TableInfo.missionList["CRank"].Add(new modeSelect_TableInfo.Mission { id = mission.Key, enabled = C.MissionConfig[mission.Key].Enabled });
                        else if (mission.Value.Rank == 1)
                            modeSelect_TableInfo.missionList["DRank"].Add(new modeSelect_TableInfo.Mission { id = mission.Key, enabled = C.MissionConfig[mission.Key].Enabled });

                        if (C.MissionConfig.ContainsKey(mission.Key) && C.MissionConfig[mission.Key].Enabled)
                        {
                            modeSelect_TableInfo.missionList["All Enabled"].Add(new modeSelect_TableInfo.Mission { id = mission.Key, enabled = C.MissionConfig[mission.Key].Enabled });
                        }
                    }
                }

                int criticalEnabled = modeSelect_TableInfo.missionList.ContainsKey("Critical") ? modeSelect_TableInfo.missionList["Critical"].Count(mission => mission.enabled) : 0;
                int sequenceEnabled = modeSelect_TableInfo.missionList.ContainsKey("Sequence") ? modeSelect_TableInfo.missionList["Sequence"].Count(mission => mission.enabled) : 0;
                int weatherEnabled = modeSelect_TableInfo.missionList.ContainsKey("Weather") ? modeSelect_TableInfo.missionList["Weather"].Count(mission => mission.enabled) : 0;
                int timedEnabled = modeSelect_TableInfo.missionList.ContainsKey("Timed") ? modeSelect_TableInfo.missionList["Timed"].Count(mission => mission.enabled) : 0;
                int aRankEnabled = modeSelect_TableInfo.missionList.ContainsKey("ARank") ? modeSelect_TableInfo.missionList["ARank"].Count(mission => mission.enabled) : 0;
                int bRankEnabled = modeSelect_TableInfo.missionList.ContainsKey("BRank") ? modeSelect_TableInfo.missionList["BRank"].Count(mission => mission.enabled) : 0;
                int cRankEnabled = modeSelect_TableInfo.missionList.ContainsKey("CRank") ? modeSelect_TableInfo.missionList["CRank"].Count(mission => mission.enabled) : 0;
                int dRankEnabled = modeSelect_TableInfo.missionList.ContainsKey("DRank") ? modeSelect_TableInfo.missionList["DRank"].Count(mission => mission.enabled) : 0;
                int allEnabled = modeSelect_TableInfo.missionList.ContainsKey("All Enabled") ? modeSelect_TableInfo.missionList["All Enabled"].Count(mission => mission.enabled) : 0;

                float scrollbarSize = ImGui.GetStyle().ScrollbarSize;
                float buttonRowHeight = (ImGui.GetTextLineHeight() + 8 * scale + 4 * scale) + scrollbarSize;

                using (var missionButtons = ImRaii.Child("##tab_scroll", new Vector2(0, buttonRowHeight), false, ImGuiWindowFlags.HorizontalScrollbar))
                {
                    if (!missionButtons.Success)
                        return;

                    ImGui_Tools.DrawCategoryButton($"紧急探索任务 [{criticalEnabled}]", "main_Critical");
                    ImGui_Tools.DrawCategoryButton($"连续任务 [{sequenceEnabled}]", "main_Sequence");
                    ImGui_Tools.DrawCategoryButton($"天气限定任务 [{weatherEnabled}]", "main_Weather");
                    ImGui_Tools.DrawCategoryButton($"时间限定任务 [{timedEnabled}]", "main_Timed");
                    ImGui_Tools.DrawCategoryButton($"A 类任务 [{aRankEnabled}]", "main_ARank");
                    ImGui_Tools.DrawCategoryButton($"B 类任务 [{bRankEnabled}]", "main_BRank");
                    ImGui_Tools.DrawCategoryButton($"C 类任务 [{cRankEnabled}]", "main_CRank");
                    ImGui_Tools.DrawCategoryButton($"D 类任务 [{dRankEnabled}]", "main_DRank");
                    var selectedClass = C.SelectedJob;
                    var jobIcon = CosmicHelper.JobIconDict[selectedClass];
                    ImGui_Tools.DrawImageBox(jobIcon, "当前选定", spacingAfter: 5);
                    if (allEnabled > 0)
                    {
                        ImGui_Tools.DrawCategoryButton($"全部启用 [{allEnabled}]", "main_AllEnabled");
                    }

                    ImGui_Tools.EndCategoryButtonRow();
                }

                if (C.ShowExtraMissionInfo)
                {
                    if (ImGui.BeginTable("Mission Info | Extra Details", 2, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.Resizable, Vector2.Zero))
                    {
                        ImGui.TableSetupColumn("Mission Selection Viewer", ImGuiTableColumnFlags.WidthFixed, 200f);
                        ImGui.TableSetupColumn("Specific Mission Info", ImGuiTableColumnFlags.WidthStretch);

                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        MissionTableInfo();

                        ImGui.TableNextColumn();
                        using (var missionInfoChild = ImRaii.Child("##modeSelect_MissionInfo", new Vector2(0, 0), false))
                        {
                            modeSelect_TableInfo.DrawMissionDetails();
                        }

                        ImGui.EndTable();
                    }
                }
                else
                {
                    MissionTableInfo();
                }
            }
        }

        private static void MissionTableInfo()
        {
            using (var missionTableChild = ImRaii.Child("##modeSelect_MissionTables", new Vector2(0, 0), false))
            {
                var enabledTabs = ImGui_Tools.CategoryStates;
                modeSelect_TableInfo.missionList["All Enabled"] = modeSelect_TableInfo.missionList["All Enabled"]
                    .OrderBy(x => {
                        var missionInfo = CosmicHelper.SheetMissionDict[x.id];
                        var attributes = missionInfo.Attributes;

                        // Determine which provisional type this mission is
                        if (attributes.HasFlag(MissionAttributes.ProvisionalWeather))
                            return C.MissionPrio.IndexOf(ProvisionalTypes.ProvisionalWeather);
                        else if (attributes.HasFlag(MissionAttributes.ProvisionalSequential))
                            return C.MissionPrio.IndexOf(ProvisionalTypes.ProvisionalSequential);
                        else if (attributes.HasFlag(MissionAttributes.ProvisionalTimed))
                            return C.MissionPrio.IndexOf(ProvisionalTypes.ProvisionalTimed);
                        else
                            return int.MaxValue; // Non-provisional missions sort last
                    })
                    .ThenBy(x => {
                        var firstJob = CosmicHelper.SheetMissionDict[x.id].Jobs.First();
                        return C.JobPrio.IndexOf(firstJob);
                    })
                    .ThenByDescending(x => CosmicHelper.SheetMissionDict[x.id].Rank)
                    .ToList();

                modeSelect_TableInfo.missionList["Sequence"] = modeSelect_TableInfo.missionList["Sequence"]
                    .OrderBy(x => C.JobPrio.IndexOf(CosmicHelper.SheetMissionDict[x.id].Jobs.First()))
                    .ToList();

                modeSelect_TableInfo.missionList["Weather"] = modeSelect_TableInfo.missionList["Weather"]
                    .OrderBy(x => C.JobPrio.IndexOf(CosmicHelper.SheetMissionDict[x.id].Jobs.First()))
                    .ToList();

                modeSelect_TableInfo.missionList["Timed"] = modeSelect_TableInfo.missionList["Timed"]
                    .OrderBy(x => C.JobPrio.IndexOf(CosmicHelper.SheetMissionDict[x.id].Jobs.First()))
                    .ToList();

                if (enabledTabs["main_AllEnabled"])
                {
                    int allEnabled = modeSelect_TableInfo.missionList.ContainsKey("All Enabled") ? modeSelect_TableInfo.missionList["All Enabled"].Count(mission => mission.enabled) : 0;
                    if (allEnabled == 0)
                        enabledTabs["main_AllEnabled"] = false;

                    if (modeSelect_TableInfo.missionList["All Enabled"].Count > 0)
                    {
                        modeSelect_TableInfo.DrawMissionTablev2("全部启用", "All_Enabled", modeSelect_TableInfo.SortMissionList(modeSelect_TableInfo.missionList["All Enabled"]));
                    }
                    else
                    {
                        ImGui.Text("嘿！启用一些任务, 我们才能在这里显示内容。");
                    }
                }
                if (enabledTabs["main_Critical"])
                    modeSelect_TableInfo.DrawMissionTablev2("紧急探索", "Critical_Missions", modeSelect_TableInfo.SortMissionList(modeSelect_TableInfo.missionList["Critical"]));
                if (enabledTabs["main_Sequence"])
                    modeSelect_TableInfo.DrawMissionTablev2("连续", "Sequence_Missions", modeSelect_TableInfo.SortMissionList(modeSelect_TableInfo.missionList["Sequence"]));
                if (enabledTabs["main_Weather"])
                    modeSelect_TableInfo.DrawMissionTablev2("天气限定", "Weather_Missions", modeSelect_TableInfo.SortMissionList(modeSelect_TableInfo.missionList["Weather"]));
                if (enabledTabs["main_Timed"])
                    modeSelect_TableInfo.DrawMissionTablev2("时间限定", "Timed_Missions", modeSelect_TableInfo.SortMissionList(modeSelect_TableInfo.missionList["Timed"]));
                if (enabledTabs["main_ARank"])
                    modeSelect_TableInfo.DrawMissionTablev2("A 类", "A_RankMissions", modeSelect_TableInfo.SortMissionList(modeSelect_TableInfo.missionList["ARank"]));
                if (enabledTabs["main_BRank"])
                    modeSelect_TableInfo.DrawMissionTablev2("B 类", "B_RankMissions", modeSelect_TableInfo.SortMissionList(modeSelect_TableInfo.missionList["BRank"]));
                if (enabledTabs["main_CRank"])
                    modeSelect_TableInfo.DrawMissionTablev2("C 类", "C_RankMissions", modeSelect_TableInfo.SortMissionList(modeSelect_TableInfo.missionList["CRank"]));
                if (enabledTabs["main_DRank"])
                    modeSelect_TableInfo.DrawMissionTablev2("D 类", "D_RankMissions", modeSelect_TableInfo.SortMissionList(modeSelect_TableInfo.missionList["DRank"]));
            }
        }
    }
}
