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
	public class AsteroidDebris : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 5;
			NPCID.Sets.ImmuneToAllBuffs[Type] = true;

			NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new() { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, bestiaryData);
		}
		
		public override void SetDefaults()
		{
			NPC.width = 36;
			NPC.height = 36;
			NPC.damage = 0;
			NPC.defense = 0;
			NPC.lifeMax = 1;
			NPC.dontCountMe = true;
			NPC.knockBackResist = .8f;
			NPC.noGravity = true;
			NPC.chaseable = false;
            NPC.lavaImmune = true;
			NPC.value = 0f;
			NPC.aiStyle = -1;
			NPC.npcSlots = 0;
			NPC.alpha = 255; //The NPC will fade in on spawn
			NPC.ShowNameOnHover = false;

            AIType = 0;
		}

		private int HitCooldown
		{
			get => (int)NPC.ai[0];
			set => NPC.ai[0] = value;
		}

		public override bool CheckDead()
		{
			if (NPC.life < 1)
				NPC.life = 1;

			return false;
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
			else 
				return true;

			return false;
		}

		public override void AI()
		{
			if (HitCooldown <= 0)
			{
				NPC.TargetClosest();

				if (Main.player[NPC.target].Hitbox.Intersects(NPC.Hitbox)) //Allow the player to bump asteroid debris by moving into them
				{
					Bump(Main.player[NPC.target].velocity * .1f, 5);
				}
				else
				{
					foreach (Projectile proj in Main.projectile)
					{
						if (proj.getRect().Intersects(NPC.getRect()))
							Bump(proj.velocity * .4f * NPC.knockBackResist, 12);
					}
				}

				void Bump(Vector2 newVelocity, int cooldown)
				{
					NPC.velocity += newVelocity;
					HitCooldown = cooldown;

					NPC.netUpdate = true;
				}
			}
			else 
				HitCooldown--;

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
				NPC.velocity *= 0.99f;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Texture2D texture = TextureAssets.Npc[NPC.type].Value;
			Rectangle rect = texture.Frame(1, Main.npcFrameCount[Type], 0, (int)NPC.frameCounter, sizeOffsetY: -2);

			spriteBatch.Draw(texture, NPC.Center - screenPos, rect, NPC.GetAlpha(drawColor), NPC.rotation, rect.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
			
			return false;
		}

		public override bool? CanBeHitByProjectile(Projectile projectile) => false;
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;
		public override void ModifyHoverBoundingBox(ref Rectangle boundingBox) => boundingBox = NPC.Hitbox;
	}
}