﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.GameContent;

namespace SpiritMod.Items.Accessory.Rangefinder
{
	internal class RangeFinderLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.ArmOverItem);

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			Player drawPlayer = drawInfo.drawPlayer;
			Texture2D texture = TextureAssets.Extra[47].Value;
			Vector2 drawPlayerCenter = drawPlayer.MountedCenter;

			//Base the rotation of the beam on drawPlayer.itemRotation and not the player's angle to Main.MouseWorld
			float rotation = MathHelper.WrapAngle(drawPlayer.itemRotation + ((drawPlayer.direction == -1) ? MathHelper.Pi : 0));
			Vector2 mouseItemWorld = drawPlayerCenter + new Vector2(drawPlayer.Distance(Main.MouseWorld), 0).RotatedBy(rotation);
			
			Vector2 distToProj = mouseItemWorld - drawPlayerCenter;
			float projRotation = distToProj.ToRotation() - 1.57f;
			float distance = distToProj.Length();

			if (drawPlayer.GetModPlayer<RangefinderPlayer>().active && drawPlayer.itemAnimation > 0 && drawPlayer.HeldItem.IsRanged() && drawPlayer.HeldItem.useAnimation > 0 && (drawPlayer.HeldItem.noUseGraphic == false || drawPlayer.heldProj > -1) && !drawPlayer.mount.Active)
			{
				while (distance > 30 && !float.IsNaN(distance))
				{
					distToProj.Normalize();
					distToProj *= 36f;
					drawPlayerCenter += distToProj;
					distToProj = mouseItemWorld - drawPlayerCenter;
					distance = distToProj.Length();
					var drawData = new DrawData(texture,
						new Vector2(drawPlayerCenter.X - Main.screenPosition.X, drawPlayerCenter.Y - Main.screenPosition.Y),
						new Rectangle(0, 0, texture.Width, texture.Height),
						Color.DeepSkyBlue * (distToProj.Length() / 255),
						projRotation,
						new Vector2(texture.Width * 0.5f, texture.Height * 0.5f),
						0.5f,
						SpriteEffects.None,
						0);
					drawInfo.DrawDataCache.Add(drawData);
				}
			}
		}
	}
}
