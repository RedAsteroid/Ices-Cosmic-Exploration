using Dalamud.Game.ClientState.Conditions;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ICE.Utilities.Cosmic_Helper;
using System;
using System.Collections.Generic;
using System.Text;
using static ECommons.UIHelpers.AddonMasterImplementations.AddonMaster;
using static ICE.Ui.DebugWindowTabs.Ui_OyzinMap;

namespace ICE.Scheduler.Tasks
{
    internal class Task_ArtifactSearch
    {
        public static void EnqueueBuy()
        {
            P.TaskManager.EnqueueMulti
                (
                    new(Drone_PathToVendor, "Drone NPC: Path to"),
                    new(TalkToDroneNpc, "Drone NPC: Talk"),
                    new(SelectShop, "Drone NPC: Select Shop"),
                    new(BuyDroneCrates, "Drone NPC: Buying"),
                    new(ExitDroneShop, "Drone NPC: Leaving Shop")
                );
        }

        private static bool? Drone_PathToVendor()
        {
            string handle = "[Task_Artifact: PathTo]";
            var zoneId = Player.Territory.RowId;
            var npcEntry = NpcData.MoonNpcs[zoneId].Where(x => x.type == NpcData.NpcType.Drone).FirstOrDefault();

            if (npcEntry != null)
            {
                Vector3 randomPos = NpcData.GetRandomPointInCircle(npcEntry.Location_Circle, 0.5f);
                if (!Task_NavmeshMove.Task_NavTo(randomPos, distance: 6, npcLoc: npcEntry.Location_Npc).Value)
                {
                    if (EzThrottler.Throttle("Drone Move Message", 1000))
                        IceLogging.Verbose($"Pathing to drone NPC. Current distance: {Player.DistanceTo(npcEntry.Location_Npc)}", handle);
                }
                else
                {
                    IceLogging.Debug("We're close enough to the repair npc! Continuing on", handle);
                    return true;
                }
            }
            else
            {
                if (EzThrottler.Throttle("Error message: NPC", 5000))
                    IceLogging.Error("Hey! We don't have this npc coded yet, which means I forgot bout it, could you let me know\n" +
                                     $"Planet Territory ID: {Player.Territory.RowId}", handle);
            }
            return false;
        }
        private static bool? TalkToDroneNpc()
        {
            if (GenericHelpers.TryGetAddonMaster<SelectString>("SelectString", out var iconString) && iconString.IsAddonReady)
            {
                IceLogging.Info("Icon string is visible! Time to shop");
                return true;
            }
            else if (GenericHelpers.TryGetAddonMaster<Talk>("Talk", out var talk) && talk.IsAddonReady)
            {
                if (EzThrottler.Throttle("Throttle talking", 100))
                    talk.Click();
            }
            else
            {
                var droneInfo = NpcData.MoonNpcs[Player.Territory.RowId].Where(x => x.type == NpcData.NpcType.Drone).FirstOrDefault().NpcId;

                Utils.TryGetObjectByDataId(droneInfo, out var droneNpc);
                if (EzThrottler.Throttle("Interacting with researchingway"))
                {
                    Utils.TargetgameObject(droneNpc);
                    Utils.InteractWithObject(droneNpc);
                }
            }

            return false;
        }
        private static bool? SelectShop()
        {
            if (GenericHelpers.TryGetAddonMaster<ShopExchangeCurrency>("ShopExchangeCurrency", out var shopExchange) && shopExchange.IsAddonReady)
            {
                IceLogging.Debug("Shop Exchange Currency Addon is Ready!");
                return true;
            }
            else if (GenericHelpers.TryGetAddonMaster<SelectString>("SelectString", out var selectString) && selectString.IsAddonReady)
            {
                if (EzThrottler.Throttle("Selecting Materia Selection"))
                {
                    var select = selectString.Entries[0];
                    IceLogging.Verbose($"Selecting: {select.Text}");
                    select.Select();
                }
            }

            return false;
        }
        private static bool? BuyDroneCrates()
        {
            string tag = "[Task_ArtifactSearch: Buy Drones]";

            if (GenericHelpers.TryGetAddonMaster<SelectYesno>("SelectYesno", out var YesNo) && YesNo.IsAddonReady)
            {
                if (EzThrottler.Throttle("Buy Drone Boxes"))
                {
                    YesNo.Yes();
                }
            }
            else if (GenericHelpers.TryGetAddonMaster<ShopExchangeCurrency>("ShopExchangeCurrency", out var shopExchange) && shopExchange.IsAddonReady)
            {
                // There's only 1 item here. So we just need to make sure we have enough to buy it
                var currency = shopExchange.CurrencyAmount;
                var item = shopExchange.BasicShopItems[0];

                if (item.CostAmount <= currency)
                {
                    if (EzThrottler.Throttle("Selecting to buy this item", 1000))
                    {
                        int amount = (int)(currency / item.CostAmount);
                        item.Select(amount);
                    }
                }
                else
                {
                    IceLogging.Debug("We don't have any more currency to buy the dronebit, so continuing on", tag);
                    return true;
                }
            }

            return false;
        }
        private static bool? ExitDroneShop()
        {
            string tag = "[Task: ArtifactBuy | Exit Shop]";
            if (GenericHelpers.TryGetAddonMaster<ShopExchangeCurrency>("ShopExchangeCurrency", out var shopExchange) && shopExchange.IsAddonReady)
            {
                if (EzThrottler.Throttle("Closing the window"))
                {
                    IceLogging.Verbose("Closing the shop exchange window", tag);
                    GenericHandlers.FireCallback("ShopExchangeCurrency", true, -1);
                }
            }
            else
            {
                return true;
            }

            return false;
        }

