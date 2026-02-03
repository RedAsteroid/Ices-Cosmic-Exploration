using ECommons.Automation.NeoTaskManager;
using ECommons.Configuration;
using ECommons.GameHelpers;
using ICE.ConfigFiles;
using ICE.IPC;
using ICE.OldYamlConfig;
using ICE.Ui;
using ICE.Ui.MainUi;
using ICE.Utilities.Cosmic_Helper;
using ICE.Utilities.GatheringHelper;
using Pictomancy;
using System.Collections.Generic;
using Dalamud.IoC;
using Dalamud.Plugin.Services;
using static ICE.Utilities.CosmicHelper;

namespace ICE;

public sealed partial class ICE : IDalamudPlugin
{
    public static string Name => "ICE";

    internal static ICE P = null!;
    private static MissionConfigs missionConfigs;
    private Config config;
    public static Config C => P.config;
    
    // Missing ECommons PluginService. Update to Svc when ECommons get updated
    [PluginService] public static IUnlockState UnlockState { get; set; } = null!;
    
    public MissionTimer MissionTimer { get; private set; }
    // public static MissionConfigs C => missionConfigs ??= LoadConfig<MissionConfigs>();

    // Yaml Config Loaders. For both loading a yaml in the config folder, and for embedded
    private static T LoadConfig<T>() where T : IYamlConfig, new()
    {
        var path = typeof(T).GetProperty("ConfigPath")!.GetValue(null)!.ToString()!;
        var config = YamlConfig.Load<T>(path);

        if (config == null)
        {
            PluginLog.Warning($"[{typeof(T).Name}] Config was null. Creating new default.");
            config = new T();
            YamlConfig.SaveSync(config, path); // Use synchronous save for initialization
        }

        PluginLog.Information($"[{typeof(T).Name}] Loaded from {path}");
        return config;
    }

    // Window's that I use, base window to the settings... need these to actually show shit 
    internal WindowSystem windowSystem;
    internal MainWindow mainWindow;
    internal OverlayWindow overlayWindow;
    internal DebugWindow debugWindow;
    internal InfoWindow infoWindow;
    internal Window_ExternalDetails externalDetails;

    // Taskmanager from Ecommons
    internal TaskManager TaskManager;

    // Internal IPC's that I use for... well plugins. 
    internal LifestreamIPC Lifestream;
    internal NavmeshIPC Navmesh;
    internal PandoraIPC Pandora;
    internal ArtisanIPC Artisan;
    internal VislandIPC Visland;
    internal AutoHookIPC AutoHook;
    internal IceCosmicExplorationIPC IceIpc;

    public ICE(IDalamudPluginInterface pi)
    {
        P = this;
        ECommonsMain.Init(pi, P, Module.DalamudReflector, ECommons.Module.ObjectFunctions);
        new ECommons.Schedulers.TickScheduler(Load);
        PictoService.Initialize(pi);
    }
    public void Load()
    {
        EzConfig.Migrate<Config>();
        config = EzConfig.Init<Config>();

        //IPC's that are used
        Lifestream = new();
        Navmesh = new();
        Pandora = new();
        Artisan = new();
        Visland = new();
        AutoHook = new();
        IceIpc = new();

        // all the windows
        windowSystem = new();
        mainWindow = new();
        overlayWindow = new();
        debugWindow = new();
        infoWindow = new();
        externalDetails = new();

        EzCmd.Add("/icecosmic", OnCommand, """
            打开插件窗口
            /ice debug - 打开调试窗口
            /ice help - 显示所有命令
            /ice clear - 移除所有任务
            /ice stop - 停止 ICE 运行
            /ice start - 启动 ICE 运行
            /ice add | remove | toggle | only 
            /ice flag [id] - 打开地图并标记采集区域位置
            """);
        EzCmd.Add("/ice", OnCommand);
        EzCmd.Add("/IceCosmic", OnCommand);
        Init();
        Svc.Framework.Update += Tick;
        Svc.PluginInterface.UiBuilder.Draw += OnDraw;

        TaskManager = new(new(showDebug: false, timeLimitMS: 10 * 60 * 3000));
        Svc.PluginInterface.UiBuilder.Draw += windowSystem.Draw;
        Svc.PluginInterface.UiBuilder.OpenMainUi += () =>
        {
            mainWindow.IsOpen = true;
        };
        Svc.PluginInterface.UiBuilder.OpenConfigUi += () =>
        {
            mainWindow.IsOpen = true;
            SelectableSidebar.currentSelection = "helpSelect_AllSettings";
        };

        // timer stuff
        MissionTimer = new MissionTimer();

        MigrateConfigV1();

        DictionaryCreation();
        Task_Gamba.EnsureGambaWeightsInitialized();
        GatheringUtil.UpdateCriticalWeather();
        TestLoadRoutes();
    }

