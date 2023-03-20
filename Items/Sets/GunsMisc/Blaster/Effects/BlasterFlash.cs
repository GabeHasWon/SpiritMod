using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using Terraria;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster.Effects
{
	public class BlasterFlash : Particle
	{
		private const int _numFrames = 3;
		private readonly int _frame;
		private readonly int _direction;
		private const int _displayTime = 2;

		public BlasterFlash(Vector2 position, float scale, float rotation)
		{
			Position = position;
			Scale = scale;
			_direction = Main.rand.NextBool() ? -1 : 1;
			Rotation = (_direction < 0) ? (rotation - MathHelper.Pi) : rotation;
			_frame = Main.rand.Next(_numFrames);
		}

		public override void Update()
		{
			Lighting.AddLight(Position, Color.Yellow.ToVector3() / 3);
			if (TimeActive > _displayTime)
				Kill();
		}

		public override bool UseCustomDraw => true;

		public override void CustomDraw(SpriteBatch spriteBatch)
		{
			Texture2D tex = ParticleHandler.GetTexture(Type);
			var DrawFrame = new Rectangle(0, _frame * tex.Height / _numFrames, tex.Width, (tex.Height / _numFrames) - 2);
			spriteBatch.Draw(tex, Position - Main.screenPosition, DrawFrame, Color.White, Rotation, DrawFrame.Size() / 2, Scale, (_direction > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		}
	}
}