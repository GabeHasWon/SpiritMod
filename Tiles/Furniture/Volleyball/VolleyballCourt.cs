using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace SpiritMod.Tiles.Furniture.Volleyball;

public class VolleyballCourt : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileBrick[Type] = true;

		AddMapEntry(new Color(53, 59, 74));

		DustType = -1;
        HitSound = SoundID.Tink;
		ItemDrop = ModContent.ItemType<VolleyballCourtItem>();
	}
}

public class VolleyballCourtItem : ModItem
{
	public override void SetStaticDefaults() => DisplayName.SetDefault("Volleyball Court");
	public override void SetDefaults() => Item.DefaultToPlaceableTile(ModContent.TileType<VolleyballCourt>());

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe(25);
		recipe.AddIngredient(ModContent.ItemType<Items.Material.SynthMaterial>(), 1);
		recipe.AddTile(TileID.Anvils);
		recipe.Register();
	}
}