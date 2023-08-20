using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs;
using SpiritMod.Mechanics.Trails;
using SpiritMod.NPCs;
using SpiritMod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Thrown
{
	public class OakHeart : ModProjectile, ITrailProjectile
	{
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private const int velocityLengthMax = 5;

		private bool desiredSpeed = false;

		public void DoTrailCreation(TrailManager tManager)
		{
			tManager.CreateTrail(Projectile, new GradientTrail(Color.LightGreen, Color.Yellow), new RoundCap(), new DefaultTrailPosition(), 8f, 200f, new DefaultShader());
			tManager.CreateTrail(Projectile, new StandardColorTrail(Color.Orange * .3f), new RoundCap(), new DefaultTrailPosition(), 14f, 200f, new DefaultShader());
		}

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Oak Heart");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			Projectile.width = 10;
			Projectile.height = 16;
			Projectile.friendly = true;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 600;
			Projectile.extraUpdates = 1;
			Projectile.DamageType = DamageClass.Melee;
		}

		public override void AI()
		{
			if (++Counter > 30)
				Projectile.velocity.Y += 0.15f;

			if (Counter > 10)
			{
				if (!desiredSpeed)
				{
					if (Projectile.velocity.Length() > velocityLengthMax)
						Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Zero, 0.2f); //Harshly decelerate to the specified speed
					else
					{
						desiredSpeed = true;

						for (int i = 0; i < 10; i++)
						{
							Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FartInAJar);
							dust.velocity = (Projectile.velocity * Main.rand.NextFloat(0.5f, 1.0f)).RotatedBy(Main.rand.NextBool(2) ? -1.57f : 1.57f).RotatedByRandom(0.3f);
							dust.noGravity = true;
						}
					}
				}
			}
			else
			{
				if (!Main.dedServ)
				{
					if (Main.rand.NextBool(2))
						ParticleHandler.SpawnParticle(new SmokeParticle(Projectile.Center, (Projectile.velocity / 3).RotatedByRandom(0.2f), Color.LightGreen, Main.rand.NextFloat(0.1f, 0.2f) * (Counter / 2.5f), Main.rand.Next(10, 30)));
					if (Main.rand.NextBool(5))
					{
						Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.FartInAJar, Projectile.velocity.X, Projectile.velocity.Y, 100);
						dust.noGravity = true;
					}
				}
			}

			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

			if (Main.rand.NextBool(2))
			{
				Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, Vector2.Zero);
				dust.noGravity = true;
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			GNPC modNPC = target.GetGlobalNPC<GNPC>();
			int increment = 60;
			modNPC.oakHeartStacks += increment;

			if (modNPC.oakHeartStacks >= (modNPC.oakHeartStacksMax * (increment / 1.5f)))
			{
				for (int k = 0; k < 5; k++)
				{
					int p = Projectile.NewProjectile(Projectile.GetSource_OnHit(target), target.Center.X + Main.rand.Next(-20, 20), target.position.Y - 60, 0f, 8f, ModContent.ProjectileType<PoisonCloud>(), Projectile.damage / 2, 0f, Projectile.owner, 0f, 0f);
					Main.projectile[p].penetrate = 2;

					Vector2 position = target.getRect().ClosestPointInRect(Projectile.Center);
					float rotation = Projectile.velocity.ToRotation();

					if (!Main.dedServ)
						ParticleHandler.SpawnParticle(new ImpactLine(position - new Vector2(22, 0).RotatedBy(rotation), Projectile.velocity / 8, Color.Lerp(Color.LimeGreen, Color.Yellow, Main.rand.NextFloat()), new Vector2(Main.rand.NextFloat(0.5f, 1.5f), Main.rand.NextFloat(1.0f, 2.2f)), Main.rand.Next(8, 12))
						{
							Origin = ModContent.Request<Texture2D>(Texture).Size() / 2,
						});

					Dust dust = Dust.NewDustPerfect(position, DustID.CursedTorch, -(new Vector2(8, 0).RotatedBy(rotation) * Main.rand.NextFloat()).RotatedByRandom(0.5f), 80);
					dust.noGravity = true;
				}

				modNPC.oakHeartStacks -= increment;
			}

			MyPlayer modPlayer = Main.player[Projectile.owner].GetModPlayer<MyPlayer>();

			if (!desiredSpeed && modPlayer.oakHeartStacks < modPlayer.oakHeartStacksMax)
				modPlayer.oakHeartStacks++;

			target.AddBuff(BuffID.Poisoned, 120 + (int)(modPlayer.oakHeartStacks * 10));
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 5; i++)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GrassBlades);

				Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, -(Projectile.velocity * Main.rand.NextFloat()).RotatedByRandom(0.5f), 80);
				dust.noGravity = true;
			}

			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
		}

		public override bool PreDraw(ref Color lightColor)
		{
		    Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Type].Value.Width / 2, Projectile.height / 2);
		    for (int k = 0; k < Projectile.oldPos.Length; k++)
		    {
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
		        Color color = Projectile.GetAlpha(Color.LightGreen) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, drawPos, null, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}
			return true;
		}
	}
}