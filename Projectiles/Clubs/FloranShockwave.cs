using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace SpiritMod.Projectiles.Clubs
{
	public class FloranShockwave : ModProjectile
	{
		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Shockwave");

		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.damage = 1;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
			Projectile.timeLeft = 10;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.DamageType = DamageClass.Melee;
		}

		public override bool PreAI()
		{
			Projectile.position.X -= Projectile.velocity.X;

			for (int i = 0; i < 2; i++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height * 2, DustType<Dusts.FloranClubDust>());
				Main.dust[dust].scale *= Main.rand.NextFloat(.75f, 1.05f);
			}

			if (Projectile.timeLeft == 5 && Projectile.ai[0] > 0)
			{
				Vector2 dirUnit = Vector2.UnitX * Math.Sign(Projectile.velocity.X);

				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + (dirUnit * Projectile.width), dirUnit, ProjectileType<FloranShockwave>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0] - 1);
			}

			return false;
		}
	}
}