    private static void Init()
    {
        ExcelHelper.Init();
        ConsumableInfo.Init();
    }

    private void Tick(object _)
    {
        if (Player.Available)
        {
            PlayerHandlers.Tick();
            if (SchedulerMain.State != IceState.Idle)
                SchedulerMain.Tick();
            WeatherForecastHandler.Tick();
        }
        else
        {
            if (SchedulerMain.State != IceState.Idle)
                PlayerHandlers.DisablePlugin();
            if (PlayerHandlers.PlayerFirstCosmicZone)
                PlayerHandlers.PlayerFirstCosmicZone = false;
        }
        GenericManager.Tick();
        TextAdvancedManager.Tick();
        YesAlreadyManager.Tick();
    }

    private void OnDraw()
    {
        if (PlayerHelper.IsInCosmicZone())
            PictoManager.DrawPicto();
    }

    public void Dispose()
    {
        GenericHelpers.Safe(() => Svc.Framework.Update -= Tick);
        GenericHelpers.Safe(() => Svc.PluginInterface.UiBuilder.Draw -= OnDraw);
        GenericHelpers.Safe(() => Svc.PluginInterface.UiBuilder.Draw -= windowSystem.Draw);
        GenericHelpers.Safe(TextAdvancedManager.UnlockTA);
        GenericHelpers.Safe(YesAlreadyManager.Unlock);
        ECommonsMain.Dispose();
        PictoService.Dispose();
    }

