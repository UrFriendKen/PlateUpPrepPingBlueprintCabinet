using Kitchen.Modules;
using Kitchen;
using KitchenLib.Utils;
using KitchenLib;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenPrepPingBlueprintCabinet.Menus
{
    public class PrepPingBlueprintCabinetPreference<T> : KLMenu<T>
    {
        public PrepPingBlueprintCabinetPreference(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

        private Option<string> PingBlueprintInCabinet;

		public override void Setup(int player_id)
		{
			List<string> PingBlueprintInCabinet_Text = new List<string>() { "Off" , "On"  };
			this.PingBlueprintInCabinet = new Option<string>(PingBlueprintInCabinet_Text,
															 Main.PingBlueprintInCabinetPreference.Load(Main.MOD_GUID),
															 PingBlueprintInCabinet_Text);

			AddLabel(Main.PingBlueprintInCabinetPreference.PreferenceDisplayText);
			Add<string>(this.PingBlueprintInCabinet).OnChanged += delegate (object _, string f)
			{
				PreferenceUtils.Get<KitchenLib.StringPreference>(Main.MOD_GUID, Main.PING_BLUEPRINT_IN_CABINET_ID).Value = f;
				Main.LogInfo($"\"Ping Blueprint In Cabinet\" switched to {f}");
			};

			New<SpacerElement>();
			New<SpacerElement>();

			AddButton("Apply", delegate
			{
				PreferenceUtils.Save();
				RequestPreviousMenu();
			});

			AddButton(base.Localisation["MENU_BACK_SETTINGS"], delegate
			{
				RequestPreviousMenu();
			});

		}
	}
}
