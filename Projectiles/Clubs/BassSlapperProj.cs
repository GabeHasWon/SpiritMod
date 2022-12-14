using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using SpiritMod.Dusts;

namespace SpiritMod.Projectiles.Clubs
{
	class BassSlapperProj : ClubProj
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bass Slapper");
			Main.projFrames[Projectile.type] = 2;
		}
		public override void Smash(Vector2 position)
		{
			Player player = Main.player[Projectile.owner];
			for (int k = 0; k <= 100; k++) {
				Dust.NewDustPerfect(Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), DustType<Dusts.EarthDust>(), new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 10f);
			}
            SoundEngine.PlaySound(SoundID.NPCHit1, Projectile.position);
		}
		public BassSlapperProj() : base(66, 13, 26, -1, 58, 13, 27, 1.7f, 12f){}
	}
}
