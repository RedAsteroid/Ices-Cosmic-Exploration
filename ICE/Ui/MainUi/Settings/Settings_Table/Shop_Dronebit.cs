using Dalamud.Interface;
using ICE.Utilities.ImGuiTools;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.Ui.MainUi.Settings.Settings_Table
{
    internal class Shop_Dronebit
    {
        public static void Draw()
        {
            bool buyDrones = C.Cosmodrone_Buy;
            if (ImGui.Checkbox("购买俄匊斯能源包", ref buyDrones)) // Buy Drones
            {
                C.Cosmodrone_Buy = buyDrones;
                C.Save();
            }
            ImGui_Tools.IconWithTooltip(FontAwesomeIcon.QuestionCircle,
                "想要购买俄匊斯能源包? 如果需要, 请启用此选项" // Do you want to buy drones? If yes, enable this
                );

            int drone_buyAtAmount = C.Cosmodrone_BuyAt;
            ImGui.SetNextItemWidth(200);
            if (ImGui.SliderInt("开始购买阈值", ref drone_buyAtAmount, 200, 5000))
            {
                drone_buyAtAmount = (int)Math.Round(drone_buyAtAmount / 200.0) * 200;
                C.Cosmodrone_BuyAt = drone_buyAtAmount;
                C.SaveDebounced();
            }
            ImGui_Tools.IconWithTooltip(FontAwesomeIcon.QuestionCircle, 
                "您的无人机晶片达到多少数量时, 从商店购买俄匊斯能源包?\n" +
                "以 200 为递增单位, 最大 5000"
                );

            int maxCrateAmount = C.Cosmodrone_MaxKeep;
            ImGui.SetNextItemWidth(200);
            if (ImGui.InputInt("最大持有", ref maxCrateAmount)) // Maximum Drones
            {
                if (maxCrateAmount < 0)
                    maxCrateAmount = 0;
                C.Cosmodrone_MaxKeep = maxCrateAmount;
                C.SaveDebounced();
            }
            ImGui_Tools.IconWithTooltip(FontAwesomeIcon.QuestionCircle,
                "您想持有的能源包数量上限是多少?\n" +
                "0 = 持续购买\n" +
                "任何大于 0 的值将作为硬上限, 持有数量达到此值时停止购买"
                );

            bool runDroneFinder = C.Cosmodrone_Run;
            if (ImGui.Checkbox("自动遗物探索", ref runDroneFinder)) // Automate cosmodrone
            {
                C.Cosmodrone_Run = runDroneFinder;
                C.Save();
            }
            ImGui_Tools.IconWithTooltip(FontAwesomeIcon.QuestionCircle,
                "要启用自动遗物探索吗? 如果需要, 请启用此选项\n" + // Do you want to run the automated drone finding? If yes, enable this
                "请注意: 千万不要放着无人看管! 此功能仍在大幅度完善中。"); // PLEASE NOTE. DO. NOT. LEAVE. THIS. ALONE. This is still being worked on heavily

            int runAtAmount = C.Cosmodrone_RunAt;
            ImGui.SetNextItemWidth(200);
            if (ImGui.InputInt("开始遗物探索阈值", ref runAtAmount)) // Start finding drones at
            {
                C.Cosmodrone_RunAt = runAtAmount;
                C.Save();
            }
            ImGui_Tools.IconWithTooltip(FontAwesomeIcon.QuestionCircle,
                "持有多少个俄匊斯能源包时开始运行?\n" + // How many drone boxes do you want to have before it starts running?
                "如果设为 1, 只要获得任意 1 个, 就会开始使用能源包并寻找星球遗物" // If you set this at 1, the moment you get any it will proceed to open -> find
                );

            bool finishCurrent = C.Cosmodrone_FinishCurrent;
            if (ImGui.Checkbox("如果无人机已激活, 尝试完成当前遗物探索", ref finishCurrent)) // If drone is active, finish current
            {
                C.Cosmodrone_FinishCurrent = finishCurrent;
                C.Save();
            }
            ImGui_Tools.IconWithTooltip(FontAwesomeIcon.QuestionCircle,
                "如果检测到您在俄匊斯行星, 并且地图中已标记了遗物位置, \n" + // If it detects that you're on Oizys and have a drone that is currently on the map,
                "插件将自动寻路并回收星球遗物"); // It will proceed to pathfind -> collecting said drone for you
        }
    }
}
