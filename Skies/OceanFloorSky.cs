using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;

namespace SpiritMod.Skies
{
	public class OceanFloorSky : CustomSky
	{
		private bool _isActive;
		private float _fadeOpacity;

		public override float GetCloudAlpha() => 1f - _fadeOpacity;

		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
		}

		public override void Update(GameTime gameTime)
		{
			if (_isActive && _fadeOpacity < 1f)
				_fadeOpacity += 0.05f;
			else if (!_isActive && _fadeOpacity > 0f)
				_fadeOpacity -= 0.005f;
		}

		public override void Activate(Vector2 position, params object[] args) => _isActive = true;
		public override void Deactivate(params object[] args) => _isActive = true;
		public override void Reset() => _isActive = false;

		public override bool IsActive()
		{
			if (!_isActive) {
				return _fadeOpacity > 0.001f;
			}
			return true;
		}
	}
}
