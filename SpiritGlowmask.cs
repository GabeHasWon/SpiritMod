using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod
{
	public class SpiritGlowmask : ModPlayer
	{
		internal static readonly Dictionary<int, Texture2D> ItemGlowMask = new();

		internal new static void Unload() => ItemGlowMask.Clear();
		public static void AddGlowMask(int itemType, string texturePath) => ItemGlowMask[itemType] = ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
	}

	public class SpiritGlowMaskItemLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.ArmOverItem);

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			Item item = drawInfo.drawPlayer.HeldItem;

			if (item.type >= ItemID.Count && SpiritGlowmask.ItemGlowMask.TryGetValue(item.type, out Texture2D textureItem) && (drawInfo.drawPlayer.itemTime > 0 || item.useStyle != ItemUseStyleID.None)) //Held ItemType
				GlowmaskUtils.DrawItemGlowMask(textureItem, drawInfo);
		}
	}

	public class SpiritGlowMaskLegsLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Leggings);

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			if (drawInfo.drawPlayer.armor[12].type >= ItemID.Count && SpiritGlowmask.ItemGlowMask.TryGetValue(drawInfo.drawPlayer.armor[12].type, out Texture2D vanity)) //Vanity Legs
				GlowmaskUtils.DrawArmorGlowMask(GlowmaskUtils.ArmorContext.Legs, vanity, drawInfo);
			else if (drawInfo.drawPlayer.armor[2].type >= ItemID.Count && SpiritGlowmask.ItemGlowMask.TryGetValue(drawInfo.drawPlayer.armor[2].type, out Texture2D armor))
				GlowmaskUtils.DrawArmorGlowMask(GlowmaskUtils.ArmorContext.Legs, armor, drawInfo);
		}
	}

	public class SpiritGlowMaskBodyLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Torso);

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			if (drawInfo.drawPlayer.armor[11].type >= ItemID.Count && SpiritGlowmask.ItemGlowMask.TryGetValue(drawInfo.drawPlayer.armor[11].type, out Texture2D vanity)) //Vanity Body
				GlowmaskUtils.DrawArmorGlowMask(GlowmaskUtils.ArmorContext.Body, vanity, drawInfo);
			else if (drawInfo.drawPlayer.armor[1].type >= ItemID.Count && SpiritGlowmask.ItemGlowMask.TryGetValue(drawInfo.drawPlayer.armor[1].type, out Texture2D armor))
				GlowmaskUtils.DrawArmorGlowMask(GlowmaskUtils.ArmorContext.Body, armor, drawInfo);
		}
	}

	public class SpiritGlowMaskArmsLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.ArmOverItem);

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			if (drawInfo.drawPlayer.armor[11].type >= ItemID.Count && SpiritGlowmask.ItemGlowMask.TryGetValue(drawInfo.drawPlayer.armor[11].type, out Texture2D vanity)) //Vanity Body
				GlowmaskUtils.DrawArmorGlowMask(GlowmaskUtils.ArmorContext.Arms, vanity, drawInfo);
			else if (drawInfo.drawPlayer.armor[1].type >= ItemID.Count && SpiritGlowmask.ItemGlowMask.TryGetValue(drawInfo.drawPlayer.armor[1].type, out Texture2D armor))
				GlowmaskUtils.DrawArmorGlowMask(GlowmaskUtils.ArmorContext.Arms, armor, drawInfo);
		}
	}

	public class SpiritGlowMaskHeadLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			if (drawInfo.drawPlayer.armor[10].type >= ItemID.Count && SpiritGlowmask.ItemGlowMask.TryGetValue(drawInfo.drawPlayer.armor[10].type, out Texture2D vanity)) //Vanity Head
				GlowmaskUtils.DrawArmorGlowMask(GlowmaskUtils.ArmorContext.Head, vanity, drawInfo);
			else if (drawInfo.drawPlayer.armor[0].type >= ItemID.Count && SpiritGlowmask.ItemGlowMask.TryGetValue(drawInfo.drawPlayer.armor[0].type, out Texture2D armor))
				GlowmaskUtils.DrawArmorGlowMask(GlowmaskUtils.ArmorContext.Head, armor, drawInfo);
		}
	}
}
