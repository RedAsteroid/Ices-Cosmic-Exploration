using ICE.Utilities.Cosmic_Helper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICE.Ui.DebugWindowTabs
{
    internal class Ui_TaskManagerInfo
    {
        private static uint mission = 0;
        private static int frameDelay = 4;
        private static List<Vector3> pathTo = new List<Vector3>();
        private static Vector3 pathToArea = new Vector3();

        public static void Draw()
        {
            ImGui.Text($"Running task: {P.TaskManager.NumQueuedTasks != 0} | Amount of queue'd task: {P.TaskManager.NumQueuedTasks}");
            string currentTask = P.TaskManager.CurrentTask?.Name ?? "";
            ImGui.Text($"Current task running: {currentTask}");
            ImGui.Text($"Current State: {SchedulerMain.State}");
            ImGui.Text($"Task Count: {P.TaskManager.Tasks.Count}");
            if (ImGui.Button("Set State to Idle"))
            {
                SchedulerMain.State = IceState.Idle; 
            }

            if (ImGui.Button("Stop Task"))
            {
                P.TaskManager.Tasks.Clear();
                P.TaskManager.Abort();
            }

            ImGui.SetNextItemWidth(100);
            ImGui.InputUInt("Mission", ref mission);

            if (ImGui.Button("Path to mission"))
            {
                P.TaskManager.Enqueue(() => Task_FindMission.Navmesh_MoveToMission(mission), "Testing Moveto Task", Utils.TaskConfig);
            }
            ImGui.InputInt("Frame Delay", ref frameDelay);
            if (ImGui.Button("Running Mission Test"))
            {
                Task_FindMission.Enqueue();
            }
            if (ImGui.Button("Abandon Mission"))
            {
                Task_AbandonMission.Enqueue();
            }
            if (ImGui.Button("Path to repair NPC"))
            {
                P.TaskManager.Enqueue(() => Task_Repair.Repair_PathTo(), "Pathing to repair NPC");
            }
            if (ImGui.Button("Test Repair Function"))
            {
                Task_Repair.Enqueue();
            }
            ImGui.Text($"Current waypoint list count: {pathTo.Count}");

            ImGui.SetNextItemWidth(250);
            ImGui.InputFloat3("Destination", ref pathToArea);
            if (ImGui.Button("Set Area"))
            {
                pathToArea = ECommons.GameHelpers.Player.Position;
            }
            if (ImGui.Button("Create waypoint list"))
            {
                Vector3 currentPos = ECommons.GameHelpers.Player.Position;

                // Fire and forget - this will update pathTo when complete
                _ = Task.Run(async () =>
                {
                    pathTo = await FindTask(currentPos);
                });
            }
            if (ImGui.Button("Test Fishing Moveto"))
            {
                P.TaskManager.Enqueue(() => Task_FindMission.Navmesh_MoveToMission(mission), "Testing fishing moveto",configuration: Utils.TaskConfig);
            }
            if (ImGui.Button("Test Crafting"))
            {
                Task_Craft.Enqueue();
            }
            if (ImGui.Button("Test Gather Targeting"))
            {
                Task_Gather.Enqueue();
            }
            if (ImGui.Button("Buy Items from shop"))
            {
                Task_BuyCosmoItems.Enqueue();
            }

            if (ImGui.Button("Test Drone Buy Item"))
            {
                Task_ArtifactSearch.EnqueueBuy();
            }
            if (ImGui.Button("Test Drone Pathing"))
            {
                P.TaskManager.Enqueue(() => Task_ArtifactSearch.CheckBoxStatus());
            }
        }

        private static async Task<List<Vector3>> FindTask(Vector3 currentPos)
        {
            IceLogging.DestinationLogs.Log(pathToArea);
            return await P.Navmesh.Pathfind(currentPos, pathToArea, false);
        }
    }
}
