using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using System.Collections.Generic;
using System;
using Terraria.GameContent.ItemDropRules;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.Linq;

namespace FlightControl.Npcs
{
	// The main part of the boss, usually refered to as "body"
	[AutoloadBossHead] // This attribute looks for a texture called "ClassName_Head_Boss" and automatically registers it as the NPC boss head icon
	public class EmpressEternal : ModNPC
	{/*
		// This code here is called a property: It acts like a variable, but can modify other things. In this case it uses the NPC.ai[] array that has four entries.
		// We use properties because it makes code more readable ("if (SecondStage)" vs "if (NPC.ai[0] == 1f)").
		// We use NPC.ai[] because in combination with NPC.netUpdate we can make it multiplayer compatible. Otherwise (making our own fields) we would have to write extra code to make it work (not covered here)
		public bool SecondStage {
			get => NPC.ai[0] == 1f;
			set => NPC.ai[0] = value ? 1f : 0f;
		}
		// If your boss has more than two stages, and since this is a boolean and can only be two things (true, false), concider using an integer or enum

		// Auto-implemented property, acts exactly like a variable by using a hidden backing field
		public Vector2 LastFirstStageDestination { get; set; } = Vector2.Zero;

		// This property uses NPC.localAI[] instead which doesn't get synced, but because SpawnedMinions is only used on spawn as a flag, this will get set by all parties to true.
		// Knowing what side (client, server, all) is in charge of a variable is important as NPC.ai[] only has four entries, so choose wisely which things you need synced and not synced
		public bool SpawnedMinions {
			get => NPC.localAI[0] == 1f;
			set => NPC.localAI[0] = value ? 1f : 0f;
		}

		private const int FirstStageTimerMax = 90;
		// This is a reference property. It lets us write FirstStageTimer as if it's NPC.localAI[1], essentially giving it our own name
		public ref float FirstStageTimer => ref NPC.localAI[1];

		public ref float RemainingShields => ref NPC.localAI[2];

		// We could also repurpose FirstStageTimer since it's unused in the second stage, or write "=> ref FirstStageTimer", but then we have to reset the timer when the state switch happens
		public ref float SecondStageTimer_SpawnEyes => ref NPC.localAI[3];

		// Do NOT try to use NPC.ai[4]/NPC.localAI[4] or higher indexes, it only accepts 0, 1, 2 and 3!
		// If you choose to go the route of "wrapping properties" for NPC.ai[], make sure they don't overlap (two properties using the same variable in different ways), and that you don't accidently use NPC.ai[] directly

/*
		public override void Load() {
			
		}
*/
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 63;

			// Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
			NPCID.Sets.MPAllowedEnemies[Type] = true;
			// Automatically group with other bosses
			NPCID.Sets.BossBestiaryPriority.Add(Type);

			// Specify the debuffs it is immune to
			NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
			{
				SpecificallyImmuneTo = new int[] {

					BuffID.Confused // Most NPCs have this
				}
			};
			NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
		}

