using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.VinewrathDrops.VinewrathPet
{
	public class VinewrathPetProjectile_Bash : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wrathful Seedling");
			Main.projFrames[Projectile.type] = 6;
		}

		public override void SetDefaults()
		{
			Projectile.width = 12;
			Projectile.height = 12;
			Projectile.aiStyle = 0;
			Projectile.friendly = true;
			Projectile.timeLeft = 60;
			Projectile.alpha = 0;

			AIType = 0;
		}

		public override void AI()
		{
			if (++Projectile.frameCounter >= 3)
			{
				Projectile.frame++;
				if (Projectile.frame >= Main.projFrames[Projectile.type])
					Projectile.Kill();
			}
		}
		public override bool? CanDamage() => false;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Rectangle rect = new Rectangle(0, texture.Height / Main.projFrames[Projectile.type] * Projectile.frame, 
				texture.Width, texture.Height / Main.projFrames[Projectile.type] - 2);
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect, Projectile.GetAlpha(Color.White), 
				Projectile.rotation, rect.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
	}
}
