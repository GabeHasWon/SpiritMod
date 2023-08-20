using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ID;
using SpiritMod.Mechanics.Trails;
using SpiritMod.Utilities;

namespace SpiritMod.Items.Sets.AccessoriesMisc.CrystalFlower
{
	public class CrystalFlowerProjectile : ModProjectile, IDrawAdditive, ITrailProjectile
	{
		private const int fadeoutTime = 24;

		public void DoTrailCreation(TrailManager tM) 
			=> tM.CreateTrail(Projectile, new GradientTrail(Color.White, Color.Magenta), new RoundCap(), new DefaultTrailPosition(), 6f, 35f, new ImageShader(Mod.Assets.Request<Texture2D>("Textures/Trails/Trail_2").Value, 0.01f, 1f, 1f));

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
			// DisplayName.SetDefault("Crystal Flower");
		}

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.extraUpdates = 1;
			Projectile.timeLeft = fadeoutTime;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			Projectile.alpha += 255 / fadeoutTime;
		}

		public override void Kill(int timeLeft)
		{
			if (timeLeft <= 0)
				return;

			for (int k = 0; k < 14; k++)
			{
				float unitMax = 8.0f;

				Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.0f, unitMax);
				if (k < 5)
					velocity = (-Vector2.UnitX * Main.rand.NextFloat(3.0f, unitMax)).RotatedBy(Projectile.rotation).RotatedByRandom(1f);

				Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(2) ? DustID.CrystalSerpent_Pink : DustID.CrystalSerpent, velocity, 0, default, 1.5f - (float)(velocity.Length() / unitMax)).noGravity = true;
			}
		}

		public void AdditiveCall(SpriteBatch spriteBatch, Vector2 screenPos)
		{
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Color color = new Color(255, 255, 255) * 0.45f * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				float scale = Projectile.scale * 1.4f;

				spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, Projectile.oldPos[k] + Projectile.Size / 2 - screenPos, null, color, Projectile.oldRot[k], TextureAssets.Projectile[Projectile.type].Value.Size() / 2, scale, default, default);
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = Color.White;

			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
			
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				float quoteant = (float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length;

				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * quoteant;

				DrawAberration.DrawChromaticAberration(Vector2.UnitX.RotatedBy(Projectile.rotation), 1.5f, delegate (Vector2 offset, Color colorMod)
				{
					Main.EntitySpriteDraw(texture, drawPos + offset, null, colorMod, Projectile.rotation, drawOrigin, Projectile.scale * quoteant, SpriteEffects.None, 0);
				});
			}
			return false;
		}
	}
}