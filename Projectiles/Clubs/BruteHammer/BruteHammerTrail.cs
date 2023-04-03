using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Mechanics.Trails;
using SpiritMod.Prim;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Clubs.BruteHammer
{
	public class BruteHammerTrail : BaseTrail
	{
		public float DissolveSpeed { get; set; }
		private readonly List<Vector2> _points;
		private readonly int _maxPoints;
		private int _pointsToKill;
		private readonly float _width;
		private float _deathProgress;
		private readonly Color _color;

		public BruteHammerTrail(Projectile projectile, Color color, float width, int maxPoints, TrailLayer layer = TrailLayer.UnderProjectile) : base(projectile, layer)
		{
			_points = new List<Vector2>();
			_width = width;
			_maxPoints = maxPoints;
			_deathProgress = 1f;
			_color = color;
		}

		public override void Dissolve()
		{
			_deathProgress = _points.Count / (float)_pointsToKill;

			if (_points.Count > 0)
				_points.RemoveAt(_points.Count - 1);

			if (_points.Count == 0)
				Dead = true;
		}

		public override void OnStartDissolve() => _pointsToKill = _points.Count;

		public override void Update()
		{
			_points.Insert(0, MyProjectile.Center);
			if (_points.Count > _maxPoints)
				_points.RemoveAt(_points.Count - 1);
		}

		public override void Draw(Effect effect, GraphicsDevice device)
		{
			if (Dead || _points.Count <= 1) return;

			//set the parameters for the shader
			Effect trailEffect = ModContent.Request<Effect>("SpiritMod/Effects/MotionNoiseTrail", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			trailEffect.Parameters["uTexture"].SetValue(ModContent.Request<Texture2D>("SpiritMod/Textures/voronoiLooping", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
			trailEffect.Parameters["uColor"].SetValue(_color.ToVector4());
			trailEffect.Parameters["progress"].SetValue(Main.GlobalTimeWrappedHourly * -0.2f);
			trailEffect.Parameters["xMod"].SetValue(1.5f);
			trailEffect.Parameters["yMod"].SetValue(0.5f);

			static float GetWidthMod(float progress = 0) => ((float)Math.Sin((Main.GlobalTimeWrappedHourly - progress) * MathHelper.TwoPi * 1.5f) * 0.33f + 1.33f) / (float)Math.Pow(1 - progress, 0.1f);

			PrimitiveStrip primStrip = new PrimitiveStrip
			{
				Color = Color.White * _deathProgress,
				Width = _width * _deathProgress,
				PositionArray = _points.ToArray(),
				TaperingType = StripTaperType.TaperEnd,
				WidthDelegate = delegate (float progress) { return GetWidthMod(progress); }
			};

			PrimitiveRenderer.DrawPrimitiveShape(primStrip, trailEffect);
		}
	}
}