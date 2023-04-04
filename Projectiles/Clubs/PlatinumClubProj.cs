using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;

namespace SpiritMod.Projectiles.Clubs
{
	class PlatinumClubProj : ClubProj
	{
		public override void SafeSetStaticDefaults()
		{
			DisplayName.SetDefault("Platinum Clobberer");
			Main.projFrames[Projectile.type] = 2;
		}

		public override void Smash(Vector2 position)
		{
			for (int k = 0; k <= 110; k++)
				Dust.NewDustPerfect(Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), DustType<Dusts.EarthDust>(), new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 10f);
		}

		public PlatinumClubProj() : base(55, new Point(58, 58), 18f){}
	}
}
