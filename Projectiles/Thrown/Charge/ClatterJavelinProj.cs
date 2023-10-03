using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Thrown.Charge
{
	public class ClatterJavelinProj : JavelinProj
	{
		internal override int ChargeTime => 100;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Clatter Javelin");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void OnKill(int timeLeft)
		{
			if (!Released)
				return;

			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

			for (int i = 0; i < 20; i++)
				Dust.NewDustPerfect(Projectile.Center, DustID.Torch, -(Vector2.Normalize(Projectile.velocity) * Main.rand.NextFloat(1.0f, 3.0f)).RotatedByRandom(1f), 0, default, 1f).noGravity = true;
		}
	}
}
