using ICE.Utilities.Cosmic_Helper;

namespace ICE.Ui.MainUi.Settings.Settings_Table;

public static class Settings_TableColumns
{
    private static string[] missionSortOptions = ["ID", "任务名称", "宇宙信用点", "行星信用点", "研究数据 I", "研究数据 II", "研究数据 III", "研究数据 IV", "研究数据 V", "研究数据 VI", "地图位置", "职业技巧点", "职业经验值"];

    public static void ColumnSettings()
    {
        int missionSelectedOption = C.TableSortOption;
        if (ImGui.BeginCombo("排序方式", missionSortOptions[missionSelectedOption]))
        {
            for (int i = 0; i < missionSortOptions.Length; i++)
            {
                bool isSelected = (i == missionSelectedOption);
                if (ImGui.Selectable(missionSortOptions[i], isSelected))
                {
                    missionSelectedOption = i;
                }
                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
                if (missionSelectedOption != C.TableSortOption)
                {
                    C.TableSortOption = missionSelectedOption;
                    C.Save();
                }
            }
            ImGui.EndCombo();
        }

        bool hideUnsupported = C.HideUnsupportedMissions;
        if (ImGui.Checkbox("隐藏不支持的任务", ref hideUnsupported))
        {
            C.HideUnsupportedMissions = hideUnsupported;
            C.Save();
        }

        bool grindAllProvisionals = C.GrindAllProvisionals;
        if (ImGui.Checkbox("允许执行所有临时性任务", ref grindAllProvisionals))
        {
            C.GrindAllProvisionals = grindAllProvisionals;
            C.Save();
        }
        ImGuiEx.HelpMarker("启用后, 将显示所有可刷取的 天气限定/时间限定/连续 任务。\n" + // Enabling this will show you all weather/timed/sequence missions that you can grind, 
                           "这些任务会在当前选定职业的普通任务之外额外显示。\n" + // ON TOP OF doing the normal missions for whichever class you start on.
                           "如果您只想专注于一个职业进行任务, 请禁用此选项。\n\n" + // If you just want to focus one specific class, set this to false
                           "请注意: 此选项实际取代了原先的\"临时性任务模式\", 因为现在已经内置到标准模式中了(终于)。"); // Do note: this replaced provisional grinding, due to just being built into the standard mode now (finally)

        bool showExtraInfo = C.ShowExtraMissionInfo;
        if (ImGui.Checkbox("显示额外任务信息侧边窗口", ref showExtraInfo))
        {
            C.ShowExtraMissionInfo = showExtraInfo;
            C.Save();
        }

        bool autoShowToken = C.Auto_ShowTokens;
        if (ImGui.Checkbox("自动显示/隐藏行星票据", ref autoShowToken))
        {
            C.Auto_ShowTokens = autoShowToken;
            C.Save();
        }

        bool showManualMode = C.ShowManualMode;
        if (ImGui.Checkbox("显示手动模式表格列", ref showManualMode))
        {
            C.ShowManualMode = showManualMode;
            if (!showManualMode)
            {
                foreach (var mission in C.MissionConfig)
                {
                    mission.Value.ManualMode = false;
                }
            }
            C.Save();
        }
        ImGuiEx.HelpMarker("只在您打算亲自完成任务, 而不是依靠插件自动化完成时, 才需要启用此选项。\n" +
                           "另外, 如果您使用其他插件来处理汇报、制作、采集等自动化操作, 并且不希望 I.C.E. 与这些插件交互, 也可以启用此选项。");
    }

    private static bool ApplyToAllClasses = true;
    private static bool ApplyToSpecicClass = false;
    private static int SpecificClass = 8;
    private static int selectedClassIndex = 0;

    private static readonly string[] classOptions = new[]
    {
        "刻木匠",      // 0
        "锻铁匠",     // 1
        "铸甲匠",        // 2
        "雕金匠",      // 3
        "制革匠",  // 4
        "裁衣匠",         // 5
        "炼金术士",      // 6
        "烹调师",     // 7
        "采矿工",          // 8
        "园艺工",       // 9
        "捕鱼人"          // 10
    };

    private static readonly int[] classIds = new[]
    {
        8,  // Carpenter
        9,  // Blacksmith
        10, // Armorer
        11, // Goldsmith
        12, // Leatherworker
        13, // Weaver
        14, // Alchemist
        15, // Culinarian
        16, // Miner
        17, // Botanist
        18  // Fisher
    };

    private static bool AnyTurnin = true;
    private static bool TurninGold = false;
    private static bool TurninSilver = false;
    private static bool TurninBronze = false;

