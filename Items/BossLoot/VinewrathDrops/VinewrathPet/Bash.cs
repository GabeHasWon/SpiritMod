using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using Terraria;

namespace SpiritMod.Items.BossLoot.VinewrathDrops.VinewrathPet
{
	public class Bash : Particle
	{
		private const int _numFrames = 6;
		private int _frame;
		private readonly int _direction;
		private const int _displayTime = 18;

		public Bash(Vector2 position, float scale, float rotation)
		{
			Position = position;
			Scale = scale;
			_direction = Main.rand.NextBool() ? -1 : 1;
			Rotation = (_direction > 0) ? (rotation - MathHelper.Pi) : rotation;
		}

		public override void Update()
		{
			_frame = (int)(_numFrames * TimeActive / _displayTime);
			Lighting.AddLight(Position, Color.LightBlue.ToVector3() / 3);
			if (TimeActive > _displayTime)
				Kill();
		}

		public override bool UseCustomDraw => true;
		public override void CustomDraw(SpriteBatch spriteBatch)
		{
			Texture2D tex = ParticleHandler.GetTexture(Type);
			var DrawFrame = new Rectangle(0, _frame * tex.Height / _numFrames, tex.Width, tex.Height / _numFrames);
			spriteBatch.Draw(tex, Position - Main.screenPosition, DrawFrame, Color.White, Rotation, DrawFrame.Size() / 2, Scale, (_direction > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		}
	}
}
