using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.FlailsMisc;
using SpiritMod.Particles;
using SpiritMod.Projectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GraniteSet.GraniteFlail
{
	public class GraniteMaceProj : BaseFlailProj
	{
		private bool maxMomentum = false;

		public GraniteMaceProj() : base(220, 40, 25, 60) { }

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Granite Mace");
			Main.projFrames[Type] = 2;
		}

		public override bool PreAI()
		{
			if (State == SPINNING)
			{
				if ((Timer + 1) == momentum) //Play a charge indicator sound
					SoundEngine.PlaySound(SoundID.MaxMana, Owner.Center);

				maxMomentum = Timer >= momentum;
			}

			if (Main.rand.NextBool(5) && Projectile.frame < 1)
			{
				Vector2 position = Projectile.Center + Main.rand.NextVector2Unit() * Main.rand.NextFloat(10.0f);
				Dust dust = Dust.NewDustPerfect(position, DustID.Electric, Vector2.Zero, 0, default, Main.rand.NextFloat(0.5f, 1.0f));
				dust.noGravity = true;
			}
			return true;
		}

		public override void SafeTileCollide(Vector2 oldVelocity, bool highImpact)
		{
			if (highImpact)
			{
				Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
				SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

				for (int i = 0; i < 5; i++)
					Dust.NewDust(Projectile.position + oldVelocity, Projectile.width, Projectile.height, DustID.Electric, oldVelocity.X / 2, oldVelocity.Y / 2, 0, default, Main.rand.NextFloat(0.5f, 1.0f));

				if (State == LAUNCHING && maxMomentum)
					ExplosionEffect();
			}
		}

		public override bool? CanDamage()
		{
			if (Projectile.frame > 0)
				return false;
			return base.CanDamage();
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			base.OnHitNPC(target, damage, knockback, crit);

			if (State == SPINNING)
			{
				Vector2 position = target.getRect().ClosestPointInRect(Owner.MountedCenter);
				for (int i = 0; i < 8; i++)
					Dust.NewDustPerfect(position, DustID.Electric, Main.rand.NextVector2Unit() * Main.rand.NextFloat(1.0f, 3.0f), 0, default, Main.rand.NextFloat(0.5f, 1.0f));
			}
			else if (State == LAUNCHING && maxMomentum)
				ExplosionEffect();
		}

		private void ExplosionEffect()
		{
			for (int i = 0; i < 15; i++)
			{
				Vector2 randomVel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1.0f, 5.0f);

				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool(3) ? DustID.Granite : DustID.Electric, randomVel.X, randomVel.Y, 0, default, Main.rand.NextFloat(0.5f, 1.2f));
				if (dust.type == DustID.Electric)
					dust.noGravity = true;

				if (i < 6)
				{
					int type = (i < 4) ? ModContent.ProjectileType<GraniteFragment>() : ModContent.ProjectileType<GraniteShard1>();
					randomVel = (new Vector2(randomVel.X, -5.0f) + (Projectile.velocity * 0.12f)) * Main.rand.NextFloat(0.3f, 1.0f);
					Projectile.NewProjectile(Entity.GetSource_FromAI("OnHitNPC"), Projectile.Center, randomVel, type, Projectile.damage, 2, Owner.whoAmI);
				}
				if (i < 4 && !Main.dedServ)
				{
					Vector2 randomPos = Projectile.Center + Main.rand.NextVector2Unit() * 20;
					ParticleHandler.SpawnParticle(new ImpactLine(randomPos, Projectile.Center.DirectionTo(randomPos) * Main.rand.NextFloat(1.0f, 3.0f), Color.Lerp(Color.White, Color.Blue, Main.rand.NextFloat(0.0f, 1.0f)), new Vector2(0.5f, Main.rand.NextFloat(0.5f, 1.5f)), 20)
						{ Rotation = randomPos.AngleTo(Projectile.Center) });
				}
			}

			SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

			Projectile.velocity = Vector2.Zero;
			Timer = LaunchTime; //Return
			Projectile.frame = 1;
			Owner.GetModPlayer<MyPlayer>().Shake += 20;

			Projectile.netUpdate = true;
		}

		public override bool PreDrawExtras()
		{
			Texture2D ChainTexture = Mod.Assets.Request<Texture2D>(Texture.Remove(0, Mod.Name.Length + 1) + "_Chain").Value;

			Vector2 armPos = Main.GetPlayerArmPosition(Projectile);
			armPos.Y -= Owner.gfxOffY;
			int numFrames = 4;

			int numDraws = Math.Max((int)(Projectile.Distance(armPos) / (ChainTexture.Height / numFrames)), 1);
			for (int i = 0; i < numDraws; i++)
			{
				int yFrame = (int)MathHelper.Clamp(i / 2, 0, numFrames - 1);
				Rectangle frame = new Rectangle(0, ChainTexture.Height / numFrames * yFrame, ChainTexture.Width, ChainTexture.Height / numFrames);

				Vector2 chaindrawpos = Vector2.Lerp(Projectile.Center, armPos, i / (float)numDraws);
				Color lightColor = Lighting.GetColor((int)chaindrawpos.X / 16, (int)chaindrawpos.Y / 16);

				Main.EntitySpriteDraw(ChainTexture, chaindrawpos - Main.screenPosition, frame, lightColor * (numFrames - yFrame), Projectile.AngleFrom(Owner.MountedCenter) + 1.57f, frame.Size() / 2, 1f, SpriteEffects.None, 0);
			}
			return true;
		}

		public override void PostDraw(Color lightColor)
		{
			Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
			Rectangle frame = new Rectangle(0, texture.Height / Main.projFrames[Type] * Projectile.frame, texture.Width, (texture.Height / Main.projFrames[Type]) - 2);

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

			#region additional vfx
			if (Projectile.frame > 0)
				return;
			float quoteant = MathHelper.Min((float)Timer / momentum, 1f);
			if (maxMomentum)
			{
				for (int i = 0; i < 2; i++)
				{
					Texture2D specialNoise = Mod.Assets.Request<Texture2D>("Textures/T_Lu_Noise_31").Value;
					float fxCounter = (float)(Main.timeForVisualEffects / 10f);
					Main.EntitySpriteDraw(specialNoise, Projectile.Center - Main.screenPosition, null, (Color.Blue * 0.1f) with { A = 0 }, fxCounter * ((i < 1) ? -1 : 1), specialNoise.Size() / 2, Projectile.scale / 14, SpriteEffects.None, 0);
				}
				quoteant = 1f;
			}

			Color color = Color.Cyan * .2f;
			Vector2 scale = new Vector2(0.25f) * Projectile.scale;

			for (int i = 0; i < 2; i++)
			{
				Texture2D glowTex = (i > 0) ? Mod.Assets.Request<Texture2D>("Effects/Masks/Star").Value : Mod.Assets.Request<Texture2D>("Effects/Masks/CircleGradient").Value;
				if (i > 0)
				{
					color = Color.Cyan * 3f;
					scale = new Vector2(0.8f, 0.2f) * quoteant * Projectile.scale;
				}
				Main.EntitySpriteDraw(glowTex, Projectile.Center + (Main.rand.NextVector2Unit() * quoteant * i) - Main.screenPosition, null, color with { A = 0 }, 0f, glowTex.Size() / 2, scale, SpriteEffects.None, 0);
			}
			#endregion
		}
	}
}