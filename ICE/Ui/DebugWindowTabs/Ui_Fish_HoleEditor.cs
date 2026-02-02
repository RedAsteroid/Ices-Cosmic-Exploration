using Dalamud.Interface.Utility.Raii;
using ECommons.Automation;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using ICE.Utilities.Cosmic_Helper;
using ICE.Utilities.GatheringHelper;
using Pictomancy;
using System.Collections.Generic;
using System.Text;
using static ICE.Utilities.GatheringHelper.GatheringUtil;

namespace ICE.Ui.DebugWindowTabs
{
    internal class Ui_Fish_HoleEditor
    {
        private static uint selectedZone = 0;
        private static Vector2 selectedFlag = Vector2.Zero;
        private static int selectedSpotIndex = -1;

        private static bool viewAllFishingSpots = false;
        private static bool viewNavSpot = false;
        private static List<Vector3> fishingPath = new();
        private static FishingDebug _fishingDebug = null;

        public static unsafe void Draw()
        {
            if (_fishingDebug == null)
            {
                _fishingDebug = new FishingDebug();
            }

            if (ImGui.Button("Add Missing Fishing Holes"))
            {
                foreach (var mission in CosmicHelper.SheetMissionDict.Where(x => x.Value.Jobs.Contains(18)))
                {
                    var zoneId = mission.Value.TerritoryId;
                    var flag = mission.Value.MapPosition;
                    if (GatheringUtil.MoonFishingLocations.TryGetValue(zoneId, out var moon))
                    {
                        if (!moon.ContainsKey(flag))
                        {
                            moon[flag] = new();
                        }
                    }
                    else
                    {
                        GatheringUtil.MoonFishingLocations[zoneId] = new()
                        {
                            [flag] = new()
                        };
                    }
                }
            }

            ImGui.SameLine();
            if (ImGui.Button("Export All Fishing Data"))
            {
                var exportData = ExportAllFishingData();
                ImGui.SetClipboardText(exportData);
                Svc.Chat.Print("All fishing data exported to clipboard!");
            }

            ImGui.SameLine();
            using (ImRaii.Disabled(selectedFlag == Vector2.Zero))
            {
                if (ImGui.Button("Export Selected Flag"))
                {
                    var exportData = ExportSingleFishingFlag(selectedZone, selectedFlag);
                    ImGui.SetClipboardText(exportData);
                    Svc.Chat.Print($"Fishing flag data for Zone {selectedZone} at ({selectedFlag.X}, {selectedFlag.Y}) exported to clipboard!");
                }
            }

            ImGui.Checkbox("Show fishing spot raycast", ref _fishingDebug.ShowFishRay);
            if (PlayerHelper.LocalPlayer is { } player && _fishingDebug.ShowFishRay)
            {
                _fishingDebug.Draw();
            }

            ImGui.Separator();

            if (ImGui.BeginTable("Fishing Editor Table", 2, ImGuiTableFlags.Resizable | ImGuiTableFlags.SizingFixedFit))
            {
                ImGui.TableSetupColumn("Fishing Hole Selector", ImGuiTableColumnFlags.WidthFixed, 200);
                ImGui.TableSetupColumn("Fishing Hole Editor", ImGuiTableColumnFlags.WidthStretch);

                ImGui.TableNextRow();

                // First Column, Viewer for all the routes
                ImGui.TableSetColumnIndex(0);
                if (ImGui.BeginChild("Fishing Location Selector", new Vector2(200, 0), true))
                {
                    foreach (var moon in GatheringUtil.MoonFishingLocations)
                    {
                        ImGui.Text($"Zone: {moon.Key}");
                        var sortedFlags = moon.Value.OrderBy(flag => flag.Key.X);
                        foreach (var flag in sortedFlags)
                        {
                            var isSelected = selectedZone == moon.Key && selectedFlag == flag.Key;

                            ImGui.PushID($"{moon.Key}_{flag.Key.X}_{flag.Key.Y}");

                            string mapPos = $"X: {flag.Key.X} Z: {flag.Key.Y}";
                            string label = isSelected ? $"→ {mapPos}" : $"{mapPos}";

                            if (ImGui.Selectable(label, isSelected))
                            {
                                selectedZone = moon.Key;
                                selectedFlag = flag.Key;
                                selectedSpotIndex = -1;
                            }

                            ImGui.PopID();
                        }
                    }
                }
                ImGui.EndChild();

                // Second Column, Editor for that route
                ImGui.TableNextColumn();
                if (ImGui.BeginChild("Fishing Hole Editor", new Vector2(0, 0), true))
                {
                    if (selectedZone != 0 && selectedFlag != Vector2.Zero)
                    {
                        if (ImGui.Button("Open map position"))
                        {
                            var missionEntry = CosmicHelper.SheetMissionDict.Where(x => x.Value.MapPosition == selectedFlag
                                                                                 && x.Value.TerritoryId == selectedZone).FirstOrDefault();
                            if (missionEntry.Key != 0)
                            {
                                var mission = missionEntry.Value;
                                Utils.SetGatheringRing(mission.TerritoryId, (int)mission.MapPosition.X, (int)mission.MapPosition.Y, mission.Radius, mission.Name);
                            }
                        }
                        if (ImGui.CollapsingHeader("All Missions for this hole"))
                        {
                            if (ImGui.BeginTable("Mission Viewer", 2, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Borders))
                            {
                                foreach (var mission in CosmicHelper.SheetMissionDict)
                                {
                                    if (!mission.Value.Jobs.Contains(18))
                                        continue;

                                    if (mission.Value.MapPosition != selectedFlag)
                                        continue;

                                    var key = mission.Key;

                                    ImGui.TableNextRow();
                                    ImGui.TableSetColumnIndex(0);
                                    ImGui.Text($"{key}");

                                    ImGui.TableNextColumn();
                                    var enabled = C.MissionConfig[key].Enabled;
                                    if (ImGui.Checkbox($"##{key}_mission{mission.Value.Name}", ref enabled))
                                    {
                                        C.MissionConfig[key].Enabled = enabled;
                                        C.SaveDebounced();
                                    }
                                }

                                ImGui.EndTable();
                            }
                        }

                        if (ImGui.Button("Move to Flag"))
                        {
                            Chat.SendMessage("/vnav moveflag");
                        }
                        ImGui.SameLine();
                        if (ImGui.Button("Stop naving"))
                        {
                            P.Navmesh.Stop();
                        }
                        ImGui.SameLine();
                        if (ImGui.Button("Mount"))
                        {
                            Utils.MountAction();
                        }
                        ImGui.Text($"Viable fishing spot: {_fishingDebug.IsFishable()}");
                        if (_fishingDebug.FindFishableLocation(out var fishablePosition))
                        {
                            ImGui.Text($"First Available Fishing Spot: {fishablePosition.Value.X:N2}, {fishablePosition.Value.Y:N2}, {fishablePosition.Value.Z:N2}");
                            if (ImGui.Button("Face toward spot"))
                            {
                                if (_fishingDebug.FindFishableLocation(out var fishPosition, searchSteps: 128))
                                {
                                    P.TaskManager.Enqueue(() => Task_Fishing.FacePosition(fishPosition.Value));
                                }
                            }
                        }

                        ImGui.Checkbox("View Fishing Spots", ref viewAllFishingSpots);
                        ImGui.SameLine();
                        Vector4 circleColor = Utils.FromUintABGR(C.PictoColor_Circle);
                        ImGui.SetNextItemWidth(200);
                        if (ImGui.ColorEdit4("##CircleColorEditor", ref circleColor))
                        {
                            C.PictoColor_Circle = Utils.ToUintABGR(circleColor);
                            C.Save();
                        }

                        ImGui.Checkbox("View Nav Spots", ref viewNavSpot);
                        ImGui.SameLine();
                        Vector4 dotColor = Utils.FromUintABGR(C.PictoColor_Dot);
                        ImGui.SetNextItemWidth(200);
                        if (ImGui.ColorEdit4("##DotColorEditor", ref dotColor))
                        {
                            C.PictoColor_Dot = Utils.ToUintABGR(dotColor);
                            C.Save();
                        }

                        Vector4 coneColor = Utils.FromUintABGR(C.PictoColor_Cone);
                        ImGui.SetNextItemWidth(200);
                        if (ImGui.ColorEdit4("Cone Color Editor##ConeColorEditor", ref coneColor))
                        {
                            C.PictoColor_Cone = Utils.ToUintABGR(coneColor);
                            C.Save();
                        }

                        var fishingHole = GatheringUtil.MoonFishingLocations[selectedZone][selectedFlag];

                        ImGui.Text($"Zone {selectedZone} - X:{selectedFlag.X} Z:{selectedFlag.Y}");

                        if (ImGui.Button("Add Fishing Spot"))
                        {
                            fishingHole.Add(new FisherSpotInfo()
                            {
                                FishingSpot = Player.Position,
                                FacePosition = GetPositionInFrontOfPlayer(Player.Position, Player.Rotation)
                            });
                        }

                        ImGui.Separator();

                        // Simple list of spots
                        for (int i = 0; i < fishingHole.Count; i++)
                        {
                            ImGui.PushID(i);

                            bool isSelected = selectedSpotIndex == i;
                            if (ImGui.Selectable($"Spot {i + 1}", isSelected))
                            {
                                selectedSpotIndex = i;
                            }
                            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right) && ImGui.IsItemHovered())
                            {
                                ImGui.OpenPopup("Option to Delete");
                            }
                            if (ImGui.BeginPopup("Option to Delete"))
                            {
                                if (ImGui.MenuItem("Delete"))
                                {
                                    fishingHole.RemoveAt(i);
                                    if (selectedSpotIndex >= i) selectedSpotIndex--;
                                }
                                ImGui.EndPopup();
                            }

                            ImGui.PopID();
                        }

                        // Edit selected spot
                        if (selectedSpotIndex >= 0 && selectedSpotIndex < fishingHole.Count)
                        {
                            ImGui.Separator();
                            ImGui.Text($"Editing Spot {selectedSpotIndex + 1}:");

                            var spot = fishingHole[selectedSpotIndex];

                            var fish = spot.FishingSpot;
                            ImGui.SetNextItemWidth(200);
                            if (ImGui.InputFloat3("Fishing Position", ref fish))
                            {
                                spot.FishingSpot = fish;
                            }
                            ImGui.SameLine();
                            if (ImGui.Button("Set fishing to current"))
                            {
                                spot.FishingSpot = Player.Position;
                            }

                            var nav = spot.FacePosition;
                            ImGui.SetNextItemWidth(200);
                            if (ImGui.InputFloat3("Nav Position", ref nav))
                            {
                                spot.FacePosition = nav;
                            }
                            ImGui.SameLine();
                            if (ImGui.Button("Set Fishing Rotation"))
                            {
                                var currentRotation = Player.Rotation;
                                spot.FacePosition = GetPositionInFrontOfPlayer(Player.Position, currentRotation);
                            }
                            ImGui.SameLine();
                            ImGui.Text($"{Player.Rotation}");
                            ImGui.SetNextItemWidth(200);
                            float toleranceDegrees = spot.RotationTolerance * (180f / (float)Math.PI);
                            if (ImGui.SliderFloat("Rotation Tolerance (degrees)", ref toleranceDegrees, 1f, 45f))
                            {
                                spot.RotationTolerance = toleranceDegrees * ((float)Math.PI / 180f);
                            }

                            if (ImGui.Button("Test Naving to [New]"))
                            {
                                P.TaskManager.Enqueue(() => Task_NavmeshMove.Task_NavTo(spot.FishingSpot));
                                P.TaskManager.EnqueueDelay(200);
                                if (_fishingDebug.FindFishableLocation(out var fisablePosition, searchSteps: 128))
                                {
                                    P.TaskManager.Enqueue(() => Task_Fishing.FacePosition(fishablePosition.Value));
                                }
                            }
                        }

                        uint holeNumber = 1;

                        using (var drawList = PictoService.Draw(hints: Utils.GetPictoHints()))
                        {
                            if (viewAllFishingSpots)
                            {
                                foreach (var location in fishingHole)
                                {
                                    PictoService.VfxRenderer.AddCircle($"Location: {location.FishingSpot.X}", location.FishingSpot, 1f, Utils.FromUintABGR(C.PictoColor_Circle));

                                    var floatPos = new Vector3(location.FishingSpot.X, location.FishingSpot.Y + 3.5f, location.FishingSpot.Z);
                                    drawList.AddText(floatPos, 2667577343, $"Spot #: {holeNumber}", 1);
                                    holeNumber+= 1;
                                }
                            }
                            if (viewNavSpot)
                            {
                                foreach (var navPoint in fishingHole)
                                {
                                    drawList.AddDot(navPoint.FishingSpot, 5f, C.PictoColor_Dot);
                                    drawList.AddDot(navPoint.FacePosition, 5f, C.PictoColor_Dot);
                                    drawList.AddLine(navPoint.FishingSpot, navPoint.FacePosition, 0f, C.PictoColor_Circle);
                                    Vector3 direction = navPoint.FacePosition - navPoint.FishingSpot;
                                    float rotation = (float)Math.Atan2(direction.Z, direction.X) - (float)(Math.PI / 2);

                                    float coneDistance = Vector3.Distance(navPoint.FishingSpot, navPoint.FacePosition);
                                    drawList.AddConeFilled(
                                        navPoint.FishingSpot,
                                        coneDistance,
                                        rotation,
                                        navPoint.RotationTolerance * 2f,
                                        C.PictoColor_Cone
                                    );

                                }
                            }
                        }
                    }
                }
                ImGui.EndChild();

