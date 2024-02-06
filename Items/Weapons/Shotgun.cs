using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System;

namespace FlightControl.Items.Weapons
{
	public class Shotgun : ModItem
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
			Item.mana=14;
			Item.damage = 500; 
			Item.shootSpeed=20;
			Item.useTime=Item.useAnimation=(int)(Item.useAnimation);
			Item.DamageType=DamageClass.Default;
			Item.crit=10;
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			for (int index = -2; index <=2; ++index)
			{
				var dir=velocity.ToRotation();
				dir+=(float)(index*Math.PI/12);
				var newVelocity=dir.ToRotationVector2()*velocity.Length();
				Projectile.NewProjectile(source,position, newVelocity, index==0? ProjectileID.PrincessWeapon: type, (int)(index==0? damage*1.5:damage), knockback, player.whoAmI);
			}

			// We do not want vanilla to spawn a duplicate projectile.
			return false;
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if(player.GetModPlayer<PvpArmor.PvpPlayer>().slowProjectiles){
				velocity*=0.5f;
			}
        }
		public override void AddRecipes() {
			CreateRecipe().AddIngredient(ItemID.LunarBar,10)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
    }
}