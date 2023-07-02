using Kitchen;
using KitchenData;
using KitchenMods;
using PreferenceSystem;
using Unity.Entities;
using UnityEngine;

// Namespace should have "Kitchen" in the beginning
namespace KitchenPrepPingBlueprintCabinet
{
    public class Main : IModInitializer
    {
        public const string MOD_GUID = "IcedMilo.PlateUp.PrepPingBlueprintCabinet";
        public const string MOD_NAME = "PrepPingBlueprintCabinet";
        public const string MOD_VERSION = "0.1.3";

        public const string PING_BLUEPRINT_IN_CABINET_ID = "prepPingBluePrintInCabinet";
        internal static PreferenceSystemManager PrefManager;

        public Main()
        {
        }

        public void PostActivate(Mod mod)
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");

            PrefManager = new PreferenceSystemManager(MOD_GUID, MOD_NAME);
            PrefManager
                .AddLabel("Ping Blueprint In Cabinet")
                .AddOption<string>(
                    PING_BLUEPRINT_IN_CABINET_ID,
                    "On",
                    new string[] { "Off", "On" },
                    new string[] { "Off", "On" })
                .AddSpacer()
                .AddSpacer();

            PrefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);
        }

        public void PreInject() { }

        public void PostInject() { }

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
    public class NewShowPingedCabinetInfo : ShowPingedApplianceInfo, IModSystem
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
