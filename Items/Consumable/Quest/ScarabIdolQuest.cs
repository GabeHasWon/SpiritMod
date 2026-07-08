using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;
using SpiritMod.Mechanics.QuestSystem;
using SpiritMod.Mechanics.QuestSystem.Quests;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace SpiritMod.Items.Consumable.Quest;

[Sacrifice(1)]
public class ScarabIdolQuest : ModItem
{
	public override void SetDefaults()
	{
		Item.width = Item.height = 16;
		Item.rare = ItemRarityID.Green;
		Item.maxStack = Item.CommonMaxStack;
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		if (!QuestManager.GetQuest<IdleIdol>().IsCompleted)
		{
			TooltipLine line = new TooltipLine(Mod, "ItemName", Language.GetTextValue("Mods.SpiritMod.Items.DurasilkSheaf.CommonName"));
			line.OverrideColor = new Color(100, 222, 122);
			tooltips.Add(line);
		}
	}

	public override void AddRecipes()
	{
		if (ModLoader.TryGetMod("SpiritReforged", out Mod re) && re.TryFind("RedSandstoneBrickItem", out ModItem brick) && re.TryFind("PolishedAmberItem", out ModItem amber))
			CreateRecipe().AddIngredient(brick.Type, 5).AddIngredient(amber.Type, 10).AddTile(TileID.Anvils).Register();
	}
}
