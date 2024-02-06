using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System;

namespace FlightControl.Items.Weapons
{
	public class Autocannon : ModItem
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
			Item.mana=0;
			Item.damage = 1000; 
			Item.shootSpeed=21/3;
			Item.useTime=Item.useAnimation=(int)(Item.useAnimation/3);
			Item.shoot=ProjectileID.NanoBullet;
			Item.DamageType=DamageClass.Default;
			Item.crit=10;
		}
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if(player.GetModPlayer<PvpArmor.PvpPlayer>().slowProjectiles){
				velocity*=0.7f;
			}
        }

        public override void AddRecipes() {
			CreateRecipe().AddIngredient(ItemID.LunarBar,10)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
    }
}