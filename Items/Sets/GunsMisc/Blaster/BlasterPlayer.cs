using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster
{
	public class BlasterPlayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.HeldItem);

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			if (drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<Blaster>())
			{
				var blaster = drawInfo.drawPlayer.HeldItem.ModItem as Blaster;
				DrawItem(Mod.Assets.Request<Texture2D>("Items/Sets/GunsMisc/Blaster/Blaster").Value, Mod.Assets.Request<Texture2D>("Items/Sets/GunsMisc/Blaster/Blaster_Glow").Value, new Vector2(1, 4), new Vector2(0, blaster.build), drawInfo, ColorEffectsIndex.GetColor(blaster.element));
			}
			else if (drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<Exotic>())
			{
				var exotic = drawInfo.drawPlayer.HeldItem.ModItem as Exotic;
				DrawItem(Mod.Assets.Request<Texture2D>("Items/Sets/GunsMisc/Blaster/Exotic").Value, Mod.Assets.Request<Texture2D>("Items/Sets/GunsMisc/Blaster/Exotic_Glow").Value, new Vector2(1, 4), new Vector2(0, exotic.style), drawInfo);
			}
		}

		private static void DrawItem(Texture2D texture, Texture2D glow, Vector2 numFrames, Vector2 frame, PlayerDrawSet info, Color? glowColor = null)
		{
			Item item = info.heldItem;
			if (info.shadow != 0f || info.drawPlayer.frozen || ((info.drawPlayer.itemAnimation <= 0 || item.useStyle != ItemUseStyleID.Shoot) && (item.holdStyle <= 0 || info.drawPlayer.pulley)) || info.drawPlayer.dead || (info.drawPlayer.wet && item.noWet))
				return;

			Rectangle drawFrame = texture.Bounds;

			int numXFrames = (int)numFrames.X;
			drawFrame.Width /= numXFrames;
			drawFrame.X = drawFrame.Width * (int)frame.X;

			int numYFrames = (int)numFrames.Y;
			drawFrame.Height /= numYFrames;
			drawFrame.Y = drawFrame.Height * (int)frame.Y;

			Vector2 offset = new Vector2(0, texture.Height / 2);

			ItemLoader.HoldoutOffset(info.drawPlayer.gravDir, item.type, ref offset);
			Vector2 origin = new Vector2(0, texture.Height / (2 * numYFrames));

			offset = new Vector2(texture.Width / 2, offset.Y);
			if (info.drawPlayer.direction == -1)
			{
				origin.X += drawFrame.Width;
			}

			//Is texture padding necessary?
			if (numFrames.Y > 1)
				drawFrame.Height -= 2;
			if (numFrames.X > 1)
				drawFrame.Width -= 2;

			info.DrawDataCache.Add(new DrawData(
				texture,
				info.ItemLocation - Main.screenPosition + offset,
				drawFrame,
				Lighting.GetColor((int)info.ItemLocation.X / 16, (int)info.ItemLocation.Y / 16),
				info.drawPlayer.itemRotation,
				origin,
				item.scale,
				info.itemEffect,
				0
			));

			info.DrawDataCache.Add(new DrawData(
				 glow,
				 info.ItemLocation - Main.screenPosition + offset,
				 drawFrame,
				 glowColor is null ? Color.White : glowColor.Value,
				 info.drawPlayer.itemRotation,
				 origin,
				 item.scale,
				 info.itemEffect,
				 0
			 ));
		}
	}
}