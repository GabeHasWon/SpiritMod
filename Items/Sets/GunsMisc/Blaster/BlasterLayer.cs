using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.Blaster
{
	public class BlasterPlayer : ModPlayer
	{
		public bool hide;

		public override void ResetEffects()
		{
			if (Player.ItemAnimationEndingOrEnded)
				hide = false;
		}
	}

	public class BlasterLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.HeldItem);

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			if (drawInfo.heldItem != null && !drawInfo.heldItem.IsAir && (drawInfo.heldItem.type == ModContent.ItemType<Blaster>() || drawInfo.heldItem.type == ModContent.ItemType<Exotic>()) && !drawInfo.drawPlayer.GetModPlayer<BlasterPlayer>().hide)
			{
				if (drawInfo.heldItem.ModItem is Exotic exotic)
					DrawItem(exotic.Texture, drawInfo, Color.White, exotic.style);
				else if (drawInfo.heldItem.ModItem is Blaster blaster)
					DrawItem(blaster.Texture, drawInfo, SubtypeProj.GetColor(blaster.element), blaster.build);
			}
		}

		private static void DrawItem(string texturePath, PlayerDrawSet info, Color glowColor, int frame = 0)
		{
			Item item = info.heldItem;
			if (info.shadow != 0f || info.drawPlayer.frozen || ((info.drawPlayer.itemAnimation <= 0 || item.useStyle == ItemUseStyleID.None) && (item.holdStyle <= 0 || info.drawPlayer.pulley)) || info.drawPlayer.dead || (info.drawPlayer.wet && item.noWet))
				return;

			Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
			Rectangle drawFrame = texture.Frame(1, Main.itemAnimations[item.type].FrameCount, 0, frame, 0, -2);

			Vector2 offset = new Vector2(10, drawFrame.Height / 2);
			ItemLoader.HoldoutOffset(info.drawPlayer.gravDir, item.type, ref offset);
			Vector2 origin = new Vector2(-offset.X, drawFrame.Height / 2);
			float alpha = (255f - item.alpha) / 255f;

			if (info.drawPlayer.direction == -1)
				origin.X = drawFrame.Width + offset.X;
			offset = new Vector2(0, offset.Y);

			for (int i = 0; i < 2; i++)
				info.DrawDataCache.Add(new DrawData
				(
					(i == 0) ? texture : ModContent.Request<Texture2D>(texturePath + "_Glow").Value,
					new Vector2((int)(info.ItemLocation.X - Main.screenPosition.X + offset.X), (int)(info.ItemLocation.Y - Main.screenPosition.Y + offset.Y)),
					drawFrame,
					((i == 0) ? Lighting.GetColor((int)info.ItemLocation.X / 16, (int)info.ItemLocation.Y / 16) : glowColor) * alpha,
					info.drawPlayer.itemRotation,
					origin,
					item.scale,
					info.playerEffect,
					0
				));
		}
	}
}