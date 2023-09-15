using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Items.Sets.TideDrops;
using Terraria.GameContent.Bestiary;
using SpiritMod.Biomes.Events;

namespace SpiritMod.NPCs.Tides
{
	public class MangoJelly : ModNPC
	{
		private ref float AiTimer => ref NPC.ai[0];
		private bool Attacking => AiTimer >= cooldownTime;

		private bool Jumping
		{
			get => NPC.ai[1] != 0;
			set => NPC.ai[1] = value ? 1 : 0;
		}

		private readonly int cooldownTime = 400;
		private readonly int chargeTime = 150;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Mang-O War");
			Main.npcFrameCount[NPC.type] = 4;
			NPCID.Sets.TrailCacheLength[NPC.type] = 4;
			NPCID.Sets.TrailingMode[NPC.type] = 0;
		}

		public override void SetDefaults()
		{
			NPC.width = 40;
			NPC.height = 50;
			NPC.damage = 30;
			NPC.defense = 6;
			NPC.lifeMax = 225;
			NPC.noGravity = true;
			NPC.knockBackResist = .9f;
			NPC.value = 200f;
			NPC.alpha = 35;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit25;
			NPC.DeathSound = SoundID.NPCDeath28;
			Banner = NPC.type;
			BannerItem = ModContent.ItemType<Items.Banners.MangoWarBanner>();
			SpawnModBiomes = new int[1] { ModContent.GetInstance<TideBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) => bestiaryEntry.AddInfo(this, "");

		public override void HitEffect(NPC.HitInfo hit)
		{
			for (int k = 0; k < 20; k++)
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.ShadowbeamStaff, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, 0.7f);

			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
				for (int i = 1; i < 7; ++i)
					Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("MangoJelly" + i).Type, 1f);
		}

		public override void AI()
		{
			NPC.TargetClosest();
			Player player = Main.player[NPC.target];

			int xOffset = 24 * Math.Sign(player.position.X - NPC.position.X);

			if (++AiTimer >= cooldownTime)
			{
				if (AiTimer == (cooldownTime + chargeTime) && Main.netMode != NetmodeID.MultiplayerClient)
				{
					Vector2 vel = new Vector2(30f, 0).RotatedBy((float)(Main.rand.Next(90) * Math.PI / 180));
					SoundEngine.PlaySound(SoundID.Item91, NPC.Center);

					for (int i = 0; i < 4; i++)
					{
						Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.position + vel.RotatedBy(i * 1.57f) + new Vector2(xOffset, 6), Vector2.Zero, ModContent.ProjectileType<MangoLaser>(), NPC.damage / 3, 0, Main.myPlayer);
						proj.netUpdate = true;
					}

					AiTimer = 0;
					NPC.netUpdate = true;
				}

				NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Zero, 0.1f);
				NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.07f);

				NPC.knockBackResist = 0;
			}
			else
			{
				NPC.knockBackResist = .9f;

				#region movement
				NPC.velocity.X *= 0.99f;
				if (!Jumping)
				{
					if (NPC.velocity.Y < 2.5f)
					{
						NPC.velocity.Y += 0.1f;
					}
					if (player.position.Y < NPC.position.Y && NPC.ai[0] % 30 == 0)
					{
						Jumping = true;

						NPC.velocity.X = xOffset / 1.25f;
						NPC.velocity.Y = -6;
					}
				}
				if (Jumping)
				{
					NPC.velocity *= 0.97f;
					if (Math.Abs(NPC.velocity.X) < 0.125f)
						Jumping = false;

					NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation() + 1.57f, 0.08f);
				}
				#endregion
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon<MagicConch>(25);
			npcLoot.AddCommon<MangoJellyStaff>(25);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			Texture2D texture = TextureAssets.Npc[NPC.type].Value;

			bool isAttacking = AiTimer >= cooldownTime;
			int frameWidth = texture.Width / 2;
			Rectangle drawFrame = NPC.frame with { Width = frameWidth, X = frameWidth * (isAttacking ? 1 : 0) };

			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			Vector2 drawOrigin = new Vector2(drawFrame.Width * 0.5f, NPC.height * 0.5f);

			for (int k = 0; k < NPC.oldPos.Length; k++)
			{
				Vector2 drawPos = NPC.oldPos[k] - screenPos + (NPC.Size / 2) + new Vector2(0f, NPC.gfxOffY);
				Color color = NPC.GetAlpha(drawColor) * (float)(((float)(NPC.oldPos.Length - k) / (float)NPC.oldPos.Length) / 2);

				spriteBatch.Draw(texture, drawPos, drawFrame, color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
			}

			spriteBatch.Draw(texture, NPC.Center - screenPos, drawFrame, NPC.GetAlpha(drawColor), NPC.rotation, drawFrame.Size() / 2, NPC.scale, effects, 0);
			spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, NPC.Center - screenPos, drawFrame, NPC.GetAlpha(Color.White * .7f), NPC.rotation, drawFrame.Size() / 2, NPC.scale, effects, 0);

			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (Attacking)
			{
				Texture2D bloomTexture = Mod.Assets.Request<Texture2D>("Effects/Masks/CircleGradient").Value;
				float bloom = (float)((float)AiTimer - cooldownTime) / chargeTime;
				Color color = (new Color(180, 112, 172) * bloom) with { A = 0 };

				spriteBatch.Draw(bloomTexture, NPC.Center - screenPos, null, color, 0f, bloomTexture.Size() / 2, bloom, SpriteEffects.None, 0f);
			}
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.2f;
			NPC.frameCounter %= Main.npcFrameCount[Type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}
	}
}