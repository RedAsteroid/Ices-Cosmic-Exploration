using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ECommons.GameHelpers;
using ICE.Ui.MainUi.ModeSelect;
using ICE.Utilities.ImGuiTools;
using SharpDX.Direct2D1.Effects;
using System.Collections.Generic;
using System.Reflection;

namespace ICE.Ui.MainUi
{
    internal class SelectableSidebar
    {
        public static Dictionary<string, bool> categoryStates = new Dictionary<string, bool>();

        private static string PluginIcon = "ICE.Resources.Icon.png";
        public static string currentSelection = "modeSelect_Standard";

        public static void Draw()
        {
            var scale = ImGuiHelpers.GlobalScaleSafe;
            int baseSize = 200;
            var scaledWidth = baseSize * scale;

            if (ImGui.BeginChild("MainUi_Sidebar", new Vector2(scaledWidth, -1), true))
            {
                // Image/Icon of the plugin
                var pluginIcon = Svc.Texture.GetFromManifestResource(Assembly.GetExecutingAssembly(), PluginIcon).GetWrapOrEmpty();

                if (pluginIcon != null)
                {
                    // Getting the image size via the texture wrap (using the * after to manipulate the size cause, she big)
                    Vector2 imageSize = new Vector2(pluginIcon.Width, pluginIcon.Height) * 0.35f;

                    // Calculate the offset/centering here
                    float sidebarWidth = ImGui.GetContentRegionAvail().X;
                    float offsetX = (sidebarWidth - imageSize.X) / 2.0f;

                    // Center and drawing the image now
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + offsetX);
                    ImGui.Image(pluginIcon.Handle, imageSize);
                    if (ImGui.IsItemClicked())
                    {
                        var random = new Random();
                        modeSelect_TableInfo.jokeId = random.Next(0, modeSelect_TableInfo.JokeList.Count-1);
                        modeSelect_TableInfo.selectedMission = 0;
                    }

                    // Add spacing after image
                    ImGui.Dummy(new Vector2(0, 10));
                    ImGui.Separator();
                    ImGui.Dummy(new Vector2(0, 10));
                }

                if (ImGui_Tools.DrawCategoryHeader_AutoSize("宇宙助手", icon: FontAwesomeIcon.ListAlt)) // Cosmic Helper
                {
                    DrawSelectableWithIcon(FontAwesomeIcon.List, "标准", "modeSelect_Standard"); // Standard
                    DrawSelectableWithIcon(FontAwesomeIcon.Trophy, "完成情况", "modeSelect_Completion"); // Completion
                }
                if (ImGui_Tools.DrawCategoryHeader_AutoSize("设置", icon: FontAwesomeIcon.Cog)) // Settings
                {
                    if (C.Show_StopWhen)
                        DrawSelectableWithIcon(FontAwesomeIcon.Stop, "停止条件", "setting_StopWhen"); // Stop When...
                    if (C.Show_GatheringProfile)
                        DrawSelectableWithIcon(FontAwesomeIcon.Leaf, "采集配置", "setting_GatheringProfile"); // Gathering Profile
                    if (C.Show_MissionPriority)
                        DrawSelectableWithIcon(FontAwesomeIcon.SortAmountUp, "任务优先级", "setting_MissionPriority"); // Mission Priority
                    if (C.Show_MiscSettings)
                        DrawSelectableWithIcon(FontAwesomeIcon.UserCog, "杂项设置", "setting_Misc"); // Misc Settings

                    DrawSelectableWithIcon(FontAwesomeIcon.Cog, "所有设置", "helpSelect_AllSettings"); // All Settings
                }
                if (C.Show_HubActivities)
                {
                    if (ImGui_Tools.DrawCategoryHeader_AutoSize("基地活动", icon: FontAwesomeIcon.Home)) // Hub Activities
                    {
                        DrawSelectableWithImage(65112, "信用点购物", "hubActivities_CreditShopping");
                        DrawSelectableWithImage(65127, "宇宙好运道设置", "hubActivites_GambaSetting");
                        DrawSelectableWithImage(65138, "无人机晶片购物", "hubActivies_DroneSetting");
                    }
                }
                var currentJob = C.SelectedJob;
                if (ImGui_Tools.DrawCategoryHeader_AutoSize("地图选择", FontAwesomeIcon.Moon)) // Moon Selection
                {
                    string SinusAsset = "ICE.Resources.Sinus_Ardorum.png";
                    string PhaennaAsset = "ICE.Resources.Phaenna.png";
                    string OizysAsset = "ICE.Resources.Oizys.png";

                    bool autoSelectMoon = C.AutoSelectMoon;
                    if (ImGui.Checkbox("自动选择地图", ref autoSelectMoon)) // Auto Select Moon
                    {
                        C.AutoSelectMoon = autoSelectMoon;
                        C.Save();
                    }
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
                    ImGui.Dummy(new(0, 3));

                    float iconSize = 23 * scale;
                    float iconSpacing = 4;
                    float availWidth = ImGui.GetContentRegionAvail().X;
                    float startX = (availWidth - (iconSize + iconSpacing) * 4 + iconSpacing) * 0.5f;

                    ImGui.SetCursorPosX(startX);
                    bool sinusEnabled = C.ShowSinusMissions;
                    var SinusTexture = Svc.Texture.GetFromManifestResource(Assembly.GetExecutingAssembly(), SinusAsset).GetWrapOrEmpty();
                    if (StyledImageButton.DrawStyledImageButton(SinusTexture, new Vector2(iconSize, iconSize), sinusEnabled))
                    {
                        C.ShowSinusMissions = !sinusEnabled;
                        C.AutoSelectMoon = false;
                        C.Save();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("憧憬湾"); // Sinus Ardorum
                    }

                    ImGui.SameLine();
                    bool phaennaEnabled = C.ShowPhaennaMissions;
                    var PhaennaTextures = Svc.Texture.GetFromManifestResource(Assembly.GetExecutingAssembly(), PhaennaAsset).GetWrapOrEmpty();

                    if (StyledImageButton.DrawStyledImageButton(PhaennaTextures, new Vector2(iconSize, iconSize), phaennaEnabled))
                    {
                        C.ShowPhaennaMissions = !phaennaEnabled;
                        C.AutoSelectMoon = false;
                        C.Save();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("法恩娜行星"); // Phaenna
                    }

                    ImGui.SameLine();
                    bool oizysEnabled = C.ShowOizysMissions;
                    var OizysTexture = Svc.Texture.GetFromManifestResource(Assembly.GetExecutingAssembly(), OizysAsset).GetWrapOrEmpty();

                    if (StyledImageButton.DrawStyledImageButton(OizysTexture, new Vector2(iconSize, iconSize), oizysEnabled))
                    {
                        C.ShowOizysMissions = !oizysEnabled;
                        C.AutoSelectMoon = false;
                        C.Save();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("俄匊斯行星");
                    }
                }
                if (C.AutoPickCurrentJob && (CosmicHelper.CrafterJobList.Contains((uint)Player.Job) || CosmicHelper.GatheringJobList.Contains((uint)Player.Job)) && C.SelectedJob != (uint)Player.Job)
                {
                    C.SelectedJob = (uint)Player.Job;
                    C.Save();
                }
                if (ImGui_Tools.DrawCategoryHeader_AutoSize("职业选择", imageTexture: GreyscaleJob())) // Class Selection
                {
                    float iconSize = 26 * scale;
                    float iconSpacing = 4;
                    float availWidth = ImGui.GetContentRegionAvail().X;
                    float startX = (availWidth - (iconSize + iconSpacing) * 4 + iconSpacing) * 0.5f;

                    ImGui.SetCursorPosX(startX);
                    bool autoSelectJob = C.AutoPickCurrentJob;
                    if (ImGui.Checkbox("自动选择##AutoSelectJob", ref autoSelectJob)) // Auto Select
                    {
                        C.AutoPickCurrentJob = autoSelectJob;
                        C.Save();
                    }

                    ImGui.SetCursorPosX(startX);
                    ImGui_Tools.DrawJobButtons(8, "刻木匠");
                    ImGui.SameLine(0, iconSpacing);
                    ImGui_Tools.DrawJobButtons(9, "锻铁匠");
                    ImGui.SameLine(0, iconSpacing);
                    ImGui_Tools.DrawJobButtons(10, "铸甲匠");
                    ImGui.SameLine(0, iconSpacing);
                    ImGui_Tools.DrawJobButtons(11, "雕金匠");

                    ImGui.SetCursorPosX(startX);
                    ImGui_Tools.DrawJobButtons(12, "制革匠");
                    ImGui.SameLine(0, iconSpacing);
                    ImGui_Tools.DrawJobButtons(13, "裁衣匠");
                    ImGui.SameLine(0, iconSpacing);
                    ImGui_Tools.DrawJobButtons(14, "炼金术士");
                    ImGui.SameLine(0, iconSpacing);
                    ImGui_Tools.DrawJobButtons(15, "烹调师");

                    ImGui.SetCursorPosX(startX);
                    ImGui_Tools.DrawJobButtons(16, "采矿工");
                    ImGui.SameLine(0, iconSpacing);
                    ImGui_Tools.DrawJobButtons(17, "园艺工");
                    ImGui.SameLine(0, iconSpacing);
                    ImGui_Tools.DrawJobButtons(18, "捕鱼人");
                }
                if (ImGui_Tools.DrawCategoryHeader_AutoSize("宇宙研究数据", icon: FontAwesomeIcon.ArrowUpRightDots)) // Tool Relic XP
                {
                    if (PlayerHelper.IsInCosmicZone())
                    {
                        var jobId = C.SelectedJob;
                        ImGui.Image(CosmicHelper.JobIconDict[jobId].GetWrapOrEmpty().Handle, new(24, 24));
                        ImGui.SameLine(0, 2);
                        ImGui.AlignTextToFramePadding();
                        Relic_XP.DrawRelicXP(jobId);
                    }
                    else
                    {
                        ImGui.TextWrapped("您必须在宇宙探索地图中, 我们才能查看这些信息。\n这得怪 SE 没把它做成随时可访问的。");
                    }
                }
                if (ImGui_Tools.DrawCategoryHeader_AutoSize("帮助", icon: FontAwesomeIcon.QuestionCircle)) // Help
                {
                    DrawSelectableWithIcon(FontAwesomeIcon.Question, "插件需求", "helpSelect_Requirements"); // Requirements
                    DrawSelectableWithIcon(FontAwesomeIcon.Book, "Ice 日志", "helpSelect_Logs"); // Ice Logs
                }
            }
            ImGui.EndChild();
        }

