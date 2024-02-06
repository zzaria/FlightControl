using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FlightControl.Items
{
	public class NeutroniumBar : ModItem
	{
		public override void SetDefaults() {
			Item.maxStack = Item.CommonMaxStack;
		}
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<NovaFragment>(10)
				.AddIngredient(ItemID.LunarBar,10)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}
}