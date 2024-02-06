
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using System.Collections.Generic;
using System;
using Terraria.GameContent.ItemDropRules;
using FlightControl.Items;
using FlightControl.Items.Equipment;

namespace FlightControl
{
    public class ModGlobalNPC : GlobalNPC
    {
        public override void SetDefaults(NPC npc){
            NPC.LunarShieldPowerMax=NPC.LunarShieldPowerNormal=0;
            switch(npc.type){
                case NPCID.LunarTowerNebula:
                case NPCID.LunarTowerSolar:
                case NPCID.LunarTowerStardust:
                case NPCID.LunarTowerVortex:
                    npc.lifeMax= 200000;
                    break;
            }
        }
        public override void ApplyDifficultyAndPlayerScaling (NPC npc,int numPlayers, float balance, float bossAdjustment){

        }
        public override void BuffTownNPC(ref float damageMult,ref int defense)
        {
			if(Main.expertMode){
				defense=(int)(defense * 1.5);
			}
			if(Main.masterMode){
				defense*=2;
			}
        }
        
        //Change the spawn pool
        public override void EditSpawnPool(IDictionary< int, float > pool, NPCSpawnInfo spawnInfo)
        {
            //If the custom invasion is up and the invasion has reached the spawn pos
            if(Main.invasionType==CelebrationEvent.EventId)
            {
                //Clear pool so that only the stuff you want spawns
                pool.Clear();
   
                //key = NPC ID | value = spawn weight
                //pool.add(key, value)
       
                //For every ID inside the invader array in our CelebrationEvent file
                foreach((int,float) i in CelebrationEvent.invaders()[CelebrationEvent.wave])
                {
                    pool.Add(i.Item1, i.Item2); //Add it to the pool with the same weight of 1
                }
            }
        }

        //Changing the spawn rate
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            //Change spawn stuff if invasion up and invasion at spawn
            if(Main.invasionType==CelebrationEvent.EventId&&CelebrationEvent.spawnRate>0)
            {
                spawnRate /= CelebrationEvent.spawnRate; //Higher the number, the more spawns
                maxSpawns *= CelebrationEvent.spawnRate; //Max spawns of NPCs depending on NPC value
            }
        }
        
        static bool dayTime;
        public override bool PreAI(NPC npc)
        {
            /*if(npc.type==NPCID.DukeFishron&&Main.expertMode && (double) npc.life <= (double) npc.lifeMax * 0.15&&npc.ai[2]==0&&npc.ai[3]>=6){
                var x=Main.rand.Next(2);
                if(x==0){
                    npc.ai[3]=4;
                }
                else{
                    npc.ai[3]=6;
                }
            }*/
            if(Main.invasionType==CelebrationEvent.EventId){
                dayTime=Main.dayTime;
                Main.dayTime=false;
                Main.eclipse=true;
            }
            return true;
        }
        //Adding to the AI of an NPC
        public override void PostAI(NPC npc)
        {
            //Changes NPCs so they do not despawn when invasion up and invasion at spawn
            if(Main.invasionType==CelebrationEvent.EventId)
            {
                npc.boss=false;
                Main.dayTime=dayTime;
                Main.eclipse=false;
            }
        }

        public override void OnKill(NPC npc)
        {
            //When an NPC (from the invasion list) dies, add progress by decreasing size
            if(Main.invasionType==CelebrationEvent.EventId)
            {
                //Gets IDs of invaders from CelebrationEvent file
                foreach((int,float) invader in CelebrationEvent.invaders()[CelebrationEvent.wave])
                {
                    //If npc type equal to invader's ID decrement size to progress invasion
                    if(npc.type == invader.Item1)
                    {
                        Main.invasionSize -= 1;
                        Main.invasionProgress++;
                    }
                }
            }
        }
        public override void ModifyNPCLoot (NPC npc, NPCLoot npcLoot){
            if(npc.type==NPCID.DukeFishron){
                npcLoot.Add(ItemDropRule.NormalvsExpert(ModContent.ItemType<SeafoodSnatcher>(), 3, 2));
            }
            if(npc.type==NPCID.HallowBoss){
                npcLoot.Add(ItemDropRule.ByCondition(new Conditions.EmpressOfLightIsGenuinelyEnraged(),ModContent.ItemType<PermanantSoaringInsignia>()));
                npcLoot.Add(ItemDropRule.ByCondition(new Conditions.EmpressOfLightIsGenuinelyEnraged(),ModContent.ItemType<DivineDiffractor>()));
            }
            //doesn't work because this function doesn't regularly get called
            if(Main.invasionType==CelebrationEvent.EventId){
                var loots=npcLoot.Get();
                foreach(var loot in loots){
                    npcLoot.Remove(loot);
                }
            }
        }
    }
}