        private static void DrawSelectableWithIcon(FontAwesomeIcon icon, string label, string id)
        {
            bool isSelected = currentSelection == id;
            float scale = ImGuiHelpers.GlobalScale;

            // Change background color if selected
            if (isSelected)
            {
                ImGui.PushStyleColor(ImGuiCol.Header, ImGui.GetColorU32(ImGuiCol.HeaderActive));
            }

            // Indent for items under categories (scaled)
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16 * scale);

            float width = ImGui.GetContentRegionAvail().X;

            // Invisible selectable as the clickable area (scaled height)
            if (ImGui.Selectable($"##{id}", isSelected, ImGuiSelectableFlags.None, new Vector2(width, 25 * scale)))
            {
                currentSelection = id;
            }

            if (isSelected)
            {
                ImGui.PopStyleColor();

                // Draw colored bar on the left side
                var drawList = ImGui.GetWindowDrawList();
                var rectMin = ImGui.GetItemRectMin();
                var rectMax = ImGui.GetItemRectMax();

                // Draw a 3-4 pixel wide bar on the left (scaled)
                drawList.AddRectFilled(
                    rectMin,
                    new Vector2(rectMin.X + 3 * scale, rectMax.Y),
                    ImGui.GetColorU32(new Vector4(0.4f, 0.7f, 1.0f, 1.0f)) // Your accent color here
                );
            }

