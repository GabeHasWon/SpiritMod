using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using SpiritMod.Utilities;
using System;
using Terraria;

namespace SpiritMod.Items.Sets.CryoliteSet
{
    public class CryoParticle : Particle
	{
		public readonly int MaxTime;

		private float opacity;

		public override bool UseAdditiveBlend => true;
		public override bool UseCustomDraw => true;

		public CryoParticle(Vector2 position, Vector2 velocity, Color color, float scale, int maxTime)
		{
			Color = color;
			Position = position;
			Velocity = velocity;
			Scale = scale;
			MaxTime = maxTime;
		}

		public override void Update()
		{
			opacity = (float)Math.Pow(1 - ((float)TimeActive / MaxTime), 0.5f);
			Lighting.AddLight(Position, opacity * Color.R / 255f, opacity * Color.G / 255f, opacity * Color.B / 255f);

			Rotation = Velocity.ToRotation();

			Velocity *= 0.87f;
			Scale *= 0.96f;

			if (TimeActive >= MaxTime)
				Kill();
		}

		public override void CustomDraw(SpriteBatch spriteBatch)
		{
			Texture2D baseTex = ParticleHandler.GetTexture(Type);

			DrawAberration.DrawChromaticAberration(Vector2.UnitX.RotatedBy(Rotation), 1.5f, delegate (Vector2 offset, Color colorMod)
			{
				colorMod.B = (byte)Math.Min(Color.B + 50, 255);

				spriteBatch.Draw(baseTex, Position + offset - Main.screenPosition, null, Color.MultiplyRGB(colorMod) * opacity, Rotation, baseTex.Size() / 2, Scale / 2f, SpriteEffects.None, 0);
			});
		}
	}
}