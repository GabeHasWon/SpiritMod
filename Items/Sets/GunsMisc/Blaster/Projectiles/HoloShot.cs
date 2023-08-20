using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Mechanics.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles
{
	public class HoloShot : ModProjectile, ITrailProjectile
	{
		private bool Secondary
		{
			get => (int)Projectile.ai[0] != 0;
			set => Projectile.ai[0] = value ? 1 : 0;
		}

		public void DoTrailCreation(TrailManager tManager)
		{
			GradientTrail trail = Secondary ? new GradientTrail(Color.LightBlue, Color.DarkBlue) : new GradientTrail(Color.Orange, Color.Red);
			tManager.CreateTrail(Projectile, trail, new RoundCap(), new DefaultTrailPosition(), 8f, 300f, new DefaultShader());
		}

		// public override void SetStaticDefaults() => DisplayName.SetDefault("Star Shot");
		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 80;
			Projectile.height = 6;
			Projectile.width = 6;
			AIType = ProjectileID.Bullet;
			Projectile.extraUpdates = 1;
		}

		public override void AI()
		{
			Vector2 position = Projectile.Center + new Vector2(Main.rand.NextFloat(-1.0f, 1.0f) * 10f, Main.rand.NextFloat(-1.0f, 1.0f) * 10f);
			for (int i = 0; i < 2; i++)
			{
				int num = Dust.NewDust(position, 0, 0, Secondary ? DustID.Flare_Blue : DustID.Flare, 0, 0, 100, default, 0.7f);
				Main.dust[num].velocity = Vector2.Zero;
				Main.dust[num].noGravity = true;
			}
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
		}

		public override void Kill(int timeLeft)
		{
			if (timeLeft <= 0)
				return;

			SoundEngine.PlaySound(SoundID.NPCHit3, Projectile.position);
			for (int i = 0; i < 12; i++)
			{
				Vector2 velocity = -(Projectile.velocity * Main.rand.NextFloat(0.4f, 1.0f)).RotatedByRandom(0.52f);
				int[] dusts = Secondary ? new int[] { DustID.Flare, DustID.PinkCrystalShard } : new int[] { DustID.Flare_Blue, DustID.BlueCrystalShard }; 

				if (timeLeft <= 0)
					velocity = (Projectile.velocity * Main.rand.NextFloat(0.6f, 1.0f)).RotatedByRandom(0.11f);
				Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, dusts[Main.rand.Next(2)],
					velocity, 0, default, Main.rand.NextFloat(0.8f, 1.5f));
				dust.noGravity = true;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Color color = Secondary ? Color.LightBlue : Color.Orange;

			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(color), 
				Projectile.rotation, TextureAssets.Projectile[Projectile.type].Size() / 2, Projectile.scale, SpriteEffects.None, 0);

			Texture2D bloom = Mod.Assets.Request<Texture2D>("Effects/Masks/CircleGradient").Value;
			Main.spriteBatch.Draw(bloom, Projectile.Center - Main.screenPosition, null, (color * .5f) with { A = 0 }, 0, bloom.Size() / 2, 0.18f, SpriteEffects.None, 0);
			return false;
		}
	}
}
