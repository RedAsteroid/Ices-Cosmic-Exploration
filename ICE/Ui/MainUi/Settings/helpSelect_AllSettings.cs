using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using ICE.Ui.MainUi.Settings.Settings_Table;
using ICE.Ui.SettingTabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICE.Ui.MainUi.Settings
{
    internal class helpSelect_AllSettings
    {
        public static Dictionary<string, bool> settingsTabs = new Dictionary<string, bool>();

        public static void Draw()
        {
            ImGui.BeginChild("##tab_scroll", new Vector2(0, ImGui.GetTextLineHeight() + 16), false, ImGuiWindowFlags.HorizontalScrollbar);

            DrawCategoryTab("采集配置", "settings_GatheringProfile", FontAwesomeIcon.Leaf);
            DrawCategoryTab("信用点购物", "settings_CosmoShopping", textureId: 65112);
            DrawCategoryTab("宇宙好运道", "settings_CosmoWheel", textureId: 65127);
            DrawCategoryTab("停止条件", "settings_StopWhen", FontAwesomeIcon.Stop);
            DrawCategoryTab("任务优先级", "settings_MissionPrio", FontAwesomeIcon.SortAmountUp);
            DrawCategoryTab("杂项", "settings_Misc", icon: FontAwesomeIcon.Cog);
            EndCategoryButtonRow();

            ImGui.EndChild();

            if (settingsTabs["settings_GatheringProfile"])
            {
                GatherSettings.Draw();
            }
            else if (settingsTabs["settings_CosmoShopping"])
            {
                ShoppingTab.Draw();
            }
            else if (settingsTabs["settings_CosmoWheel"])
            {
                GambaWheel.Draw();
            }
            else if (settingsTabs["settings_StopWhen"])
            {
                StopWhen.Draw();
            }
            else if (settingsTabs["settings_MissionPrio"])
            {
                Priority_Settings.Draw();
            }
            else if (settingsTabs["settings_Misc"])
            {
                Misc_Settings.Draw();
            }

            if (settingsTabs.Count > 0 && settingsTabs.All(x => !x.Value))
            {
                var firstKey = settingsTabs.Keys.First();
                settingsTabs[firstKey] = true;
            }
        }

        public static bool DrawCategoryTab(string label, string categoryId, FontAwesomeIcon? icon = null, int? textureId = null, float spacingAfter = 5)
        {
            // Default coloring here
            var headerColor = ImGui.GetColorU32(ImGuiCol.Button);
            var textColor = ImGui.GetColorU32(ImGuiCol.Text);

            // Setting the values of the content size (padding, spacing, ect) that way it's used across the board
            float horizontalPadding = 8;
            float verticalPadding = 4;
            float iconTextSpacing = 4;

            // These are to make sure that they're drawn in place
            var drawList = ImGui.GetWindowDrawList();
            var cursorPos = ImGui.GetCursorScreenPos();

            // Calculate text size
            var textSize = ImGui.CalcTextSize(label);

            // Load texture if textureId is provided
            IDalamudTextureWrap? texture = null;
            if (textureId.HasValue)
            {
                if (Svc.Texture.TryGetFromGameIcon(textureId.Value, out var gameIcon))
                {
                    texture = gameIcon.GetWrapOrEmpty();
                }
            }

            // Calculate icon/texture width if present
            float iconWidth = 0;
            if (icon.HasValue || texture != null)
            {
                iconWidth = textSize.Y + iconTextSpacing;
            }

            // Calculate button dimensions based on content
            float contentWidth = horizontalPadding * 2 + iconWidth + textSize.X;
            float contentHeight = verticalPadding * 2 + textSize.Y;

            // If it doesn't already exist, then creating an entry in the category state
            if (!settingsTabs.ContainsKey(categoryId))
                settingsTabs[categoryId] = false;

            bool isExpanded = settingsTabs[categoryId];
            bool isHovered = ImGui.IsMouseHoveringRect(cursorPos, new Vector2(cursorPos.X + contentWidth, cursorPos.Y + contentHeight))
                          && ImGui.IsWindowHovered(ImGuiHoveredFlags.AllowWhenBlockedByPopup | ImGuiHoveredFlags.ChildWindows);
            bool isClicked = isHovered && ImGui.IsMouseClicked(ImGuiMouseButton.Left);

            if (isClicked)
            {
                foreach (var tab in settingsTabs)
                    settingsTabs[tab.Key] = false;

                settingsTabs[categoryId] = true;
                isExpanded = true;
            }

            // Color changing! Based on the various states
            if (isExpanded)
                headerColor = ImGui.GetColorU32(ImGuiCol.TabActive);
            if (isHovered)
                headerColor = ImGui.GetColorU32(ImGuiCol.TabHovered);

            // Draw background rectangle with rounded corners
            drawList.AddRectFilled(cursorPos, new Vector2(cursorPos.X + contentWidth, cursorPos.Y + contentHeight), headerColor, 5.0f);

            // Handle drawing icon/texture
            if (texture != null)
            {
                // Draw FFXIV texture first
                ImGui.SetCursorScreenPos(new Vector2(cursorPos.X + horizontalPadding, cursorPos.Y + verticalPadding));
                ImGui.Image(texture.Handle, new Vector2(textSize.Y * 1.2f, textSize.Y * 1.2f));

                // Drawing the text afterwards
                ImGui.SetCursorScreenPos(new Vector2(cursorPos.X + horizontalPadding + textSize.Y * 1.2f + iconTextSpacing, cursorPos.Y + verticalPadding));
                ImGui.TextUnformatted(label);
            }
            else if (icon.HasValue)
            {
                // Draw FontAwesome icon first
                ImGui.SetCursorScreenPos(new Vector2(cursorPos.X + horizontalPadding, cursorPos.Y + verticalPadding));
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextUnformatted(icon.Value.ToIconString());
                ImGui.PopFont();

                // Drawing the text afterwards
                ImGui.SetCursorScreenPos(new Vector2(cursorPos.X + horizontalPadding + textSize.Y + iconTextSpacing, cursorPos.Y + verticalPadding));
                ImGui.TextUnformatted(label);
            }
            else
            {
                // Just draw text
                ImGui.SetCursorScreenPos(new Vector2(cursorPos.X + horizontalPadding, cursorPos.Y + verticalPadding));
                ImGui.TextUnformatted(label);
            }

            // Create an invisible button to properly reserve space and handle layout
            ImGui.SetCursorScreenPos(cursorPos);
            ImGui.InvisibleButton($"##{categoryId}_btn", new Vector2(contentWidth, contentHeight));

            // Adding the space afterwards, don't want these bumping uglies
            ImGui.SameLine(0, spacingAfter);

            return isExpanded;
        }
        public static void EndCategoryButtonRow()
        {
            ImGui.NewLine();
            ImGui.Separator();
        }
    }
}
