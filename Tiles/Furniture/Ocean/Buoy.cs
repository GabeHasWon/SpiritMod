using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Furniture.Ocean;

public class Buoy : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileNoAttach[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.newTile.WaterDeath = false;
		TileObjectData.newTile.CoordinatePadding = 2;
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.CoordinateHeights = new int[] { 24 };
		TileObjectData.newTile.DrawYOffset = -12;
		TileObjectData.newTile.Style = 0;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.WaterPlacement = Terraria.Enums.LiquidPlacement.OnlyInLiquid;
		TileObjectData.newTile.UsesCustomCanPlace = true;
		TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
		TileObjectData.addTile(Type);

		TileID.Sets.SwaysInWindBasic[Type] = true;

		DustType = DustID.Iron;
		HitSound = SoundID.Dig;

		AddMapEntry(new Color(250, 67, 74));
	}

	public override void PlaceInWorld(int i, int j, Item item)
	{
		SoundEngine.PlaySound(SoundID.Splash, new Vector2(i, j) * 16);
		for (int x = 0; x < 10; x++)
			Dust.NewDustDirect(new Vector2(i, j) * 16, 16, 16, Dust.dustWater(), 0, 0, 150, new Color(), Main.rand.NextFloat(1f, 2f)).velocity = Vector2.UnitY * -Main.rand.NextFloat();
	}

	public override bool CanPlace(int i, int j) => CanPlaceStatic(i, j);

	internal static bool CanPlaceStatic(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		Tile above = Main.tile[i, j - 1];
		return !tile.HasTile && tile.LiquidAmount > 50 && tile.LiquidType != LiquidID.Lava && !WorldGen.SolidOrSlopedTile(above) && above.LiquidAmount <= 0;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 2;

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		TileSwaySystem.DrawGrassSway(spriteBatch, Texture, i, j - 1, Lighting.GetColor(i, j), new Vector2(0, 4), new Point(16, 24), SpriteEffects.None);
		TileSwaySystem.DrawGrassSway(spriteBatch, Texture + "_Glow", i, j - 1, Color.White, new Vector2(0, 4), new Point(16, 24), SpriteEffects.None);
		return false;
	}
}