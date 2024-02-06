using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System;

namespace FlightControl.Items.Weapons
{
	public class NovaGun : ModItem
	{
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.DiamondStaff);
			
			Item.shoot=ModContent.ProjectileType<Projectiles.RailgunBullet>();
			Item.mana=0;
			Item.damage = 16384; 
			Item.shootSpeed=20;
			Item.knockBack=50;
			Item.useTime=Item.useAnimation=(int)(Item.useAnimation*2);
			Item.DamageType=DamageClass.Default;
			Item.crit=30;
		}
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<NovaFragment>(4)
				.AddIngredient<Items.Weapons.Railgun>(10)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
    }
}