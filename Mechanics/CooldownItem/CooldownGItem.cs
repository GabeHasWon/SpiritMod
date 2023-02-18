using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.CooldownItem
{
	public class CooldownGItem : GlobalItem
	{
		public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.ModItem is ICooldownItem;

		public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			MyPlayer modPlayer = Main.LocalPlayer.GetModPlayer<MyPlayer>();

			if (GetCooldown(item.type, Main.LocalPlayer) == 0) //Draw an indicator for items on cooldown
				return;

			Texture2D texture = Mod.Assets.Request<Texture2D>("Mechanics/CooldownItem/CoolDown").Value;

			Vector2 drawPos = position + (frame.Size() / 2 * scale);
			Color color = Color.White * MathHelper.Clamp((float)modPlayer.cooldowns[item.type] / 60f, 0, 1);
			spriteBatch.Draw(texture, drawPos, texture.Frame(), color, 0f, texture.Size() / 2, Main.inventoryScale, SpriteEffects.None, 0f);
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			MyPlayer modPlayer = Main.LocalPlayer.GetModPlayer<MyPlayer>();

			if (GetCooldown(item.type, Main.LocalPlayer) == 0)
				return;

			int tooltipTime = modPlayer.cooldowns[item.type] / 60;

			string text = (tooltipTime < 60) ? $"({tooltipTime}s remaining)" : $"({tooltipTime / 60}m remaining)";
			tooltips.Add(new TooltipLine(Mod, "Cooldown", text) { OverrideColor = Color.HotPink });
		}

		/// <summary>
		/// Safely gets the cooldown amount associated with the specified item type
		/// </summary>
		/// <param name="itemType">The item type associated with the cooldown</param>
		/// <param name="player">The player associated with the item's ModPlayer instance</param>
		/// /// <param name="assignment">Assigns a cooldown value used by the item type, leave at 0 if you don't want to assign a value</param>
		/// <returns></returns>
		internal static int GetCooldown(int itemType, Player player, ushort assignment = 0)
		{
			MyPlayer modPlayer = player.GetModPlayer<MyPlayer>();

			if (!modPlayer.cooldowns.ContainsKey(itemType)) //Register a cooldown key
				modPlayer.cooldowns.Add(itemType, 0);

			if (assignment != 0)
				modPlayer.cooldowns[itemType] = assignment;

			return modPlayer.cooldowns[itemType];
		}
	}
}