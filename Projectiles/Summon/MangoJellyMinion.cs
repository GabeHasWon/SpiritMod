using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs.Summon;
using SpiritMod.Projectiles.BaseProj;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace SpiritMod.Projectiles.Summon
{
	[AutoloadMinionBuff("Mango Jelly", "A mini mango jelly fights for you!")]
	public class MangoJellyMinion : BaseMinion
	{
		public MangoJellyMinion() : base(640, 900, new Vector2(22, 22)) { }

		public override void AbstractSetStaticDefaults()
		{
			// DisplayName.SetDefault("Mango Jelly");
			Main.projFrames[Type] = 4;
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void AbstractSetDefaults() => AIType = ProjectileID.Raven;

		private ref float AiTimer => ref Projectile.ai[0];
		private bool Dashing => Projectile.velocity.Length() >= ProjectileID.Sets.TrailCacheLength[Projectile.type];

		public override void PostAI()
		{
			if (Projectile.Distance(Player.Center) > 1500)
			{
				Projectile.position = Player.position + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(125f));

				for (int i = 0; i < 25; i++)
					Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WitherLightning);
			}

			Projectile.velocity *= 0.97f;
			Projectile.velocity.Y = Math.Min(7.5f, Projectile.velocity.Y + 0.035f); //Gravity
		}

		public override void IdleMovement(Player player)
		{
			AiTimer = ++AiTimer % 60;
			int xOffset = (Main.rand.Next(4, 8) + 8) * ((player.position.X > Projectile.position.X) ? 1 : -1);

			if (player.position.Y < Projectile.position.Y && AiTimer == 0) //Charge
			{
				bool close = Projectile.Distance(player.Center) < 120;

				Projectile.velocity.X = xOffset * (close ? .1f : 1f);
				Projectile.velocity.Y = close ? -3f : -7f;
			}

			Projectile.rotation = Projectile.velocity.X / 10;
		}

		public override void TargettingBehavior(Player player, NPC target)
		{
			int cooldownTime = 40;
			AiTimer = ++AiTimer % cooldownTime;

			int xOffset = (Main.rand.Next(2, 4) + 10) * ((target.position.X > Projectile.position.X) ? 1 : -1);

			if (AiTimer == 0) //Charge
			{
				bool close = Projectile.Distance(target.Center) < 120;
				Projectile.velocity = new Vector2(xOffset * (close ? .5f : 1f), -MathHelper.Clamp(((Projectile.Center - target.Center) / (cooldownTime - 5)).Y, -7f, 7f));
			}

			Projectile.rotation = Projectile.velocity.X / 10;

			if (Dashing)
			{
				for (int i = 0; i < 2; i++)
				{
					Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.WitherLightning, 0f, 0f, 100, default, .5f);
					dust.velocity = Projectile.velocity / 2;
					dust.noGravity = true;
				}
			}
		}

		public override Color? GetAlpha(Color lightColor) => new Color(250, 210, 230);

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle drawFrame = new Rectangle(0, texture.Height / Main.projFrames[Type] * Projectile.frame, texture.Width, texture.Height / Main.projFrames[Type]);
			SpriteEffects effects = (Projectile.direction == -1) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, drawFrame, Projectile.GetAlpha(lightColor), Projectile.rotation, drawFrame.Size() / 2, Projectile.scale, effects, 0);

			if (Dashing)
			{
				for (int k = 0; k < Projectile.oldPos.Length; k++)
				{
					Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + (Projectile.Size / 2) + new Vector2(0f, Projectile.gfxOffY);
					Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
					
					Main.EntitySpriteDraw(texture, drawPos, drawFrame, color * .6f, Projectile.rotation, drawFrame.Size() / 2, Projectile.scale, effects, 0);
				}
			}

			return false;
		}

		public override bool DoAutoFrameUpdate(ref int framespersecond, ref int startframe, ref int endframe)
		{
			framespersecond = 8;
			return true;
		}

		public override bool MinionContactDamage() => Dashing;
	}
}