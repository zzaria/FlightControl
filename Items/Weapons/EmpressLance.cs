using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System;

namespace FlightControl.Items.Weapons
{
	public class EmpressLance : ModItem
	{
			public override void SetDefaults() {
			// This method right here is the backbone of what we're doing here; by using this method, we copy all of
			// the meowmere's SetDefault stats (such as Item.melee and Item.shoot) on to our item, so we don't have to
			// go into the source and copy the stats ourselves. It saves a lot of time and looks much cleaner; if you're
			// going to copy the stats of an item, use CloneDefaults().

			Item.CloneDefaults(ItemID.LunarFlareBook);
			// Check out ExampleCloneProjectile to see how this projectile is different from the Vanilla Meowmere projectile.

			// While we're at it, let's make our weapon's stats a bit stronger than the Meowmere, which can be done
			// by using math on each given stat.
			Item.useStyle=5;
			Item.mana=8;
			Item.damage = 6000; 
			Item.useAnimation=Item.useTime/=3;
			Item.shoot=ProjectileID.FairyQueenLance;
			Item.DamageType=DamageClass.Default;
			Item.crit=35;
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {

			var mousePos=new Vector2(Main.mouseX,Main.mouseY);
			var lastMousePos=new Vector2(Main.lastMouseX,Main.lastMouseY);
			var dir=mousePos-lastMousePos;
			if(dir==Vector2.Zero){
				dir=Main.rand.NextVector2Unit();
			}
			dir.Normalize();
			var spawnPos=mousePos-dir*1000+Main.screenPosition;
			var proj=Projectile.NewProjectile(source, spawnPos,Vector2.Zero, type, damage, knockback, player.whoAmI,dir.ToRotation(),Main.rand.Next(0,100)/100f);
			Main.projectile[proj].friendly = true;
			Main.projectile[proj].hostile = false;
			Main.projectile[proj].localAI[0]=30;
			// We do not want vanilla to spawn a duplicate projectile.
			return false;
		}
    }
}