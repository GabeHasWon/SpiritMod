using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SpiritMod.GlobalClasses.Players;
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
		/// <summary>
		/// Use this if you want the pet to follow the player to their close position, as opposed 
		/// to only moving over temporary obstacles when transitioning to the RUNNING State
		/// </summary>
		private bool stopLate = false;
		private int State
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		private const int WALKING = 0;
		private const int IDLING = 1;
		private const int RUNNING = 2;
		private const int CHASING = 3;

		private ref float Counter => ref Projectile.ai[1];

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Inferno");
			Main.projFrames[Type] = 9;
			ProjectileID.Sets.TrailCacheLength[Type] = 5;
			ProjectileID.Sets.TrailingMode[Type] = 0;
			Main.projPet[Type] = true;
			ProjectileID.Sets.CharacterPreviewAnimations[Type] = ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Type], 6)
				.WithOffset(-20, 0)
				.WithSpriteDirection(-1)
				.WhenNotSelected(0, 0);
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
			Main.player[Projectile.owner].GetModPlayer<PetPlayer>().PetFlag(Projectile);

			if (State == CHASING)
				StuckMovement();
			else if (State == RUNNING)
				FarMovement();
			else
				NearbyMovement();

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

			if (Main.rand.NextBool(25 - (int)MathHelper.Clamp((int)(Projectile.velocity.Length() / 10), 0, 12)) && Math.Abs(Projectile.velocity.X) > 3f)
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

		private void NearbyMovement()
		{
			float dist = Projectile.DistanceSQ(Main.player[Projectile.owner].Center);

			float targetSpeed;
			//The pet is within two tile range of the player on either side
			bool inHorizontalRange = Projectile.Center.X > Owner.Center.X - 32 && Projectile.Center.X < Owner.Center.X + 32;
			if (dist <= 60 * 60 || inHorizontalRange)
				targetSpeed = 0f;
			else
				targetSpeed = 2.45f;

			float randomVariance = (targetSpeed != 0) ? Main.rand.NextFloat(-0.12f, 0.12f) : 0;
			if (Owner.Center.X < Projectile.Center.X)
				Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, -targetSpeed + randomVariance, 0.04f);
			else
				Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, targetSpeed + randomVariance, 0.04f);

			Projectile.rotation = 0;
			//Add gravity
			Projectile.velocity.Y += 0.2f;
			Projectile.tileCollide = true;
			float throwaway = 6;
			Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref throwaway, ref Projectile.gfxOffY);

			if ((int)Projectile.velocity.X == 0 && targetSpeed == 0f)
				ResetState(IDLING);
			//Rudimentary transition to RUNNING
			else if (dist > 300 * 300)
				ResetState(CHASING);
			else
				ResetState(WALKING);
		}

		private void FarMovement()
		{
			//Control the pet's falling frame
			if ((int)Projectile.velocity.Y > 0)
			{
				Projectile.frameCounter = 0;
				Projectile.frame = 4;
			}
			float targetSpeed = 5.8f;

			float dist = Projectile.DistanceSQ(Main.player[Projectile.owner].Center);
			if (dist <= 60 * 60)
				targetSpeed = 0f;

			float randomVariance = (targetSpeed != 0) ? Main.rand.NextFloat(-0.2f, 0.2f) : 0;
			if (Owner.Center.X < Projectile.Center.X)
				Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, -targetSpeed + randomVariance, 0.08f);
			else
				Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, targetSpeed + randomVariance, 0.08f);

			Projectile.rotation = Projectile.velocity.X / 26;
			//Add gravity
			Projectile.velocity.Y += 0.3f;
			Projectile.tileCollide = true;
			float throwaway = 8;
			Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref throwaway, ref Projectile.gfxOffY);

			if (dist >= 920 * 920)
			{
				Projectile.velocity.Y = -4f;
				stopLate = true;
				ResetState(CHASING);
				return;
			}
			if ((int)Projectile.velocity.X == 0)
				ResetState(IDLING);
		}

		private void StuckMovement()
		{
			float dist = Projectile.DistanceSQ(Main.player[Projectile.owner].Center);
			int num24 = (int)((Main.LocalPlayer.miscCounter / 300.0f * MathHelper.TwoPi).ToRotationVector2().Y * 4.0) * 4;
			if (dist <= 30 * 30)
			{
				if (TryLanding())
				{
					//Landing dusts
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
							Gore.NewGoreDirect(Projectile.GetSource_FromAI(), spawnPos, new Vector2(0f, -1f), Main.rand.NextBool(2) ? GoreID.Smoke1 : GoreID.Smoke2);
					}
					stopLate = false;
					State = WALKING;
					Projectile.netUpdate = true;
				}
			}
			else if (!stopLate && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, Owner.position, Owner.width, Owner.height))
			{
				//Fancy running dusts
				if (Math.Abs(Projectile.velocity.X) > 3f)
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
				State = RUNNING;
				Projectile.netUpdate = true;
			}
			if (Main.rand.NextBool(3))
			{
				//Fancy dusts
				for (int i = 0; i < 2; i++)
				{
					int type = Main.rand.NextBool(2) ? DustID.Smoke : Main.rand.NextBool(2) ? DustID.Torch : DustID.Firework_Yellow;
					Dust dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(0, num24 * ((i > 0) ? -1 : 1)).RotatedBy(Projectile.velocity.ToRotation()), type, Vector2.Zero, 0, default, Main.rand.NextFloat(1.0f, 1.5f));
					dust.velocity = Vector2.Zero;
					dust.noGravity = true;
				}
			}

			const float MaxSpeedDistance = 1400;
			float magnitude = 13;
			if (dist > MaxSpeedDistance * MaxSpeedDistance)
				magnitude = 13 + (((float)Math.Sqrt(dist) - MaxSpeedDistance) * 0.1f);
			Vector2 goToPosition = Owner.Center + new Vector2(num24 * 3);
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(goToPosition) * magnitude, 0.042f);
			Projectile.rotation += 0.42f * Projectile.spriteDirection;
			Projectile.tileCollide = false;
		}

		private bool TryLanding()
		{
			Projectile.tileCollide = true;
			Vector2 position = new Vector2(Projectile.getRect().Center.X, Projectile.getRect().Bottom + 24) / 16;
			if (WorldGen.SolidTile(Main.tile[position.ToPoint()]))
				return true;
			return false;
		}

		private void ResetState(int newState)
		{
			State = newState;
			if (State != newState)
			{
				Projectile.frame = 0;
				Projectile.frameCounter = 0;
			}
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
					Color color = Projectile.GetAlpha(new Color(255, 200, 25)) * (float)((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length / 2);
					Main.EntitySpriteDraw(texture.Value, drawPos, rect, color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
				}
			}
			//Draw the projectile normally
			Main.EntitySpriteDraw(texture.Value, position - Main.screenPosition, rect, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
			//Draw a glowmask
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>($"{Texture}_Glow").Value, position - Main.screenPosition, rect, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
			if (Counter > 0f)
			{
				Texture2D fireTex = TextureAssets.Extra[55].Value;
				float speedometer = Projectile.velocity.Length() / 22;

				int frameCount = 4;
				int frameDur = 4;
				int frame = Main.LocalPlayer.miscCounter / frameDur % frameCount;

				Rectangle rectangle = new Rectangle(0, fireTex.Height / frameCount * frame, fireTex.Width, fireTex.Height / frameCount);
				Vector2 origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
				float rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
				Color color = Color.Lerp(Color.LightBlue, Color.Gold, (float)(speedometer / 1f)) * (float)(speedometer / 1f) * Counter;
				Main.EntitySpriteDraw(fireTex, Projectile.Center - Projectile.velocity - Main.screenPosition, rectangle, color, rotation, origin, Projectile.scale * .8f, SpriteEffects.None, 0);
			}
			//Counter is used to modify the transparency of the pet's fireball effect
			if (State == CHASING)
			{
				if (Counter < 1)
					Counter += 0.05f;
			}
			else if (Counter > 0)
				Counter -= 0.05f;
			return false;
		}
	}
}
