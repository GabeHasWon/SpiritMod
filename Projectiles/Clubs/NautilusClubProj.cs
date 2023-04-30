using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Dusts;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Clubs
{
	class NautilusClubProj : ClubProj
	{
		private static Point size = new(80, 102);

		public override void SafeSetStaticDefaults()
		{
			DisplayName.SetDefault("Nautilobber");
			Main.projFrames[Projectile.type] = 3;
		}

		public override void Smash(Vector2 position)
		{
            for (int k = 0; k <= 70; k++)
                Dust.NewDustPerfect(Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), ModContent.DustType<EarthDust>(), new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 10f);
            for (int k = 0; k <= 30; k++)
                Dust.NewDustPerfect(Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), ModContent.DustType<CryoDust>(), new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 10f);

			for (int i = 0; i < 5; i++)
			{
				Vector2 randomVel = (Main.rand.NextVector2Unit() * Main.rand.NextFloat(2.0f, 5.0f)) - (Vector2.UnitY * Main.rand.NextFloat(1.5f, 3.2f));
				randomVel.Y = Math.Min(5f, randomVel.Y); //Random velocity with an upward bias

				Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI("ClubSmash"), Projectile.Center + (Vector2.UnitY * 6), randomVel, ModContent.ProjectileType<NautilusBubbleProj>(), Projectile.damage / 4, 0f, Projectile.owner);
				proj.scale = Main.rand.NextFloat(.7f, 1f);
				proj.timeLeft = Main.rand.Next(400, 580);
			}
        }

        public override void SafeDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (Projectile.ai[0] >= ChargeTime)
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, Main.player[Projectile.owner].Center - Main.screenPosition, new Rectangle(0, size.Y * 2, size.X, size.Y), Color.White * 0.9f, TrueRotation, Origin, Projectile.scale, Effects, 1);
        }

        public NautilusClubProj() : base(64, size, 19){}
	}
}
