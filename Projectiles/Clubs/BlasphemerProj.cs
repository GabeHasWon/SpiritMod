using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using SpiritMod.Dusts;

namespace SpiritMod.Projectiles.Clubs
{
	class BlasphemerProj : ClubProj
	{
		private static Point size = new(84, 82);

		public override void SafeSetStaticDefaults()
		{
			DisplayName.SetDefault("Blasphemer");
			Main.projFrames[Projectile.type] = 3;
        }

		public override void Smash(Vector2 position)
		{
			Player player = Main.player[Projectile.owner];
			for (int k = 0; k <= 100; k++)
				Dust.NewDustPerfect(Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), DustType<BoneDust>(), new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 10f);

            for (int k = 0; k <= 100; k++)
                Dust.NewDustPerfect(Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), DustType<FireClubDust>(), new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 10f);

			int a = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, 0, -12, ProjectileType<Magic.Firespike>(), Projectile.damage/3, Projectile.knockBack / 2, Projectile.owner, 0, player.direction);
            Main.projectile[a].DamageType = DamageClass.Melee;
            SoundEngine.PlaySound(SoundID.NPCHit20, Projectile.position);
        }

        public override void SafeDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (Projectile.ai[0] >= ChargeTime)
			{
				Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, Main.player[Projectile.owner].Center - Main.screenPosition, new Rectangle(0, size.Y * 2, size.X, size.Y), Color.White * 0.9f, TrueRotation, Origin, Projectile.scale, Effects, 1);
                Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
				for (int k = 0; k < Projectile.oldPos.Length; k++)
                {
                    Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, TrueRotation, drawOrigin, Projectile.scale, Effects, 0f);
                }
            }
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.OnFire, 180);
        }

        public BlasphemerProj() : base(66, size, 17f){}
	}
}
