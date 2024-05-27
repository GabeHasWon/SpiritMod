using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod
{
	public static class GlowmaskUtils
	{
		public enum ArmorContext
		{
			Head,
			Body,
			Arms,
			Legs
		}

		public static void DrawNPCGlowMask(SpriteBatch spriteBatch, NPC npc, Texture2D texture, Vector2 screenPos, Color? color = null)
		{
			var effects = npc.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			Main.EntitySpriteDraw(
				texture,
				npc.Center - screenPos + new Vector2(0, npc.gfxOffY),
				npc.frame,
				npc.GetNPCColorTintedByBuffs(color ?? Color.White),
				npc.rotation,
				npc.frame.Size() / 2,
				npc.scale,
				effects,
				0
			);
		}

		public static void DrawExtras(SpriteBatch spriteBatch, NPC npc, Texture2D texture)
		{
			var effects = npc.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			spriteBatch.Draw(
				texture,
				npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY),
				npc.frame,
				new Color(200, 200, 200),
				npc.velocity.X * .1f,
				npc.frame.Size() / 2,
				npc.scale,
				effects,
				0
			);
		}

		public static void DrawArmorGlowMask(ArmorContext type, Texture2D texture, PlayerDrawSet info)
		{
			switch (type)
			{
				case ArmorContext.Head:
					{
						Vector2 pos = new Vector2((int)(info.Position.X - Main.screenPosition.X) + ((info.drawPlayer.width - info.drawPlayer.bodyFrame.Width) / 2), (int)(info.Position.Y - Main.screenPosition.Y) + info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + 4) + info.drawPlayer.headPosition + info.rotationOrigin;
						DrawData drawData = new DrawData(texture, pos, info.drawPlayer.bodyFrame, info.headGlowColor, info.drawPlayer.headRotation, info.rotationOrigin, 1f, info.playerEffect, 0)
						{ shader = info.cHead };

						info.DrawDataCache.Add(drawData);
					}
					return;

				case ArmorContext.Body:
					{
						if (info.drawPlayer.invis)
							return;

						Vector2 pos = new Vector2((int)(info.Position.X - Main.screenPosition.X - (info.drawPlayer.bodyFrame.Width / 2) + (info.drawPlayer.width / 2)), (int)(info.Position.Y - Main.screenPosition.Y + info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + 2)) + info.drawPlayer.bodyPosition + info.rotationOrigin;
						Vector2 bobOff = Main.OffsetsPlayerHeadgear[info.drawPlayer.bodyFrame.Y / info.drawPlayer.bodyFrame.Height] * info.drawPlayer.gravDir;
						if (info.drawPlayer.gravDir == -1)
							bobOff.Y += 4;

						if (info.usesCompositeTorso)
						{
							DrawData drawData = new DrawData(texture, pos + bobOff, info.compTorsoFrame, info.bodyGlowColor, info.drawPlayer.bodyRotation, info.rotationOrigin, 1f, info.playerEffect)
							{ shader = info.cBody };

							info.DrawDataCache.Add(drawData);
						}
						else
						{
							DrawData drawData = new DrawData(texture, pos + bobOff, info.drawPlayer.bodyFrame, info.bodyGlowColor, info.drawPlayer.bodyRotation, info.rotationOrigin, 1f, info.playerEffect, 0)
							{ shader = info.cBody };

							info.DrawDataCache.Add(drawData);
						}
					}
					return;

				case ArmorContext.Arms:
					{
						if (info.drawPlayer.invis)
							return;

						Vector2 bobOff = Main.OffsetsPlayerHeadgear[info.drawPlayer.bodyFrame.Y / info.drawPlayer.bodyFrame.Height] * info.drawPlayer.gravDir;
						if (info.drawPlayer.gravDir == -1)
							bobOff.Y += 4;

						if (info.usesCompositeTorso)
						{
							static Vector2 GetCompositeOffset_FrontArm(ref PlayerDrawSet drawinfo)
								=> new(-5 * ((!drawinfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally)) ? 1 : (-1)), 0f);

							Vector2 pos = new Vector2((int)(info.Position.X - Main.screenPosition.X - (info.drawPlayer.bodyFrame.Width / 2) + (info.drawPlayer.width / 2)), (int)(info.Position.Y - Main.screenPosition.Y + info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + 2)) + info.drawPlayer.bodyPosition + (info.drawPlayer.bodyFrame.Size() / 2);
							pos += GetCompositeOffset_FrontArm(ref info);

							Vector2 bodyVect = info.bodyVect + GetCompositeOffset_FrontArm(ref info);
							Vector2 shoulderPos = pos + info.frontShoulderOffset;
							if (info.compFrontArmFrame.X / info.compFrontArmFrame.Width >= 7)
								pos += new Vector2((!info.playerEffect.HasFlag(SpriteEffects.FlipHorizontally)) ? 1 : (-1), (!info.playerEffect.HasFlag(SpriteEffects.FlipVertically)) ? 1 : (-1));

							float rotation = info.drawPlayer.bodyRotation + info.compositeFrontArmRotation;
							DrawData drawData = new DrawData(texture, pos + bobOff, info.compFrontArmFrame, info.bodyGlowColor, rotation, bodyVect, 1f, info.playerEffect)
							{ shader = info.cBody };

							info.DrawDataCache.Add(drawData);

							if (!info.hideCompositeShoulders)
							{
								DrawData drawData2 = new DrawData(texture, shoulderPos + bobOff, info.compFrontShoulderFrame, info.bodyGlowColor, info.drawPlayer.bodyRotation, bodyVect, 1f, info.playerEffect)
								{ shader = info.cBody };

								info.DrawDataCache.Add(drawData2);
							}
						}
						else
						{
							Vector2 pos = new Vector2((int)(info.Position.X - Main.screenPosition.X - (info.drawPlayer.bodyFrame.Width / 2) + (info.drawPlayer.width / 2)), (int)(info.Position.Y - Main.screenPosition.Y + info.drawPlayer.height - info.drawPlayer.bodyFrame.Height + 2)) + info.drawPlayer.bodyPosition + info.rotationOrigin;
							DrawData drawData = new DrawData(texture, pos + bobOff, info.drawPlayer.bodyFrame, info.bodyGlowColor, info.drawPlayer.bodyRotation, info.rotationOrigin, 1f, info.playerEffect, 0)
							{ shader = info.cBody };

							info.DrawDataCache.Add(drawData);
						}
					}
					return;

				case ArmorContext.Legs:
					{
						if (info.drawPlayer.invis || info.isSitting)
							return;

						if (info.drawPlayer.shoe != 15 || info.drawPlayer.wearsRobe)
						{
							Vector2 pos = new Vector2((int)(info.Position.X - Main.screenPosition.X - (info.drawPlayer.legFrame.Width / 2) + (info.drawPlayer.width / 2)), (int)(info.Position.Y - Main.screenPosition.Y + info.drawPlayer.height - info.drawPlayer.legFrame.Height + 4)) + info.drawPlayer.legPosition + info.rotationOrigin;
							DrawData drawData = new DrawData(texture, pos, info.drawPlayer.legFrame, info.legsGlowColor, info.drawPlayer.legRotation, info.rotationOrigin, 1f, info.playerEffect, 0)
							{ shader = info.cLegs };

							info.DrawDataCache.Add(drawData);
						}
					}
					return;
			}
		}

		public static void DrawItemGlowMask(Texture2D texture, PlayerDrawSet info)
		{
			Item item = info.drawPlayer.HeldItem;
			if (info.shadow != 0f || info.drawPlayer.frozen || ((info.drawPlayer.itemAnimation <= 0 || item.useStyle == ItemUseStyleID.None) && (item.holdStyle <= 0 || info.drawPlayer.pulley)) || info.drawPlayer.dead || item.noUseGraphic || (info.drawPlayer.wet && item.noWet))
				return;

			Vector2 offset = Vector2.Zero;
			Vector2 origin = Vector2.Zero;
			float rotOffset = 0;

			if (item.useStyle == ItemUseStyleID.Shoot)
			{
				if (Item.staff[item.type])
				{
					rotOffset = 0.785f * info.drawPlayer.direction;
					if (info.drawPlayer.gravDir == -1f)
						rotOffset -= 1.57f * info.drawPlayer.direction;

					origin = new Vector2(texture.Width * 0.5f * (1 - info.drawPlayer.direction), (info.drawPlayer.gravDir == -1f) ? 0 : texture.Height);

					int oldOriginX = -(int)origin.X;
					ItemLoader.HoldoutOrigin(info.drawPlayer, ref origin);
					offset = new Vector2(origin.X + oldOriginX, 0);
				}
				else
				{
					offset = new Vector2(10, texture.Height / 2);
					ItemLoader.HoldoutOffset(info.drawPlayer.gravDir, item.type, ref offset);

					origin = new Vector2((int)-offset.X, texture.Height / 2);
					if (info.drawPlayer.direction == -1)
						origin.X = texture.Width + offset.X;

					offset = new Vector2(0, offset.Y);
				}
			}
			else
			{
				origin = new Vector2(texture.Width * 0.5f * (1 - info.drawPlayer.direction), (info.drawPlayer.gravDir == -1f) ? 0 : texture.Height);
			}

			info.DrawDataCache.Add(new DrawData(
				texture,
				new Vector2((int)(info.ItemLocation.X - Main.screenPosition.X + offset.X), (int)(info.ItemLocation.Y - Main.screenPosition.Y + offset.Y)),
				texture.Bounds,
				Color.White * ((255f - item.alpha) / 255f),
				info.drawPlayer.itemRotation + rotOffset,
				origin,
				item.scale,
				info.playerEffect,
				0
			));
		}

		public static void DrawItemGlowMaskWorld(SpriteBatch spriteBatch, Item item, Texture2D texture, float rotation, float scale)
		{
			Main.spriteBatch.Draw(
				texture,
				new Vector2(item.position.X - Main.screenPosition.X + item.width / 2, item.position.Y - Main.screenPosition.Y + item.height - (texture.Height / 2)),
				new Rectangle(0, 0, texture.Width, texture.Height),
				Color.White * ((255f - item.alpha) / 255f),
				rotation,
				new Vector2(texture.Width / 2, texture.Height / 2),
				scale,
				SpriteEffects.None,
				0f
			);
		}
	}
}