using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.Ui.DebugWindowTabs
{
    internal class Ui_Gearsets
    {
        public class GearsetInfo
        {
            public ushort Id { get; set; } = 0;
            public string Name { get; set; } = "";
            public uint JobId { get; set; } = 0;
        }

        public static List<GearsetInfo> CurrentGearsets = new();
        
        private static unsafe void UpdateGearsets()
        {
            var gsm = RaptureGearsetModule.Instance();
            if (gsm == null)
                return;

            CurrentGearsets.Clear();

            foreach (var gearset in gsm->Entries)
            {
                GearsetInfo newGs = new()
                {
                    Id = gearset.Id,
                    Name = gearset.NameString,
                    JobId = gearset.ClassJob,
                };
                CurrentGearsets.Add(newGs);
            }
        }

        public static void Draw()
        {
            if (ImGui.Button("Update Gearsets"))
            {
                UpdateGearsets();
            }

            if (ImGui.BeginTable("Gearset Viewer", 3, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders))
            {
                ImGui.TableSetupColumn("ID");
                ImGui.TableSetupColumn("Name");
                ImGui.TableSetupColumn("JobID");

                ImGui.TableHeadersRow();

                foreach (var entry in CurrentGearsets)
                {
                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text($"{entry.Id}");

                    ImGui.TableNextColumn();
                    ImGui.Text($"{entry.Name}");

                    ImGui.TableNextColumn();
                    ImGui.Text($"{entry.JobId}");
                }

                ImGui.EndTable();
            }
        }
    }
}
