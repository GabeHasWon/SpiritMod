using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs.DoT;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.AsteroidDebris
{
	public class GoldDebris : ModNPC
	{
		public const int Chance = 80;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Debris");
			Main.npcFrameCount[NPC.type] = 5;
			NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
			NPCHelper.ImmuneTo<FesteringWounds>(this, BuffID.OnFire, BuffID.OnFire3, BuffID.Poisoned, BuffID.Venom);
		}
		
		public override void SetDefaults()
		{
			NPC.width = 36;
			NPC.height = 36;
			NPC.damage = 0;
			NPC.defense = 0;
			NPC.lifeMax = 500;
			NPC.dontCountMe = true;
			NPC.knockBackResist = .3f;
			NPC.noGravity = true;
			NPC.chaseable = false;
            NPC.lavaImmune = true;
			NPC.value = 43500f;
			NPC.aiStyle = -1;
			NPC.npcSlots = 0;
			NPC.HitSound = SoundID.NPCHit42;
			NPC.DeathSound = SoundID.NPCDeath44;
			NPC.GivenName = "Hit Me! ";
			NPC.alpha = 255; //The NPC will fade in on spawn

			AIType = 0;
		}

		private int Cooldown
		{
			get => (int)NPC.ai[0];
			set => NPC.ai[0] = value;
		}

		public override bool PreAI()
		{
			if (NPC.alpha > 0)
			{
				if (NPC.alpha == 255)
				{
					NPC.frameCounter = Main.rand.Next(Main.npcFrameCount[NPC.type]);
					NPC.netUpdate = true;
				} //Just spawned; select a random frame

				NPC.alpha = Math.Max(0, NPC.alpha - (255 / 10));

				//This is used in tandem with custom NPC spawning in MyPlayer to make sure it doesn't spawn on screen
				Rectangle screenRect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
				if (screenRect.Contains(NPC.Hitbox))
				{
					NPC.active = false;
					NPC.netUpdate = true;
				}
			}
			else return true;

			return false;
		}

		public override void AI()
		{
			NPC.TargetClosest();
			//Allow the player to bump asteroid debris by moving into them
			if (Main.player[NPC.target].Hitbox.Intersects(NPC.Hitbox) && Cooldown <= 0)
			{
				NPC.velocity += Main.player[NPC.target].velocity * .1f;
				Cooldown = 5;
			}

			if (Cooldown > 0)
				Cooldown--;

			if (Main.rand.NextBool(12))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, 
					0f, 0f, 0, default, Main.rand.NextFloat(0.5f, 1.0f));
				dust.noGravity = true;
				dust.velocity = Vector2.Zero;
			}

			NPC.rotation += 0.002f + NPC.velocity.Length() / 40;

			float top = (float)(Main.worldSurface * 0.34);
			if (NPC.position.Y / 16 > Main.worldSurface * 0.36f) //Apply progressive gravity effects below space
			{
				NPC.velocity.Y += 0.2f;

				if (NPC.collideY && Math.Abs(NPC.velocity.Y) > 4)
				{
					if (Main.netMode != NetmodeID.Server)
					{
						static float Direction() => Main.rand.NextFloat(-1.0f, 1.0f) * 3f;

						int randomAmount = Main.rand.Next(4, 7);
						for (int i = 0; i < randomAmount; i++)
							Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Direction(), Direction()), Mod.Find<ModGore>("AsteroidDebrisSmall").Type);
					}

					NPC.active = false;
				} //Impact
			}
			else if (NPC.position.Y / 16 > top)
			{
				float dist = (NPC.position.Y / 16f) - top;
				NPC.velocity.Y += 0.2f * (dist / (float)(Main.worldSurface * 0.36f - top));
			}
			else //Decelerate otherwise
			{
				NPC.velocity *= 0.99f;
			}
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				static float Direction() => Main.rand.NextFloat(-1.0f, 1.0f) * 3f;

				int randomAmount = Main.rand.Next(4, 7);
				for (int i = 0; i < randomAmount; i++)
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Direction(), Direction()), Mod.Find<ModGore>("AsteroidDebrisSmall").Type);
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			float quoteant = 1f - (float)(NPC.life / (float)NPC.lifeMax);
			float offset = Main.rand.NextFloat((float)(quoteant * 3f));
			Texture2D glowTex = ModContent.Request<Texture2D>("SpiritMod/Effects/Masks/CircleGradient").Value;
			Color glowCol = (Color.Goldenrod * quoteant) with { A = 0 };

			spriteBatch.Draw(glowTex, NPC.Center - screenPos, null, glowCol, NPC.rotation, glowTex.Size() / 2, NPC.scale * .35f, SpriteEffects.None, 0f);
			Utilities.DrawGodray.DrawGodrays(spriteBatch, NPC.Center - screenPos, Color.Goldenrod * (quoteant * .75f), quoteant * 45f, 20f, 5);

			Texture2D texture = TextureAssets.Npc[NPC.type].Value;
			Rectangle rect = texture.Frame(1, Main.npcFrameCount[Type], 0, (int)NPC.frameCounter, sizeOffsetY: -2);

			spriteBatch.Draw(texture, NPC.Center + new Vector2(offset) - screenPos, rect, NPC.GetAlpha(drawColor), NPC.rotation, rect.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
			
			return false;
		}

		public override void ModifyHoverBoundingBox(ref Rectangle boundingBox) => boundingBox = NPC.Hitbox;
	}
}