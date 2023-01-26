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
	public class StarshotOrange : ModProjectile, ITrailProjectile
	{
		public void DoTrailCreation(TrailManager tManager) => tManager.CreateTrail(Projectile, new GradientTrail(Color.Orange, Color.Red), new NoCap(), new DefaultTrailPosition(), 8f, 300f, new DefaultShader());

		public override void SetStaticDefaults() => DisplayName.SetDefault("Star Shot");
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
				int num = Dust.NewDust(position, 0, 0, DustID.Flare, 0, 0, 100, default, 0.8f);
				Main.dust[num].velocity = Vector2.Zero;
				Main.dust[num].noGravity = true;
				Main.dust[num].color = Color.Yellow;
			}
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
		}

		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.NPCHit3, Projectile.position);
			for (int i = 0; i < 12; i++)
			{
				Vector2 velocity = -(Projectile.velocity * Main.rand.NextFloat(0.4f, 1.0f)).RotatedByRandom(0.52f);
				if (timeLeft <= 0)
					velocity = (Projectile.velocity * Main.rand.NextFloat(0.6f, 1.0f)).RotatedByRandom(0.11f);
				Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, Main.rand.NextBool(2) ? DustID.Flare : DustID.PinkCrystalShard,
					velocity, 0, default, Main.rand.NextFloat(0.8f, 1.5f));
				dust.noGravity = true;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), 
				Projectile.rotation, TextureAssets.Projectile[Projectile.type].Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}
