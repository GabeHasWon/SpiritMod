﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using System;
using SpiritMod.Prim;

namespace SpiritMod.Items.Sets.FlailsMisc.JadeDao
{
	public class JadeDaoBasicTrail : PrimTrail
	{
		public JadeDaoBasicTrail(Projectile projectile)
		{
			Entity = projectile;
			EntityType = projectile.type;
			DrawType = PrimTrailManager.DrawProjectile;
			Color = new Color(18, 163, 85) * 0.6f;
			Width = 20;
			Cap = 5;
			AlphaValue = 1f;
			Counter = Main.rand.Next(100);
		}

		public override void PrimStructure(SpriteBatch spriteBatch)
		{
			if (PointCount <= 6) 
				return;

			float widthVar;
			var proj = Entity as Projectile;
			bool flip = false;

			if (proj.ModProjectile is JadeDaoProj modproj)
				flip = !modproj.Flip;

			if (Main.player[proj.owner].direction == -1)
				flip = !flip;

			for (int i = 0; i < Points.Count; i++)
			{
				if (i == 0)
				{
					widthVar = Width;
					Color colorvar = Color;
					Vector2 normalAhead = CurveNormal(Points, i + 1);
					Vector2 secondUp = Points[i + 1] - normalAhead * widthVar;
					Vector2 secondDown = Points[i + 1] + normalAhead * widthVar;
					AddVertex(Points[i], colorvar * AlphaValue, new Vector2(0, 0.5f));
					AddVertex(secondUp, colorvar * AlphaValue, new Vector2((i + 1) / (float)Points.Count, flip ? 1 : 0));
					AddVertex(secondUp, colorvar * AlphaValue, new Vector2((i + 1) / (float)Points.Count, flip ? 0 : 1));
				}
				else
				{
					if (i != Points.Count - 1)
					{
						widthVar = Width;
						Color colorvar = Color;
						Vector2 normal = CurveNormal(Points, i);
						Vector2 normalAhead = CurveNormal(Points, i + 1);
						Vector2 firstUp = Points[i] - normal * widthVar;
						Vector2 firstDown = Points[i] + normal * widthVar;
						Vector2 secondUp = Points[i + 1] - normalAhead * widthVar;
						Vector2 secondDown = Points[i + 1] + normalAhead * widthVar;

						AddVertex(firstDown, colorvar * AlphaValue, new Vector2((i / ((float)Points.Count)), flip ? 0 : 1));
						AddVertex(firstUp, colorvar * AlphaValue, new Vector2((i / ((float)Points.Count)), flip ? 1 : 0));
						AddVertex(secondDown, colorvar * AlphaValue, new Vector2((i + 1) / ((float)Points.Count), flip ? 0 : 1));

						AddVertex(secondUp, colorvar * AlphaValue, new Vector2((i + 1) / ((float)Points.Count), flip ? 1 : 0));
						AddVertex(secondDown, colorvar * AlphaValue, new Vector2((i + 1) / ((float)Points.Count), flip ? 0 : 1));
						AddVertex(firstUp, colorvar * AlphaValue, new Vector2((i / ((float)Points.Count)), flip ? 1 : 0));
					}
				}
			}
		}

		public override void SetShaders()
		{
			Effect effect = ModContent.Request<Effect>("SpiritMod/Effects/JadeTrailShader", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			effect.Parameters["white"].SetValue(new Color(106, 255, 35).ToVector4());
			effect.Parameters["vnoiseLooping"].SetValue(ModContent.Request<Texture2D>("SpiritMod/Textures/voronoiLooping", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
			PrepareShader(effect, "MainPS", -Main.GlobalTimeWrappedHourly);
		}

		public override void OnUpdate()
		{
			if (Entity is not Projectile)
				return;

			Counter++;
			PointCount = Points.Count * 6;

			if (Cap < PointCount / 6)
				Points.RemoveAt(0);

			if ((!Entity.active && Entity != null) || Destroyed)
				OnDestroy();
			else
				AddPoints();
		}

		public void AddPoints()
		{
			var modProj = (Entity as Projectile).ModProjectile as JadeDaoProj;

			if (modProj.oldBase.Count > 0)
				Points.Add(modProj.oldBase[^1]);
		}

		public override void OnDestroy()
		{
			Destroyed = true;
			Width *= 0.75f;
			AlphaValue *= 0.9f;
			Width += (float)Math.Sin(Counter * 2) * 0.3f;

			if (Width < 0.05f)
				Dispose();
		}
	}
}