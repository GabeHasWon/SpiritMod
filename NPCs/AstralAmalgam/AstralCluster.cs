using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Mechanics.Trails;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SpiritMod.NPCs.AstralAmalgam
{
	public class AstralCluster : ModProjectile, IDrawAdditive, ITrailProjectile
	{
		private Vector2? targetVelocity = null;

		private const int timeLeftMax = 120;

		public void DoTrailCreation(TrailManager tManager)
			=> tManager.CreateTrail(Projectile, new StandardColorTrail(Color.LightBlue), new RoundCap(), new DefaultTrailPosition(), 10f, 80f, new DefaultShader());

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Astral Cluster");

		public override void SetDefaults()
		{
			Projectile.penetrate = -1;
			Projectile.tileCollide = true;
			Projectile.hostile = true;
			Projectile.friendly = false;
			Projectile.timeLeft = timeLeftMax;
			Projectile.extraUpdates = 1;
			Projectile.scale = 0f;
			Projectile.width = Projectile.height = 20;
		}

		public override void AI()
		{
			int fadeTime = 10;
			float maxScale = 0.7f;

			if (Projectile.timeLeft <= fadeTime)
				Projectile.scale -= maxScale / fadeTime;
			else if (Projectile.scale < maxScale)
				Projectile.scale += maxScale / fadeTime;

			if (Projectile.timeLeft == timeLeftMax)
			{
				int close = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);

				targetVelocity = (Projectile.DirectionTo(Main.player[close].Center) * 12f).RotatedByRandom(0.35f);
				Projectile.netUpdate = true;
			}

			if (targetVelocity.HasValue)
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity.Value, 0.05f);

			Projectile.rotation += 0.02f;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);
			Texture2D bloom = TextureAssets.Extra[49].Value;
			SpiritMod.SunOrbShader.Parameters["colorMod"].SetValue(new Color(120, 190, 255).ToVector4());
			SpiritMod.SunOrbShader.Parameters["colorMod2"].SetValue(Color.LightBlue.ToVector4());
			SpiritMod.SunOrbShader.Parameters["timer"].SetValue(Main.GlobalTimeWrappedHourly / 3 % 1);
			SpiritMod.SunOrbShader.CurrentTechnique.Passes[0].Apply();

			float scale = MathHelper.Lerp(0.3f, 0.5f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2) / 2 + 0.5f);
			Color drawcolor = Projectile.GetAlpha(Color.Blue);
			Vector2 drawcenter = Projectile.Center - Main.screenPosition;

			Main.spriteBatch.Draw(bloom, drawcenter, null, drawcolor, Projectile.rotation, bloom.Size() / 2, Projectile.scale * 0.5f * MathHelper.Lerp(scale, 1, 0.25f), SpriteEffects.None, 0);

			Main.spriteBatch.Draw(bloom, drawcenter, null, drawcolor * 0.2f, Projectile.rotation, bloom.Size() / 2, Projectile.scale * scale, SpriteEffects.None, 0);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);

			return false;
		}

		public void AdditiveCall(SpriteBatch spriteBatch, Vector2 screenPos)
		{
			Vector2 primScale = new Vector2(30);

			for (int i = 0; i < 2; i++)
			{
				float zRot = Math.Abs(((Projectile.rotation * 25) + (MathHelper.PiOver2 * i)).ToRotationVector2().Y);
				primScale += Vector2.One * ((float)Math.Sin(Main.GlobalTimeWrappedHourly * (3 + i)) - (i * 5));

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

		public override void SendExtraAI(BinaryWriter writer)
		{
			if (targetVelocity.HasValue)
				writer.WriteVector2(targetVelocity.Value);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			if (targetVelocity.HasValue)
				targetVelocity = reader.ReadVector2();
		}
	}
}