using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs;
using SpiritMod.Dusts;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Returning
{
	public class FloraP : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Floran Roller");
			Main.projFrames[Projectile.type] = 3;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = 3;
			Projectile.timeLeft = 300;
			Projectile.extraUpdates = 1;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Main.rand.NextBool(5))
				target.AddBuff(ModContent.BuffType<VineTrap>(), 180);
		}

		public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 40; i++)
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.JungleGrass, Main.rand.Next(-4, 5), Main.rand.Next(-4, 5), 133);
		}

		float rotation = 2;
		int jumpCounter = 6;
		bool stopped = false;

		public override bool PreAI()
		{
			jumpCounter++;
			rotation *= 1.005f;

			Projectile.velocity.Y += 0.2f;
			Projectile.velocity.X = MathHelper.Clamp(Projectile.velocity.X * 1.005f, -10, 10);
			Projectile.rotation += Projectile.velocity.X < 0 ? -rotation / 25f : rotation / 25f;

			for (float i = Projectile.rotation; i < Projectile.rotation + 6.28f; i += 1.57f)
			{
				Dust dust = Dust.NewDustPerfect(Projectile.Center + new Vector2((float)Math.Cos(i) * 24, (float)Math.Sin(i) * 24), ModContent.DustType<FloranDust>());
				dust.scale = 0.66f;
				dust.velocity = Vector2.Zero;
			}

			if (Math.Abs(Projectile.velocity.X) > 5)
				Projectile.frame = 2;
			else if (Math.Abs(Projectile.velocity.X) > 3)
				Projectile.frame = 1;

			Projectile.spriteDirection = Projectile.direction;
			return false;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (oldVelocity.X != Projectile.velocity.X)
			{
				if (jumpCounter > 5 && !stopped)
				{
					jumpCounter = 0;
					Projectile.position.Y -= 20;
					Projectile.velocity.X = oldVelocity.X;
				}
				else if (!stopped)
				{
					stopped = true;
					Projectile.velocity.X = 0;
				}
			}
			return false;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (Math.Abs(Projectile.velocity.X) > 5)
			{
				Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
				for (float k = 0; k < Projectile.oldPos.Length; k += (Projectile.oldPos.Length / 3))
				{
					Vector2 drawPos = Projectile.oldPos[(int)k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
					Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / Projectile.oldPos.Length);
					Main.spriteBatch.Draw(ModContent.Request<Texture2D>("SpiritMod/Projectiles/Returning/FloraSpin", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
				}
				return false;
			}
			return true;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			fallThrough = false;
			return true;
		}
	}
}