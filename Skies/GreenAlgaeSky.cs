using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.NPCs.StarjinxEvent;
using System;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace SpiritMod.Skies
{
	public class GreenAlgaeSky : CustomSky
	{
		private UnifiedRandom _random = new UnifiedRandom();

		private Texture2D _bgTexture;


		private bool _isActive;

		private float _fadeOpacity;

		public override void OnLoad()
		{
			this._bgTexture = ModContent.Request<Texture2D>("Terraria/Images/Misc/VortexSky/Background", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
		}

		public override void Update(GameTime gameTime)
		{
			if (this._isActive && !Main.LocalPlayer.GetModPlayer<StarjinxPlayer>().zoneStarjinxEvent) {
				this._fadeOpacity = Math.Min(1f, 0.01f + this._fadeOpacity);
			}
			else {
				this._fadeOpacity = Math.Max(0f, this._fadeOpacity - 0.01f);
			}
		}

		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f) {
				spriteBatch.Draw(_bgTexture, new Rectangle(0, Math.Max(0, (int)((Main.worldSurface * 16.0 - Main.screenPosition.Y - 700.0) * 0.1)), Main.screenWidth, Main.screenHeight), new Color(94, 255, 250, 240) * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f * this._fadeOpacity));
			}
		}

		public override float GetCloudAlpha() => 1f;
		public override void Activate(Vector2 position, params object[] args) => _isActive = true;
		public override void Deactivate(params object[] args) => _isActive = false;
		public override void Reset() => _isActive = false;

		public override bool IsActive()
		{
			if (!_isActive)
				return _fadeOpacity > 0.001f;
			return true;
		}
	}
}
