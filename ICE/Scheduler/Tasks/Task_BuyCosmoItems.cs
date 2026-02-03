using ECommons.GameHelpers;
using ICE.ConfigFiles;
using ICE.Utilities.Cosmic_Helper;
using static ECommons.UIHelpers.AddonMasterImplementations.AddonMaster;
using static ICE.ConfigFiles.Config;

namespace ICE.Scheduler.Tasks
{
    internal static class Task_BuyCosmoItems
    {
        public static void Enqueue()
        {
            P.TaskManager.EnqueueMulti
                (
                    new(Credit_PathToVendor, "Pathing to the credit vendor"),
                    new(TalkToCreditNPC, "Talking to the credit NPC to start the buying process"),
                    new(SelectShop, "Selecting the shop entry we want to go to"),
                    new(BuyItems, "Buying items from the vendor", Utils.TaskConfig),
                    new(CloseShop, "Closing the shop menu")
                );
        }

        private static unsafe bool? Credit_PathToVendor()
        {
            // 关闭 ShopExchangeCurrency 窗口，避免卡死寻路
            if (GenericHelpers.TryGetAddonMaster<ShopExchangeCurrency>("ShopExchangeCurrency", out var shop) && shop.IsAddonReady)
            {
                if (EzThrottler.Throttle("ClosingShopExchangeCurrency"))
                    shop.Addon->Close(true);

                return false;
            }

            string handle = "[Task_Credits: PathTo]";
            var zoneId = Player.Territory.RowId;
            var npcEntry = NpcData.MoonNpcs[zoneId].Where(x => x.type == NpcData.NpcType.Credit).FirstOrDefault();

            if (npcEntry != null)
            {
                Vector3 randomPos = NpcData.GetRandomPointInCircle(npcEntry.Location_Circle, 0.5f);
                if (!Task_NavmeshMove.Task_NavTo(randomPos, distance: 6, npcLoc: npcEntry.Location_Npc).Value)
                {
                    if (EzThrottler.Throttle("Repair move message", 1000))
                        IceLogging.Verbose($"Pathing to repair NPC. Current distance: {Player.DistanceTo(npcEntry.Location_Npc)}", handle);
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
        private static bool? TalkToCreditNPC()
        {
            if (GenericHelpers.TryGetAddonMaster<SelectIconString>("SelectIconString", out var iconString) && iconString.IsAddonReady)
            {
                IceLogging.Info("Icon string is visible! Time to shop");
                return true;
            }
            else
            {
                var researchId = NpcData.MoonNpcs[Player.Territory.RowId].Where(x => x.type == NpcData.NpcType.Credit).FirstOrDefault().NpcId;

                Utils.TryGetObjectByDataId(researchId, out var researchNpc);
                if (EzThrottler.Throttle("Interacting with researchingway"))
                {
                    Utils.TargetgameObject(researchNpc);
                    Utils.InteractWithObject(researchNpc);
                }
            }

            return false;
        }
        private static bool? SelectShop()
        {
            // Something to consider here... there's 2 different shops. Probably going to need to add a check to see which one we're going to select because I *-know-* people are going to ask about it >.>
            // For now, just going to support the one shop
            if (GenericHelpers.TryGetAddonMaster<SelectIconString>("SelectIconString", out var iconString) && iconString.IsAddonReady)
            {
                if (EzThrottler.Throttle("Selecting Materia Selection"))
                {
                    var select = iconString.Entries[1];
                    IceLogging.Debug($"Selecting: {select.Text}");
                    select.Select();
                }
            }
            else if (GenericHelpers.TryGetAddonMaster<ShopExchangeCurrency>("ShopExchangeCurrency", out var shopExchange) && shopExchange.IsAddonReady)
            {
                return true;
            }

            return false;
        }
        private static bool? CloseShop()
        {
            if (GenericHelpers.TryGetAddonMaster<ShopExchangeCurrency>("ShopExchangeCurrency", out var shopExchange) && shopExchange.IsAddonReady)
            {
                if (EzThrottler.Throttle("Close Shop"))
                    GenericHandlers.FireCallback("ShopExchangeCurrency", true, -1);
                return false;
            }
            else
                return true;
        }

        private static int BuyAmount = 0;
        private static uint ItemId = 0;
        private static int KeepAmount = 0;

        private static bool? BuyItems()
        {
            if (GenericHelpers.TryGetAddonMaster<SelectYesno>("SelectYesno", out var YesNo) && YesNo.IsAddonReady)
            {
                if (EzThrottler.Throttle("Buy Item", 500))
                {
                    YesNo.Yes();
                    if (BuyAmount != 0)
                    {
                        if (C.CosmoShopping.TryGetValue(ItemId, out var config))
                        {
                            config.BuyAmount -= BuyAmount;
                            if (config.BuyAmount <= 0)
                                config.BuyAmount = 0;
                            C.Save();
                        }
                        BuyAmount = 0;
                        KeepAmount = 0;
                    }
                }
            }
            else if (GenericHelpers.TryGetAddonMaster<ShopExchangeCurrency>("ShopExchangeCurrency", out var shopExchange) && shopExchange.IsAddonReady)
            {
                var currencyAmount = shopExchange.CurrencyAmount - (uint)C.CosmoKeepAmount;

                // Then try KeepAmount (accounting for what player already has)
                if (TryPurchaseItem(shopExchange, currencyAmount,
                    (item, itemId) =>
                    {
                        PlayerHelper.GetItemCount(itemId, out int currentCount);
                        return Math.Max(0, item.KeepAmount - currentCount);
                    },
                    (amount) => KeepAmount = amount))
                    return false;

                // Try BuyAmount first
                if (TryPurchaseItem(shopExchange, currencyAmount,
                    (item, itemId) => item.BuyAmount,
                    (amount) => BuyAmount = amount))
                    return false;

                // Finally try KeepBuying (buy max affordable)
                if (TryPurchaseItem(shopExchange, currencyAmount,
                    (item, itemId) => item.KeepBuying ? int.MaxValue : 0,
                    (amount) => KeepAmount = amount))
                    return false;

                return true;
            }

            return false;
        }

        private static bool TryPurchaseItem(
            ShopExchangeCurrency shopExchange,
            uint currencyAmount,
            Func<CosmoShoppingList, uint, int> getTargetAmount,
            Action<int> setAmount)
        {
            foreach (var itemId in C.CosmoShoppingOrder)
            {
                if (!C.CosmoShopping.TryGetValue(itemId, out var item))
                    continue;

                int targetAmount = getTargetAmount(item, itemId);
                if (targetAmount <= 0)
                    continue;

                var shopExchangeItem = shopExchange.BasicShopItems.FirstOrDefault(x => x.ItemId == itemId);
                if (shopExchangeItem == null)
                    continue;

                int maxAffordable = (int)(currencyAmount / shopExchangeItem.CostAmount);
                if (maxAffordable <= 0)
                    continue;

                int buyAmount = Math.Min(maxAffordable, targetAmount);
                if (buyAmount > 99) buyAmount = 99;

                if (EzThrottler.Throttle("Selecting Item to Buy"))
                {
                    shopExchangeItem.Select(buyAmount);
                    setAmount(buyAmount);
                    ItemId = itemId;
                }
                return true;
            }
            return false;
        }

        public static bool CanPurchaseAnyItem()
        {
            // Get current currency amount (you'll need to determine how to get this without the shop window)

            PlayerHelper.GetItemCount(45690, out var currencyAmount);
            currencyAmount -= C.CosmoKeepAmount;

            // Try BuyAmount first
            if (CanPurchaseItem(currencyAmount,
                (item, itemId) => item.BuyAmount))
                return true;

            // Then try KeepAmount (accounting for what player already has)
            if (CanPurchaseItem(currencyAmount,
                (item, itemId) =>
                {
                    PlayerHelper.GetItemCount(itemId, out int currentCount);
                    return Math.Max(0, item.KeepAmount - currentCount);
                }))
                return true;

            // Finally try KeepBuying (buy max affordable)
            if (CanPurchaseItem(currencyAmount,
                (item, itemId) => item.KeepBuying ? int.MaxValue : 0))
                return true;

            // Nothing can be purchased
            return false;
        }

        private static bool CanPurchaseItem(int currencyAmount, Func<CosmoShoppingList, uint, int> getTargetAmount)
        {
            foreach (var itemId in C.CosmoShoppingOrder)
            {
                if (!C.CosmoShopping.TryGetValue(itemId, out var item))
                    continue;

                int targetAmount = getTargetAmount(item, itemId);
                if (targetAmount <= 0)
                    continue;

                // Check if item exists in the cosmocredit shop dictionary
                if (!Shop_Cosmocredits.CosmocreditShop.TryGetValue(itemId, out var shopItem))
                    continue;

                int maxAffordable = (int)(currencyAmount / shopItem.Cost);
                if (maxAffordable <= 0)
                    continue;

                // We can afford to buy at least one of this item
                return true;
            }
            return false;
        }
    }
}
