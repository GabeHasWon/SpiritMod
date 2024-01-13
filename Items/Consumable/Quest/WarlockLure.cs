using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria;
using Terraria.ID;
using SpiritMod.Mechanics.QuestSystem;
using SpiritMod.Mechanics.QuestSystem.Quests;
using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.Localization;

namespace SpiritMod.Items.Consumable.Quest
{
	[Sacrifice(1)]
	public class WarlockLureCorruption : ModItem
	{
		public override void SetStaticDefaults()
		{
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(8, 7));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }

		public override void SetDefaults()
		{
			Item.width = Item.height = 16;
			Item.rare = ItemRarityID.Green;
			Item.maxStack = Item.CommonMaxStack;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (!QuestManager.GetQuest<ZombieOriginQuest>().IsCompleted)
			{
				TooltipLine line = new TooltipLine(Mod, "ItemName", Language.GetTextValue("Mods.SpiritMod.Items.DurasilkSheaf.CommonName"));
				line.OverrideColor = new Color(100, 222, 122);
				tooltips.Add(line);
				TooltipLine line1 = new TooltipLine(Mod, "FavoriteDesc", Language.GetTextValue("Mods.SpiritMod.Items.WarlockLureCorruption.CustomTooltip"));
				line1.OverrideColor = new Color(255, 255, 255);
				tooltips.Add(line1);
			}
			else
			{
				TooltipLine line1 = new TooltipLine(Mod, "FavoriteDesc", Language.GetTextValue("Mods.SpiritMod.Items.WarlockLureCorruption.MoreCustomTooltip"));
				line1.OverrideColor = new Color(255, 255, 255);
				tooltips.Add(line1);
			}
		}

		public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200);

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddCondition(QuestCondition(Type));
			recipe.AddIngredient(ItemID.FallenStar, 2);
			recipe.AddIngredient(ItemID.RottenChunk, 5);
			recipe.Register();
		}

		public static Condition QuestCondition(int type) => new(Language.GetText("Mods.SpiritMod.Quests.RecipeConditions.WarlockLure"), () =>
		{
			Mechanics.QuestSystem.Quest quest = QuestManager.GetQuest<ZombieOriginQuest>();
			return (quest.CurrentTask is RetrievalTask task && task.GetItemID() == type);
		});
	}

    public class WarlockLureCrimson : WarlockLureCorruption
    {
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddCondition(QuestCondition(Type));
			recipe.AddIngredient(ItemID.FallenStar, 2);
			recipe.AddIngredient(ItemID.Vertebrae, 5);
			recipe.Register();
		}
	}
}