                ImGui.EndTable();
            }
        }

        private static string ExportAllFishingData()
        {
            var sb = new StringBuilder();
            sb.AppendLine("public static Dictionary<uint, Dictionary<Vector2, List<FisherSpotInfo>>> MoonFishingLocations = new()");
            sb.AppendLine("{");

            // Sort zones by key
            foreach (var zone in GatheringUtil.MoonFishingLocations.OrderBy(z => z.Key))
            {
                sb.AppendLine($"\t[{zone.Key}] = new()");
                sb.AppendLine("\t{");

                // Sort flags by X coordinate (west to east)
                var sortedFlags = zone.Value.OrderBy(flag => flag.Key.X);

                foreach (var flag in sortedFlags)
                {
                    sb.AppendLine($"\t\t[new Vector2({flag.Key.X}f, {flag.Key.Y}f)] = new()");
                    sb.AppendLine("\t\t{");

                    foreach (var spot in flag.Value)
                    {
                        sb.AppendLine("\t\t\tnew FisherSpotInfo()");
                        sb.AppendLine("\t\t\t{");
                        sb.AppendLine($"\t\t\t\tNavtoSpot = new Vector3({spot.FacePosition.X:F2}f, {spot.FacePosition.Y:F2}f, {spot.FacePosition.Z:F2}f),");
                        sb.AppendLine($"\t\t\t\tFishingSpot = new Vector3({spot.FishingSpot.X:F2}f, {spot.FishingSpot.Y:F2}f, {spot.FishingSpot.Z:F2}f),");
                        sb.AppendLine("\t\t\t},");
                    }

                    sb.AppendLine("\t\t},");
                }

                sb.AppendLine("\t},");
            }

            sb.AppendLine("};");
            return sb.ToString();
        }

        private static string ExportSingleFishingFlag(uint zoneId, Vector2 flag)
        {
            if (!GatheringUtil.MoonFishingLocations.TryGetValue(zoneId, out var zone) ||
                !zone.TryGetValue(flag, out var spots))
            {
                return "// No fishing data found for the selected flag";
            }

            var sb = new StringBuilder();
            sb.AppendLine($"// Export for Fishing Zone {zoneId}, Flag ({flag.X}, {flag.Y})");
            sb.AppendLine($"[new Vector2({flag.X}f, {flag.Y}f)] = new()");
            sb.AppendLine("\t\t\t{");

            foreach (var spot in spots)
            {
                sb.AppendLine("\t\t\t\tnew FisherSpotInfo()");
                sb.AppendLine("\t\t\t\t{");
                sb.AppendLine($"\t\t\t\t\tFacePosition = new Vector3({spot.FacePosition.X:F2}f, {spot.FacePosition.Y:F2}f, {spot.FacePosition.Z:F2}f),");
                sb.AppendLine($"\t\t\t\t\tFishingSpot = new Vector3({spot.FishingSpot.X:F2}f, {spot.FishingSpot.Y:F2}f, {spot.FishingSpot.Z:F2}f),");
                sb.AppendLine("\t\t\t\t},");
            }

            sb.AppendLine("\t\t\t},");
            return sb.ToString();
        }

        public static Vector3 GetPositionInFrontOfPlayer(Vector3 currentPosition, float rotationRadians, float distance = 2f)
        {
            // No conversion needed - already in radians!
            Vector3 direction = new Vector3(
                (float)Math.Sin(rotationRadians),
                0,
                (float)Math.Cos(rotationRadians)
            );

            return currentPosition + (direction * distance);
        }

        private static bool? StartNavmesh(Vector3 moveTo)
        {
            if (P.Navmesh.IsRunning())
                return true;
            else
            {
                if (EzThrottler.Throttle("Starting Test Navmesh"))
                {
                    IceLogging.DestinationLogs.Log(moveTo);
                    P.Navmesh.PathfindAndMoveTo(moveTo, false);
                }
                return false;
            }
        }

        private static bool? TestPathV2()
        {
            if (!P.Navmesh.IsRunning())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static float GetShortestAngleDifference(float currentAngle, float targetAngle)
        {
            float difference = targetAngle - currentAngle;

            // Normalize to [-π, π] for shortest path
            while (difference > Math.PI) difference -= (float)(2 * Math.PI);
            while (difference < -Math.PI) difference += (float)(2 * Math.PI);

            return difference;
        }
    }
}