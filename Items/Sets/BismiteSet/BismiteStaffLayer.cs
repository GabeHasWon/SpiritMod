using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;

namespace SpiritMod.Items.Sets.BismiteSet
{
	public class BismiteStaffLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.HeldItem);

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			if (drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<BismiteStaff>())
				DrawGlow(Mod.Assets.Request<Texture2D>("Items/Sets/BismiteSet/BismiteStaff_Glow").Value, drawInfo);
		}

		public static void DrawGlow(Texture2D texture, PlayerDrawSet info)
		{
			Item item = info.drawPlayer.HeldItem;
			if (info.shadow != 0f || info.drawPlayer.frozen || ((info.drawPlayer.itemAnimation <= 0 || item.useStyle == ItemUseStyleID.None) && (item.holdStyle <= 0 || info.drawPlayer.pulley)) || info.drawPlayer.dead || (info.drawPlayer.wet && item.noWet))
				return;

			float opacity = (float)info.drawPlayer.itemAnimation / info.drawPlayer.itemAnimationMax;

			Vector2 offset = Vector2.Zero;
			ItemLoader.HoldoutOffset(info.drawPlayer.gravDir, item.type, ref offset);

			float rotation = info.drawPlayer.itemRotation + (0.785f * info.drawPlayer.direction);

			Vector2 origin = new Vector2(0, texture.Height);
			if (info.drawPlayer.direction == -1)
				origin = texture.Size();

			info.DrawDataCache.Add(new DrawData(
				texture,
				info.ItemLocation - Main.screenPosition + offset,
				texture.Bounds,
				Color.White * opacity,
				rotation,
				origin,
				item.scale,
				info.itemEffect,
				0
			));
		}
	}
}