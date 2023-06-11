using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.GunsMisc.Blaster.Projectiles;
using System.Collections.Generic;
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
			if ((drawInfo.heldItem.type != ModContent.ItemType<Blaster>() && drawInfo.heldItem.type != ModContent.ItemType<Exotic>()) || drawInfo.drawPlayer.GetModPlayer<BlasterPlayer>().hide)
				return;

			bool isExotic = drawInfo.heldItem.ModItem is Exotic;

			Color color = isExotic ? Color.White : SubtypeProj.GetColor((drawInfo.heldItem.ModItem as Blaster).element);
			int frame = isExotic ? (drawInfo.heldItem.ModItem as Exotic).style : (drawInfo.heldItem.ModItem as Blaster).build;
			string texture = isExotic ?
				"SpiritMod/Items/Sets/GunsMisc/Blaster/Exotic" :
				"SpiritMod/Items/Sets/GunsMisc/Blaster/Blaster";

			DrawItem(texture, drawInfo, color, frame);
		}

		private static void DrawItem(string texturePath, PlayerDrawSet info, Color glowColor, int frame = 0)
		{
			Item item = info.drawPlayer.HeldItem;
			if (info.shadow != 0f || info.drawPlayer.frozen || ((info.drawPlayer.itemAnimation <= 0 || item.useStyle == ItemUseStyleID.None) && (item.holdStyle <= 0 || info.drawPlayer.pulley)) || info.drawPlayer.dead || (info.drawPlayer.wet && item.noWet))
				return;

			Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
			Rectangle drawFrame = new Rectangle(0, texture.Height / Main.itemAnimations[item.type].FrameCount * frame, texture.Width, (texture.Height / Main.itemAnimations[item.type].FrameCount) - 2);

			Vector2 offset = new Vector2(10, drawFrame.Height / 2);
			ItemLoader.HoldoutOffset(info.drawPlayer.gravDir, item.type, ref offset);
			Vector2 origin = new Vector2(-offset.X, drawFrame.Height / 2);

			if (info.drawPlayer.direction == -1)
				origin.X = drawFrame.Width + offset.X;

			offset = new Vector2(drawFrame.Width / 2, offset.Y);

			info.DrawDataCache.AddRange(new List<DrawData>() { 
			new DrawData
			(
				texture,
				info.ItemLocation - Main.screenPosition + offset,
				drawFrame,
				Lighting.GetColor((int)info.ItemLocation.X / 16, (int)info.ItemLocation.Y / 16) * ((255f - item.alpha) / 255f),
				info.drawPlayer.itemRotation,
				origin,
				item.scale,
				info.playerEffect,
				0
			),
			new DrawData
			(
				ModContent.Request<Texture2D>(texturePath + "_Glow").Value,
				info.ItemLocation - Main.screenPosition + offset,
				drawFrame,
				glowColor * ((255f - item.alpha) / 255f),
				info.drawPlayer.itemRotation,
				origin,
				item.scale,
				info.playerEffect,
				0
			)});
		}
	}
}