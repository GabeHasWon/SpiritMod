using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.ShieldCore
{
	public class InterstellarShield : ModProjectile, IDrawAdditive
	{
		public const float rechargeRate = .1f;
		public const int cooldownTime = 1200; //20 seconds

		public ref float Degrees => ref Projectile.ai[0];

		public ref float Life => ref Projectile.ai[1];

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(48);
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.hostile = false;
			Projectile.friendly = true;
			Projectile.timeLeft = 10;
			Projectile.extraUpdates = 1;
			Projectile.alpha = 255;
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);

			for (int k = 0; k < 15; k++)
				Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.DungeonSpirit, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			if (player.GetSpiritPlayer().shieldCore)
				Projectile.timeLeft = 10;

			int lifeMax = Math.Max(40, (int)(player.statDefense * 1.5f));
			Life = Math.Min(lifeMax, Life + rechargeRate);

			float quoteant = MathHelper.Clamp(Life / lifeMax, 0, 1);
			Projectile.Opacity = Projectile.scale = quoteant;

			Projectile.rotation += .02f;
			Degrees = (Degrees + .5f) % 360;
			Projectile.Center = player.Center + new Vector2(80, 0).RotatedBy(MathHelper.ToRadians(Degrees));

			//If the shield is on cooldown, don't run functional logic
			if (Life < 0)
				return;
			if (player.whoAmI == Main.myPlayer)
				foreach (Projectile proj in Main.projectile)
					if (proj.hostile && proj.active && proj.Hitbox.Intersects(Projectile.Hitbox))
					{
						if ((Life -= proj.damage) < 0)
						{
							Life = -(cooldownTime * rechargeRate);
							SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);
							Projectile.netUpdate = true;

							break;
						} //Destroy the shield
						else 
						{
							SoundEngine.PlaySound(SoundID.Item93, Projectile.position);
							proj.active = false;
							proj.netUpdate = true;
						} //Destroy the enemy projectile
					}

			if (Main.rand.NextBool(Math.Max(1, 30 - (int)(quoteant * 30f))))
			{
				float rotUnit = player.miscCounter / 60f;
				for (int i = 0; i < 3; i++)
				{
					Vector2 position = Projectile.Center + (rotUnit * 6.28f + 2.09f * i).ToRotationVector2() * 8f;

					Dust dust = Dust.NewDustPerfect(position, DustID.DungeonSpirit, Vector2.Zero, 100, default, .8f);
					dust.noGravity = true;
					dust.velocity = player.velocity;
					dust.noLight = true;
				}
			}
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			Texture2D bloom = TextureAssets.Extra[49].Value;
			SpiritMod.SunOrbShader.Parameters["colorMod"].SetValue(new Color(120, 190, 255).ToVector4());
			SpiritMod.SunOrbShader.Parameters["colorMod2"].SetValue(Color.LightBlue.ToVector4());
			SpiritMod.SunOrbShader.Parameters["timer"].SetValue(Main.GlobalTimeWrappedHourly / 3 % 1);
			SpiritMod.SunOrbShader.CurrentTechnique.Passes[0].Apply();

			float scale = MathHelper.Lerp(.4f, .6f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2) / 2 + 0.5f);
			Color drawcolor = Projectile.GetAlpha(Color.Blue);
			Vector2 drawcenter = Projectile.Center - Main.screenPosition;

			Main.spriteBatch.Draw(bloom, drawcenter, null, drawcolor, Projectile.rotation, bloom.Size() / 2, Projectile.scale * .66f * MathHelper.Lerp(scale, 1, .25f), SpriteEffects.None, 0);

			Main.spriteBatch.Draw(bloom, drawcenter, null, drawcolor * .2f, Projectile.rotation, bloom.Size() / 2, Projectile.scale * scale, SpriteEffects.None, 0);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			return false;
		}

		public void AdditiveCall(SpriteBatch spriteBatch, Vector2 screenPos)
		{
			Vector2 primScale = new Vector2(30);

			for (int i = 0; i < 2; i++)
			{
				float zRot = Math.Abs(((Degrees / 30) + (MathHelper.PiOver2 * i)).ToRotationVector2().Y);
				primScale += Vector2.One * ((float)Math.Sin(Main.GlobalTimeWrappedHourly * (3 + i)) - (i * 10));

				Effect effect = SpiritMod.ShaderDict["PulseCircle"];
				Color rColor = Projectile.GetAlpha(new Color(100, 180, 255));

				effect.Parameters["BaseColor"].SetValue(rColor.ToVector4());
				effect.Parameters["RingColor"].SetValue(rColor.ToVector4());
				var square = new Prim.SquarePrimitive
				{
					Color = rColor * .5f,
					Length = primScale.X * zRot * Projectile.scale,
					Height = primScale.Y * Projectile.scale,
					Position = Projectile.Center - Main.screenPosition,
					Rotation = Projectile.rotation + (MathHelper.PiOver2 * i),
					ColorXCoordMod = 1 - zRot
				};
				Prim.PrimitiveRenderer.DrawPrimitiveShape(square, effect);
			}
		}

		public override bool? CanDamage() => false;

		public override bool? CanCutTiles() => false;
	}
}