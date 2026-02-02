using Dalamud.Interface;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.Ui.DebugWindowTabs
{
    internal class Table_FishInfo
    {
        public static void Draw()
        {
            var fishMissions = CosmicHelper.SheetMissionDict.Where(x => x.Value.Jobs.Contains(18))
                .OrderBy(x => x.Key)
                .ToDictionary();

            if (ImGui.BeginTable("Fishing Info", 6, ImGuiTableFlags.RowBg | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Borders))
            {
                ImGui.TableSetupColumn("MissionID");
                ImGui.TableSetupColumn("Mission Name");
                ImGui.TableSetupColumn("Attribute");
                ImGui.TableSetupColumn("Specific");
                ImGui.TableSetupColumn("Total Req");
                ImGui.TableSetupColumn("Variety Req", ImGuiTableColumnFlags.WidthStretch);

                ImGui.TableHeadersRow();

                foreach (var mission in fishMissions)
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text($"{mission.Key}");

                    ImGui.TableNextColumn();
                    ImGui.Text($"{mission.Value.Name}");

                    ImGui.TableNextColumn();
                    ImGui.Text($"{mission.Value.Attributes}");

                    ImGui.TableNextColumn();
                    if (mission.Value.Gathering_Min.Count > 0)
                    {
                        ImGuiEx.Icon(FontAwesomeIcon.Fish);
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.BeginTooltip();

                            if (ImGui.BeginTable("Fish Item Info", 3, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders))
                            {
                                foreach (var fishItem in mission.Value.Gathering_Min)
                                {
                                    ImGui.TableNextRow();
                                    ImGui.TableSetColumnIndex(0);
                                    if (Svc.Data.GetExcelSheet<Item>().TryGetRow(fishItem.Key, out var fishInfo))
                                    {
                                        ImGui.Text($"{fishInfo.Name.ToString()}");
                                    }

                                    ImGui.TableNextColumn();
                                    ImGui.Text($"{fishItem.Value}");

                                    ImGui.TableNextColumn();
                                    ImGui.Text($"{fishItem.Key}");
                                }

                                ImGui.EndTable();
                            };

                            ImGui.EndTooltip();
                        }
                    }
                    else
                    {
                        ImGui.Text("-");
                    }

                    ImGui.TableNextColumn();
                    ImGui.Text($"{mission.Value.Fish_AmountRequired}");

                    ImGui.TableNextColumn();
                    ImGui.Text($"{mission.Value.Fish_VarietyAmount}");
                }

                ImGui.EndTable();
            }
        }
    }
}
