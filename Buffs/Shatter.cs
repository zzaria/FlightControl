using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace FlightControl.Buffs
{
    public class Shatter : ModBuff
    {
		const int factor=4;
		public override void Update(Player player, ref int buffIndex) {
			player.hurtCooldowns[1]=player.hurtCooldowns[1];
			if(player.immuneTime>factor){
				player.immuneTime-=factor;
			}
			else if(player.immuneTime>1){
				player.immuneTime=1;
			}
			if(player.hurtCooldowns[1]>factor){
				player.hurtCooldowns[1]-=factor;
			}
			else if(player.hurtCooldowns[1]>1){
				player.hurtCooldowns[1]=1;
			}
		}
    }
}