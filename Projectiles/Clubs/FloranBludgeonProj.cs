using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;
using SpiritMod.Dusts;

namespace SpiritMod.Projectiles.Clubs
{
	class FloranBludgeonProj : ClubProj
	{
		public override void SafeSetStaticDefaults()
		{
			DisplayName.SetDefault("Floran Bludgeon");
			Main.projFrames[Projectile.type] = 2;
		}

		public override void Smash(Vector2 position)
		{
			for (int k = 0; k <= 50; k++)
				Dust.NewDustPerfect(Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), DustType<EarthDust>(), new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 10f);
            
			Projectile.NewProjectile(Projectile.GetSource_FromAI("ClubSmash"), Projectile.Center - (Vector2.UnitY * 32), Vector2.UnitX * Main.player[Projectile.owner].direction * 2, ProjectileType<FloranShockwave>(), Projectile.damage / 4, Projectile.knockBack / 2, Projectile.owner, 8);
		}

		public FloranBludgeonProj() : base(52, new Point(80, 84), 21f){}
	}
}
