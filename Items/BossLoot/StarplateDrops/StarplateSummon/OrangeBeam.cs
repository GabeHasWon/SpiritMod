using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.StarplateDrops.StarplateSummon
{
	public class OrangeBeam : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Orange Beam");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.penetrate = 2;
			Projectile.timeLeft = 80;
			Projectile.height = 6;
			Projectile.width = 6;
			AIType = ProjectileID.Bullet;
			Projectile.extraUpdates = 1;
		}

		public override void AI() => Projectile.rotation = Projectile.velocity.ToRotation();
		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.NPCHit3, Projectile.position);
			for (int i = 0; i < 12; i++)
			{
				Vector2 velocity = -(Projectile.velocity * Main.rand.NextFloat(0.4f, 1.0f)).RotatedByRandom(0.52f);
				if (timeLeft <= 0)
					velocity = (Projectile.velocity * Main.rand.NextFloat(0.6f, 1.0f)).RotatedByRandom(0.11f);
				Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, Main.rand.NextBool(2) ? DustID.Flare : DustID.SolarFlare,
					velocity, 0, default, Main.rand.NextFloat(0.8f, 1.5f));
				dust.noGravity = true;
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
			{
				float opacityMod = (ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / (float)ProjectileID.Sets.TrailCacheLength[Projectile.type];
				Vector2 drawPosition = Projectile.oldPos[i] + (Projectile.Size / 2) - Main.screenPosition;
				Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPosition, null, Projectile.GetAlpha(Color.White) * opacityMod,
					Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			}
			Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), 
				Projectile.rotation, TextureAssets.Projectile[Projectile.type].Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}
