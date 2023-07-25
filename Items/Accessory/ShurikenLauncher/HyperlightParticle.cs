using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.ShurikenLauncher
{
	public class HyperlightParticle : Particle
	{
		private const int _displayTime = 8;

		private readonly int _numFrames = 3;
		private int _frame;

		private readonly int _numStyles = 3;
		private readonly int _style;
		private readonly Entity entity;

		public HyperlightParticle(Vector2 position, float rotation, float scale = 1, Entity attachedEntity = null)
		{
			Position = position;
			if (attachedEntity != null)
				_offset = Position - attachedEntity.Center;

			Rotation = rotation;
			Scale = scale;
			_style = Main.rand.Next(_numStyles);
			entity = attachedEntity;
		}

		private Vector2 _offset = Vector2.Zero;
		public override void Update()
		{
			if (entity != null)
			{
				if (!entity.active)
				{
					Kill();
					return;
				}
				Position = entity.Center + _offset;
			}

			Color = Color.White * (1.25f - ((float)TimeActive / _displayTime));
			_frame = (int)(_numFrames * TimeActive / _displayTime);

			if (TimeActive > _displayTime)
				Kill();
		}

		public override bool UseAdditiveBlend => true;

		public override bool UseCustomDraw => true;

		public override void CustomDraw(SpriteBatch spriteBatch)
		{
			Rectangle drawFrame = Texture.Frame(_numStyles, _numFrames, _style, _frame);
			Texture2D extra = ModContent.Request<Texture2D>("SpiritMod/Effects/Masks/CircleGradient").Value;

			spriteBatch.Draw(extra, Position - Main.screenPosition, null, Color.Cyan * .6f * (1f - ((float)TimeActive / _displayTime)), 1.57f, extra.Size() / 2, .4f, SpriteEffects.None, 0);
			spriteBatch.Draw(Texture, Position - Main.screenPosition, drawFrame, Color, Rotation, drawFrame.Size() / 2, Scale, SpriteEffects.None, 0);
		}
	}
}
