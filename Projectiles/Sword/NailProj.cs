using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using SpiritMod.Dusts;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace SpiritMod.Projectiles.Sword
{
	public class NailProj : ModProjectile
	{
		private readonly int frameTime = 3;
		public const int swingStates = 2;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Hollow Nail");
			Main.projFrames[Type] = 8;
		}

		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 50;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = frameTime * (Main.projFrames[Type] / swingStates);
			Projectile.ignoreWater = true;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override void AI()
		{
			if (++Projectile.frameCounter >= frameTime) 
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;
			}

			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.Center = Main.player[Projectile.owner].Center + (Vector2.Normalize(Projectile.velocity) * 38);

			if (Projectile.timeLeft % 3 == 0)
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<HollowDust>());
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Player player = Main.player[Projectile.owner];
			Vector2 angle = Vector2.Normalize(Projectile.velocity);

			if (angle.Y > 0 && player.velocity.Y != 0)
				player.velocity.Y = -(angle * 8f).Y;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle drawFrame = new Rectangle(0, texture.Height / Main.projFrames[Type] * Projectile.frame, texture.Width, texture.Height / Main.projFrames[Type]);
			SpriteEffects effects = (Projectile.direction == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			float rotation = Projectile.rotation + ((effects == SpriteEffects.FlipHorizontally) ? MathHelper.Pi : 0);

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, drawFrame, Projectile.GetAlpha(lightColor), rotation, drawFrame.Size() / 2, Projectile.scale, effects, 0);

			return false;
		}
	}
}