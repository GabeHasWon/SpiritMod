using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Summon.Dragon
{
	public class DragonHeadTwo : ModProjectile
	{
		int num;
		int counter = 0;
		float distance = 8;
		readonly int rotationalSpeed = 4;

		public override LocalizedText DisplayName => Language.GetText("Mods.SpiritMod.Projectiles.DragonBodyOne.DisplayName");

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.penetrate = 6;
			Projectile.tileCollide = false;
			Projectile.hostile = false;
			Projectile.friendly = true;
			Projectile.timeLeft = 95;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.width = Projectile.height = 32;
		}
		
		public override Color? GetAlpha(Color lightColor) => new Color(66 - (int)(num / 3 * 2), 245 - (int)(num / 3 * 2), 120 - (int)(num / 3 * 2), 255 - num);

		public override bool PreDraw(ref Color lightColor)
		{
			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++) {
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			}
			return false;
		}

		public override bool OnTileCollide(Vector2 oldVelocity) => false;

		public override void AI()
		{
			num += 4;
			Projectile.alpha += 12;
			Projectile.spriteDirection = 1;

			if (Projectile.ai[0] > 0)
				Projectile.spriteDirection = 0;

			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
			distance += 0.03f;
			counter += rotationalSpeed;
			Vector2 initialSpeed = new Vector2(Projectile.ai[0], Projectile.ai[1]);
			Vector2 offset = initialSpeed.RotatedBy(Math.PI / 2);
			offset.Normalize();
			offset *= (float)(Math.Cos(counter * (Math.PI / 180)) * (distance / 3));
			Projectile.velocity = initialSpeed + offset;
			bool flag25 = false;
			int jim = 1;
			for (int index1 = 0; index1 < 200; index1++) {
				if (Main.npc[index1].CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[index1].Center, 1, 1)) {
					float num23 = Main.npc[index1].position.X + (float)(Main.npc[index1].width / 2);
					float num24 = Main.npc[index1].position.Y + (float)(Main.npc[index1].height / 2);
					float num25 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num23) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num24);
					if (num25 < 500f) {
						flag25 = true;
						jim = index1;
					}

				}
			}
			if (flag25)
			{
				float num1 = 12.5f;
				Vector2 vector2 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
				float num2 = Main.npc[jim].Center.X - vector2.X;
				float num3 = Main.npc[jim].Center.Y - vector2.Y;
				Vector2 direction5 = Main.npc[jim].Center - Projectile.Center;
				direction5.Normalize();
				Projectile.rotation = Projectile.DirectionTo(Main.npc[jim].Center).ToRotation() + 1.57f;
				float num4 = (float)Math.Sqrt((double)num2 * (double)num2 + (double)num3 * (double)num3);
				float num5 = num1 / num4;
				float num6 = num2 * num5;
				float num7 = num3 * num5;
				int num8 = 10;

				if (Main.rand.NextBool(16))
				{
					Projectile.velocity.X = (Projectile.velocity.X * (float)(num8 - 1) + num6) / (float)num8;
					Projectile.velocity.Y = (Projectile.velocity.Y * (float)(num8 - 1) + num7) / (float)num8;
				}
			}
		}
	}
}