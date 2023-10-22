using SpiritMod.Tiles.Block;
using SpiritMod.Tiles.Furniture.SpaceJunk;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace SpiritMod.Items.Placeable.Tiles
{
	public class SpaceJunkItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<AsteroidBlock>();
			ItemID.Sets.ExtractinatorMode[Type] = Type;
		}

		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 14;
			Item.maxStack = 999;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<SpaceJunkTile>();
		}

		public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack)
		{
			WeightedRandom<int> drop = new();
			drop.Add(ItemID.CopperOre);
			drop.Add(ItemID.TinOre);
			drop.Add(ItemID.IronOre);
			drop.Add(ItemID.LeadOre);
			drop.Add(ItemID.SilverOre);
			drop.Add(ItemID.TungstenOre);
			drop.Add(ItemID.GoldOre);
			drop.Add(ItemID.PlatinumOre);
			drop.Add(ItemID.Obsidian);

			drop.Add(ModContent.ItemType<ScrapItem2>(), .17);
			drop.Add(ModContent.ItemType<ScrapItem3>(), .17);
			drop.Add(ModContent.ItemType<ScrapItem5>(), .17);

			drop.Add(ModContent.ItemType<ScrapItem1>(), .1);
			drop.Add(ModContent.ItemType<ScrapItem4>(), .1);
			drop.Add(ModContent.ItemType<ScrapItem6>(), .1);

			drop.Add(ItemID.OldShoe, .11);
			drop.Add(ItemID.TinCan, .11);

			int itemDrop = drop;
			resultType = itemDrop;

			if (new int[] { ModContent.ItemType<ScrapItem2>(), ModContent.ItemType<ScrapItem3>(), ModContent.ItemType<ScrapItem5>() }.Contains(itemDrop))
				resultStack = Main.rand.Next(1, 4);
			else if (new int[] { ItemID.CopperOre, ItemID.TinOre, ItemID.IronOre, ItemID.LeadOre, ItemID.SilverOre, ItemID.TungstenOre, ItemID.GoldOre, ItemID.PlatinumOre, ItemID.Obsidian }.Contains(itemDrop))
				resultStack = Main.rand.Next(2, 4);
			else
				resultStack = 1;
		}
	}
}