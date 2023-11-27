using Microsoft.Xna.Framework;
using Terraria;
using SpiritMod.Dusts;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SpiritMod.Projectiles.Clubs
{
	class WoodClubProj : ClubProj
	{
		public WoodClubProj() : base(new Vector2(58)) { }

		public override LocalizedText DisplayName => Language.GetText("Mods.SpiritMod.Items.WoodClub.DisplayName");

		public override void SafeSetStaticDefaults()
		{
			// DisplayName.SetDefault("Wooden Club");
			Main.projFrames[Projectile.type] = 2;
		}

		public override void Smash(Vector2 position)
		{
			for (int k = 0; k <= 100; k++)
				Dust.NewDustPerfect(Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), ModContent.DustType<EarthDust>(), new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 10f);
		}
	}
}
