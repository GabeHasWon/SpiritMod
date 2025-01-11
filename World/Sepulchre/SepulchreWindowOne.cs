using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;

namespace SpiritMod.World.Sepulchre;

[LegacyName("SepulchreWindowTwo")]
public class SepulchreWindowOne : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileID.Sets.FramesOnKillWall[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
		TileObjectData.newTile.Height = 5;
		TileObjectData.newTile.Width = 3;
		TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16, 16];
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
		TileObjectData.newTile.AnchorTop = AnchorData.Empty;
		TileObjectData.newTile.AnchorWall = true;
		TileObjectData.addTile(Type);

		DustType = -1;
		LocalizedText name = CreateMapEntryName();
		// name.SetDefault("Sepulchre Window");
		AddMapEntry(new Color(100, 100, 100), name);
		RegisterItemDrop(ModContent.ItemType<SepulchreWindowItem>());
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
}

[Sacrifice(1)]
public class SepulchreWindowItem : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 30;
		Item.maxStack = Item.CommonMaxStack;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = 10;
		Item.useAnimation = 15;
		Item.useTurn = true;
		Item.autoReuse = true;
		Item.consumable = true;
		Item.createTile = ModContent.TileType<SepulchreWindowOne>();
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe(1);
		recipe.AddIngredient(ModContent.ItemType<Items.Placeable.Tiles.SepulchreBrickTwoItem>(), 10);
		recipe.AddTile(TileID.HeavyWorkBench);
		recipe.Register();
	}
}