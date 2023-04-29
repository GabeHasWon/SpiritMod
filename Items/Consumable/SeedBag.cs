using SpiritMod.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable
{
	public class SeedBag : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Floral Seed Bag");
			Tooltip.SetDefault("Contains exotic grass seeds");
		}


		public override void SetDefaults()
		{
			Item.width = Item.height = 16;
			Item.rare = ItemRarityID.Green;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.maxStack = 99;
			Item.value = Item.buyPrice(0, 0, 10, 0);
			Item.useTime = Item.useAnimation = 20;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.noMelee = true;
			Item.autoReuse = false;
		}

		public override bool CanRightClick() => true;

		public override void ModifyItemLoot(ItemLoot itemLoot)
		{
			itemLoot.AddCommon(ItemID.MushroomGrassSeeds, 1, 1, 5);
			itemLoot.AddCommon(ItemID.JungleGrassSeeds, 1, 1, 5);
			itemLoot.AddCommon<Placeable.Tiles.BriarGrassSeeds>(1, 1, 5);
		}
	}
}
