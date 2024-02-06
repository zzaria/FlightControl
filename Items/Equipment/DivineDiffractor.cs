using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace FlightControl.Items.Equipment
{
	public class DivineDiffractor : ModItem
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.FlightControl.hjson file.

		public override void SetDefaults()
		{
			Item.value = 10000;
			Item.rare = 7;
			Item.accessory=true;
			Item.defense=10;
		}

		public override void UpdateAccessory(Player player,bool hideVisual ){
			player.onHitDodge =true;
		}
	}
}