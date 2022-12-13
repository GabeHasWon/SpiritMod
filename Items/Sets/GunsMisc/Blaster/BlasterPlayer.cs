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
			if (drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<Blaster>() && drawInfo.drawPlayer.itemAnimation > 0)
			{
				var blaster = drawInfo.drawPlayer.HeldItem.ModItem as Blaster;
				DrawItem(Mod.Assets.Request<Texture2D>("Items/Sets/GunsMisc/Blaster/Blaster").Value, Mod.Assets.Request<Texture2D>("Items/Sets/GunsMisc/Blaster/Blaster_Glow").Value, blaster.element, blaster.usingAltTexture ? 1 : 0, drawInfo);
			}
		}

		private static void DrawItem(Texture2D texture, Texture2D glow, int frameX, int frameY, PlayerDrawSet info)
		{
			Item item = info.drawPlayer.HeldItem;
			if (info.shadow != 0f || info.drawPlayer.frozen || ((info.drawPlayer.itemAnimation <= 0 || item.useStyle == ItemUseStyleID.None) && (item.holdStyle <= 0 || info.drawPlayer.pulley)) || info.drawPlayer.dead || (info.drawPlayer.wet && item.noWet))
				return;

			Rectangle drawFrame = texture.Bounds;

			int numXFrames = 4;
			drawFrame.Width /= numXFrames;
			drawFrame.X = drawFrame.Width * frameX;

			int numYFrames = 2;
			drawFrame.Height /= numYFrames;
			drawFrame.Y = drawFrame.Height * frameY;

			Vector2 offset = new Vector2(0, texture.Height / 2);

			ItemLoader.HoldoutOffset(info.drawPlayer.gravDir, item.type, ref offset);
			Vector2 origin = new Vector2(0, texture.Height / (2 * numYFrames));

			offset = new Vector2(texture.Width / 2, offset.Y);
			if (info.drawPlayer.direction == -1)
			{
				origin.X += drawFrame.Width;
			}
			drawFrame.Height -= 2;
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
				 Color.White,
				 info.drawPlayer.itemRotation,
				 origin,
				 item.scale,
				 info.itemEffect,
				 0
			 ));
		}
	}
}