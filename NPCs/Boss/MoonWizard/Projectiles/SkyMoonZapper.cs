using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.Boss.MoonWizard.Projectiles
{
	public class SkyMoonZapper : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.hostile = false;
			Projectile.friendly = false;
			Projectile.timeLeft = 100;
            Projectile.hide = true;
			Projectile.damage = 13;
			Projectile.width = Projectile.height = 32;

		}

        float alphaCounter;
		public override void AI()
        {
            alphaCounter += 0.04f;

			if (Projectile.timeLeft > 20 && Projectile.timeLeft % 10 == 0)
				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, 25), ModContent.ProjectileType<MoonPredictorTrail>(), 0, 0);

			if (Projectile.timeLeft == 10)
			{
				SoundEngine.PlaySound(SoundID.Item112, Projectile.position);
				int p = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, 500), Vector2.Zero, ModContent.ProjectileType<MoonThunder>(), Projectile.damage, 0);
                Main.projectile[p].hostile = true;
                Main.projectile[p].friendly = true;
            }
		}

        public override Color? GetAlpha(Color lightColor) => Color.White;

		public override void PostDraw(Color lightColor)
        {
            float sineAdd = (float)Math.Sin(alphaCounter) + 3;
            Main.spriteBatch.Draw(Terraria.GameContent.TextureAssets.Extra[49].Value, (Projectile.Center - Main.screenPosition), null, new Color((int)(7.5f * sineAdd), (int)(16.5f * sineAdd), (int)(18f * sineAdd), 0), 0f, new Vector2(50, 50), 0.25f * (sineAdd + .6f), SpriteEffects.None, 0f);
        }
    }
}