		public override void SetDefaults()
		{
			NPC.width = 120;
			NPC.height = 120;
			NPC.damage = 80;
			NPC.defense = 999;
			NPC.lifeMax = 100000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.value = Item.buyPrice(gold: 5);
			NPC.SpawnWithHigherTime(30);
			NPC.boss = true;
			NPC.npcSlots = 10f; // Take up open spawn slots, preventing random NPCs from spawning during the fight
			NPC.scale = 0.5f;

			// Don't set immunities like this as of 1.4:
			// NPC.buffImmune[BuffID.Confused] = true;
			// immunities are handled via dictionaries through NPCID.Sets.DebuffImmunitySets

			// Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
			//NPC.aiStyle=-1;
			NPC.aiStyle = 120;
			AIType = 636;
			//AnimationType = 636;

			// The following code assigns a music track to the boss in a simple way.
			if (!Main.dedServ)
			{
				//Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Ropocalypse2");
			}
		}
		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * balance * bossAdjustment);
			NPC.damage = NPC.damage * 110 / 160;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// Sets the description of this NPC that is listed in the bestiary
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("Example Minion Boss that spawns minions on spawn, summoned with a spawn item. Showcases boss minion handling, multiplayer conciderations, and custom boss bar.")
			});
		}
		public override void UpdateLifeRegen(ref int damage)
		{
			NPC.lifeRegen += 2 * 200;
			
		}
		float prevAI;
		float prevAttack;
		//
		readonly float[] attacks = { 2, 4, 5, 6, 7, 8, 11, 12 };
		public void SummonFastHoming()
		{
			Vector2 vector2_24 = Main.rand.NextVector2Circular(1f, 1f) + Main.rand.NextVector2CircularEdge(3f, 3f);
			if ((double)vector2_24.Y > 0.0)
				vector2_24.Y *= -1f;
			//float num78 = (float) ((double) this.itemAnimation / (double) this.itemAnimationMax * 0.660000026226044) + this.miscCounterNormalized;
			var dmg = Main.expertMode ? 100 / 2 / 2 : 50 / 2;
			var id = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vector2_24, ProjectileID.FairyQueenMagicItemShot, dmg, 2.5f, Main.myPlayer);
			Main.projectile[id].friendly = false;
			Main.projectile[id].hostile = true;
		}
		/*
		homing: 2,12
		spiral: 5
		spinning blades: 6
		projectiles: 7, 4, 11
		dash: 8
		*/
		public override bool PreAI()
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if(NPC.realLife ==-1){
					foreach (var npc in Main.npc) if(npc.type==ModContent.NPCType<EmpressEternal>()){
						if(npc.realLife==-1&&NPC.whoAmI!=npc.whoAmI){
							npc.realLife=NPC.whoAmI;
						}
					}
				}
				if (prevAI == 10&&NPC.ai[0]!=10)
				{
					for (var i = 0; i < 16; i++)
					{
						SummonFastHoming();
					}
					foreach (var npc in Main.npc) if(npc.type==ModContent.NPCType<EmpressEternal>()){
						if(npc.realLife==NPC.whoAmI){
							npc.EncourageDespawn(10);
							npc.life=-9999;
						}
					}
				}
				if(NPC.life<NPC.lifeMax/10){
					if(Main.rand.Next(3)==0){
						SummonFastHoming();
					}
					return false;
				}
				Player player = Main.player[NPC.target];
				player.AddBuff(ModContent.BuffType<Buffs.Shatter>(), 60000);
				player.AddBuff(BuffID.WitheredArmor, 60000);
				player.AddBuff(BuffID.Ichor, 60000);
				if (prevAI == 1 && attacks.Contains(NPC.ai[0]))
				{
					if ((NPC.Center - player.Center).Length() < 320 && Main.rand.Next(2) == 0)
					{
						NPC.ai[0] = 5;
					}
					else if (prevAttack == 5 && Main.rand.Next(3) == 0)
					{
						NPC.ai[0] = Main.rand.Next(2) == 0 ? 2 : 12;
					}
					else
					{
						var x = Main.rand.Next(attacks.Length);

						NPC.ai[0] = attacks[x];
						if (x == 7) { }//do the other type of attack
					}
					if (NPC.life < NPC.lifeMax / 2 && Main.rand.Next(5) == 0)
					{
						for (var i = 0; i < 4; i++)
						{
							SummonFastHoming();
						}
					}
					prevAttack = NPC.ai[0];
				}
			}
			if (NPC.life < NPC.lifeMax / 2 && (NPC.ai[0]==7||NPC.ai[0] == 11 || NPC.ai[0] == 4))
			{
				var newDelay = NPC.ai[0] == 11 ? 50 : NPC.ai[0] == 4 ? 40:30;
				foreach (var proj in Main.projectile)
				{
					if (proj.type == ProjectileID.FairyQueenLance && proj.localAI[0] < 2)
					{
						proj.localAI[0] = newDelay;
					}
				}
			}
			prevAI = NPC.ai[0];
			return true;
		}
		Vector2 velocityReplace;
		public override void PostAI()
		{
			float[] attacks = {8,9}; 
			if(NPC.life<NPC.lifeMax/10){
				NPC.velocity=Vector2.Zero;
			}
			else if (!attacks.Contains(NPC.ai[0]))// && (NPC.velocity.Length() < 60 || velocityReplace.Length() > 16))
			{
				Player player = Main.player[NPC.target];
				DoFirstStage(player);
			}
			else{
				NPC.defense=9999;
			}
			velocityReplace = NPC.velocity;
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			// Trophies are spawned with 1/10 chance
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.NovaFragment>(), 1,60,100));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.EmpressLance>(), 2));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.EmpressLance2>(), 3));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.Weapons.EmpressLance3>(), 5));
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.EmpressOfLightIsGenuinelyEnraged(),ModContent.ItemType<Items.PlayerPoppet.PlayerVoodooPoppet>()));
		}
		/*
				public override void OnKill() {
					// This sets downedMinionBoss to true, and if it was false before, it initiates a lantern night
					//NPC.SetEventFlagCleared(ref DownedBossSystem.downedMinionBoss, -1);

					// Since this hook is only ran in singleplayer and serverside, we would have to sync it manually.
					// Thankfully, vanilla sends the MessageID.WorldData packet if a BOSS was killed automatically, shortly after this hook is ran

					// If your NPC is not a boss and you need to sync the world (which includes ModSystem, check DownedBossSystem), use this code:
					/*
					if (Main.netMode == NetmodeID.Server) {
						NetMessage.SendData(MessageID.WorldData);
					}

				}
		/*
				public override void BossLoot(ref string name, ref int potionType) {
					// Here you'd want to change the potion type that drops when the boss is defeated. Because this boss is early pre-hardmode, we keep it unchanged
					// (Lesser Healing Potion). If you wanted to change it, simply write "potionType = ItemID.HealingPotion;" or any other potion type
				}

				public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
					cooldownSlot = ImmunityCooldownID.Bosses; // use the boss immunity cooldown counter, to prevent ignoring boss attacks by taking damage from other sources
					return true;
				}
		*/
		public override void FindFrame(int frameHeight)
		{
			// This NPC animates with a simple "go from start frame to final frame, and loop back to start frame" rule
			// In this case: First stage: 0-1-2-0-1-2, Second stage: 3-4-5-3-4-5, 5 being "total frame count - 1"
			int startFrame = 0;
			int finalFrame = 62;
			/*
						if (SecondStage) {
							startFrame = 3;
							finalFrame = Main.npcFrameCount[NPC.type] - 1;

							if (NPC.frame.Y < startFrame * frameHeight) {
								// If we were animating the first stage frames and then switch to second stage, immediately change to the start frame of the second stage
								NPC.frame.Y = startFrame * frameHeight;
							}
						}*/

			int frameSpeed = 2;
			NPC.frameCounter += 0.5f;
			NPC.frameCounter += NPC.velocity.Length()*NPC.velocity.Length() / 10f; // Make the counter go faster with more movement speed
			if (NPC.frameCounter > frameSpeed)
			{
				NPC.frameCounter = 0;
				NPC.frame.Y += frameHeight;

				if (NPC.frame.Y > finalFrame * frameHeight)
				{
					NPC.frame.Y = startFrame * frameHeight;
				}
			}
		}
		/*
				public override void AI() {
					// This should almost always be the first code in AI() as it is responsible for finding the proper player target
					if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active) {
						NPC.TargetClosest();
					}

					Player player = Main.player[NPC.target];

					if (player.dead) {
						// If the targeted player is dead, flee
						NPC.velocity.Y -= 0.04f;
						// This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
						NPC.EncourageDespawn(10);
						return;
					}
					DoFirstStage(player);
				}//*/

		/*public override bool CheckActive (){
			return false;
		}*/
		readonly int[] ownProjectles={ProjectileID.HallowBossLastingRainbow,ProjectileID.FairyQueenLance,ProjectileID.HallowBossRainbowStreak,ProjectileID.FairyQueenSunDance};
		private void DoFirstStage(Player player)
		{
			// Each time the timer is 0, pick a random position a fixed distance away from the player but towards the opposite side
			// The NPC moves directly towards it with fixed speed, while displaying its trajectory as a telegraph


			float dodgeDistance = NPC.height * NPC.scale * 1.5f;
			float speed = 15;
			float acceleration = 1.5f;
			float followDistance = 320;
			float teleportDistance = 3200;
			//NPC.DiscourageDespawn(1000);

			Vector2 fromPlayer = NPC.Center - player.Center;

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				// Important multiplayer concideration: drastic change in behavior (that is also decided by randomness) like this requires
				// to be executed on the server (or singleplayer) to keep the boss in sync
				if (NPC.ai[3] >= 2)
				{
					speed = 20;
					acceleration = 4;
				}
				if (fromPlayer.Length() > teleportDistance)
				{
					NPC.position = player.position + new Vector2(0, -320);
					NPC.velocity = Vector2.Zero;
				}
				float bestScore = -999;
				float bestDist = 99999;
				Vector2 bestVector = new();
				float[] scores = new float[9];

				for (var i = 0; i < 9; i++)
				{
					Vector2 newVelocity = (i * 2 * MathHelper.Pi / 8).ToRotationVector2() * speed;
					if (i == 0)
					{
						newVelocity = new();
					}
					Vector2 newPos = NPC.Center + newVelocity;
					float score = 90;
					foreach (var p in Main.projectile)
					{
						if (p.CanHitWithOwnBody(NPC) && p.penetrate != 0 && p.timeLeft != 0 && p.damage > 500)
						{//why aren't some projectiles get deleted lol
							if(ownProjectles.Contains(p.type)&&p.hostile==true){
								continue; //why does this pass CanHitWithOwnBody???
							}
							float dodgeDistance2 = (float)(dodgeDistance + Math.Sqrt(p.width * p.width + p.height * p.height));
							if (p.velocity.Length() > 0)
							{
								float a = Vector2.Dot(newPos - p.Center, p.velocity) / p.velocity.Length();
								float b = (newPos - p.Center - a * Vector2.Normalize(p.velocity)).Length();

								if (b < dodgeDistance2 && a > 0)
								{

									float score2 = a / p.velocity.Length() - (dodgeDistance2 - b) / speed * 2;
									score = Math.Min(score, score2);
								}
							}
							else if ((newPos - p.Center).Length() < dodgeDistance2)
							{
								score = -999;
							}
						}
					}
					scores[i] = score;

					bestScore = Math.Max(bestScore, score);
				}
				NPC.velocity = velocityReplace;
				for (var i = 0; i < 9; i++)
				{
					Vector2 newVelocity = (i * 2 * MathHelper.Pi / 8).ToRotationVector2() * speed;
					if (i == 0)
					{
						newVelocity = new();
					}
					Vector2 newPos = NPC.Center + newVelocity;
					if (scores[i] >= bestScore)
					{
						if (i == 0 && (newPos - player.Center).Length() - followDistance < 0)
						{
							bestDist = -1;
							bestVector = newVelocity;
						}
						else if (Math.Max(0, (newPos - player.Center).Length() - followDistance) < bestDist)
						{
							bestDist = Math.Max((newPos - player.Center).Length() - followDistance, 0);
							bestVector = newVelocity;
						}
					}
				}
				if(bestVector.Length()>0){
					bestVector.Normalize();
				}
				if (fromPlayer.Length() > Math.Max(700,bestDist))
				{
					speed *= 1 + (fromPlayer.Length() - 700) / 100;
				}
				var proj = Math.Max(0, Vector2.Dot(NPC.velocity, bestVector)) * bestVector ;
				NPC.velocity = (NPC.velocity - proj) * 0.8f + proj + bestVector * acceleration;
				
				if (NPC.velocity.Length() > speed)
				{
					NPC.velocity.Normalize();
					NPC.velocity *= speed;
				}
				if (bestScore == -999 && Main.rand.Next(300) == 0)
				{
					NPC.velocity = Vector2.Normalize(fromPlayer) * 160;
				}
				NPC.netUpdate = true;
			}
		}
		/*
				private void DoSecondStage(Player player) {
					Vector2 toPlayer = player.Center - NPC.Center;

					float offsetX = 200f;

					Vector2 abovePlayer = player.Top + new Vector2(NPC.direction * offsetX, -NPC.height);

					Vector2 toAbovePlayer = abovePlayer - NPC.Center;
					Vector2 toAbovePlayerNormalized = toAbovePlayer.SafeNormalize(Vector2.UnitY);

					// The NPC tries to go towards the offsetX position, but most likely it will never get there exactly, or close to if the player is moving
					// This checks if the npc is "70% there", and then changes direction
					float changeDirOffset = offsetX * 0.7f;

					if (NPC.direction == -1 && NPC.Center.X - changeDirOffset < abovePlayer.X ||
						NPC.direction == 1 && NPC.Center.X + changeDirOffset > abovePlayer.X) {
						NPC.direction *= -1;
					}

					float speed = 8f;
					float inertia = 40f;

					// If the boss is somehow below the player, move faster to catch up
					if (NPC.Top.Y > player.Bottom.Y) {
						speed = 12f;
					}

					Vector2 moveTo = toAbovePlayerNormalized * speed;
					NPC.velocity = (NPC.velocity * (inertia - 1) + moveTo) / inertia;

					DoSecondStage_SpawnEyes(player);

					NPC.damage = NPC.defDamage;

					NPC.alpha = 0;

					NPC.rotation = toPlayer.ToRotation() - MathHelper.PiOver2;
				}*/
	};
}
