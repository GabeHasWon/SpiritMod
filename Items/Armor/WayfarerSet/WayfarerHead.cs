using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Mechanics.QuestSystem;
using SpiritMod.Mechanics.QuestSystem.Quests;
using SpiritMod.Mechanics.QuestSystem.Tasks;
using Terraria.Localization;

namespace SpiritMod.Items.Armor.WayfarerSet
{
	[AutoloadEquip(EquipType.Head)]
	public class WayfarerHead : ModItem
	{
		public override void SetStaticDefaults() => ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 24;
			Item.value = Item.sellPrice(0, 0, 60, 0);
			Item.rare = ItemRarityID.Blue;
			Item.defense = 1;
		}

		public override void UpdateEquip(Player player) => player.buffImmune[BuffID.Darkness] = true;

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = Language.GetTextValue("Mods.SpiritMod.SetBonuses.Wayfarer");
			player.GetSpiritPlayer().wayfarerSet = true;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) => body.type == ModContent.ItemType<WayfarerBody>() && legs.type == ModContent.ItemType<WayfarerLegs>();

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddCondition(QuestCondition());
			recipe.AddIngredient(ModContent.ItemType<Consumable.Quest.DurasilkSheaf>(), 1);
			recipe.AddRecipeGroup(RecipeGroupID.IronBar, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}

		public static Condition QuestCondition() => new Condition(Language.GetText("Mods.SpiritMod.Quests.Conditions.DurasilkSheaf"), () =>
		{
			Quest quest = QuestManager.GetQuest<FirstAdventure>();
			return quest.IsCompleted || quest.CurrentTask is ParallelTask || (quest.CurrentTask is RetrievalTask task && task.GetItemID() != ModContent.ItemType<Consumable.Quest.DurasilkSheaf>());
		});
	}
}
