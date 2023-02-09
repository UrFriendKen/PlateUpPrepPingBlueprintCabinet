using Kitchen;
using KitchenLib;
using KitchenLib.Utils;
using KitchenLib.Event;
using KitchenMods;
using System.Reflection;
using UnityEngine;
using System;
using Unity.Entities;
using KitchenData;
using KitchenPrepPingBlueprintCabinet.Menus;

// Namespace should have "Kitchen" in the beginning
namespace KitchenPrepPingBlueprintCabinet
{
    public class Main : BaseMod
    {
        // guid must be unique and is recommended to be in reverse domain name notation
        // mod name that is displayed to the player and listed in the mods menu
        // mod version must follow semver e.g. "1.2.3"
        public const string MOD_GUID = "IcedMilo.PlateUp.PrepPingBlueprintCabinet";
        public const string MOD_NAME = "PrepPingBlueprintCabinet";
        public const string MOD_VERSION = "0.1.2";
        public const string MOD_AUTHOR = "IcedMilo";
        public const string MOD_GAMEVERSION = ">=1.1.1";
        // Game version this mod is designed for in semver
        // e.g. ">=1.1.1" current and all future
        // e.g. ">=1.1.1 <=1.2.3" for all from/until

        public Main() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        public const string PING_BLUEPRINT_IN_CABINET_ID = "prepPingBluePrintInCabinet";
        public const string PING_BLUEPRINT_IN_CABINET_INITIAL = "On";
        public string PingBlueprintInCabinet;
        public static readonly MenuPreference PingBlueprintInCabinetPreference = new MenuPreference(PING_BLUEPRINT_IN_CABINET_ID,
                                                                                                    PING_BLUEPRINT_IN_CABINET_INITIAL,
                                                                                                    "Ping Blueprint In Cabinet");

        public bool ModEnabled = false;

        NewShowPingedCabinetInfo NewShowPingedCabinetInfoSystem = new NewShowPingedCabinetInfo();

        protected override void OnPostActivate(Mod mod)
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
            PingBlueprintInCabinetPreference.Register(MOD_GUID);
            PreferenceUtils.Load();
            PingBlueprintInCabinet = PingBlueprintInCabinetPreference.Load(MOD_GUID);
            ModEnabled = PingBlueprintInCabinet == "On" ? true : false;
            SetupPreferences();
        }

        protected override void OnInitialise()
        {
            base.OnInitialise();
            try
            {
                World.AddSystem(NewShowPingedCabinetInfoSystem);
            }
            catch
            {
                LogInfo("Could not add system KitchenPrepPingBlueprintCabinet.NewShowPingedCabinetInfoSystem!");
            }
        }

        protected override void OnUpdate()
        {
            try
            {
                ModEnabled = PingBlueprintInCabinetPreference.Load(MOD_GUID) == "On" ? true : false;
                World.GetExistingSystem(typeof(ShowPingedCabinetInfo)).Enabled = !ModEnabled;
                World.GetExistingSystem(typeof(NewShowPingedCabinetInfo)).Enabled = ModEnabled;
            }
            catch (NullReferenceException)
            {
            }
        }
        private void SetupPreferences()
        {
            ModsPreferencesMenu<MainMenuAction>.RegisterMenu(MOD_NAME, typeof(PrepPingBlueprintCabinetPreference<MainMenuAction>), typeof(MainMenuAction));
            Events.PreferenceMenu_MainMenu_CreateSubmenusEvent += (s, args) =>
            {
                args.Menus.Add(typeof(PrepPingBlueprintCabinetPreference<MainMenuAction>), new PrepPingBlueprintCabinetPreference<MainMenuAction>(args.Container, args.Module_list));
            };

            //Setting Up For Pause Menu
            ModsPreferencesMenu<PauseMenuAction>.RegisterMenu(MOD_NAME, typeof(PrepPingBlueprintCabinetPreference<PauseMenuAction>), typeof(PauseMenuAction));
            Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) =>
            {
                args.Menus.Add(typeof(PrepPingBlueprintCabinetPreference<PauseMenuAction>), new PrepPingBlueprintCabinetPreference<PauseMenuAction>(args.Container, args.Module_list));
            };

            Events.PreferencesSaveEvent += (s, args) =>
            {
                PingBlueprintInCabinet = PingBlueprintInCabinetPreference.Load(MOD_GUID);
            };
        }

        #region Logging
        // You can remove this, I just prefer a more standardized logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }


    [UpdateBefore(typeof(ShowPingedApplianceInfo))]
    public class NewShowPingedCabinetInfo : ShowPingedApplianceInfo
    {
        private CBlueprintStore BlueprintStore;

        protected override bool AllowAnyMode => true;

        protected override bool IsPossible(ref InteractionData data)
        {
            if (!Require<CBlueprintStore>(data.Target, out BlueprintStore))
            {
                return false;
            }
            if (Has<CShowApplianceInfo>(data.Target))
            {
                return false;
            }
            if (!GameData.Main.TryGet<Appliance>(BlueprintStore.ApplianceID, out var output) || output.Name == "")
            {
                return false;
            }

            return true;
        }

        protected override void Perform(ref InteractionData data)
        {
            data.Context.Set(data.Target, new CTemporaryApplianceInfo
            {
                RemainingLifetime = 0.2f
            });
            data.Context.Set(data.Target, new CShowApplianceInfo
            {
                Appliance = BlueprintStore.ApplianceID,
                ShowPrice = true,
                Price = BlueprintStore.Price
            });
        }
    }

}
