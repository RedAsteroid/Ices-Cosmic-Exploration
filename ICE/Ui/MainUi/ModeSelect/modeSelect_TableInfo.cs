using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game.WKS;
using ICE.Utilities.Cosmic;
using ICE.Utilities.GatheringHelper;
using System.Collections.Generic;
using static ECommons.UIHelpers.AddonMasterImplementations.AddonMaster;
using static MissionTimer;

namespace ICE.Ui.MainUi.ModeSelect
{
    internal class modeSelect_TableInfo
    {
        public static HashSet<string> selectedTabs = new HashSet<string>();
        public static uint selectedMission = 0;
        public static List<string> JokeList = new()
        {
            "What is a pirates favorite letter?\n" +
            "You might thing it's R, but tis first love was the C\n" +
            "(It helps if you verbally say it like a pirate)",

            "You know, I was reading this book about anti-gravity recently,\n" +
            "and honestly I'm having a hard time putting it down",

            "Why are tennis pros always hugging each other?\n" +
            "Because they start their match at \"Love All\"",

            "Why can't ghost have babies?\n" +
            "Because they have hallow-eenies",

            "How do you save a drowning pirate?\n" +
            "You give him Cprrrrrr",

            "What is a skeleton's favorite snack?\n" +
            "Ribs! Spare Ribs!",

            "Honestly, just wanted to say thank you for using my plugin, you're appreciated <3",


        };
        public static int jokeId = 0;

        public static Dictionary<string, bool> headerStates = new();

        public static Dictionary<string, List<Mission>> missionList = new()
        {
            ["Critical"] = new List<Mission>(),
            ["Weather"] = new List<Mission>(),
            ["Timed"] = new List<Mission>(),
            ["Sequence"] = new List<Mission>(),
            ["ARank"] = new List<Mission>(),
            ["BRank"] = new List<Mission>(),
            ["CRank"] = new List<Mission>(),
            ["DRank"] = new List<Mission>(),
            ["All Enabled"] = new List<Mission>(),
        };

        public class Mission
        {
            public uint id;
            public bool enabled;
        }

        public static List<Mission> SortMissionList(List<Mission> missions)
        {
            int sortOption = C.TableSortOption;
            var missionInfo = CosmicHelper.SheetMissionDict;

            switch (sortOption)
            {
                case 0: // Sorting by Id
                    return missions.ToList();
                case 1: // Name 
                    return missions.OrderBy(m => missionInfo[m.id].Name).ToList();
                case 2: // Cosmo Credits
                    return missions.OrderByDescending(m => missionInfo[m.id].CosmoCredit).ToList();
                case 3: // Lunar Credits
                    return missions.OrderByDescending(m => missionInfo[m.id].LunarCredit).ToList();
                case 4: // Exp Type 1:
                    return missions.OrderByDescending(m => missionInfo[m.id].RelicXpInfo
                                                     .Where(exp => exp.Key == 1)
                                                     .Sum(exp => exp.Value)).ToList();
                case 5: // Exp Type 2:
                    return missions.OrderByDescending(m => missionInfo[m.id].RelicXpInfo
                                                     .Where(exp => exp.Key == 2)
                                                     .Sum(exp => exp.Value)).ToList();
                case 6: // Exp Type 3:
                    return missions.OrderByDescending(m => missionInfo[m.id].RelicXpInfo
                                                     .Where(exp => exp.Key == 3)
                                                     .Sum(exp => exp.Value)).ToList();
                case 7: // Exp Type 4:
                    return missions.OrderByDescending(m => missionInfo[m.id].RelicXpInfo
                                                     .Where(exp => exp.Key == 4)
                                                     .Sum(exp => exp.Value)).ToList();
                case 8: // Exp Type 5:
                    return missions.OrderByDescending(m => missionInfo[m.id].RelicXpInfo
                                                     .Where(exp => exp.Key == 5)
                                                     .Sum(exp => exp.Value)).ToList();
                case 9: // Exp Type 6:
                    return missions.OrderByDescending(m => missionInfo[m.id].RelicXpInfo
                                                     .Where(exp => exp.Key == 6)
                                                     .Sum(exp => exp.Value)).ToList();
                case 10: // Map Location
                    return missions.OrderBy(m => missionInfo[m.id].MarkerId).ToList();
                case 11: // Mission Score
                    return missions.OrderByDescending(m => missionInfo[m.id].ClassScore).ToList();
                case 12: // Class Exp
                    return missions.OrderByDescending(m => Math.Max(
                                                           Math.Max(missionInfo[m.id].ExpModifier_1, missionInfo[m.id].ExpModifier_2),
                                                           missionInfo[m.id].ExpModifier_3
                                                     )).ToList();
                default:
                    return missions.ToList();
            }
        }

        public static void DrawCollapsibleHeader(string id, string label, float spacing = 4f, Vector4? borderColor = null, Vector4? backgroundColor = null)
        {
            const float padding = 6.0f;
            const float borderRadius = 2.0f;

            // Initialize header state if needed
            if (!headerStates.ContainsKey(id))
                headerStates[id] = false;

            // Calculate dimensions
            var drawList = ImGui.GetWindowDrawList();
            var cursorPos = ImGui.GetCursorScreenPos();
            var windowWidth = ImGui.GetContentRegionAvail().X;
            var textSize = ImGui.CalcTextSize(label);
            var bgHeight = textSize.Y + padding * 2;

            // Define header bounds
            var headerRectMin = cursorPos;
            var headerRectMax = new Vector2(cursorPos.X + windowWidth, cursorPos.Y + bgHeight);

            // Use provided colors or defaults
            var bgColor = backgroundColor ?? new Vector4(0.2f, 0.2f, 0.2f, 1f);
            var borderCol = borderColor ?? ImGuiColors.ParsedGold;

            // Draw background and border
            drawList.AddRectFilled(headerRectMin, headerRectMax, ImGui.GetColorU32(bgColor), borderRadius);
            drawList.AddRect(headerRectMin, headerRectMax, ImGui.GetColorU32(borderCol), borderRadius);

            // Draw centered label
            var textPos = new Vector2(
                cursorPos.X + (windowWidth - textSize.X) * 0.5f,
                cursorPos.Y + padding
            );
            drawList.AddText(textPos, ImGui.GetColorU32(new Vector4(1f, 1f, 1f, 1f)), label);

            // Handle interaction
            ImGui.SetCursorScreenPos(cursorPos);
            ImGui.PushID(id);
            ImGui.InvisibleButton("##header", new Vector2(windowWidth, bgHeight));
            if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                headerStates[id] = !headerStates[id];
            ImGui.PopID();

            // Move cursor past header
            ImGui.SetCursorScreenPos(new Vector2(cursorPos.X, cursorPos.Y + bgHeight + spacing));
        }

        public static void DrawCollapsibleSection(string id, string label, int enabled, List<Mission> missions)
        {
            DrawCollapsibleHeader(id, $"{label} | Enabled: {enabled}");
            if (headerStates.TryGetValue(id, out var isOpen) && isOpen)
            {
                // MissionInfoV2(id, SortMissionList(missions));
            }
        }

        public static bool DrawTabButton(string label, string tabIndex)
        {
            if (selectedTabs.Contains(tabIndex))
            {
                var activeColor = ImGui.GetStyle().Colors[(int)ImGuiCol.TabActive];
                ImGui.PushStyleColor(ImGuiCol.Button, activeColor);
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, activeColor);
            }

            bool clicked = ImGui.Button(label);

            if (selectedTabs.Contains(tabIndex))
                ImGui.PopStyleColor(2);

            if (clicked)
            {
                if (selectedTabs.Contains(tabIndex))
                    selectedTabs.Remove(tabIndex);
                else
                    selectedTabs.Add(tabIndex);
            }

            return clicked;
        }

