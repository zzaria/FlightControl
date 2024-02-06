using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FlightControl.Items
{
	public class NovaFragment : ModItem
	{
		public override void SetDefaults() {
			Item.maxStack = Item.CommonMaxStack;
		}
	}
}