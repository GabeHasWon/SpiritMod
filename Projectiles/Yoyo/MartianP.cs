using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Yoyo
{
	public class MartianP : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Terrestrial Ultimatum");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.TheEyeOfCthulhu);
			Projectile.damage = 124;
			Projectile.extraUpdates = 1;
			AIType = ProjectileID.TheEyeOfCthulhu;
		}

		public override void PostAI() => Projectile.rotation -= 10;

		public override void AI()
		{
			if (++Projectile.frameCounter >= 200)
			{
				Projectile.frameCounter = 0;

				Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ProjectileID.Electrosphere, Projectile.damage, 0f, Main.player[Projectile.owner].whoAmI);
				proj.friendly = true;
				proj.hostile = false;
			}
		}

	}
}