    private void OnCommand(string command, string args)
    {
        var subcommands = args.Split(' ');

        if (subcommands.Length == 0 || args == "")
        {
            mainWindow.IsOpen = !mainWindow.IsOpen;
            return;
        }

        var firstArg = subcommands[0];

        if (firstArg.ToLower() == "d" || firstArg.ToLower() == "debug")
        {
            debugWindow.IsOpen = true;
            return;
        }
        else if (firstArg.ToLower() == "i")
        {
            infoWindow.IsOpen = true;
            return;
        }
        else if (firstArg.ToLower() == "s" || firstArg.ToLower() == "settings")
        {
            mainWindow.IsOpen = true;
            SelectableSidebar.currentSelection = "helpSelect_AllSettings";
            return;
        }
        else if (firstArg.ToLower() == "clear")
        {
            foreach (var mission in C.MissionConfig)
            {
                mission.Value.Enabled = false;
            }
            C.Save();
        }
        else if (firstArg.ToLower() == "stop")
        {
            SchedulerMain.DisablePlugin();
        }
        else if (firstArg.ToLower() == "start")
        {
            SchedulerMain.EnablePlugin();
        }
        else if (firstArg.ToLower() == "add")
        {
            uint[] ids = [.. subcommands.Skip(1).Select(uint.Parse)];
            var idSet = new HashSet<uint>(ids);
            if (ids.Length == 0) return;

            foreach (var id in idSet)
            {
                if (C.MissionConfig.TryGetValue(id, out var mission))
                {
                    mission.Enabled = true;
                }
            }
            C.Save();
        }
        else if (firstArg.ToLower() == "remove")
        {
            uint[] ids = [.. subcommands.Skip(1).Select(uint.Parse)];
            var idSet = new HashSet<uint>(ids);
            if (ids.Length == 0) return;

            foreach (var id in idSet)
            {
                if (C.MissionConfig.TryGetValue(id, out var mission))
                {
                    mission.Enabled = false;
                }
            }
            C.Save();
        }
        else if (firstArg.ToLower() == "toggle")
        {
            uint[] ids = [.. subcommands.Skip(1).Select(uint.Parse)];
            var idSet = new HashSet<uint>(ids);
            if (ids.Length == 0) return;

            foreach (var id in idSet)
            {
                if (C.MissionConfig.TryGetValue(id, out var mission))
                {
                    mission.Enabled = !mission.Enabled;
                }
            }
            C.Save();
        }
        else if (firstArg.ToLower() == "only")
        {
            uint[] ids = [.. subcommands.Skip(1).Select(uint.Parse)];
            var idSet = new HashSet<uint>(ids);
            if (ids.Length == 0) return;

            foreach (var mission in C.MissionConfig.Where(x => x.Value.Enabled))
            {
                mission.Value.Enabled = false;
            }
            foreach (var id in idSet)
            {
                if (C.MissionConfig.TryGetValue(id, out var mission))
                {
                    mission.Enabled = true;
                }
            }
        }
        else if (firstArg.ToLower() == "flag")
        {
            if (subcommands.Length != 2) return;
            if (!PlayerHelper.IsInCosmicZone()) return;

            int missionId = int.Parse(subcommands[1]);
            var info = SheetMissionDict.FirstOrDefault(mission => mission.Key == missionId);
            if (info.Value == default) return;
            if (info.Value.MarkerId == 0) return;

            Utils.SetGatheringRing(info.Value.TerritoryId, (int)info.Value.MapPosition.X, (int)info.Value.MapPosition.Y, info.Value.Radius, info.Value.Name);
        }
        else if (firstArg.ToLower() == "help")
        {
            string helpMessage = $"- - ICE 命令帮助 - - \n" +
                                 $"/ice help - 显示所有可用命令\n" +
                                 $"/ice -> 打开插件窗口\n" +
                                 $"/ice s -> 打开设置菜单\n" +
                                 $" - - - 任务相关命令 - - - \n" +
                                 $"/ice stop - 停止 ICE 运行\n" +
                                 $"/ice start - 启动 ICE 运行\n" +
                                 $"以下命令可以使用单个或多个任务 ID, 一次性执行\n" +
                                 $"例如: /ice add 10 155 185\n" +
                                 $"/ice add (ids) - 启用指定任务\n" +
                                 $"/ice remove (ids) - 移除/禁用指定任务\n" +
                                 $"/ice toggle (ids) - 切换指定任务的启用状态" +
                                 $"/ice only (ids) - 仅启用指定任务" +
                                 $"/ice flag (id) - 打开地图并标记任务(如果该任务有地图标记)\n";
            Svc.Chat.Print(helpMessage);
        }
    }

    public void TestLoadRoutes()
    {
        try
        {
            // Clear cache first to force reload
            GatheringRouteLoader.ClearCache();

            var routes = GatheringRouteLoader.LoadAllRoutes();

            IceLogging.Info($"Successfully loaded {routes.Count} zones with {routes.Sum(x => x.Value.Count)} total routes");

            // Test getting a specific route
            var testRoute = GatheringRouteLoader.GetRoute(1237, new Vector2(-690f, -752f));
            if (testRoute != null)
            {
                IceLogging.Info($"Test route loaded successfully with {testRoute.Count} nodes");
            }
        }
        catch (Exception ex)
        {
            IceLogging.Error($"Failed to load routes: {ex.Message}");
            IceLogging.Error(ex.StackTrace ?? "No stack trace");
        }
    }
}