    public static void GeneralMissionSettings()
    {
        // Todo: 上游删除并移动到 Debug 模块, 由于中国服务器可能存在插件联动需求, 暂时保留此选项
        bool onlyGrabMission = C.OnlyGrabMission_Debug;
        if (ImGui.Checkbox($"只刷取任务", ref onlyGrabMission))
        {
            C.OnlyGrabMission_Debug = onlyGrabMission;
            C.Save();
        }
        ImGui.SameLine();
        ImGuiEx.HelpMarker("启用后, 将在刷取到目标任务后以手动模式运行。\n如果您没有特殊需求, 请不要启用。");

        bool removeGold = C.RemoveAfterGold;
        if (ImGui.Checkbox("金星完成时移除任务", ref removeGold))
        {
            C.RemoveAfterGold = removeGold;
            C.Save();
        }

        ImGui.Checkbox("当前任务结束后停止", ref Mission_Settings.StopAfterCurrent);
        bool relicTurnin = C.TurninRelic;
        if (ImGui.Checkbox($"宇宙工具可报告时提交##RelicTurnin_GeneralSetting", ref relicTurnin))
        {
            C.TurninRelic = relicTurnin;
            C.Save();
        }
        ImGui.SameLine();
        ImGui.TextDisabled("?");
        if (ImGui.IsItemHovered())
        {
            ImGui.SetTooltip("这是此功能的工作方式说明。如果我未来修改了这个功能，这个提示也会随之改变。\n" +
                             "1: 此功能将检查你的当前职业 [不是菜单中选择的职业, 是实际当前职业] 提交宇宙工具。\n" +
                             "2: 此功能的优先级高于 \"宇宙工具可报告时停止\" 选项，如果两个都启用, 它会选择提交宇宙工具而不是停止, 并继续执行任务。\n" +
                             "3: 如果你当前是能工巧匠职业, 报告后将前往设置好的制作返回点位置。\n" +
                             "\t- 这是可选的, 你可以自由关闭。我个人喜欢这样设置, 方便我回到自己选定的安静区域。");
        }
        if (ImGui.Button("快捷汇报模式应用")) // Quick Apply Turnins
        {
            ImGui.OpenPopup("Quick Apply_Mission Turnins");
        }

        if (ImGui.BeginPopup("Quick Apply_Mission Turnins"))
        {
            if (ImGui.RadioButton("应用到全部职业", ApplyToAllClasses)) // Apply to all classes
            {
                ApplyToAllClasses = true;
                ApplyToSpecicClass = false;
            }

            if (ImGui.RadioButton("应用到指定职业", ApplyToSpecicClass)) // Apply to specific class
            {
                ApplyToAllClasses = false;
                ApplyToSpecicClass = true;
            }
            if (ImGui.Combo("##ClassSelector", ref selectedClassIndex, classOptions, classOptions.Length))
            {
                // Update SpecificClass when selection changes
                SpecificClass = classIds[selectedClassIndex];
                IceLogging.Debug($"Selected class: {classOptions[selectedClassIndex]}, ID: {SpecificClass}");
            }
            ImGui.Separator();
            ImGui.Text("选择汇报选项");
            ImGui.Dummy(new Vector2(0, 2));

            if (ImGui.Checkbox("自动", ref AnyTurnin))
            {
                if (AnyTurnin)
                {
                    TurninGold = false;
                    TurninSilver = false;
                    TurninBronze = false;

                    AnyTurnin = true;
                }
                else
                {
                    if (!(TurninBronze && TurninSilver && TurninGold))
                    {
                        AnyTurnin = true;
                    }
                }

                C.Save();
            }
            ImGuiEx.HelpMarker("此选项将尽力获得最佳结果, 但在必要时也会汇报任意结果而避免中止。");

            ImGui.Separator();

            if (ImGui.Checkbox("金星", ref TurninGold))
            {
                if (AnyTurnin && TurninGold)
                    AnyTurnin = false;

            }
            if (ImGui.Checkbox("银星", ref TurninSilver))
            {
                if (AnyTurnin && TurninSilver)
                    AnyTurnin = false;

            }
            if (ImGui.Checkbox("铜星", ref TurninBronze))
            {
                if (AnyTurnin && TurninBronze)
                    AnyTurnin = false;

            }

            if (!AnyTurnin && !TurninGold && !TurninSilver && !TurninBronze)
                AnyTurnin = true;

            ImGui.Separator();

            if (ImGui.Button("应用"))
            {
                var amountApplied = 0;
                foreach (var mission in C.MissionConfig)
                {
                    if (CosmicHelper.SheetMissionDict.TryGetValue(mission.Key, out var sheetInfo))
                    {
                        if (ApplyToSpecicClass && !sheetInfo.Jobs.Contains((uint)SpecificClass))
                            continue;

                        if (sheetInfo.Attributes.HasFlag(MissionAttributes.ScoreTimeRemaining))
                            continue;

                        if (C.MissionConfig.TryGetValue(mission.Key, out var config))
                        {
                            config.AutoTurnin = AnyTurnin;
                            config.TurninGold = TurninGold;
                            config.TurninSilver = TurninSilver;
                            config.TurninBronze = TurninBronze;
                        }
                        amountApplied += 1;
                    }
                }
                C.SaveDebounced();

                Notify.Success($"已应用设置到 {amountApplied} 个任务\n专门为你做的, 兄弟。");
                ImGui.CloseCurrentPopup();
            }


            ImGui.EndPopup();
        }
    }
}
