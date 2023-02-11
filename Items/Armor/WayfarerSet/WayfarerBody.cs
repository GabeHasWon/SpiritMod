using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Armor.WayfarerSet
{
	[AutoloadEquip(EquipType.Body)]
	public class WayfarerBody : ModItem
	{
		public override void Load()
		{
			if (Main.netMode == NetmodeID.Server)
				return;

			EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Back}", EquipType.Back, this);
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wayfarer's Rucksack");
			Tooltip.SetDefault("5% increased movement speed");

			ArmorIDs.Body.Sets.NeedsToDrawArm[Item.bodySlot] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 20;
			Item.value = Item.sellPrice(0, 0, 40, 0);
			Item.rare = ItemRarityID.Blue;
			Item.defense = 3;
		}
		
		public override void UpdateEquip(Player player)
		{
			player.moveSpeed += 0.05f;
			player.runAcceleration += .01f;

			player.back = (sbyte)EquipLoader.GetEquipSlot(Mod, $"{Texture}_{EquipType.Back}", EquipType.Back);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddCondition(WayfarerHead.QuestCondition());
			recipe.AddIngredient(ModContent.ItemType<Consumable.Quest.DurasilkSheaf>(), 1);
			recipe.AddRecipeGroup(RecipeGroupID.IronBar, 2);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
