using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System;

namespace FlightControl.Items.Weapons
{
	public class Spiral : ModItem
	{
			public override void SetDefaults() {
			// This method right here is the backbone of what we're doing here; by using this method, we copy all of
			// the meowmere's SetDefault stats (such as Item.melee and Item.shoot) on to our item, so we don't have to
			// go into the source and copy the stats ourselves. It saves a lot of time and looks much cleaner; if you're
			// going to copy the stats of an item, use CloneDefaults().

			Item.CloneDefaults(ItemID.DiamondStaff);
			// Check out ExampleCloneProjectile to see how this projectile is different from the Vanilla Meowmere projectile.

			// While we're at it, let's make our weapon's stats a bit stronger than the Meowmere, which can be done
			// by using math on each given stat.
			Item.damage = 750; 
			Item.shootSpeed=16;
			Item.useTime=Item.useAnimation=5;
			Item.shoot=ProjectileID.StarCannonStar;
			Item.mana=4;
			Item.DamageType=DamageClass.Default;
		}
		static int timer=0;
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			timer++;
			for (int index = 0; index <6; ++index)
			{	
				var newVelocity=((float)((timer*Item.useTime/60f/4+index/6f)*2*Math.PI)).ToRotationVector2()*Item.shootSpeed;
				Projectile.NewProjectile(source,position, newVelocity, type, damage, knockback, player.whoAmI);
			}

			// We do not want vanilla to spawn a duplicate projectile.
			return false;
		}
		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.LunarBar,10)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
    }
}