            // Get the position of that selectable we just drew
            float itemY = ImGui.GetItemRectMin().Y;

            // Set cursor back to draw icon and text on top (scaled offsets)
            ImGui.SetCursorScreenPos(new Vector2(ImGui.GetItemRectMin().X + 8 * scale, itemY + 4 * scale));

            ImGuiEx.Icon(icon);
            ImGui.SameLine();
            ImGui.Text(label);

            // Add small spacing between items (scaled)
            ImGui.Dummy(new Vector2(0, 2 * scale));
        }

        private static void DrawSelectableWithImage(uint iconId, string label, string id)
        {
            bool isSelected = currentSelection == id;
            float scale = ImGuiHelpers.GlobalScale;

            // Change background color if selected
            if (isSelected)
            {
                ImGui.PushStyleColor(ImGuiCol.Header, ImGui.GetColorU32(ImGuiCol.HeaderActive));
            }

            // Indent for items under categories (scaled)
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 16 * scale);

            float width = ImGui.GetContentRegionAvail().X;

            // Invisible selectable as the clickable area (scaled height)
            if (ImGui.Selectable($"##{id}", isSelected, ImGuiSelectableFlags.None, new Vector2(width, 25 * scale)))
            {
                currentSelection = id;
            }

            if (isSelected)
            {
                ImGui.PopStyleColor();
            }

            // Get the position of that selectable we just drew
            float itemY = ImGui.GetItemRectMin().Y;

            // Set cursor back to draw image and text on top (scaled offsets)
            ImGui.SetCursorScreenPos(new Vector2(ImGui.GetItemRectMin().X + 8 * scale, itemY + 2 * scale));

            Svc.Texture.TryGetFromGameIcon(iconId, out var iconImage);
            if (iconImage != null)
            {
                var image = iconImage.GetWrapOrEmpty();
                Vector2 imageSize = new Vector2(25 * scale, 25 * scale); // Scaled image
                                                                         // Center image vertically in the scaled height
                float imageYOffset = (25 * scale - imageSize.Y) / 2;
                ImGui.SetCursorScreenPos(new Vector2(ImGui.GetCursorScreenPos().X, ImGui.GetCursorScreenPos().Y + imageYOffset));
                ImGui.Image(image.Handle, imageSize);
            }

            ImGui.SameLine();
            ImGui.AlignTextToFramePadding();
            ImGui.Text(label);

            // Add small spacing between items (scaled)
            ImGui.Dummy(new Vector2(0, 2 * scale));
        }

        private static IDalamudTextureWrap? GreyscaleJob()
        {
            var jobId = C.SelectedJob;
            string greyJobIcon = jobId switch
            {
                8 => "ICE.Resources.GreyscaleJobs.CRP.png",
                9 => "ICE.Resources.GreyscaleJobs.BSM.png",
                10 => "ICE.Resources.GreyscaleJobs.ARM.png",
                11 => "ICE.Resources.GreyscaleJobs.GSM.png",
                12 => "ICE.Resources.GreyscaleJobs.LTW.png",
                13 => "ICE.Resources.GreyscaleJobs.WVR.png",
                14 => "ICE.Resources.GreyscaleJobs.ALC.png",
                15 => "ICE.Resources.GreyscaleJobs.CUL.png",
                16 => "ICE.Resources.GreyscaleJobs.MIN.png",
                17 => "ICE.Resources.GreyscaleJobs.BTN.png",
                18 => "ICE.Resources.GreyscaleJobs.FSH.png",
                _ => "ICE.Resources.GreyscaleJobs.Default.png",
            };

            return Svc.Texture.GetFromManifestResource(Assembly.GetExecutingAssembly(), greyJobIcon).GetWrapOrEmpty();
        }

        public static bool DrawCategoryHeader(string label, FontAwesomeIcon? icon = null, IDalamudTextureWrap? imageTexture = null, int? badgeCount = null)
        {
            var drawList = ImGui.GetWindowDrawList();
            var cursorPos = ImGui.GetCursorScreenPos();

            // Get colors from current theme
            var headerColor = ImGui.GetColorU32(ImGuiCol.Header);
            var textColor = ImGui.GetColorU32(ImGuiCol.Text);
            var textDisabledColor = ImGui.GetColorU32(ImGuiCol.TextDisabled);

            float width = ImGui.GetContentRegionAvail().X;
            float height = 30;

            // Check if this category is expanded (default to true)
            string categoryId = label;
            if (!categoryStates.ContainsKey(categoryId))
                categoryStates[categoryId] = true;

            bool isExpanded = categoryStates[categoryId];

            // Check for click
            bool isHovered = ImGui.IsMouseHoveringRect(cursorPos,
                new Vector2(cursorPos.X + width, cursorPos.Y + height));
            bool isClicked = isHovered && ImGui.IsMouseClicked(ImGuiMouseButton.Left);

            if (isClicked)
            {
                categoryStates[categoryId] = !categoryStates[categoryId];
                isExpanded = categoryStates[categoryId];
            }

            // Change header color slightly on hover
            if (isHovered)
                headerColor = ImGui.GetColorU32(ImGuiCol.HeaderHovered);

            // Draw background rectangle WITH ROUNDED CORNERS
            drawList.AddRectFilled(cursorPos,
                new Vector2(cursorPos.X + width, cursorPos.Y + height),
                headerColor,
                5.0f);

            // Calculate vertical centering
            float imageSize = 23;
            float textHeight = ImGui.CalcTextSize(label).Y;
            float verticalPadding = (height - textHeight) / 2;

            // Add left padding
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + verticalPadding);
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 8);

            // Draw icon or image
            if (imageTexture != null)
            {
                // Calculate offset to center image with text
                float imageYOffset = (textHeight - imageSize) / 2;
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + imageYOffset);

                ImGui.Image(imageTexture.Handle, new Vector2(imageSize, imageSize));

                // Reset Y position for text
                ImGui.SameLine();
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() - imageYOffset);
            }
            else if (icon.HasValue)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, textDisabledColor);
                ImGuiEx.Icon(icon.Value);
                ImGui.PopStyleColor();
                ImGui.SameLine();
            }
            else
            {
                // No icon, just add sameline spacing
                ImGui.SameLine();
            }

            ImGui.PushStyleColor(ImGuiCol.Text, textDisabledColor);
            ImGui.Text(label);
            ImGui.PopStyleColor();

            // Draw badge if count provided
            if (badgeCount.HasValue && badgeCount.Value > 0)
            {
                float badgeSize = 24;
                float rightPadding = 10;

                float badgeXPos = cursorPos.X + width - badgeSize - rightPadding;
                float badgeYPos = cursorPos.Y + (height / 2);

                var badgeColor = ImGui.GetColorU32(ImGuiCol.ButtonActive);
                var badgeCenter = new Vector2(badgeXPos + (badgeSize / 2), badgeYPos);

                drawList.AddCircleFilled(badgeCenter, 12, badgeColor);

                var numberStr = badgeCount.Value.ToString();
                var textSize = ImGui.CalcTextSize(numberStr);
                drawList.AddText(
                    new Vector2(badgeCenter.X - textSize.X / 2, badgeCenter.Y - textSize.Y / 2),
                    textColor,
                    numberStr);
            }

            ImGui.Dummy(new Vector2(0, 5));

            return isExpanded;
        }
    }
}
