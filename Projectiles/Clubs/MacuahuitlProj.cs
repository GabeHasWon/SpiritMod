using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using SpiritMod.Dusts;

namespace SpiritMod.Projectiles.Clubs
{
	class MacuahuitlProj : ClubProj
	{
		public MacuahuitlProj() : base(new Vector2(82)) { }

		public override void SafeSetStaticDefaults()
		{
			// DisplayName.SetDefault("Macuahuitl");
			Main.projFrames[Projectile.type] = 2;
		}

		public override void Smash(Vector2 position)
		{
			for (int k = 0; k <= 120; k++)
				Dust.NewDustPerfect(Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), DustType<MacuahuitlDust>(), new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 10f);
		}

		public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.GetArmorPenetration(DamageClass.Melee) += (int)(Projectile.ai[0] / 2);
        }
	}
}
