using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using Terraria;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster.Effects
{
	public class GoldMark : Particle
	{
		private const int _displayTime = 32;
		private readonly int TargetIndex;

		public GoldMark(Vector2 position, float scale, int targetWhoAmI)
		{
			Position = position;
			Scale = scale;
			TargetIndex = targetWhoAmI;
		}

		public override void Update()
		{
			if (Scale < 1)
				Scale += 0.025f;

			Position = Main.npc[TargetIndex].Center;
			Rotation += (1f - (float)((float)TimeActive / _displayTime)) * .5f;

			Lighting.AddLight(Position, Color.Goldenrod.ToVector3() / 3);
			if (TimeActive > _displayTime)
				Kill();
		}

		public override bool UseCustomDraw => true;

		public override void CustomDraw(SpriteBatch spriteBatch)
		{
			Texture2D tex = ParticleHandler.GetTexture(Type);
			var DrawFrame = tex.Bounds;

			spriteBatch.Draw(tex, Position - Main.screenPosition, DrawFrame, Color.White, Rotation, DrawFrame.Size() / 2, Scale, SpriteEffects.None, 0);
		}
	}
}
