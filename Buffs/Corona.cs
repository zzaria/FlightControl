using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using Microsoft.Xna.Framework;

namespace FlightControl.Buffs
{
	public class Corona : ModBuff
	{
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<Player1>().corona = true;

			int[] debuffs = { BuffID.Venom, BuffID.OnFire, BuffID.OnFire3, BuffID.CursedInferno, BuffID.Frostburn, BuffID.Frostburn2, BuffID.ShadowFlame };
			int effectiveRange = 400;
			foreach (NPC target in Main.npc)
			{
				if (target.active && !target.friendly && !target.CountsAsACritter
					&& player.CanNPCBeHitByPlayerOrPlayerProjectile(target) 
					&& (double)Vector2.Distance(player.Center, target.Center) <= (double)effectiveRange)
				{
					foreach (int debuff in debuffs)
					{
						if (!target.buffImmune[debuff])
						{
							target.AddBuff(debuff, 300);
						}
					}
				}
			}
			player.AddBuff(BuffID.CursedInferno,5);
		}
	}
}