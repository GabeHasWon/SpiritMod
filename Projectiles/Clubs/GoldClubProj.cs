using Microsoft.Xna.Framework;
using Terraria;
using static Terraria.ModLoader.ModContent;
using SpiritMod.Dusts;
using Terraria.Localization;

namespace SpiritMod.Projectiles.Clubs
{
	class GoldClubProj : ClubProj
	{
		public GoldClubProj() : base(new Vector2(82)) { }

		public override LocalizedText DisplayName => Language.GetText("Mods.SpiritMod.Items.GoldClub.DisplayName");

		public override void SafeSetStaticDefaults()
		{
			// DisplayName.SetDefault("Golden Greathammer");
			Main.projFrames[Projectile.type] = 2;
		}

		public override void Smash(Vector2 position)
		{
			for (int k = 0; k <= 110; k++)
				Dust.NewDustPerfect(Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), DustType<EarthDust>(), new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 10f);
		}
	}
}
