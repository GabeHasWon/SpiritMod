using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using SpiritMod.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Items.Weapon.Magic.ShadowbreakWand
{
	public class Shatter : Particle
	{
		private readonly int MaxTime;

		public Shatter(Vector2 position, int maxTime)
		{
			Position = position;
			MaxTime = maxTime;
			Scale = 1;
		}

		public override void Update()
		{
			float Progress = TimeActive / (float)MaxTime;
			Color = Color.White * (1f - Progress);

			if (TimeActive > MaxTime)
				Kill();
		}

		public override bool UseAdditiveBlend => true;

		public override bool UseCustomDraw => true;

		public override void CustomDraw(SpriteBatch spriteBatch)
		{
			Texture2D gradient = ModContent.Request<Texture2D>("SpiritMod/Effects/Masks/CircleGradient").Value;
			spriteBatch.Draw(gradient, Position - Main.screenPosition, null, Color.HotPink * (1f - (TimeActive / (float)MaxTime)) * .75f, 0, gradient.Size() / 2, Scale, SpriteEffects.None, 0);

			DrawAberration.DrawChromaticAberration(Vector2.UnitX.RotatedBy(Rotation), 1.5f, delegate (Vector2 offset, Color colorMod)
			{
				spriteBatch.Draw(Texture, Position + offset - Main.screenPosition, null, Color.MultiplyRGB(colorMod), Rotation, Texture.Size() / 2, Scale, SpriteEffects.None, 0);
				spriteBatch.Draw(Texture, Position + offset - Main.screenPosition, null, Color.MultiplyRGB(colorMod) * .3f, Rotation + MathHelper.Pi, Texture.Size() / 2, Scale * 1.25f, SpriteEffects.None, 0);
			});
		}
	}
}
