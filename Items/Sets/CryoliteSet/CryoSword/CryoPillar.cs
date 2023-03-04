using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using SpiritMod.Buffs.DoT;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SpiritMod.Items.Sets.CryoliteSet.CryoSword
{
	public class CryoPillar : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cryo Pillar");
			Main.projFrames[Type] = 6;
		}

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 60;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.extraUpdates = 1;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			for (int i = 0; i < 2; i++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueCrystalShard);
				Main.dust[dust].velocity = (Vector2.UnitY * Main.rand.NextFloat(0.0f, 5.0f)).RotatedByRandom(0.3f);
				Main.dust[dust].noGravity = true;
			}

			if (++Projectile.frameCounter > 3)
			{
				Projectile.frameCounter = 0;

				if (++Projectile.frame >= Main.projFrames[Type])
					Projectile.Kill();
			}

			Projectile.direction = Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);
			Projectile.velocity.Y = 1;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<CryoCrush>(), 300);

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.velocity.Y--;
			return false;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) => fallThrough = false;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Rectangle frame = new Rectangle(0, texture.Height / Main.projFrames[Type] * Projectile.frame, texture.Width, (texture.Height / Main.projFrames[Type]) - 2);
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, frame.Size() / 2, Projectile.scale, (Projectile.direction == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
			return false;
		}
	}
}