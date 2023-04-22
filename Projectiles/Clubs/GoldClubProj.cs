using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;
using SpiritMod.Dusts;

namespace SpiritMod.Projectiles.Clubs
{
	class GoldClubProj : ClubProj
	{
		public override void SafeSetStaticDefaults()
		{
			DisplayName.SetDefault("Golden Greathammer");
			Main.projFrames[Projectile.type] = 2;
		}

		public override void Smash(Vector2 position)
		{
			for (int k = 0; k <= 110; k++)
				Dust.NewDustPerfect(Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), DustType<EarthDust>(), new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 10f);
		}

		public GoldClubProj() : base(60, new Point(82, 82), 18f){}
	}
}
