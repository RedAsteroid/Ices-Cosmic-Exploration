using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Common.Lua;
using ICE.Utilities.Cosmic_Helper;
using System;
using System.Collections.Generic;
using System.Text;
using TerraFX.Interop.Windows;

namespace ICE.Ui.DebugWindowTabs
{
    internal class Ui_FishPresets
    {
        private static uint search_MissionId = 0;
        private static string search_MissionName = "";

        private static uint selectedMission = 0;

        public static void Draw()
        {
            if (ImGui.BeginTable("Fish Editor | Window Selector", 2, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit, ImGui.GetContentRegionAvail()))
            {
                ImGui.TableSetupColumn("Mission Selector");
                ImGui.TableSetupColumn("Mission Details", ImGuiTableColumnFlags.WidthStretch);

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.InputText("Search Name", ref search_MissionName, 100);
                ImGui.InputUInt("Search ID", ref search_MissionId);
                using (var missionSelection = ImRaii.Child("Mission Selection Child", new(300, ImGui.GetContentRegionAvail().Y)))
                {
                    ImGui.Separator();

                    MissionSelector();
                }

                ImGui.TableNextColumn();
                using (var missionDetails = ImRaii.Child("Mission Selection Info", ImGui.GetContentRegionAvail()))
                {
                    Mission_DetailedView();
                }

                ImGui.EndTable();
            }
        }

        private static void MissionSelector()
        {
            var list = CosmicHelper.SheetMissionDict.Where(x => x.Value.Jobs.Contains(18));

            if (search_MissionId != 0)
            {
                list = list.Where(x => x.Key == search_MissionId);
            }

            if (!string.IsNullOrEmpty(search_MissionName))
            {
                var searchTerm = search_MissionName.Trim().ToLowerInvariant();
                list = list.Where(x => x.Value.Name.ToLowerInvariant().Contains(searchTerm));
            }

            foreach (var mission in list)
            {
                bool isSelected = mission.Key == selectedMission;
                if (ImGui.Selectable($"[{mission.Key}] - {mission.Value.Name}##{mission.Key}", isSelected))
                {
                    selectedMission = mission.Key;
                }
            }
        }

        private static void Mission_DetailedView()
        {
            if (CosmicHelper.SheetMissionDict.TryGetValue(selectedMission, out var missionInfo))
            {
                if (ImGui.Button("Export All Presets"))
                {
                    var clipboard = ExportAllMissions();
                    ImGui.SetClipboardText(clipboard);
                }

                if (ImGui.Button("Export Selected Mission"))
                {
                    var clipboard = ExportSelected();
                    ImGui.SetClipboardText(clipboard);
                }

                ImGui.Text($"[{selectedMission}] {missionInfo.Name}");
                if (ImGui.Button("Import New Preset"))
                {
                    var clipboard = ImGui.GetClipboardText();
                    if (clipboard.StartsWith("AH4_"))
                    {
                        missionInfo.Fish_Presets.Add(clipboard);
                    }
                    else
                    {
                        IceLogging.Error("Not a valid autohook preset.\n" +
                                         "Expected to start with: AH4_\n" +
                                         $"Text: {clipboard}");
                    }
                }
                ImGui.SameLine(0, 10);
                if (ImGui.Button("Temp Set Presets"))
                {
                    P.AutoHook.DeleteAllAnonymousPresets();
                    foreach (var preset in missionInfo.Fish_Presets)
                    {
                        P.AutoHook.CreateAndSelectAnonymousPreset(preset);
                    }
                }
                if (ImGui.BeginTable("Autohook Presets", 3, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg))
                {
                    for (int i = 0; i < missionInfo.Fish_Presets.Count; i++)
                    {
                        var preset = missionInfo.Fish_Presets[i];
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.Text($"{i}");

                        ImGui.TableNextColumn();
                        ImGui.SetNextItemWidth(200);
                        ImGui.InputText($"##{i}_{preset}", ref preset, 1000);

                        ImGui.TableNextColumn();
                        if (ImGuiEx.IconButton(FontAwesomeIcon.Trash, $"##{i}"))
                        {
                            missionInfo.Fish_Presets.RemoveAt(i);
                        }
                    }

                    ImGui.EndTable();
                }
            }
            else
            {
                ImGui.Text($"No mission selected currently. Woops [{selectedMission}]");
            }
        }

        private static string ExportAllMissions()
        {
            var sb = new StringBuilder();
            foreach (var mission in CosmicHelper.SheetMissionDict.Where(x => x.Value.Jobs.Contains(18)))
            {
                sb.AppendLine($"\t\t[{mission.Key}] = new()");
                sb.AppendLine("\t\t{");

                foreach (var preset in mission.Value.Fish_Presets)
                {
                    sb.AppendLine($"\t\t\t\"{preset}\",");
                }

                sb.AppendLine("\t\t},");
            }

            return sb.ToString();
        }

        private static string ExportSelected()
        {
            var sb = new StringBuilder();
            if (CosmicHelper.SheetMissionDict.TryGetValue(selectedMission, out var mission))
            {
                sb.AppendLine($"\t\t[{selectedMission}] = new()");
                sb.AppendLine("\t\t{");

                foreach (var preset in mission.Fish_Presets)
                {
                    sb.AppendLine($"\t\t\t\"{preset}\",");
                }

                sb.AppendLine("\t\t},");
            }

            return sb.ToString();
        }
    }
}
