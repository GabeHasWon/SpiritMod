using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;

namespace SpiritMod.Items.Sets.SepulchreLoot.GraveyardTome
{
	public class GraveyardPlayer : ModPlayer
	{
		public float GraveyardFrame { get; set; }
		public override void Initialize() => GraveyardFrame = 0;
		public override void ResetEffects()
		{
			if (Player.HeldItem.type != ModContent.ItemType<Graveyard>() || Player.itemAnimation == 0)
				GraveyardFrame = 0;
		}
	}

	public class GraveyardLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.HeldItem);
		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			if (drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<Graveyard>())
				DrawItem(Mod.Assets.Request<Texture2D>("Items/Sets/SepulchreLoot/GraveyardTome/Graveyard_Held").Value, Mod.Assets.Request<Texture2D>("Items/Sets/SepulchreLoot/GraveyardTome/Graveyard_HeldGlow").Value, drawInfo);
		}

		public void DrawItem(Texture2D texture, Texture2D glow, PlayerDrawSet info)
		{
			Item item = info.drawPlayer.HeldItem;
			if (info.shadow != 0f || info.drawPlayer.frozen || ((info.drawPlayer.itemAnimation <= 0 || item.useStyle == ItemUseStyleID.None) && (item.holdStyle <= 0 || info.drawPlayer.pulley)) || info.drawPlayer.dead || (info.drawPlayer.wet && item.noWet))
				return;

			Rectangle drawFrame = texture.Bounds;
			int numFrames = 5;
			drawFrame.Height /= numFrames;
			drawFrame.Y = (drawFrame.Height) * (int)(info.drawPlayer.GetModPlayer<GraveyardPlayer>().GraveyardFrame);

			Vector2 offset = new Vector2(10, texture.Height / (2 * numFrames));

			ItemLoader.HoldoutOffset(info.drawPlayer.gravDir, item.type, ref offset);
			Vector2 origin = new Vector2(-offset.X, texture.Height / (2 * numFrames));

			offset = new Vector2(texture.Width / 2, offset.Y);
			if (info.drawPlayer.direction == -1)
			{
				origin.X = texture.Width - origin.X;
				offset.X = texture.Width / 4;
			}

			info.DrawDataCache.Add(new DrawData(
				texture,
				info.ItemLocation - Main.screenPosition + offset,
				drawFrame,
				Lighting.GetColor((int)info.ItemLocation.X / 16, (int)info.ItemLocation.Y / 16),
				info.drawPlayer.itemRotation,
				origin,
				item.scale,
				1 - info.itemEffect,
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
				 1 - info.itemEffect,
				 0
			 ));
		}
	}
}