        public static unsafe void DrawMissionTablev2(string headerName, string tableName, List<Mission> missions)
        {
            // Setting up the size of the table here, ideally I want this to be the width of the current available space
            // Also setting up the header for here as well
            var availableSpace = ImGui.GetContentRegionAvail().X;
            var textSize = ImGui.CalcTextSize(headerName);
            var headerPadding = new Vector2(10, 5);
            var headerHeight = textSize.Y + headerPadding.Y * 2;

            // Custom header to display above the table. This is moreso for quick user viewability
            using (ImRaii.Child($"Table Header: {headerName}", new Vector2(availableSpace, headerHeight), true, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
            {
                var centeredPosX = (availableSpace - textSize.X) / 2;

                ImGui.SetCursorPosY(headerPadding.Y);
                ImGui.SetCursorPosX(centeredPosX);
                ImGui.Text($"{headerName}任务"); // Missions
            }

            // Table settings, just so I can sort it out visibly vs... being shoved in the table
            ImGuiTableFlags tableFlags = ImGuiTableFlags.RowBg |
                                ImGuiTableFlags.Borders |
                                ImGuiTableFlags.Reorderable |
                                ImGuiTableFlags.Hideable |
                                ImGuiTableFlags.SizingFixedFit;
            int tableTotalColumns = 19; // How many columns am I using. 

            // This is here to auto show/hide specific columns that might not be necessary (gathering profiles, and planetary tokens as Ex.)
            bool hasGathering = false;
            bool hasToken = false;

            foreach (var mission in missions)
            {
                var id = mission.id;
                var missionInfo = CosmicHelper.SheetMissionDict[id];

                if (missionInfo.Jobs.Any(x => CosmicHelper.GatheringJobList.Contains(x)))
                    hasGathering = true;
                if (missionInfo.RewardItemAmount > 0)
                    hasToken = true;
            }

            if (ImGui.BeginTable($"MissionList_{tableName}", tableTotalColumns, tableFlags))
            {
                #region Table Column Setup

                ImGui.TableSetupColumn("启用"); // 0
                ImGui.TableSetupColumn("职业");
                ImGui.TableSetupColumn("手动");
                ImGui.TableSetupColumn("ID");
                ImGui.TableSetupColumn("✓");
                ImGui.TableSetupColumn("任务名称");
                ImGui.TableSetupColumn("宇宙信用点");
                ImGui.TableSetupColumn("行星信用点");
                ImGui.TableSetupColumn("技巧点");
                ImGui.TableSetupColumn("物品奖励"); // 9

                // Xp Columns Here
                float padding = 10f;
                float xpWidth = ImGui.CalcTextSize("III").X + padding;
                ImGui.TableSetupColumn("I"); // 10
                ImGui.TableSetupColumn("II");
                ImGui.TableSetupColumn("III");
                ImGui.TableSetupColumn("IV");
                ImGui.TableSetupColumn("V");
                ImGui.TableSetupColumn("VI"); // 15

                ImGui.TableSetupColumn("汇报模式"); // 16
                ImGui.TableSetupColumn("采集配置"); // 17
                ImGui.TableSetupColumn("任务备注"); // 18

                #endregion

                #region Auto-Hiding Columns

                ImGui.TableSetColumnEnabled(0, !((C.XPRelicGrind && !C.XPRelicOnlyEnabled) || C.XPLeveling_Mode));
                ImGui.TableSetColumnEnabled(1, (C.ShowCompletionWindow || C.GrindAllProvisionals)); // Job Column (Useful for provisionals/Timed)
                ImGui.TableSetColumnEnabled(2, C.ShowManualMode);

                if (C.Auto_ShowTokens)
                {
                    ImGui.TableSetColumnEnabled(9, hasToken);
                }

                ImGui.TableSetColumnEnabled(17, hasGathering);

                #endregion

                #region Custom Header Stuff

                ImGui.TableNextRow(ImGuiTableRowFlags.Headers);
                int columnIndexCount = 0;

                #region Enabled Column

                ImGui.TableSetColumnIndex(columnIndexCount);
                ImGui.TableHeader("启用");
                if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    ImGui.OpenPopup("Enabled Options");
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("启用/禁用 自动执行任务");
                    ImGui.Text($"左键点击查看选项");
                    ImGui.EndTooltip();
                }
                if (ImGui.BeginPopup("Enabled Options"))
                {
                    if (ImGui.Button("全部启用"))
                    {
                        foreach (var mission in missions)
                        {
                            C.MissionConfig[mission.id].Enabled = true;
                            if (GetOnlyPreviousMissionsRecursive(mission.id).Count > 0)
                            {
                                foreach (var prevMission in GetOnlyPreviousMissionsRecursive(mission.id))
                                {
                                    var prevMissionConfig = C.MissionConfig[prevMission];
                                    prevMissionConfig.Enabled = true;
                                }
                            }
                        }
                        C.Save();
                    }

                    if (ImGui.Button("全部禁用"))
                    {
                        foreach (var mission in missions)
                        {
                            C.MissionConfig[mission.id].Enabled = false;
                        }
                        C.Save();
                    }

                    ImGui.EndPopup();
                }
                columnIndexCount++;

                #endregion

                #region Jobs

                ImGui.TableSetColumnIndex(columnIndexCount);
                ImGui.TableHeader("职业");
                if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    ImGui.OpenPopup("Jobs Options");
                }
                if (ImGui.BeginPopup("Jobs Options"))
                {
                    bool showAllJobs = C.ShowCompletionOnlyJob;
                    if (ImGui.RadioButton("显示全部职业", !showAllJobs))
                    {
                        C.ShowCompletionOnlyJob = false;
                        C.Save();
                    }
                    if (ImGui.RadioButton("只显示当前职业", showAllJobs))
                    {
                        C.ShowCompletionOnlyJob = true;
                        C.Save();
                    }
                    ImGui.EndPopup();
                }
                columnIndexCount++;

                #endregion

                #region Manual

                ImGui.TableSetColumnIndex(columnIndexCount);
                ImGui.TableHeader("手动");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("手动模式 - 需要手动干预");
                    ImGui.EndTooltip();
                }
                columnIndexCount++;

                #endregion

                #region ID

                ImGui.TableSetColumnIndex(columnIndexCount);
                ImGui.TableHeader("ID");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("任务 ID 数字");
                    ImGui.EndTooltip();
                }
                columnIndexCount++;

                #endregion

                #region Completed

                ImGui.TableSetColumnIndex(columnIndexCount);
                ImGui.TableHeader("✓");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("任务完成状态");
                    ImGui.EndTooltip();
                }
                columnIndexCount++;

                #endregion

                #region Mission Name

                ImGui.TableSetColumnIndex(columnIndexCount);
                ImGui.TableHeader("任务名称");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("点击任务名称查看详情");
                    ImGui.EndTooltip();
                }
                columnIndexCount++;

                #endregion

                #region Cosmocredits

                ImGui.TableSetColumnIndex(columnIndexCount);
                ImGui.TableHeader("宇宙信用点");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("宇宙信用点奖励");
                    ImGui.EndTooltip();
                }
                columnIndexCount++;

                #endregion

                #region Planetary Credits

                ImGui.TableSetColumnIndex(columnIndexCount);
                ImGui.TableHeader("行星信用点");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("行星(憧憬湾/法恩娜/俄匊斯)信用点奖励");
                    ImGui.EndTooltip();
                }
                columnIndexCount++;

                #endregion

                #region Score

                ImGui.TableSetColumnIndex(columnIndexCount);
                ImGui.TableHeader("技巧点");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("职业技巧点奖励");
                    ImGui.EndTooltip();
                }
                columnIndexCount++;

                #endregion

                #region Planet Tokens

                ImGui.TableSetColumnIndex(columnIndexCount);
                ImGui.TableHeader("票据");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("完成此任务可以获得的票据");
                    ImGui.EndTooltip();
                }
                columnIndexCount++;

                #endregion

                #region Relic XP

                string[] xpLabels = { "I", "II", "III", "IV", "V", "VI", "VII" };
                for (int i = 0; i < 6; i++)
                {
                    ImGui.TableSetColumnIndex(columnIndexCount);
                    ImGui.TableHeader(xpLabels[i]);
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text($"宇宙研究数据类型: {xpLabels[i]} 奖励");
                        ImGui.EndTooltip();
                    }
                    columnIndexCount++;
                }

                #endregion

                #region Turnin Mode

                ImGui.TableSetColumnIndex(columnIndexCount);
                ImGui.TableHeader("汇报模式");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("配置任务汇报设置");
                    ImGui.EndTooltip();
                }
                columnIndexCount++;

                #endregion

                #region Gathering Profile

                ImGui.TableSetColumnIndex(columnIndexCount);
                ImGui.TableHeader("采集配置");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("为采集任务选择采集配置");
                    ImGui.EndTooltip();
                }
                columnIndexCount++;

                #endregion

                #region Mission Notes

                ImGui.TableSetColumnIndex(columnIndexCount);
                ImGui.TableHeader("任务备注");
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("任务的补充说明与要求");
                    ImGui.EndTooltip();
                }
                columnIndexCount++;

                #endregion

                #endregion

                foreach (var entry in missions)
                {
                    var Id = entry.id;
                    var missionConfig = C.MissionConfig[Id];
                    var missionInfo = CosmicHelper.SheetMissionDict[Id];

                    bool unsupported = UnsupportedMissions.Ids.Contains(Id);
                    bool hideUnsupported = C.HideUnsupportedMissions;

                    if (unsupported && hideUnsupported)
                        continue;

                    if (C.ShowCompletionWindow)
                    {
                        if (C.ShowCompletion_MissingGold)
                        {
                            var managerPtr = WKSManager.Instance();
                            if (managerPtr == null) continue;

                            var manager = (WKSManagerCustom*)managerPtr;
                            var isGold = manager->IsMissionGolded(Id);

                            if (isGold)
                                continue;
                        }
                        if (C.ShowSelectedJobOnly && !CosmicHelper.SheetMissionDict[Id].Jobs.Contains(C.SelectedJob))
                        {
                            continue;
                        }
                    }

                    ImGui.TableNextRow();
                    ImGui.PushID(Id);

                    #region Enabled Column Stuff

                    ImGui.TableSetColumnIndex(0);
                    bool enabled = missionConfig.Enabled;
                    if (Table_CenterCheckbox("##EnableMission", ref enabled))
                    {
                        missionConfig.Enabled = enabled;
                        if (missionConfig.Enabled == true)
                        {
                            if (GetOnlyPreviousMissionsRecursive(Id).Count >0)
                            {
                                foreach (var prevMission in GetOnlyPreviousMissionsRecursive(Id))
                                {
                                    var prevMissionConfig = C.MissionConfig[prevMission];
                                    prevMissionConfig.Enabled = true;
                                }
                            }
                        }

                        C.Save();
                    }
                    if (ImGui.IsItemClicked())
                    {
                        selectedMission = Id;
                    }

                    #endregion

                    #region Job Info

                    ImGui.TableNextColumn();
                    if (missionInfo.Jobs.Count > 1)
                    {
                        ISharedImmediateTexture? job1Icon = CosmicHelper.JobIconDict[missionInfo.Jobs.First()];
                        ISharedImmediateTexture? job2Icon = CosmicHelper.JobIconDict[missionInfo.Jobs.Last()];
                        Vector2 imageSize = new Vector2(23, 23);

                        ImGui.Image(job1Icon.GetWrapOrEmpty().Handle, imageSize);
                        ImGui.SameLine(0, 2);
                        ImGui.Image(job2Icon.GetWrapOrEmpty().Handle, imageSize);
                    }
                    else
                    {
                        ISharedImmediateTexture? job1Icon = CosmicHelper.JobIconDict[missionInfo.Jobs.First()];
                        Vector2 imageSize = new Vector2(23, 23);
                        ImGui.Image(job1Icon.GetWrapOrEmpty().Handle, imageSize);
                    }

                    #endregion

                    #region Manual Mode

                    ImGui.TableNextColumn();
                    bool manualMode = missionConfig.ManualMode;
                    if (Table_CenterCheckbox("##Manual Mode", ref manualMode))
                    {
                        missionConfig.ManualMode = manualMode;
                        C.Save();
                    }
                    if (ImGui.IsItemClicked())
                    {
                        selectedMission = Id;
                    }

                    #endregion

                    #region Mission Id

                    ImGui.TableNextColumn();

                    if (C.HighlightVisibleMissions)
                    {
                        if (GenericHelpers.TryGetAddonMaster<WKSMission>("WKSMission", out var wksMission) && wksMission.IsAddonReady)
                        {
                            if (wksMission.StellerMissions.Any(x => x.MissionId == Id))
                            {
                                ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, ImGui.GetColorU32(new Vector4(1.0f, 0.0f, 0.0f, 0.3f))); // Red with 30% alpha
                            }
                        }
                    }

                    Table_FullCenterText(Id.ToString());

                    #endregion

                    #region Completion Status

                    ImGui.TableNextColumn();
                    CompletionStatus_Formatted(Id);

                    #endregion

                    #region Mission Name + Flag Info

                    ImGui.TableNextColumn();
                    if (unsupported)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.0f, 0.0f, 1.0f)); // Red color (RGBA)
                        ImGuiEx.IconWithTooltip(FontAwesomeIcon.ExclamationTriangle, "这些现在尚未支持，我正在努力迁移过来。\n" +
                                                "只是需要一些时间。");
                        ImGui.PopStyleColor();
                        ImGui.SameLine();
                    }
                    if (missionInfo.Attributes.HasFlag(MissionAttributes.ExpertCraft))
                    {
                        if (EzThrottler.Throttle("Throttling the manip update every couple of seconds", 1000))
                            PlayerHelper.UpdateHasManip();

                        var crafterJobId = missionInfo.Jobs.Where(x => CosmicHelper.CrafterJobList.Contains(x)).FirstOrDefault();
                        if (PlayerHelper.ManipClassInfo.TryGetValue(crafterJobId, out var manipInfo) && !manipInfo.HasUnlocked)
                        {
                            var color = EColor.Yellow;
                            ImGuiEx.IconWithTooltip(color, FontAwesomeIcon.ExclamationTriangle, 
                                                    "这是一个根据游戏定义的高难度配方, 但您的当前职业尚未解锁 \"掌握\" 技能。\n" +
                                                    "您可以自行启用此任务, 但请注意: 在解锁该技能之前, Artisan 将不允许您以内置的求解器进行制作。\n" +
                                                    "您仍然可以通过编写宏来完成, 否则就需要完成职业任务, 直到大约 68 级来解锁该技能。");
                        }
                        ImGui.SameLine();
                    }

                    ImGui.Text(missionInfo.Name);
                    if (ImGui.IsItemClicked())
                    {
                        selectedMission = Id;
                    }
                    if (missionInfo.MarkerId != 0)
                    {
                        ImGui.SameLine();
                        ImGui.PushFont(UiBuilder.IconFont);
                        ImGui.Text(FontAwesomeIcon.Flag.ToIconString());
                        ImGui.PopFont();
                        if (ImGui.IsItemClicked())
                        {
                            selectedMission = Id;
                            Utils.SetGatheringRing(missionInfo.TerritoryId, (int)missionInfo.MapPosition.X, (int)missionInfo.MapPosition.Y, missionInfo.Radius, missionInfo.Name);
                        }
#if DEBUG
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip($"X: {missionInfo.MapPosition.X} Y: {missionInfo.MapPosition.Y}");
                        }
