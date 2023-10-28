using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace SpiritMod.Tiles.Furniture.Volleyball;

public class VolleyballNet : ModTile
{
	public override bool IsLoadingEnabled(Mod mod) => false;

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileSolid[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileBrick[Type] = true;

		AddMapEntry(new Color(53, 59, 74));

		DustType = -1;
        HitSound = SoundID.Tink;
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		Tile above = Main.tile[i, j - 1];
		Tile tile = Main.tile[i, j];

		if (!above.HasTile || above.TileType != Type)
			tile.TileFrameY = 0;
		else
			tile.TileFrameY = 18;

		tile.TileFrameX = 0;
		return false;
	}
}

public class VolleyballNetItem : ModItem
{
	public override bool IsLoadingEnabled(Mod mod) => false;
	public override void SetDefaults() => Item.DefaultToPlaceableTile(ModContent.TileType<VolleyballNet>());

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe(25);
		recipe.AddIngredient(ModContent.ItemType<Items.Material.SynthMaterial>(), 1);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}