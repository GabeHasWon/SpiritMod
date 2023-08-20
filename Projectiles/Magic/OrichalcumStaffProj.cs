using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Magic
{
	public class OrichalcumStaffProj : ModProjectile
	{
		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		// public override void SetStaticDefaults() => DisplayName.SetDefault("Orichalcum Petal");

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.penetrate = 5;
			Projectile.alpha = 255;
			Projectile.timeLeft = 20;
			Projectile.tileCollide = false;
		}

		public override void Kill(int timeLeft)
		{
			if (Main.myPlayer != Projectile.owner)
				return;

			for (int i = 0; i < 8; ++i) {
				Vector2 targetDir = Vector2.Normalize(((float)Math.PI * 2 / 8 * i).ToRotationVector2()) * 4f;
				Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y, targetDir.X, targetDir.Y, ModContent.ProjectileType<OrichHoming>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
			}
		}
	}
}