#endif
                    }
                    if (GatheringUtil.CriticalLocations.TryGetValue(Id, out var criticalLoc))
                    {
                        ImGui.SameLine();
                        ImGuiEx.Icon(FontAwesomeIcon.FlagCheckered);
                        if (ImGui.IsItemClicked())
                        {
                            Utils.SetFlagForNPC(missionInfo.TerritoryId, criticalLoc.MapInfo.X, criticalLoc.MapInfo.Y);
                        }
                    }
                    if (!C.ShowExtraMissionInfo)
                    {
                        ImGui.SameLine();
                        ImGuiEx.Icon(FontAwesomeIcon.ArrowUpRightFromSquare);
                        if (ImGui.IsItemClicked())
                        {
                            selectedMission = Id;
                            P.externalDetails.IsOpen = true;
                        }
                    }

                    #endregion

                    #region Cosmo | Planetary | Class Score | Tokens

                    ImGui.TableNextColumn();
                    Table_FullCenterText(missionInfo.CosmoCredit.ToString());

                    ImGui.TableNextColumn();
                    Table_FullCenterText(missionInfo.LunarCredit.ToString());

                    ImGui.TableNextColumn();
                    Table_FullCenterText(missionInfo.ClassScore.ToString());

                    ImGui.TableNextColumn();
                    string itemAmount = missionInfo.RewardItemAmount > 0 ? $"{missionInfo.RewardItemAmount}" : "-";
                    Table_FullCenterText(itemAmount);

                    #endregion

                    #region Relic Xp Info

                    for (int i = 1; i < 7; i++)
                    {
                        ImGui.TableNextColumn();
                        var expReward = missionInfo.RelicXpInfo.Where(exp => exp.Key == i).FirstOrDefault();
                        var relicXp = expReward.Value.ToString();

                        if (relicXp == "0")
                        {
                            relicXp = "-";
                        }

                        Table_FullCenterText(relicXp);
                    }

                    #endregion

                    #region Mission Turnins

                    ImGui.TableNextColumn();
                    if (missionInfo.Attributes.HasFlag(MissionAttributes.ScoreTimeRemaining))
                    {
                        Table_FullCenterText("自动");
                        if (missionConfig.AutoTurnin == false)
                        {
                            missionConfig.AutoTurnin = true;
                            missionConfig.TurninGold = false;
                            missionConfig.TurninSilver = false;
                            missionConfig.TurninBronze = false;

                            C.Save();
                        }
                    }
                    else
                    {
                        Vector4 BronzeColor = new Vector4(0.804f, 0.498f, 0.196f, 1.0f);
                        Vector4 SilverColor = new Vector4(0.753f, 0.753f, 0.753f, 1.0f);
                        Vector4 GoldColor = new Vector4(1.0f, 0.843f, 0.0f, 1.0f);
                        Vector4 DisabledColor = new Vector4(0.4f, 0.4f, 0.4f, 1.0f);

                        var fontSize = ImGui.GetFontSize();
                        var framePadding = ImGui.GetStyle().FramePadding;
                        var buttonSize = new Vector2(fontSize + framePadding.X * 2, fontSize + framePadding.Y * 2);
                        var spacing = ImGui.GetStyle().ItemSpacing.X;
                        var totalWidth = (buttonSize.X * 3) + (spacing * 2);

                        // Center the group
                        var cursorPosX = ImGui.GetCursorPosX();
                        var availWidth = ImGui.GetContentRegionAvail().X;
                        ImGui.SetCursorPosX(cursorPosX + (availWidth - totalWidth) * 0.5f);

                        // Gold
                        ImGui.PushStyleColor(ImGuiCol.Text, missionConfig.TurninGold || missionConfig.AutoTurnin ? GoldColor : DisabledColor);
                        if (ImGuiEx.IconButton(FontAwesomeIcon.Trophy, "##Gold", buttonSize))
                        {
                            // If AutoTurnin is on, we're enabling individual controls
                            if (missionConfig.AutoTurnin)
                            {
                                missionConfig.AutoTurnin = false;
                                missionConfig.TurninGold = false;  // Turn off gold
                                missionConfig.TurninSilver = true; // Keep others on
                                missionConfig.TurninBronze = true;
                            }
                            else
                            {
                                // Toggle the button
                                missionConfig.TurninGold = !missionConfig.TurninGold;

                                // Check the new state
                                if (missionConfig.TurninGold && missionConfig.TurninSilver && missionConfig.TurninBronze)
                                {
                                    // All three enabled -> AutoTurnin mode
                                    missionConfig.AutoTurnin = true;
                                    missionConfig.TurninGold = false;
                                    missionConfig.TurninSilver = false;
                                    missionConfig.TurninBronze = false;
                                }
                                else if (!missionConfig.TurninGold && !missionConfig.TurninSilver && !missionConfig.TurninBronze)
                                {
                                    // All three disabled -> AutoTurnin mode (don't disable any)
                                    missionConfig.AutoTurnin = true;
                                }
                            }

                            C.SaveDebounced();
                        }
                        // Right-click to enable only this one
                        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                        {
                            missionConfig.AutoTurnin = false;
                            missionConfig.TurninGold = true;
                            missionConfig.TurninSilver = false;
                            missionConfig.TurninBronze = false;
                            C.SaveDebounced();
                        }
                        ImGui.PopStyleColor();
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();

                            if (missionConfig.AutoTurnin)
                            {
                                ImGuiEx.Icon(GoldColor, FontAwesomeIcon.Trophy);
                                ImGui.SameLine();
                                ImGui.Text("金星");

                                ImGuiEx.Icon(SilverColor, FontAwesomeIcon.Trophy);
                                ImGui.SameLine();
                                ImGui.Text("银星");

                                ImGuiEx.Icon(BronzeColor, FontAwesomeIcon.Trophy);
                                ImGui.SameLine();
                                ImGui.Text("铜星");
                            }
                            else
                            {
                                if (missionConfig.TurninGold)
                                {
                                    ImGuiEx.Icon(GoldColor, FontAwesomeIcon.Trophy);
                                    ImGui.SameLine();
                                    ImGui.Text("金星");
                                }
                                if (missionConfig.TurninSilver)
                                {
                                    ImGuiEx.Icon(SilverColor, FontAwesomeIcon.Trophy);
                                    ImGui.SameLine();
                                    ImGui.Text("银星");
                                }
                                if (missionConfig.TurninBronze)
                                {
                                    ImGuiEx.Icon(BronzeColor, FontAwesomeIcon.Trophy);
                                    ImGui.SameLine();
                                    ImGui.Text("铜星");
                                }
                            }

                            ImGui.Text("右键点击 - 只启用金星");

                            ImGui.EndTooltip();
                        }

                        ImGui.SameLine();

                        // Silver
                        ImGui.PushStyleColor(ImGuiCol.Text, missionConfig.TurninSilver || missionConfig.AutoTurnin ? SilverColor : DisabledColor);
                        if (ImGuiEx.IconButton(FontAwesomeIcon.Trophy, "##Silver", buttonSize))
                        {
                            // If AutoTurnin is on, we're enabling individual controls
                            if (missionConfig.AutoTurnin)
                            {
                                missionConfig.AutoTurnin = false;
                                missionConfig.TurninGold = true;
                                missionConfig.TurninSilver = false;  // Turn off silver
                                missionConfig.TurninBronze = true;
                            }
                            else
                            {
                                // Toggle the button
                                missionConfig.TurninSilver = !missionConfig.TurninSilver;

                                // Check the new state
                                if (missionConfig.TurninGold && missionConfig.TurninSilver && missionConfig.TurninBronze)
                                {
                                    // All three enabled -> AutoTurnin mode
                                    missionConfig.AutoTurnin = true;
                                    missionConfig.TurninGold = false;
                                    missionConfig.TurninSilver = false;
                                    missionConfig.TurninBronze = false;
                                }
                                else if (!missionConfig.TurninGold && !missionConfig.TurninSilver && !missionConfig.TurninBronze)
                                {
                                    // All three disabled -> AutoTurnin mode (don't disable any)
                                    missionConfig.AutoTurnin = true;
                                }
                            }

                            C.SaveDebounced();
                        }
                        // Right-click to enable only this one
                        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                        {
                            missionConfig.AutoTurnin = false;
                            missionConfig.TurninGold = false;
                            missionConfig.TurninSilver = true;
                            missionConfig.TurninBronze = false;
                            C.SaveDebounced();
                        }
                        ImGui.PopStyleColor();
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();

                            if (missionConfig.AutoTurnin)
                            {
                                ImGuiEx.Icon(GoldColor, FontAwesomeIcon.Trophy);
                                ImGui.SameLine();
                                ImGui.Text("金星");

                                ImGuiEx.Icon(SilverColor, FontAwesomeIcon.Trophy);
                                ImGui.SameLine();
                                ImGui.Text("银星");

                                ImGuiEx.Icon(BronzeColor, FontAwesomeIcon.Trophy);
                                ImGui.SameLine();
                                ImGui.Text("铜星");
                            }
                            else
                            {
                                if (missionConfig.TurninGold)
                                {
                                    ImGuiEx.Icon(GoldColor, FontAwesomeIcon.Trophy);
                                    ImGui.SameLine();
                                    ImGui.Text("金星");
                                }
                                if (missionConfig.TurninSilver)
                                {
                                    ImGuiEx.Icon(SilverColor, FontAwesomeIcon.Trophy);
                                    ImGui.SameLine();
                                    ImGui.Text("银星");
                                }
                                if (missionConfig.TurninBronze)
                                {
                                    ImGuiEx.Icon(BronzeColor, FontAwesomeIcon.Trophy);
                                    ImGui.SameLine();
                                    ImGui.Text("铜星");
                                }
                            }

                            ImGui.Text("右键点击 - 只启用银星");

                            ImGui.EndTooltip();
                        }

                        ImGui.SameLine();

                        // Bronze
                        ImGui.PushStyleColor(ImGuiCol.Text, missionConfig.TurninBronze || missionConfig.AutoTurnin ? BronzeColor : DisabledColor);
                        if (ImGuiEx.IconButton(FontAwesomeIcon.Trophy, "##Bronze", buttonSize))
                        {
                            // If AutoTurnin is on, we're enabling individual controls
                            if (missionConfig.AutoTurnin)
                            {
                                missionConfig.AutoTurnin = false;
                                missionConfig.TurninGold = true;
                                missionConfig.TurninSilver = true;
                                missionConfig.TurninBronze = false;  // Turn off bronze
                            }
                            else
                            {
                                // Toggle the button
                                missionConfig.TurninBronze = !missionConfig.TurninBronze;

                                // Check the new state
                                if (missionConfig.TurninGold && missionConfig.TurninSilver && missionConfig.TurninBronze)
                                {
                                    // All three enabled -> AutoTurnin mode
                                    missionConfig.AutoTurnin = true;
                                    missionConfig.TurninGold = false;
                                    missionConfig.TurninSilver = false;
                                    missionConfig.TurninBronze = false;
                                }
                                else if (!missionConfig.TurninGold && !missionConfig.TurninSilver && !missionConfig.TurninBronze)
                                {
                                    // All three disabled -> AutoTurnin mode (don't disable any)
                                    missionConfig.AutoTurnin = true;
                                }
                            }

                            C.SaveDebounced();
                        }
                        // Right-click to enable only this one
                        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                        {
                            missionConfig.AutoTurnin = false;
                            missionConfig.TurninGold = false;
                            missionConfig.TurninSilver = false;
                            missionConfig.TurninBronze = true;
                            C.SaveDebounced();
                        }
                        ImGui.PopStyleColor();
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();

                            if (missionConfig.AutoTurnin)
                            {
                                ImGuiEx.Icon(GoldColor, FontAwesomeIcon.Trophy);
                                ImGui.SameLine();
                                ImGui.Text("金星");

                                ImGuiEx.Icon(SilverColor, FontAwesomeIcon.Trophy);
                                ImGui.SameLine();
                                ImGui.Text("银星");

                                ImGuiEx.Icon(BronzeColor, FontAwesomeIcon.Trophy);
                                ImGui.SameLine();
                                ImGui.Text("铜星");
                            }
                            else
                            {
                                if (missionConfig.TurninGold)
                                {
                                    ImGuiEx.Icon(GoldColor, FontAwesomeIcon.Trophy);
                                    ImGui.SameLine();
                                    ImGui.Text("金星");
                                }
                                if (missionConfig.TurninSilver)
                                {
                                    ImGuiEx.Icon(SilverColor, FontAwesomeIcon.Trophy);
                                    ImGui.SameLine();
                                    ImGui.Text("银星");
                                }
                                if (missionConfig.TurninBronze)
                                {
                                    ImGuiEx.Icon(BronzeColor, FontAwesomeIcon.Trophy);
                                    ImGui.SameLine();
                                    ImGui.Text("铜星");
                                }
                            }

                            ImGui.Text("右键点击 - 只启用铜星");

                            ImGui.EndTooltip();
                        }

                        /*
                        if (Table_CenterEnabled(goldEnabled, silverEnabled, bronzeEnabled))
                        {
                            ImGui.OpenPopup("Mission Turnin Settings");
                        }

                        if (ImGui.BeginPopup("Mission Turnin Settings"))
                        {
                            bool anyTurnin = missionConfig.AutoTurnin;
                            bool goldTurnin = missionConfig.TurninGold;
                            bool silverTurnin = missionConfig.TurninSilver;
                            bool bronzeTurnin = missionConfig.TurninBronze;

                            ImGui.Text("选择汇报选项");
                            ImGui.Dummy(new Vector2(0, 2));

                            if (ImGui.Checkbox("自动", ref anyTurnin))
                            {
                                if (anyTurnin)
                                {
                                    missionConfig.TurninGold = false;
                                    missionConfig.TurninSilver = false;
                                    missionConfig.TurninBronze = false;

                                    missionConfig.AutoTurnin = anyTurnin;
                                }
                                else
                                {
                                    if (!(bronzeTurnin && silverTurnin && goldTurnin))
                                    {
                                        missionConfig.AutoTurnin = true;
                                    }
                                }

                                C.Save();
                            }
                            ImGuiEx.HelpMarker("此选项将尽力获得最佳结果, 但在必要时也会汇报任意结果而避免中止。");

                            ImGui.Separator();

                            if (ImGui.Checkbox("金星", ref goldTurnin))
                            {
                                if (anyTurnin && goldTurnin)
                                    missionConfig.AutoTurnin = false;

                                missionConfig.TurninGold = goldTurnin;
                                C.SaveDebounced();
                            }
                            if (ImGui.Checkbox("银星", ref silverTurnin))
                            {
                                if (anyTurnin && silverTurnin)
                                    missionConfig.AutoTurnin = false;

                                missionConfig.TurninSilver = silverTurnin;
                                C.SaveDebounced();
                            }
                            if (ImGui.Checkbox("铜星", ref bronzeTurnin))
                            {
                                if (anyTurnin && bronzeTurnin)
                                    missionConfig.AutoTurnin = false;

                                missionConfig.TurninBronze = bronzeTurnin;
                                C.SaveDebounced();
                            }

                            if (!bronzeTurnin && !silverTurnin && !goldTurnin && !anyTurnin)
                            {
                                missionConfig.AutoTurnin = true;
                                C.SaveDebounced();
                            }

                            ImGui.EndPopup();
                        }
                        */
                    }

                    #endregion

                    #region Gathering Profile Settings

                    bool gatherProfile = missionInfo.Attributes.HasFlag(MissionAttributes.Gather);
                    bool collectable = missionInfo.Attributes.HasFlag(MissionAttributes.Collectables) || missionInfo.Attributes.HasFlag(MissionAttributes.ReducedItems);

                    ImGui.TableNextColumn();
                    if (gatherProfile && !collectable)
                    {
                        string profileName = "???";
                        if (C.GatherProfiles.TryGetValue(missionConfig.GProfileId, out var profileSetting))
                        {
                            profileName = profileSetting.Name;
                        }
                        else
                        {
                            profileName = "???";
                        }

                        if (Table_CenteredButton($"{profileName}"))
                        {
                            ImGui.OpenPopup("Selecting Gathering Profile");
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text("选择要使用的配置");
                            ImGui.EndTooltip();
                        }
                        if (ImGui.BeginPopup("Selecting Gathering Profile"))
                        {
                            ImGui.Text($"当前已选择: {profileName}");
                            ImGui.Separator();

                            foreach (var profile in C.GatherProfiles)
                            {
                                var id = profile.Key;
                                bool profileSelected = missionConfig.GProfileId == id;
                                ImGui.PushID($"{id}_{profile.Value.Name}");
                                if (ImGui.RadioButton(profile.Value.Name, profileSelected))
                                {
                                    missionConfig.GProfileId = id;
                                    C.Save();
                                }
                                ImGui.PopID();
                            }

                            ImGui.EndPopup();
                        }
                    }
                    else if (gatherProfile && collectable)
                    {
                        Table_FullCenterText("自动");
                    }
                    else if (missionInfo.Attributes.HasFlag(MissionAttributes.Fish))
                    {
                        if (Table_CenteredButton($"选择配置"))
                        {
                            ImGui.OpenPopup("Select Fishing Profile");
                        }
                        if (ImGui.BeginPopup("Select Fishing Profile"))
                        {
                            ImGui.Text($"钓鱼配置: {missionInfo.Name}");
                            ImGui.Separator();
                            bool builtInPreset = missionConfig.Use_BuildinPreset;
                            if (ImGui.Checkbox("使用内置预设", ref builtInPreset))
                            {
                                missionConfig.Use_BuildinPreset = builtInPreset;
                                C.Save();
                            }
                            ImGuiEx.HelpMarker("启用此选项表示将使用插件中内置的 Autohook 默认预设。 \n" +
                                               "如果您希望使用自己在 Autohook 中已有的预设，可以取消勾选此项，并在下方输入预设名称。");
                            using (ImRaii.Disabled(builtInPreset))
                            {
                                string presetName = missionConfig.AutoHookPresetName;
                                ImGui.SetNextItemWidth(200);
                                if (ImGui.InputText("预设名称", ref presetName))
                                {
                                    missionConfig.AutoHookPresetName = presetName;
                                    C.Save();
                                }
                            }

                            ImGui.EndPopup();
                        }
                    }

                    #endregion

                    #region Notes

                    ImGui.TableNextColumn();
                    int notesCount = 0;

                    if (missionInfo.Attributes.HasFlag(MissionAttributes.ProvisionalTimed))
                    {
                        Table_FontCenter(FontAwesomeIcon.Clock);
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text($"{missionInfo.StartTime}:00 - {missionInfo.EndTime-1}:59");
                            ImGui.EndTooltip();
                        }
                        notesCount++;
                    }
                    if (missionInfo.Attributes.HasFlag(MissionAttributes.ProvisionalSequential))
                    {
                        Table_FontCenter(FontAwesomeIcon.ListOl);
                        if (ImGui.IsItemHovered())
                        {
                            var prevMissions = GetOnlyPreviousMissionsRecursive(Id);

                            ImGui.BeginTooltip();
                            ImGui.Text("连续任务");
                            ImGui.Separator();
                            for (int i = 0; i < prevMissions.Count; i++)
                            {
                                var prevMission = prevMissions[i];
                                ImGui.Text($"{i + 1}: [{prevMission}] - {CosmicHelper.SheetMissionDict[prevMission].Name}");
                            }
                            ImGui.EndTooltip();
                        }
                        notesCount++;
                    }
                    if (missionInfo.Attributes.HasFlag(MissionAttributes.ProvisionalWeather))
                    {
                        if (notesCount > 0)
                            ImGui.SameLine(0, 2);

                        var weather = missionInfo.Weather;
                        string weatherCN = CosmicWeatherCN.TryGetValue(weather, out var name) 
                            ? name 
                            : weather.ToString();

                        if (CosmicHelper.WeatherIds.ContainsKey(missionInfo.Weather))
                        {
                            ISharedImmediateTexture? weatherIcon = CosmicHelper.WeatherIconDict[missionInfo.Weather];
                            Vector2 ImageSize = new Vector2(23, 23);
                            ImGui.Image(weatherIcon.GetWrapOrEmpty().Handle, ImageSize);
                        }
                        else
                        {
                            Table_FontCenter(FontAwesomeIcon.Cloud);
                        }

                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text($"天气: {weatherCN}");
                            ImGui.EndTooltip();
                        }
                        notesCount++;
                    }
                    if (CosmicHelper.MissionUnlock.TryGetValue(Id, out var unlock))
                    {
                        if (notesCount > 0)
                            ImGui.SameLine(0, 2);

                        if (Svc.Texture.GetFromGame("ui/uld/WKSMission_hr1.tex") is { } tex)
                        {
                            if (tex.TryGetWrap(out var wrap, out var exc))
                            {
                                ImGui.Image(wrap.Handle, new Vector2(23, 23), new Vector2(0.2347f, 0.3500f), new Vector2(0.2959f, 0.6500f));
                            }
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text("需要以下任务达到金星评价后, 才能进行此任务");
                            foreach (var mission in unlock)
                            {
                                CompletionStatus_Normal(mission);
                                ImGui.SameLine();
                                ImGui.Text($"[{mission}] - {CosmicHelper.SheetMissionDict[mission].Name}");
                            }
                            ImGui.EndTooltip();
                        }
                        notesCount++;

                    }
                    if (missionInfo.Jobs.Count > 1)
                    {
                        if (notesCount > 0)
                            ImGui.SameLine(0, 2);

                        ISharedImmediateTexture? job1Icon = CosmicHelper.JobIconDict[missionInfo.Jobs.First()];
                        ISharedImmediateTexture? job2Icon = CosmicHelper.JobIconDict[missionInfo.Jobs.Last()];
                        Vector2 imageSize = new Vector2(23, 23);

                        ImGui.Image(job1Icon.GetWrapOrEmpty().Handle, imageSize);
                        ImGui.SameLine(0, 2);
                        ImGui.Image(job2Icon.GetWrapOrEmpty().Handle, imageSize);
                        notesCount++;
                    }
                    if (CosmicHelper.CustomMissionNotes.TryGetValue(Id, out var notes))
                    {
                        if (notesCount > 0)
                            ImGui.SameLine();
                        ImGuiEx.Icon(FontAwesomeIcon.Trophy);
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text(notes.NoteInfo);
                            ImGui.Text($"平均最佳每分钟技巧点: {notes.SPM:N2}");

                            ImGui.EndTooltip();
                        }
                    }

                    #endregion

                    ImGui.PopID();
                }

                ImGui.EndTable();
            }
        }

        public class ScoringInfo
        {
            public int Multipler { get; set; } = 1;
            public double Score { get; set; } = 0;
            public double Cosmocredits { get; set; } = 0;
            public double Planetcredits { get; set; } = 0;
            public int TotalCompleted { get; set; } = 0;
        }
        public static Dictionary<string, ScoringInfo> MissionScores = new()
        {
            ["Critical"] = new(),
            ["Bronze"] = new() { Multipler = 1 },
            ["Silver"] = new() { Multipler = 4 },
            ["Gold"] = new() { Multipler = 5 },
        };

        public static void DrawMissionDetails()
        {
            if (CosmicHelper.SheetMissionDict.TryGetValue(selectedMission, out var mission))
            {
                var id = selectedMission;
                ImGui.PushID($"{mission}_{id}");

                #region Mission Name

                ImGui.Text($"任务:");
                ImGui.SameLine(0, 5);
                ImGui.TextDisabled($"[{id}]");
                ImGui.SameLine(0, 5);
                ImGui.Text($"{mission.Name}");

                #endregion

                if (ImGui.BeginTable("Detailed Mission Info", 2, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Borders))
                {
                    ImGui.TableSetupColumn("Name");
                    ImGui.TableSetupColumn("Info");

                    // Row 1
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text("宇宙信用点");

                    ImGui.TableNextColumn();
                    ImGui.Text($"{mission.CosmoCredit}");

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text($"行星信用点");

                    ImGui.TableNextColumn();
                    ImGui.Text($"{mission.LunarCredit}");

                    if (mission.DronebitReward != 0)
                    {
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        if (Svc.Texture.TryGetFromGameIcon(65138, out var dronebitIcon))
                        {
                            ImGui.Image(dronebitIcon.GetWrapOrEmpty().Handle, new Vector2(24, 24));
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.Image(dronebitIcon.GetWrapOrEmpty().Handle, new Vector2(40, 40));
                                ImGui.EndTooltip();
                            }
                            ImGui.SameLine();
                        }
                        ImGui.AlignTextToFramePadding();
                        ImGui.Text($"无人机晶片");

                        ImGui.TableNextColumn();
                        ImGui.AlignTextToFramePadding();
                        ImGui.Text($"{mission.DronebitReward}");
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text($"职业技巧点");

                    ImGui.TableNextColumn();
                    ImGui.Text($"{mission.ClassScore}");

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.AlignTextToFramePadding();
                    ImGui.Text($"职业");

                    ImGui.TableNextColumn();
                    foreach (var job in mission.Jobs)
                    {
                        ISharedImmediateTexture? icon = CosmicHelper.JobIconDict[job];
                        Vector2 size = new Vector2(20, 20);
                        ImGui.Image(icon.GetWrapOrEmpty().Handle, size);
                        ImGui.SameLine();
                    }

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.AlignTextToFramePadding();
                    ImGui.Text($"完成情况");

                    ImGui.TableNextColumn();
                    CompletionStatus_Normal(selectedMission);

                    if (mission.BronzeScore != 0)
                    {
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.Text($"铜星需求");

                        ImGui.TableNextColumn();
                        ImGui.Text($"{mission.BronzeScore}");
                    }

                    if (mission.SilverScore != 0)
                    {
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.Text($"银星需求");

                        ImGui.TableNextColumn();
                        ImGui.Text($"{mission.SilverScore}");
                    }

                    if (mission.GoldScore != 0)
                    {
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.Text("金星需求");

                        ImGui.TableNextColumn();
                        ImGui.Text($"{mission.GoldScore}");
                    }

                    if (mission.MarkerId != 0)
                    {
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.Text("采集区域");

                        ImGui.TableNextColumn();

                        ImGui.PushFont(UiBuilder.IconFont);
                        ImGui.Text(FontAwesomeIcon.Flag.ToIconString());
                        ImGui.PopFont();
                        if (ImGui.IsItemClicked())
                        {
                            Utils.SetGatheringRing(mission.TerritoryId, (int)mission.MapPosition.X, (int)mission.MapPosition.Y, mission.Radius, mission.Name);
                        }
                    }

                    if (GatheringUtil.CriticalLocations.TryGetValue(selectedMission, out var criticalLoc))
                    {
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.Text("紧急探索区域");

                        ImGui.TableNextColumn();
                        ImGuiEx.Icon(FontAwesomeIcon.Flag);
                        if (ImGui.IsItemClicked())
                        {
                            Utils.SetFlagForNPC(mission.TerritoryId, criticalLoc.MapInfo.X, criticalLoc.MapInfo.Y);
                        }
                    }

                    ImGui.EndTable();
                }

                if (ImGui.BeginTable("Relic Exp Info Table", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit))
                {
                    ImGui.TableSetupColumn("研究数据类型");
                    ImGui.TableSetupColumn("数量");

                    ImGui.TableHeadersRow();

                    foreach (var xp in mission.RelicXpInfo.OrderByDescending(x => x.Key))
                    {
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        string type = "";
                        switch (xp.Key)
                        {
                            case 1:
                                type = "I";
                                break;
                            case 2:
                                type = "II";
                                break;
                            case 3:
                                type = "III";
                                break;
                            case 4:
                                type = "IV";
                                break;
                            case 5:
                                type = "V";
                                break;
                            case 6:
                                type = "VI";
                                break;
                            default:
                                type = "???";
                                break;
                        }

                        ImGui.Text($"Lv. {type}");
                        ImGui.TableNextColumn();
                        ImGui.Text($"{xp.Value}");
                    }

                    ImGui.EndTable();
                }

                if (mission.ExpModifier_3 != 0)
                {
                    if (ImGui.BeginTable("Exp Rewards", 2, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Borders))
                    {
                        ImGui.TableSetupColumn("职业经验值");
                        ImGui.TableSetupColumn("等级百分比 %");

                        ImGui.TableHeadersRow();

                        if (mission.ExpModifier_1 != 0)
                        {
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.Text("Lv. 10-49");

                            ImGui.TableNextColumn();
                            ImGui.Text($"{mission.ExpModifier_1}%");
                        }

                        if (mission.ExpModifier_2 != 0)
                        {
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.Text("Lv. 50-89");

                            ImGui.TableNextColumn();
                            ImGui.Text($"{mission.ExpModifier_2}%");
                        }

                        if (mission.ExpModifier_3 != 0)
                        {
                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.Text("Lv. 90-99");

                            ImGui.TableNextColumn();
                            ImGui.Text($"{mission.ExpModifier_3}%");
                        }

                        ImGui.EndTable();
                    }
                }

                if (mission.Crafts_Main.Count > 0)
                {
                    WindowSpacer();

                    var job = mission.Jobs.First(x => CosmicHelper.CrafterJobList.Contains(x));
                    ImGui.Text("配方详细信息");
                    var headerFlags = ImGuiTreeNodeFlags.None;
#if DEBUG
                    headerFlags = ImGuiTreeNodeFlags.DefaultOpen;
#endif

                    if (ImGui.CollapsingHeader("任务制作", headerFlags))
                    {
                        if (ImGui.BeginTable("Mission Craft Info", 5, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
                        {
                            ImGui.TableSetupColumn("物品");
                            ImGui.TableSetupColumn("难度");
                            ImGui.TableSetupColumn("最大品质");
                            ImGui.TableSetupColumn("耐久");
                            ImGui.TableSetupColumn("数量");

                            ImGui.TableHeadersRow();

                            ImGui.TableNextRow();
                            ImGui.TableSetColumnIndex(0);
                            ImGui.Text("成品");

                            foreach (var craft in mission.Crafts_Main)
                            {
                                if (Svc.Data.GetExcelSheet<Lumina.Excel.Sheets.Recipe>().TryGetRow(craft.Key, out var recipe))
                                {
                                    string itemName = recipe.ItemResult.Value.Name.ToString();
                                    var recipeInfo = CosmicHelper.SpecificRecipeInfo(job, craft.Key);

                                    ImGui.TableNextRow();
                                    ImGui.TableSetColumnIndex(0);
                                    ImGui.Text($"{itemName}");
                                    if (recipe.IsExpert)
                                    {
                                        ImGui.SameLine();
                                        ImGuiEx.Icon(new Vector4(1.0f, 0.4f, 0.0f, 1.0f), FontAwesomeIcon.Diamond);
                                        if (ImGui.IsItemHovered())
                                        {
                                            ImGui.SetTooltip("高难度配方");
                                        }
                                    }

                                    ImGui.TableNextColumn();
                                    ImGui.Text($"{recipeInfo.Progress}");

                                    ImGui.TableNextColumn();
                                    ImGui.Text($"{recipeInfo.Quality}");

                                    ImGui.TableNextColumn();
                                    ImGui.Text($"{recipeInfo.Durability}");

                                    ImGui.TableNextColumn();
                                    ImGui.Text($"{craft.Value.RequiredAmount}");
                                }
                            }

                            if (mission.Crafts_Pre.Count > 0)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableSetColumnIndex(0);
                                ImGui.Text("半成品");

                                foreach (var craft in mission.Crafts_Pre)
                                {
                                    if (Svc.Data.GetExcelSheet<Lumina.Excel.Sheets.Recipe>().TryGetRow(craft.Key, out var recipe))
                                    {
                                        string itemName = recipe.ItemResult.Value.Name.ToString();
                                        var recipeInfo = CosmicHelper.SpecificRecipeInfo(job, craft.Key);

                                        ImGui.TableNextRow();
                                        ImGui.TableSetColumnIndex(0);
                                        ImGui.Text($"{itemName}");
                                        if (recipe.IsExpert)
                                        {
                                            ImGui.SameLine();
                                            ImGuiEx.Icon(new Vector4(1.0f, 0.4f, 0.0f, 1.0f), FontAwesomeIcon.Diamond);
                                            if (ImGui.IsItemHovered())
                                            {
                                                ImGui.SetTooltip("高难度配方");
                                            }
                                        }

                                        ImGui.TableNextColumn();
                                        ImGui.Text($"{recipeInfo.Progress}");

                                        ImGui.TableNextColumn();
                                        ImGui.Text($"{recipeInfo.Quality}");

                                        ImGui.TableNextColumn();
                                        ImGui.Text($"{recipeInfo.Durability}");

                                        ImGui.TableNextColumn();
                                        ImGui.Text($"{craft.Value.RequiredAmount}");
                                    }
                                }
                            }

                            ImGui.EndTable();
                        }
                    }
                }

                WindowSpacer();

                ImGui.Text("任务属性");
                if (mission.Attributes == MissionAttributes.None)
                {
                    ImGui.Text("无");
                    return;
                }
                else
                {
                    foreach (MissionAttributes flag in Enum.GetValues<MissionAttributes>())
                    {
                        if (flag != MissionAttributes.None && mission.Attributes.HasFlag(flag))
                        {
                            ImGui.Text($"{EnumNameConverter(flag)}");
                        }
                    }
                }

                if (CosmicHelper.MissionUnlock.TryGetValue(selectedMission, out var unlock))
                {
                    ImGui.Text("需要以下任务达到金星评价后, 才能进行此任务");
                    foreach (var lockedMission in unlock)
                    {
                        CompletionStatus_Normal(lockedMission);
                        ImGui.SameLine();
                        ImGui.Text($"[{lockedMission}] - {CosmicHelper.SheetMissionDict[lockedMission].Name}");
                    }

                }

                WindowSpacer();
                ImGui.Text($"任务时间!");

                if (C.MissionConfig.TryGetValue(selectedMission, out var config))
                {
                    bool allowDelete = (ImGui.IsKeyDown(ImGuiKey.LeftShift) || ImGui.IsKeyDown(ImGuiKey.RightShift)) && (ImGui.IsKeyDown(ImGuiKey.LeftCtrl) || ImGui.IsKeyDown(ImGuiKey.RightCtrl));

                    using (ImRaii.Disabled(!allowDelete))
                    {
                        if (ImGui.Button("重置统计"))
                        {
                            P.MissionTimer.ResetTimers(selectedMission);
                        }
                    }
                    if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
                    {
                        ImGui.BeginTooltip();
                        ImGui.Text("按住 Shift + Ctrl");
                        ImGui.EndTooltip();
                    }

                    if (config.TurninRecords.Count > 0)
                    {
                        ImGui.Text($"最佳时间: {TimeSpan.FromSeconds(config.BestTime):mm\\:ss\\.ff}");
                        ImGui.Text($"平均时间: {TimeSpan.FromSeconds(config.AverageTime):mm\\:ss\\.ff}");
                    }
                    else
                    {
                        ImGui.Text("最佳时间: --:--:--");
                        ImGui.Text("平均时间: --:--:--");
                    }

                    ImGui.Text($"完成次数: {config.TotalCompletions}");
                    ImGui.Text($"超时放弃次数: {config.FailedCounters}");

                    if (CosmicHelper.SheetMissionDict.TryGetValue(selectedMission, out var missionInfo))
                    {
                        var baseScore = missionInfo.ClassScore;
                        var comsoCredit = missionInfo.CosmoCredit;
                        var planetCredit = missionInfo.LunarCredit;

                        ImGui.Separator();
                        ImGui.Text("预估每小时技巧点:");
                        ImGui.SameLine();
                        ImGui.TextDisabled("?");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text("这里的假设前提是:");
                            ImGui.Text("1: 你每次都能以完美的随机数运气拿到想要的任务");
                            ImGui.Text("2: 你每次都能达到评价阈值");
                            ImGui.Text("这些计算基于你的平均用时。 \n所以最好多跑几轮任务来得到更准确的时间。");
                            ImGui.EndTooltip();
                        }

                        bool ShowScorePerMinute = C.ShowSPM;
                        foreach (var kind in MissionScores)
                        {
                            var tier = kind.Key;
                            var info = kind.Value;

                            var averageTime = tier switch
                            {
                                "Critical" => config.AverageTime,
                                "Bronze" => config.AverageBronzeTime,
                                "Silver" => config.AverageSilverTime,
                                "Gold" => config.AverageGoldTime,
                                _ => 0
                            };

                            var totalComplete = tier switch
                            {
                                "Critical" => config.CriticalCompletions,
                                "Gold" => config.GoldCompletions,
                                "Silver" => config.SilverCompletions,
                                "Bronze" => config.BronzeCompletion,
                                _ => 0
                            };

                            info.Score = MissionStatsCalculator.CalculateCurrencyPerMinute(averageTime, baseScore, info.Multipler);
                            info.Cosmocredits = MissionStatsCalculator.CalculateCurrencyPerMinute(averageTime, comsoCredit, info.Multipler);
                            info.Planetcredits = MissionStatsCalculator.CalculateCurrencyPerMinute(averageTime, planetCredit, info.Multipler);
                            info.TotalCompleted = totalComplete;

                            if (!C.ShowSPM)
                            {
                                info.Score *= 60;
                                info.Cosmocredits *= 60;
                                info.Planetcredits *= 60;
                            }
                        }

                        if (mission.Attributes.HasFlag(MissionAttributes.Critical))
                        {
                            var criticalScore = MissionStatsCalculator.CalculateCurrencyPerMinute(config.AverageTime, baseScore, 1.0);
                            if (!C.ShowSPM)
                                criticalScore *= 60;

                            string showingX = ShowScorePerMinute ? "每分钟" : "每小时";
                            if (ImGui.Checkbox($"显示当前 {showingX} 技巧点", ref ShowScorePerMinute))
                            {
                                C.ShowSPM = ShowScorePerMinute;
                                C.Save();
                            }
                            if (ImGui.BeginTable("Critical Scoring Info", 4, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Borders))
                            {
                                ImGui.TableSetupColumn("汇报");
                                ImGui.TableSetupColumn("技巧点");
                                ImGui.TableSetupColumn("宇宙信用点");
                                ImGui.TableSetupColumn("行星信用点");

                                ImGui.TableHeadersRow();

                                var entry = MissionScores["Critical"];

                                ImGui.TableNextRow();
                                ImGui.TableSetColumnIndex(0);
                                ImGui.TextColored(new Vector4(1.0f, 0.84f, 0.0f, 1.0f), "紧急探索");

                                ImGui.TableNextColumn();
                                ImGui.Text($"{entry.Score:N2}");

                                ImGui.TableNextColumn();
                                ImGui.Text($"{entry.Cosmocredits:N2}");

                                ImGui.TableNextColumn();
                                ImGui.Text($"{entry.Planetcredits:N2}");

                                ImGui.EndTable();
                            }
                        }
                        else
                        {
                            List<(string type, Vector4 color)> turninTypes = new()
                            {
                                new() { type = "Bronze", color = new Vector4(0.8f, 0.5f, 0.3f, 1.0f)},
                                new() { type = "Silver", color = new Vector4(0.7f, 0.7f, 0.7f, 1.0f)},
                                new() { type = "Gold", color = new Vector4(1.0f, 0.84f, 0.0f, 1.0f)}
                            };
                            string showingX = ShowScorePerMinute ? "每分钟" : "每小时";
                            if (ImGui.Checkbox($"显示当前 {showingX} 技巧点", ref ShowScorePerMinute))
                            {
                                C.ShowSPM = ShowScorePerMinute;
                                C.Save();
                            }
                            if (ImGui.BeginTable("Critical Scoring Info", 4, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Borders))
                            {
                                ImGui.TableSetupColumn("汇报");
                                ImGui.TableSetupColumn("技巧点");
                                ImGui.TableSetupColumn("宇宙信用点");
                                ImGui.TableSetupColumn("行星信用点");

                                ImGui.TableHeadersRow();

                                // 映射
                                var typeNameMap = new Dictionary<string, string>
                                {
                                    ["Bronze"] = "铜星",
                                    ["Silver"] = "银星",
                                    ["Gold"] = "金星"
                                };

                                foreach (var type in turninTypes)
                                {

                                    var entry = MissionScores[type.type];
                                    ImGui.TableNextRow();
                                    ImGui.TableSetColumnIndex(0);

                                    string displayName = typeNameMap.TryGetValue(type.type, out var name) ? name : type.type;
                                    ImGui.TextColored(type.color, $"{displayName} [{entry.TotalCompleted}]");

                                    ImGui.TableNextColumn();
                                    ImGui.Text($"{entry.Score:N2}");

                                    ImGui.TableNextColumn();
                                    ImGui.Text($"{entry.Cosmocredits:N2}");

                                    ImGui.TableNextColumn();
                                    ImGui.Text($"{entry.Planetcredits:N2}");
                                }

                                ImGui.TableNextRow();
                                ImGui.TableSetColumnIndex(0);
                                ImGui.Text("平均");
                                ImGui.SameLine();
                                ImGui.TextDisabled("?");
                                if (ImGui.IsItemHovered())
                                {
                                    ImGui.SetTooltip("此数值基于您当前的铜星/银星/金星完成率计算\n" +
                                                     "它会计算你在所有任务中获得的平均技巧点, 并假设你在一小时内保持这个水平, 从而估算这一任务每小时获得的技巧点\n" +
                                                     "然后告诉单个任务对应的效率\n" +
                                                     "这是一种基于你的完成率, 用于计算更精确平均值的技术性方法");
                                }

                                ImGui.TableNextColumn();
                                var score = MissionStatsCalculator.CalculateActualScorePerMinute(config.TurninRecords, baseScore);
                                ImGui.Text($"{score:N2}");

                                ImGui.TableNextColumn();
                                var credits = MissionStatsCalculator.CalculateActualScorePerMinute(config.TurninRecords, comsoCredit);
                                ImGui.Text($"{credits:N2}");

                                ImGui.TableNextColumn();
                                var planet = MissionStatsCalculator.CalculateActualScorePerMinute(config.TurninRecords, planetCredit);
                                ImGui.Text($"{planet:N2}");



                                ImGui.EndTable();
                            }
                            /*
                            var ActualSPM = MissionStatsCalculator.CalculateActualScorePerMinute(config.TurninRecords, baseScore);
                            var bronzeScore = MissionStatsCalculator.CalculateCurrencyPerMinute(config.AverageBronzeTime, baseScore, 1.0);
                            var silverScore = MissionStatsCalculator.CalculateCurrencyPerMinute(config.AverageSilverTime, baseScore, 4.0);
                            var goldScore = MissionStatsCalculator.CalculateCurrencyPerMinute(config.AverageGoldTime, baseScore, 5.0);

                            // Convert to per-hour if needed
                            if (!C.ShowSPM)
                            {
                                ActualSPM *= 60;
                                bronzeScore *= 60;
                                silverScore *= 60;
                                goldScore *= 60;
                            }
                            string timeUnit = C.ShowSPM ? "每分钟" : "每小时";
                            string actualTimeUnit = C.ShowSPM ? "实际技巧点/分钟" : "实际技巧点/小时";

                            ImGui.Text($"实际{timeUnit}技巧点: {ActualSPM:F2}");
                            ImGui.SameLine();
                            ImGui.TextDisabled("?");
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.Text("此数值基于您当前的铜星/银星/金星完成率计算");
                                ImGui.Text("它会计算你在所有任务中获得的平均技巧点, 并假设你在一小时内保持这个水平, 从而估算这一任务每小时获得的技巧点");
                                ImGui.Text("这是一种基于你的完成率，用于计算更精确平均值的技术性方法。");
                                ImGui.EndTooltip();
                            }
                            if (missionInfo.Attributes.HasFlag(MissionAttributes.ProvisionalSequential))
                            {
                                string averageSequenceScore = $"平均每分钟技巧点 [连续任务]: {MissionStatsCalculator.CalculateAverageSequenceScorePerMinute(id, 5):N2}";
                                ImGui.Text(averageSequenceScore);
                                ImGui.SameLine();
                                ImGui.TextDisabled("?");
                                if (ImGui.IsItemHovered())
                                {
                                    ImGui.BeginTooltip();
                                    ImGui.Text("这里假设: 您在之前的所有任务中拿到了金星评价(因为这个任务必须要做)\n");
                                    ImGui.Text("这个提示主要是给考虑做连续任务的人, 方便他们计算平均值并与单纯刷普通任务进行比较\n");
                                    ImGui.Text("例如: 我发现某个连续任务比起单刷某个任务少了大约 30 技巧点/分钟\n(不使用食物/药水, 但也节省了资源)");
                                    ImGui.EndTooltip();
                                }
                            }

                            ImGui.TextColored(new Vector4(0.8f, 0.5f, 0.3f, 1.0f), $"铜星: {bronzeScore:F0} {timeUnit} [{config.BronzeCompletion}/{config.TotalCompletions}]");
                            ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1.0f), $"银星: {silverScore:F0} {timeUnit} [{config.SilverCompletions}/{config.TotalCompletions}]");
                            ImGui.TextColored(new Vector4(1.0f, 0.84f, 0.0f, 1.0f), $"金星: {goldScore:F0} {timeUnit} [{config.GoldCompletions}/{config.TotalCompletions}]");
#if DEBUG
                            var creditPerMinute = MissionStatsCalculator.CalculateCurrencyPerMinute(config.AverageGoldTime, mission.CosmoCredit, 5.0);
                            ImGui.Text($"Credit/minute: {creditPerMinute:N2}");
#endif
                            */
                        }
                    }


                    if (config.TurninRecords.Count > 0 && ImGui.CollapsingHeader("查看所有完成时间"))
                    {
                        for (int i = 0; i < config.TurninRecords.Count; i++)
                        {
                            var record = config.TurninRecords[i];

                            ImGui.Text($"[{i+1}] \u2192 {TimeSpan.FromSeconds(record.Time):mm\\:ss\\.ff}");
                            // Only draw the star when it's not None state
                            // to prevent layout issues with SameLine()
                            if (record.State != TurninState.None)
                            {
                                ImGui.SameLine();
                                DrawColoredStar(record.State);
                            }
                        }
                    }
                }

                ImGui.PopID();
            }
            else
            {
                string joke = JokeList[jokeId];
                ImGui.TextWrapped(joke);
            }
        }

        public static string EnumNameConverter(MissionAttributes attribute)
        {
            return attribute switch
            {
                MissionAttributes.Craft => "Crafting",
                MissionAttributes.Gather => "Gathering",
                MissionAttributes.Fish => "Fishing",
                MissionAttributes.Limited => "Limited Supplies",
                MissionAttributes.Collectables => "Collectable",
                MissionAttributes.ReducedItems => "Reducable Items",
                MissionAttributes.ExpertCraft => "Expert Crafts",
                MissionAttributes.ScoreTimeRemaining => "Timed Scoring",
                MissionAttributes.ScoreChains => "Chained Gather Scoring",
                MissionAttributes.ScoreGatherersBoon => "Gatherer's Boons Scoring",
                MissionAttributes.ScoreLargestSize => "Largest Fish Scored",
                MissionAttributes.ScoreVariety => "Variety of Fish Required",
                MissionAttributes.ScoreScore => "Mission Score Required",
                MissionAttributes.Critical => "Critical Mission",
                MissionAttributes.ProvisionalTimed => "Time Required",
                MissionAttributes.ProvisionalWeather => "Weather Required",
                MissionAttributes.ProvisionalSequential => "Sequential Missions Required",
                _ => attribute.ToString()
            };
        }

        #region Table Tools

        private static void WindowSpacer()
        {
            ImGui.Dummy(new Vector2(0, 5));
            ImGui.Separator();
            ImGui.Dummy(new Vector2(0, 5));
        }

        // Center a checkbox both horizontally and vertically in the current table cell
        private static bool Table_CenterCheckbox(string id, ref bool value)
        {
            var cursorPos = ImGui.GetCursorPos();
            var availWidth = ImGui.GetContentRegionAvail().X;

            var checkboxSize = ImGui.GetFrameHeight();
            ImGui.SetCursorPosX(cursorPos.X + (availWidth - checkboxSize) * 0.5f);
            ImGui.AlignTextToFramePadding();

            return ImGui.Checkbox($"##{id}", ref value);
        }
        private static void Table_VertCenterText(string text)
        {
            ImGui.AlignTextToFramePadding();
            ImGui.TextUnformatted(text);
        }
        private static void Table_FullCenterText(string text)
        {
            var cursorPosX = ImGui.GetCursorPosX();
            var availWidth = ImGui.GetContentRegionAvail().X;
            var textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX(cursorPosX + (availWidth - textWidth) * 0.5f);
            ImGui.AlignTextToFramePadding();

            ImGui.TextUnformatted(text);
        }
        private static void Table_FullCenterText(string icon, Vector4 color)
        {
            var cursorPosX = ImGui.GetCursorPosX();
            var availWidth = ImGui.GetContentRegionAvail().X;
            var textWidth = ImGui.CalcTextSize(icon).X;

            ImGui.SetCursorPosX(cursorPosX + (availWidth - textWidth) * 0.5f);
            ImGui.AlignTextToFramePadding();

            FontAwesome.Print(color, icon);
        }
        private static bool Table_CenteredButton(string label, Vector2? buttonSize = null)
        {
            var cursorPosX = ImGui.GetCursorPosX();
            var availWidth = ImGui.GetContentRegionAvail().X;

            Vector2 actualButtonSize;
            if (buttonSize.HasValue)
            {
                actualButtonSize = buttonSize.Value;
            }
            else
            {
                var textSize = ImGui.CalcTextSize(label);
                var framePadding = ImGui.GetStyle().FramePadding;
                actualButtonSize = new Vector2(textSize.X + framePadding.X * 2 + 10f, textSize.Y + framePadding.Y * 2);
            }

            ImGui.SetCursorPosX(cursorPosX + (availWidth - actualButtonSize.X) * 0.5f);
            return ImGui.Button(label, actualButtonSize);
        }

        private static void Table_FontCenter(FontAwesomeIcon icon)
        {
            ImGui.AlignTextToFramePadding();
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.Text(icon.ToIconString());
            ImGui.PopFont();
        }

        public static List<uint> GetOnlyPreviousMissionsRecursive(uint missionId)
        {
            if (!CosmicHelper.SheetMissionDict.TryGetValue(missionId, out var missionInfo) || missionInfo.PreviousMissions.Contains(0))
                return [];

            var chain = GetOnlyPreviousMissionsRecursive(missionInfo.PreviousMissions.First());
            chain.Add(missionInfo.PreviousMissions.First());
            return chain;
        }
        private static List<uint> GetOnlyNextMissionsRecursive(uint missionId)
        {
            uint? nextMissionId = CosmicHelper.SheetMissionDict
                .Where(m => m.Value.PreviousMissions.First() == missionId)
                .Select(m => (uint?)m.Key)
                .FirstOrDefault();

            if (!nextMissionId.HasValue)
                return [];

            var chain = new List<uint> { nextMissionId.Value };
            chain.AddRange(GetOnlyNextMissionsRecursive(nextMissionId.Value));
            return chain;
        }
        private static unsafe void CompletionStatus_Formatted(uint id)
        {
            var managerPtr = WKSManager.Instance();
            if (managerPtr == null) return;

            var manager = (WKSManagerCustom*)managerPtr;
            var isCompleted = manager->IsMissionCompleted(id);
            var isGold = manager->IsMissionGolded(id);

            float availableWidth = ImGui.GetContentRegionAvail().X;

            if (isCompleted)
            {
                if (isGold)
                {
                    // Center the image
                    float imageWidth = 23f;
                    float offsetX = (availableWidth - imageWidth) * 0.5f;

                    if (offsetX > 0)
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + offsetX);

                    if (Svc.Texture.GetFromGame("ui/uld/WKSMission_hr1.tex") is { } tex)
                    {
                        if (tex.TryGetWrap(out var wrap, out var exc))
                        {
                            var cursorPos = ImGui.GetCursorPos();
                            var availWidth = ImGui.GetContentRegionAvail().X;
                            var availHeight = ImGui.GetFrameHeight();

                            // Center horizontally
                            ImGui.SetCursorPosX(cursorPos.X + (availWidth - 23) * 0.5f);

                            // Center vertically
                            ImGui.SetCursorPosY(cursorPos.Y + (availHeight - 23) * 0.5f);
                            ImGui.Image(wrap.Handle, new Vector2(23, 23), new Vector2(0.2347f, 0.3500f), new Vector2(0.2959f, 0.6500f));
                        }
                    }
                }
                else
                {
                    ImGui.AlignTextToFramePadding();
                    Table_FullCenterText(FontAwesome.Check, EColor.Green);
                }
            }
            else
            {
                ImGui.AlignTextToFramePadding();
                Table_FullCenterText(FontAwesome.Cross, EColor.Red);
            }
        }
        private static unsafe void CompletionStatus_Normal(uint id)
        {
            var managerPtr = WKSManager.Instance();
            if (managerPtr == null) return;

            var manager = (WKSManagerCustom*)managerPtr;
            var isCompleted = manager->IsMissionCompleted(id);
            var isGold = manager->IsMissionGolded(id);

            var containerSize = new Vector2(23, 23);

            // Create a consistent container for all elements
            var cursorPos = ImGui.GetCursorPos();
            ImGui.InvisibleButton("##status_container", containerSize);
            ImGui.SetCursorPos(cursorPos);

            if (isCompleted)
            {
                if (isGold)
                {
                    if (Svc.Texture.GetFromGame("ui/uld/WKSMission_hr1.tex") is { } tex)
                    {
                        if (tex.TryGetWrap(out var wrap, out var exc))
                        {
                            ImGui.Image(wrap.Handle, containerSize, new Vector2(0.2347f, 0.3500f), new Vector2(0.2959f, 0.6500f));
                        }
                    }
                }
                else
                {
                    // Center the FontAwesome icon within the container
                    var textSize = ImGui.CalcTextSize(FontAwesome.Check);
                    var offset = (containerSize - textSize) * 0.5f;
                    offset += new Vector2(-2f, 1f);
                    ImGui.SetCursorPos(cursorPos + offset);
                    FontAwesome.Print(EColor.Green, FontAwesome.Check);
                }
            }
            else
            {
                var textSize = ImGui.CalcTextSize(FontAwesome.Cross);
                var offset = (containerSize - textSize) * 0.5f;
                offset += new Vector2(-2f, 1f);
                ImGui.SetCursorPos(cursorPos + offset);
                FontAwesome.Print(EColor.Red, FontAwesome.Cross);
            }

            // Reset cursor to after the container
            ImGui.SetCursorPos(cursorPos + new Vector2(containerSize.X, 0));
        }
        private static void UpdateSelectedMission(uint missionId)
        {
            if (ImGui.IsItemClicked())
            {
                selectedMission = missionId;
            }
        }
        private static void DrawColoredStar(TurninState state)
        {
            Vector4 color = state switch
            {
                TurninState.Bronze => new Vector4(0.8f, 0.5f, 0.3f, 1.0f),  // Bronze
                TurninState.Silver => new Vector4(0.75f, 0.75f, 0.75f, 1.0f), // Silver
                TurninState.Gold => new Vector4(1.0f, 0.84f, 0.0f, 1.0f),    // Gold
                TurninState.Critical => new Vector4(1.0f, 0.84f, 0.0f, 1.0f), // Gold
                _ => new Vector4(0, 0, 0, 0) // Transparent/none
            };

            if (state != TurninState.None)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, color);
                ImGui.PushFont(UiBuilder.IconFont); // Make sure you're using the icon font
                ImGui.Text(FontAwesomeIcon.Star.ToIconString());
                ImGui.PopFont();
                ImGui.PopStyleColor();
            }
        }

        #endregion
    }
}
