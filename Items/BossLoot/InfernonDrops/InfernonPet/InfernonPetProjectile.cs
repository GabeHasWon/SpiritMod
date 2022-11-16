using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.InfernonDrops.InfernonPet
{
	public class InfernonPetProjectile : ModProjectile
	{
		private Player Owner => Main.player[Projectile.owner];
		//The number of frames per column
		private readonly int[] frameCounts = new int[4] { 9, 7, 5, 1 };
		private int State
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private const int IDLING = 1;
		private const int WALKING = 0;
		private const int RUNNING = 2;
		private const int CHASING = 3;
		private int Counter
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Inferno");
			Main.projFrames[Projectile.type] = 9;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
			Main.projPet[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Truffle);
			Projectile.aiStyle = 0;
			Projectile.width = 60; //74
			Projectile.height = 54; //60
			Projectile.timeLeft = 2;

			AIType = 0;
		}

		public override void AI()
		{
			Main.player[Projectile.owner].GetModPlayer<GlobalClasses.Players.PetPlayer>().PetFlag(Projectile);

			if (State == CHASING)
				ChasePlayer();
			else
				BasicMovement();

			if (Projectile.velocity.X > 0)
				Projectile.spriteDirection = -1;
			else if (Projectile.velocity.X < 0)
				Projectile.spriteDirection = 1;
			//Increment draw frames normally
			if (++Projectile.frameCounter >= 6)
			{
				Projectile.frameCounter = 0;
				Projectile.frame = ++Projectile.frame % frameCounts[State];
			}
			if (Projectile.frame >= frameCounts[State])
				Projectile.frame = 0;
			//Spawn fancy running dusts
			if (Math.Abs(Projectile.velocity.X) > 3f)
			{
				if (Main.rand.NextBool(25))
				{
					int dustType = Main.rand.NextBool(2) ? DustID.Torch : DustID.GoldFlame;
					Vector2 dustVel = new Vector2(-Projectile.velocity.X / 5, 0f);
					Dust dust = Dust.NewDustDirect(Projectile.position, 6, Projectile.height, dustType, dustVel.X, dustVel.Y, 0, default, Main.rand.NextFloat(0.5f, 1.0f));
					dust.noGravity = Main.rand.NextBool(2);
					if (dust.noGravity)
						dust.fadeIn = 1.2f;
					dust.noLightEmittence = true;
				}
			}
		}

		private void BasicMovement()
		{
			float dist = Projectile.DistanceSQ(Main.player[Projectile.owner].Center);
			float targetSpeed = 2.47f;

			if (dist >= 900 * 900)
			{
				Projectile.velocity.Y = -4f;
				ResetState(CHASING);
				return;
			}
			else if (dist >= 280 * 280)
			{
				if (State != RUNNING)
				{
					for (int i = 0; i < 20; i++)
					{
						int dustType = Main.rand.NextBool(2) ? DustID.Torch : DustID.GoldFlame;
						Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default, Main.rand.NextFloat(0.8f, 1.2f));
						dust.noGravity = true;
						if (Main.rand.NextBool(2))
							dust.fadeIn = 1.2f;
						Dust dust2 = Dust.NewDustDirect(Projectile.position + new Vector2(0f, Projectile.height), Projectile.width, 2, DustID.Smoke, -Projectile.velocity.X / 5, -0.8f, 0, default, Main.rand.NextFloat(1.0f, 1.3f));
						dust2.noGravity = true;
						dust2.fadeIn = 1.3f;
					}
				}
				ResetState(RUNNING);
				targetSpeed = 5.8f;
			}
			else if (dist <= 240 * 240)
			{
				if ((int)Projectile.velocity.X == 0)
					ResetState(IDLING);
				else
					ResetState(WALKING);
				if (dist <= 60 * 60)
					targetSpeed = 0f;
				else targetSpeed = 2.45f;
			}
			
			if (Owner.Center.X < Projectile.Center.X)
				Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, -targetSpeed, 0.1f);
			else
				Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, targetSpeed, 0.1f);

			float throwaway = 6;
			Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref throwaway, ref Projectile.gfxOffY);
			Projectile.rotation = 0f;
			//Add gravity
			Projectile.velocity.Y += 0.2f;
			Projectile.tileCollide = true;
		}

		private void ChasePlayer()
		{
			float dist = Projectile.DistanceSQ(Main.player[Projectile.owner].Center);
			float targetSpeed = 13f;

			if (dist <= 60 * 60)
			{
				targetSpeed = 3f;
				if (dist <= 18 * 18)
				{
					for (int i = 0; i < 8; i++)
					{
						int dustType = Main.rand.NextBool(2) ? DustID.Torch : DustID.GoldFlame;
						Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default, Main.rand.NextFloat(1.0f, 1.5f));
						dust.noGravity = true;
						if (Main.rand.NextBool(2))
							dust.fadeIn = 1.2f;
						Dust dust2 = Dust.NewDustDirect(Projectile.position + new Vector2(0f, Projectile.height), Projectile.width, 2, DustID.Smoke, -Projectile.velocity.X / 5, -0.8f, 0, default, Main.rand.NextFloat(1.3f, 1.8f));
						dust2.noGravity = true;
						dust2.fadeIn = 1.3f;

						Vector2 spawnPos = Projectile.position + new Vector2(Main.rand.Next(Projectile.width), Main.rand.NextFloat(Projectile.height));
						if (i > 5)
						{
							Gore.NewGoreDirect(Projectile.GetSource_FromAI(), spawnPos, new Vector2(0f, -1f), Main.rand.NextBool(2) ? GoreID.Smoke1 : GoreID.Smoke2);
						}
					}
					State = WALKING;
				}
				//Projectile.tileCollide = true;
			}
			//else Projectile.tileCollide = false;
			Projectile.tileCollide = false;
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Owner.Center) * targetSpeed, 0.05f);
			Projectile.rotation += 0.42f * Projectile.spriteDirection;
		}

		private void ResetState(int newState)
		{
			State = newState;
			if (State != newState)
				Projectile.frameCounter = 0;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Asset<Texture2D> texture = TextureAssets.Projectile[Projectile.type];
			Rectangle rect = new Rectangle(texture.Width() / frameCounts.Length * State, texture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, 
				texture.Width() / frameCounts.Length - 2, texture.Height() / Main.projFrames[Projectile.type] - 2);
			SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Vector2 position = Projectile.Center;
			Vector2 drawOrigin = rect.Size() / 2;
			if (Math.Abs(Projectile.velocity.X) > 3f)
			{
				Vector2 oddOffset = new Vector2(-((rect.Width - Projectile.width) / 2), -((rect.Height - Projectile.height) / 2));
				for (int k = 0; k < Projectile.oldPos.Length; k++)
				{
					Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + oddOffset + new Vector2(0f, Projectile.gfxOffY);
					Color color = Projectile.GetAlpha(new Color(255, 180, 29)) * (float)((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 2);
					Main.EntitySpriteDraw(texture.Value, drawPos, rect, color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
				}
			}
			//Draw the projectile normally
			Main.EntitySpriteDraw(texture.Value, position - Main.screenPosition, rect, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
			//Draw a glowmask
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>($"{Texture}_Glow").Value, position - Main.screenPosition, rect, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
			if (State == CHASING)
			{
				Texture2D fireTex = TextureAssets.Extra[55].Value;
				float speedometer = Projectile.velocity.Length() / 24;

				int frameCount = 4;
				int frameDur = 4;
				int frame = Counter / frameDur % frameCount;
				Counter = ++Counter % (frameCount * frameDur);

				Rectangle rectangle = new Rectangle(0, fireTex.Height / frameCount * frame, fireTex.Width, fireTex.Height / frameCount);
				Vector2 origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
				float rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
				Color color = Color.Lerp(Color.LightBlue, Color.Gold, (float)(speedometer / 1f)) * (float)(speedometer / 1f);
				Main.EntitySpriteDraw(fireTex, Projectile.Center - Projectile.velocity - Main.screenPosition, rectangle, color, rotation, origin, Projectile.scale * .8f, SpriteEffects.None, 0);
			}
			return false;
		}
	}
}