        // Going to drone locations
        private static unsafe bool DroneReady()
        {
            var actionManager = ActionManager.Instance();

            // For regular items
            uint itemId = 50414; // your item ID
            var actionStatus = actionManager->GetActionStatus(ActionType.Item, itemId);

            // actionStatus == 0 means the item is ready to use
            // any other value indicates it's not ready (on cooldown, requirements not met, etc.)
            if (actionStatus == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public class TempMapMarkerData
        {
            public Vector3 Position { get; set; }
            public uint IconId { get; set; }
        }
        public unsafe static List<TempMapMarkerData> GetAllEventMarkers()
        {
            var markers = new List<TempMapMarkerData>();
            var agentMap = AgentMap.Instance();

            if (agentMap == null) return markers;

            foreach (var marker in agentMap->EventMarkers)
            {
                markers.Add(new TempMapMarkerData
                {
                    Position = marker.Position,
                    IconId = marker.IconId,
                });
            }

            return markers;
        }
        public static unsafe bool? RefreshMapInfo()
        {
            P.TaskManager.EnqueueMulti
                (
                    new(CloseMapInfo, "Making sure map is close"),
                    new(OpenMapInfo, "Re-opening map to refresh"),
                    new(CloseMapInfo, "Closing one more time cause we don't need it"),
                    new(CheckBoxStatus, "Checking Box Status")
                );
            return true;
        }
        public static unsafe bool? CheckBoxStatus()
        {
            droneLoc = Vector3.Zero;
            string tag = "[Task_Artifact: CheckBoxStatus]";

            var mapMarkers = GetAllEventMarkers();
            if (mapMarkers.Count == 0)
            {
                IceLogging.Info("No map markers were available... which isn't right.\n" +
                    "Going to refresh the map to make sure it loads properly", tag);

                if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("AreaMap", out var mapAddon) && GenericHelpers.IsAddonReady(mapAddon))
                {
                    if (EzThrottler.Throttle("Closing map temp"))
                    {
                        GenericHandlers.FireCallback("AreaMap", true, -1);
                    }
                }
                else
                {
                    if (EzThrottler.Throttle("Opening map again", 1000))
                    {
                        var map = Player.Territory.Value.Map.Value;
                        var territoryid = Player.Territory.RowId;
                        var agent = AgentMap.Instance();

                        agent->OpenMap(map.RowId, territoryid);
                    }
                }
            }
            else
            {
                var marker = mapMarkers.Where(x => x.IconId == 63989).FirstOrDefault();
                if (marker != null)
                {
                    IceLogging.Debug("We've found the map flag! Setting it for us to travel to", tag);
                    droneLoc = marker.Position;
                    P.TaskManager.EnqueueMulti
                        (
                            new(PathToDrone, "Pathing to drone"), 
                            new(InteractWithDrone, "Interact with drone")
                        );
                    return true;
                }
                else
                {
                    if (PlayerHelper.GetItemCount(50414, out var count) && count > 0)
                    {
                        IceLogging.Debug("We have a crate to use! Initiating the task to start using it", tag);
                        P.TaskManager.Enqueue(() => UseDroneBox(), "Use Drone Box");
                        return true;
                    }
                    else
                    {
                        IceLogging.Debug($"We are out of boxes, and we have no markers. So we're continuing on with the normal task");
                        SchedulerMain.State = IceState.Idle;
                        return true;
                    }
                }
            }

            return false;
        }
        private static unsafe bool? UseDroneBox()
        {
            if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("AreaMap", out var mapAddon) && GenericHelpers.IsAddonReady(mapAddon))
            {
                P.TaskManager.Enqueue(() => CheckBoxStatus(), "Task Artifact: Check Box Status", Utils.TaskConfig);
                return true;
            }
            else
            {
                if (Player.Mounted || Player.IsJumping)
                {
                    Utils.Dismount();
                    return false;
                }

                var actionManager = ActionManager.Instance();
                uint itemId = 50414;

                var status = actionManager->GetActionStatus(ActionType.Item, itemId);
                // IceLogging.Info($"Action status: {status}");

                if (status == 0)
                {
                    actionManager->UseAction(ActionType.Item, itemId, 0xE0000000);
                }
            }
                
            return false;
        }
        private static Vector3 droneLoc = Vector3.Zero;
        private static unsafe bool? PathToDrone()
        {
            if (Task_NavmeshMove.Task_NavTo(droneLoc, distance: 3.5f, waitForBusy: false).Value)
            {
                P.Navmesh.Stop();

                IceLogging.Debug("We're at the drone! (Hopefully)... probably");
                return true;
            }
            else
            {
                if (EzThrottler.Throttle("Distance Tell", 2000))
                    IceLogging.Verbose($"Distance: {Player.DistanceTo(droneLoc)}");

                return false;
            }
        }
        private static bool? InteractWithDrone()
        {
            string tag = "[Task_Artifact: Drone Interact]";

            var artifact = Svc.Objects.Where(x => x.BaseId == 2015138).FirstOrDefault();

            if (artifact != null)
            {
                if (Player.DistanceTo(artifact.Position) < 4)
                {
                    if (P.Navmesh.IsRunning())
                        P.Navmesh.Stop();

                    if (!Svc.Condition[ConditionFlag.OccupiedInQuestEvent])
                    {
                        Utils.TargetgameObject(artifact);
                        Utils.InteractWithObject(artifact);
                        IceLogging.Verbose($"Drone has been found! Interacting with it");
                    }
                }
            }
            else
            {
                P.TaskManager.EnqueueMulti
                    (
                        new(CloseMapInfo, "Making sure map is close"),
                        new(OpenMapInfo, "Re-opening map to refresh"),
                        new(CloseMapInfo, "Closing one more time cause we don't need it"),
                        new(CheckBoxStatus, "Checking to see if we have more boxes")
                    );

                return true;
            }

            return false;
        }
        public static unsafe bool? OpenMapInfo()
        {
            if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("AreaMap", out var mapAddon) && GenericHelpers.IsAddonReady(mapAddon))
            {
                return true;
            }
            else
            {
                if (EzThrottler.Throttle("Opening map again", 500))
                {
                    var map = Player.Territory.Value.Map.Value;
                    var territoryid = Player.Territory.RowId;
                    var agent = AgentMap.Instance();

                    agent->OpenMap(map.RowId, territoryid);
                }
                return false;
            }
        }
        public static unsafe bool? CloseMapInfo()
        {
            if (GenericHelpers.TryGetAddonByName<AtkUnitBase>("AreaMap", out var mapAddon) && GenericHelpers.IsAddonReady(mapAddon))
            {
                if (EzThrottler.Throttle("Closing map temp"))
                {
                    GenericHandlers.FireCallback("AreaMap", true, -1);
                }
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
