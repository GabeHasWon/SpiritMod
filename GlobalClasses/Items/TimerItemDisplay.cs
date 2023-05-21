using Microsoft.Xna.Framework;
using SpiritMod.Items;
using SpiritMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using ReLogic.Content;

namespace SpiritMod.GlobalClasses.Items;

internal class TimerItemDisplay : GlobalItem
{
	public override bool InstancePerEntity => true;
	public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.ModItem != null && entity.ModItem is ITimerItem;

	private int itemTimerMax;

	public override void UpdateInventory(Item item, Player player)
	{
		if (itemTimerMax < Main.LocalPlayer.ItemTimer(item.ModItem))
			itemTimerMax = Main.LocalPlayer.ItemTimer(item.ModItem);
	}

	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
	{
		if (!item.accessory && Main.LocalPlayer.ItemTimer(item.ModItem) > 0)
		{
			int tooltipTime = Main.LocalPlayer.ItemTimer(item.ModItem) / 60;

			string text = (tooltipTime < 60) ? $"({tooltipTime}s remaining)" : $"({tooltipTime / 60}m remaining)";
			tooltips.Add(new TooltipLine(Mod, "Cooldown", text) { OverrideColor = Color.HotPink });
		}
	}

	public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		if (!item.accessory && Main.LocalPlayer.ItemTimer(item.ModItem) > 0 && Main.mouseItem != item)
		{
			Asset<Texture2D> texture = TextureAssets.InventoryBack18;

			int timer = Main.LocalPlayer.ItemTimer(item.ModItem);
			Vector2 drawPos = position + (frame.Size() / 2 * scale);

			float quoteant = (float)timer / itemTimerMax;
			Color color = Color.Red * .5f;
			if (timer <= 5)
			{
				quoteant = 1f;
				color = Color.White * (float)(timer / 5f);
			}

			spriteBatch.Draw(texture.Value, drawPos, texture.Frame(sizeOffsetY: (int)((float)(1f - quoteant) * -texture.Height())), color, MathHelper.Pi, texture.Size() / 2, Main.inventoryScale, SpriteEffects.None, 0f);
		}
	}
}
