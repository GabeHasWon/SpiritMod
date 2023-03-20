using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Mechanics.Trails;
using SpiritMod.Particles;
using SpiritMod.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles
{
	public class LongRocket : SubtypeProj, ITrailProjectile
	{
		private int? directNPCIndex;

		public void DoTrailCreation(TrailManager tManager)
		{
			Color color = Color.Orange;

			//Manually get the weapon's element color because Subtype is assigned to after DoTrailCreation is called
			Item heldItem = Main.player[Projectile.owner].HeldItem;
			if (heldItem.ModItem is Blaster)
				color = GetColor((heldItem.ModItem as Blaster).element);

			tManager.CreateTrail(Projectile, new StandardColorTrail(color), new RoundCap(), new DefaultTrailPosition(), 12f, 100f, new DefaultShader());
		}

		public override void SetStaticDefaults() => DisplayName.SetDefault("Rocket");
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 80;
			Projectile.height = 8;
			Projectile.width = 8;
			AIType = ProjectileID.Bullet;
		}

		public override void AI()
		{
			if (Main.rand.NextBool(2))
			{
				Vector2 position = Projectile.position + new Vector2(Main.rand.NextFloat(Projectile.width), Main.rand.NextFloat(Projectile.height));
				Dust dust = Dust.NewDustPerfect(position, Main.rand.NextBool(2) ? DustID.Smoke : Dusts[1], null, 0, default, Main.rand.NextFloat(0.8f, 1.6f));
				dust.velocity = Projectile.velocity * .8f;
				dust.noGravity = true;
			}
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

			if (Projectile.timeLeft % 8 == 0 && !Main.dedServ)
				ParticleHandler.SpawnParticle(new PulseCircle(Projectile.Center + Projectile.velocity, GetColor(Subtype), 25, 10) { ZRotation = .5f, Angle = Projectile.rotation - 1.57f });
		}

		public override void Kill(int timeLeft)
		{
			ProjectileExtras.Explode(Projectile.whoAmI, 100, 100, delegate
			{
				if (!Main.dedServ)
					ExplodeEffect();
			});
		}

		private void ExplodeEffect()
		{
			SoundEngine.PlaySound(SoundID.Item14 with { PitchVariance = 0.1f }, Projectile.Center);

			int fireParticles = Main.rand.Next(5, 12);
			Color color = GetColor(Subtype);

			for (int i = 0; i < 15; i++)
			{
				if (i < 5)
					ParticleHandler.SpawnParticle(new SmokeParticle(Projectile.Center, new Vector2(Main.rand.NextFloat(-1.0f, 1.0f), Main.rand.NextFloat(-1.0f, 1.0f)), Color.Lerp(Color.DarkGray, color, Main.rand.NextFloat(1.0f)), Main.rand.NextFloat(0.8f, 1.5f), Main.rand.Next(16, 20)));
				if (i < fireParticles)
				{
					float velLength = Main.rand.NextFloat(3, 5);
					Vector2 vel = Main.rand.NextVector2Unit() * velLength;
					ParticleHandler.SpawnParticle(new FireParticle(Projectile.Center, vel, Color.White, color * .25f, Main.rand.NextFloat(0.2f, 0.5f), Main.rand.Next(15, 25)));
				}
				int[] dustType = Dusts;

				Dust dust = Dust.NewDustPerfect(Projectile.Center, dustType[Main.rand.Next(dustType.Length)], null);
				dust.velocity = new Vector2(Main.rand.NextFloat(-1.0f, 1.0f) * .5f, Main.rand.NextFloat(-1.0f, 1.0f) * .5f);
				dust.fadeIn = 1.1f;
				dust.noGravity = true;
			}
			ParticleHandler.SpawnParticle(new PulseCircle(Projectile.Center, color, 100, 8));
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			base.OnHitNPC(target, damage, knockback, crit);

			directNPCIndex = target.whoAmI;
		}

		public override bool? CanHitNPC(NPC target) => (target.whoAmI != directNPCIndex) ? null : false;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Rectangle frame = new(texture.Width / (int)Subtypes.Count * Subtype, texture.Height / Main.projFrames[Projectile.type] * Projectile.frame, (texture.Width / (int)Subtypes.Count) - 2, (texture.Height / Main.projFrames[Projectile.type]) - ((Main.projFrames[Projectile.type] > 1) ? 2 : 0));

			//Draw the projectile normally
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor),
				Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			//Draw a glowmask
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(Color.White),
				Projectile.rotation, frame.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}
