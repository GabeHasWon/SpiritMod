using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using Terraria;

namespace SpiritMod.Items.Sets.LaunchersMisc.Sharkbones
{
	public class SharkbonesExplosion : Particle
	{
		private const int _numFrames = 5;
		private int _frame;
		private const int _displayTime = 15;

		public SharkbonesExplosion(Vector2 position, float scale, float rotation)
		{
			Position = position;
			Scale = scale;
			Rotation = rotation;
		}

		public override void Update()
		{
			_frame = (int)(_numFrames * TimeActive / _displayTime);
			Lighting.AddLight(Position, Color.LightPink.ToVector3() / 2.5f);

			if (TimeActive > _displayTime)
				Kill();
		}

		public override bool UseCustomDraw => true;

		public override bool UseAdditiveBlend => true;

		public override void CustomDraw(SpriteBatch spriteBatch)
		{
			Texture2D tex = ParticleHandler.GetTexture(Type);
			var DrawFrame = new Rectangle(0, _frame * tex.Height / _numFrames, tex.Width, tex.Height / _numFrames);

			spriteBatch.Draw(tex, Position - Main.screenPosition, DrawFrame, Color.White, Rotation, DrawFrame.Size() / 2, Scale, SpriteEffects.None, 0);
		}
	}
}