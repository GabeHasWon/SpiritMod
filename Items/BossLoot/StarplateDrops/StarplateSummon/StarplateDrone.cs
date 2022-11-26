using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs.Summon;
using SpiritMod.Particles;
using SpiritMod.Projectiles.BaseProj;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.StarplateDrops.StarplateSummon
{
	[AutoloadMinionBuff("Starplate Fighter Drone", "'Sure to deliver a quick death for your foes'")]
	public class StarplateDrone : BaseMinion
	{
		public StarplateDrone() : base(700, 1200, new Vector2(40, 40)) { }

		public override void AbstractSetStaticDefaults()
		{
			DisplayName.SetDefault("Starplate Fighter Drone");
			Main.projFrames[Projectile.type] = 8;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void AbstractSetDefaults() => Projectile.localNPCHitCooldown = 20;


		public override bool DoAutoFrameUpdate(ref int framespersecond, ref int startframe, ref int endframe)
		{
			framespersecond = 18;
			startframe = 0;
			endframe = AiState switch
			{
				1 => 7,
				2 => 7,
				3 => 7,
				_ => Main.projFrames[Projectile.type]
			};
			if (AiState == MELEE)
			{
				Projectile.frameCounter++;
				if (Projectile.frameCounter > 60 / framespersecond)
				{
					Projectile.frameCounter = 0;
					if (Projectile.frame < ((meleeAttacks < meleeAttacksMax) ? 3 : 6))
						Projectile.frame++;
				}
				return false;
			}
			else return true;
		}

		public override bool MinionContactDamage() => AiState == DASH || AiState == MELEE;

		private ref float AiState => ref Projectile.ai[0];
		private ref float AiTimer => ref Projectile.ai[1];

		private const float IDLE = 0;
		private const float CHASE = 0.1f;
		private const float RANGED = 1;
		private const float MELEE = 2;
		private const float DASH = 3;

		private readonly int numColumns = 4;

		private int meleeAttacks;
		private readonly int meleeAttacksMax = 3;

		private int hitCounter;
		private readonly int hitCounterMax = 10;

		public override bool PreAI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
			return true;
		}

		public override void IdleMovement(Player player)
		{
			AiTimer++;
			if (AiState != IDLE)
				SwitchState(IDLE);

			Projectile.direction = Projectile.spriteDirection = (Projectile.Center.X < player.MountedCenter.X) ? -1 : 1;
			Vector2 targetCenter = player.MountedCenter - new Vector2(50 * (IndexOfType + 1) * player.direction, 0);
			Projectile.AccelFlyingMovement(targetCenter, 0.15f, 0.1f, 15);

			if (Projectile.Distance(targetCenter) > 1800)
			{
				Projectile.Center = targetCenter;
				Projectile.netUpdate = true;
			}
			if (hitCounter > 0)
				hitCounter--;
		}

		public override void TargettingBehavior(Player player, NPC target)
		{
			int maxDashTime = 12;
			int closeRange = 500;
			int attackFrequency = 50;
			int comboLength = 3;

			switch (AiState)
			{
				case IDLE:
					SwitchState(CHASE);
					break;
				case CHASE:
					if (player.Distance(target.Center) >= closeRange && Projectile.Distance(target.Center) >= 120f)
						SwitchState(RANGED);
					else if (player.Distance(target.Center) < closeRange)
						SwitchState(MELEE);
					Projectile.AccelFlyingMovement(target.Center, 0.25f, 0.1f, 12);
					break;
				case RANGED:
					if (Projectile.frame == 0 && Projectile.frameCounter <= 1)
					{
						Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.DirectionTo(target.Center) * 8, ProjectileID.GreenLaser, (int)(Projectile.damage * 0.8), Projectile.knockBack, Projectile.owner);
						for (int j = 0; j < 6; j++)
						{
							Dust dust = Dust.NewDustPerfect(Projectile.Center, 90, Projectile.DirectionTo(target.Center).RotatedByRandom(MathHelper.Pi / 3) * Main.rand.NextFloat(1f, 2f), 100, default, Main.rand.NextFloat(0.15f, 0.3f));
							dust.fadeIn = 0.75f;
							dust.noGravity = true;
						}
					}
					Projectile.AccelFlyingMovement(player.MountedCenter - new Vector2(50 * (IndexOfType + 1) * player.direction, 0), 0.2f, 0.1f, 3);
					Projectile.rotation = Projectile.AngleTo(target.Center);
					if (player.Distance(target.Center) < closeRange || Projectile.Distance(target.Center) < 120f)
						SwitchState(CHASE);
					break;
				case MELEE:
					if (AiTimer % attackFrequency == 0)
					{
						Projectile.velocity = Projectile.DirectionTo(target.Center).RotatedByRandom(MathHelper.Pi / 16) * Main.rand.NextFloat(10, 12) * MathHelper.Clamp(Projectile.Distance(target.Center) / 100, 1.33f, 1.75f);
					}
					Projectile.velocity = Projectile.velocity.Length() * 0.97f * Vector2.Normalize(Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * Projectile.velocity.Length(), 0.2f));
					Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.velocity.ToRotation() + MathHelper.PiOver2, 0.04f);

					if (meleeAttacks >= comboLength)
					{
						if (Projectile.Distance(target.Center) >= 120 && Projectile.frame == 6)
							SwitchState(DASH);
						else //Attempt to back up a distance before dashing
							Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionFrom(target.Center) * 8f, 0.1f);
					}
					if (player.Distance(target.Center) > closeRange)
						SwitchState(CHASE);
					break;
				case DASH:
					DoDash(Projectile.DirectionTo(target.Center) * ((Projectile.Distance(target.Center) + (target.width / 2) + 50) / (maxDashTime - 1)), maxDashTime);
					break;
			}
			AiTimer++;
		}

		private Vector2? dashVelocity;
		private void DoDash(Vector2 constVel, int maxTime)
		{
			if (dashVelocity == null)
				dashVelocity = constVel;

			CanRetarget = false;

			if (AiTimer < maxTime)
			{
				Projectile.velocity = (Vector2)dashVelocity;
				if (Projectile.frame > 2)
					Projectile.frame = 2;
				if (AiTimer <= 1)
					for (int i = 0; i < 5; i++)
						ParticleHandler.SpawnParticle(new ImpactLine(Projectile.position + new Vector2(Main.rand.NextFloat(Projectile.width), 
							Main.rand.NextFloat(Projectile.height)), Projectile.velocity * Main.rand.NextFloat(0.1f, 0.7f), Color.Lerp(Color.White, Color.Blue, Main.rand.NextFloat(0.0f, 1.0f)), new Vector2(0.5f, 2.5f), 30));
				for (int i = 0; i < 2; i++)
				{
					Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.BlueFairy,
						Projectile.velocity.X / 3, Projectile.velocity.Y / 3, 0, default, Main.rand.NextFloat(0.3f, 0.8f));
					dust.noGravity = true;
				}
			}
			else
			{
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Zero, 0.1f);
				if (Projectile.frame == 0)
				{
					SwitchState(CHASE);
					AiTimer = 0;
					dashVelocity = null;
					Projectile.netUpdate = true;
				}
			}
		}

		private void SwitchState(float newState)
		{
			Projectile.frameCounter = 0;
			Projectile.frame = 0;
			AiTimer = 0;
			meleeAttacks = 0;

			AiState = newState;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			if (AiState == MELEE)
			{
				Projectile.velocity = Projectile.velocity.RotatedByRandom(1f);
				meleeAttacks++;
				for (int i = 0; i < 5; i++)
				{
					Dust dust = Dust.NewDustPerfect(Projectile.Center + new Vector2(10, 0).RotatedBy(Projectile.rotation), DustID.BlueFairy, 
						(Projectile.velocity / 3).RotatedByRandom(0.3f), 0, default, Main.rand.NextFloat(0.3f, 0.8f));
					dust.noGravity = true;
				}
				Projectile.NewProjectile(Entity.GetSource_FromAI(), Projectile.Center, Projectile.velocity / 10, ModContent.ProjectileType<StarplateDrone_Slash>(), 0, 0f, Player.whoAmI);
			}
			hitCounter = hitCounterMax;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Rectangle rect = new Rectangle(texture.Width / numColumns * (int)AiState, texture.Height / Main.projFrames[Projectile.type] * Projectile.frame, 
				texture.Width / numColumns - 2, texture.Height / Main.projFrames[Projectile.type] - 2);

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect, Projectile.GetAlpha(lightColor), Projectile.rotation, rect.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			//Draw a glowmask
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, Projectile.Center - Main.screenPosition, rect, Projectile.GetAlpha(Color.White), Projectile.rotation, rect.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			
			if (AiState == DASH || (AiState == MELEE && hitCounter > 0))
			{
				for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
				{
					float opacityMod = (ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / (float)ProjectileID.Sets.TrailCacheLength[Projectile.type];
					Vector2 drawPosition = Projectile.oldPos[i] + (Projectile.Size / 2) - Main.screenPosition;
					Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow2").Value, drawPosition, rect, Projectile.GetAlpha(Color.White) * opacityMod,
						Projectile.oldRot[i], rect.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
				}
			}
			return false;
		}
	}
}