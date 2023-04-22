using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.AsteroidDebris
{
	public class AsteroidDebris : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Debris");
			Main.npcFrameCount[NPC.type] = 5;
			NPCID.Sets.NPCBestiaryDrawModifiers bestiaryData = new(0)
			{
				Hide = true // Hides this NPC from the bestiary
			};
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
			NPC.knockBackResist = 0;
			NPC.dontTakeDamageFromHostiles = true;
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

		private int Cooldown
		{
			get => (int)NPC.ai[0];
			set => NPC.ai[0] = value;
		}
		private bool Shiny { get => NPC.ai[1] != 0; set => NPC.ai[1] = value ? 1 : 0; }

		private int npcFrame;
		private bool fadingIn = true;

		public override bool CheckDead()
		{
			if (!Shiny)
			{
				if (NPC.life < 1)
					NPC.life = 1;
				return false;
			}
			return true;
		}

		public override void OnKill()
		{
			for (int i = 0; i < 4; i++)
				Gore.NewGore(Entity.GetSource_Death(), NPC.Center, new Vector2(Main.rand.NextFloat(1.0f, 1.0f), Main.rand.NextFloat(1.0f, 1.0f)), Mod.Find<ModGore>("AsteroidDebrisSmall").Type);
		}

		public override void OnSpawn(IEntitySource source)
		{
			Shiny = Main.rand.NextBool(80);

			if (Shiny)
			{
				NPC.lifeMax = 500;
				NPC.life = NPC.lifeMax;
				NPC.value = 43500f;
				NPC.HitSound = SoundID.NPCHit42;
				NPC.DeathSound = SoundID.NPCDeath44;
				NPC.GivenName = "Hit Me!";
			}

			NPC.frameCounter = Main.rand.Next(Main.npcFrameCount[NPC.type]);
			NPC.netUpdate = true;
		}

		public override bool PreAI()
		{
			if (fadingIn)
			{
				if (NPC.alpha > 0)
					NPC.alpha -= 255 / 10;
				else
					fadingIn = false;
				if (NPC.alpha < 0)
					NPC.alpha = 0;

				//This is used in tandem with custom NPC spawning in MyPlayer to make sure it doesn't spawn on screen
				Rectangle screenRect = new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Main.screenWidth, Main.screenHeight);
				if (screenRect.Contains(NPC.Hitbox))
				{
					NPC.active = false;
					NPC.netUpdate = true;
				}
			}
			return !fadingIn;
		}

		public override void AI()
		{
			NPC.TargetClosest();
			//Allow the player to "bump" asteroid debris by moving into them
			if (Main.player[NPC.target].Hitbox.Intersects(NPC.Hitbox))
				Bump(Main.player[NPC.target].velocity * .1f);

			if (Shiny && Main.rand.NextBool(12))
			{
				Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GoldCoin, 
					0f, 0f, 0, default, Main.rand.NextFloat(0.5f, 1.0f));
				dust.noGravity = true;
				dust.velocity = Vector2.Zero;
			}

			NPC.rotation += 0.002f + NPC.velocity.Length() / 40;
			if (Cooldown > 0)
				Cooldown--;

			float top = (float)(Main.worldSurface * 0.34);

			if (NPC.position.Y / 16 > Main.worldSurface * 0.36f)
			{
				NPC.velocity.Y += 0.2f;

				if (NPC.collideY && Math.Abs(NPC.velocity.Y) > 4)
					Impact();
			}
			else if (NPC.position.Y / 16 > top)
			{
				float dist = (NPC.position.Y / 16f) - top;
				NPC.velocity.Y += 0.2f * (dist / (float)(Main.worldSurface * 0.36f - top));
			}
			else
			{
				NPC.velocity *= 0.99f;
			}
		}

		private void Impact()
		{
			static float Direction() => Main.rand.NextFloat(-1.0f, 1.0f) * 3f;

			int randomAmount = Main.rand.Next(4, 7);
			for (int i = 0; i < randomAmount; i++)
				Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Direction(), Direction()), Mod.Find<ModGore>("AsteroidDebrisSmall").Type);

			NPC.active = false;
		}

		public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit) => Bump(NPC.DirectionFrom(player.Center) * (knockback / 2));

		public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			if (projectile.Hitbox.Intersects(NPC.Hitbox))
				Bump((projectile.velocity * .3f).RotatedByRandom(0.5f));

			return Shiny;
		}

		private void Bump(Vector2 newVelocity)
		{
			if (newVelocity == Vector2.Zero || Cooldown > 0)
				return;
			NPC.velocity += newVelocity;
			NPC.netUpdate = true;
			Cooldown = 5;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			float offset = 0;
			if (Shiny)
			{
				float gradient = 1f - (float)(NPC.life / (float)NPC.lifeMax);
				offset = Main.rand.NextFloat((float)(gradient * 3f));
				Texture2D glowTex = ModContent.Request<Texture2D>("SpiritMod/Effects/Masks/CircleGradient").Value;
				Color glowCol = Color.Goldenrod * gradient;
				glowCol.A = 0;
				spriteBatch.Draw(glowTex, NPC.Center - screenPos, null, glowCol, NPC.rotation, glowTex.Size() / 2, NPC.scale * .35f, SpriteEffects.None, 0f);
				Utilities.DrawGodray.DrawGodrays(spriteBatch, NPC.Center - screenPos, Color.Goldenrod * (gradient * .75f), gradient * 45f, 20f, 5);
			}

			Texture2D texture = TextureAssets.Npc[NPC.type].Value;
			Rectangle rect = new Rectangle(texture.Width / 2 * (Shiny ? 1 : 0), texture.Height / Main.npcFrameCount[NPC.type] * npcFrame, 
				texture.Width / 2 - 2, texture.Height / Main.npcFrameCount[NPC.type] - 2);
			spriteBatch.Draw(texture, NPC.Center + new Vector2(offset) - screenPos, rect, NPC.GetAlpha(drawColor), NPC.rotation, rect.Size() / 2, NPC.scale, SpriteEffects.None, 0f);
			return false;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => Shiny ? null : false;

		public override void ModifyHoverBoundingBox(ref Rectangle boundingBox) => boundingBox = NPC.Hitbox;

		public override void FindFrame(int frameHeight)
		{
			npcFrame = (int)NPC.frameCounter;
			NPC.frame.Y = npcFrame * frameHeight;
		}
	}
}