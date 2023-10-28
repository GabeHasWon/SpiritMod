using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Systems;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Furniture.Ocean;

public class BigBuoy : ModTile
{
	public override bool IsLoadingEnabled(Mod mod) => false;

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileNoAttach[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 10 };
		//TileObjectData.newTile.DrawYOffset = 4;
		//TileObjectData.newTile.LavaDeath = true;
		//TileObjectData.newTile.WaterDeath = false;
		//TileObjectData.newTile.CoordinatePadding = 2;
		//TileObjectData.newTile.CoordinateWidth = 16;
		//TileObjectData.newTile.Style = 0;
		//TileObjectData.newTile.StyleHorizontal = true;
		//TileObjectData.newTile.WaterPlacement = Terraria.Enums.LiquidPlacement.OnlyInLiquid;
		//TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
		TileObjectData.addTile(Type);

		TileID.Sets.SwaysInWindBasic[Type] = true;

		DustType = DustID.Iron;
		HitSound = SoundID.Dig;

		AddMapEntry(new Color(250, 67, 74));
	}

	public override bool CanPlace(int i, int j) => CanPlaceStatic(i, j);

	internal static bool CanPlaceStatic(int i, int j)
	{
		//j--;

		for (int x = i; x < i + 2; ++x)
		{
			Tile tile = Main.tile[x, j];
			Tile above = Main.tile[x, j - 1];

			if (tile.LiquidAmount < 50 || tile.LiquidType != LiquidID.Water || WorldGen.SolidOrSlopedTile(above) || above.LiquidAmount > 0)
				return false;
		}
		return true;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 2;

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0)
		{
			TileSwaySystem.DrawGrassSway(spriteBatch, Texture, i, j - 1, Lighting.GetColor(i, j), new Vector2(0, 20), new Point(32, 42), SpriteEffects.None);
			TileSwaySystem.DrawGrassSway(spriteBatch, Texture + "_Glow", i, j - 1, Color.White, new Vector2(0, 20), new Point(32, 42), SpriteEffects.None);
		}
		return false;
	}
}