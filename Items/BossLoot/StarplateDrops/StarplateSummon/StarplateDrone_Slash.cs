using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.StarplateDrops.StarplateSummon
{
	public class StarplateDrone_Slash : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Starplate Fighter Drone");
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.timeLeft = 300;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 20;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
			if (++Projectile.frameCounter >= 4)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
					Projectile.Kill();
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Rectangle rect = new Rectangle(0, texture.Height / Main.projFrames[Projectile.type] * Projectile.frame, 
				texture.Width, texture.Height / Main.projFrames[Projectile.type] - 2);
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, rect, Projectile.GetAlpha(Color.White), Projectile.rotation, rect.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
			return false;
		}
		public override bool? CanDamage() => false;
	}
}