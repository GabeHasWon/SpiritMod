﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace SpiritMod.Prim
{
	public partial class PrimTrail
	{
		public bool Destroyed { get; set; } = false;

		public Entity Entity = null;
		public bool Pixellated { get; set; } = false;

		public bool Disabled { get; set; } = false;

		public int EntityType { get; protected set; }

		public int DrawType { get; set; } = 0;

		public float Width { get; protected set; }

		public float AlphaValue { get; protected set; }

		public int Cap { get; protected set; }

		public ITrailShader TrailShader { get; protected set; }

		public int Counter { get; protected set; }

		public int PointCount { get; protected set; }

		public Color Color { get; protected set; } = new Color(255, 255, 255);

		public List<Vector2> Points { get; protected set; } = new List<Vector2>();

		public GraphicsDevice GraphicsDevice { get; protected set; }

		public Effect Effect { get; protected set; }

		public VertexPositionColorTexture[] Vertices { get; protected set; }

		public int CurrentIndex { get; protected set; }

		public PrimTrail()
		{
			if (Main.dedServ)
			{
				Dispose();
				return;
			}

			TrailShader = new DefaultShader();
			GraphicsDevice = Main.graphics.GraphicsDevice;
			SetDefaults();
			Vertices = new VertexPositionColorTexture[Cap];
		}


		public void Dispose() => SpiritMod.primitives._trails.Remove(this);

		public void Update() => OnUpdate();

		public virtual void OnUpdate() { }

		public void Draw()
		{
			Vertices = new VertexPositionColorTexture[PointCount];
			CurrentIndex = 0;

			PrimStructure(Main.spriteBatch);
			SetShaders();

			if (PointCount >= 1)
				GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, Vertices, 0, PointCount / 3);
		}

		public virtual void PrimStructure(SpriteBatch spriteBatch) { }

		public virtual void SetShaders() { }

		public virtual void SetDefaults() { }

		public virtual void OnDestroy() { }
	}
}