using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Dusts;
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
            Player player = Main.player[Projectile.owner];
            for (int k = 0; k <= 70; k++)
                Dust.NewDustPerfect(Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), ModContent.DustType<EarthDust>(), new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 10f);
            for (int k = 0; k <= 30; k++)
                Dust.NewDustPerfect(Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), ModContent.DustType<CryoDust>(), new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 10f);
            
			Projectile.NewProjectile(Projectile.GetSource_FromAI("ClubSmash"), Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<NautilusBubbleSpawner>(), Projectile.damage / 2, Projectile.knockBack / 2, Projectile.owner, 8, player.direction);
        }

        public override void SafeDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (Projectile.ai[0] >= ChargeTime)
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, Main.player[Projectile.owner].Center - Main.screenPosition, new Rectangle(0, size.Y * 2, size.X, size.Y), Color.White * 0.9f, TrueRotation, Origin, Projectile.scale, Effects, 1);
        }

        public NautilusClubProj() : base(64, size, 19){}
	}
}
