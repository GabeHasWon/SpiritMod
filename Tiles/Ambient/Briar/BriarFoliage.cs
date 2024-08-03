using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.Briar;

public class BriarFoliage : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileCut[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileLighted[Type] = true;

		TileID.Sets.IgnoredByGrowingSaplings[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.newTile.WaterDeath = false;
		TileObjectData.newTile.CoordinatePadding = 2;
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.CoordinateHeights = [20];
		TileObjectData.newTile.DrawYOffset = -2;
		TileObjectData.newTile.Style = 0;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.UsesCustomCanPlace = true;

		for (int i = 0; i < 7; i++)
		{
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.addSubTile(TileObjectData.newSubTile.Style);
		}

		TileObjectData.addTile(Type);

		TileID.Sets.SwaysInWindBasic[Type] = true;

		DustType = DustID.Plantera_Green;
		HitSound = SoundID.Grass;

		AddMapEntry(new Color(100, 150, 66));
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 2;

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (Main.rand.NextBool(8))
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 48, ModContent.ItemType<Items.Placeable.Tiles.BriarGrassSeeds>());
	}

    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Framing.GetTileSafely(i, j);

		if (tile.TileFrameX >= 108) 
		{
			Color colour = Color.White * MathHelper.Lerp(0.2f, 1f, (float)((Math.Sin(SpiritMod.GlobalNoise.Noise(i * 0.2f, j * 0.2f) * 3f + Main.GlobalTimeWrappedHourly * 1.3f) + 1f) * 0.5f));

			Texture2D glow = ModContent.Request<Texture2D>("SpiritMod/Tiles/Ambient/Briar/BriarFoliageGlow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
			spriteBatch.Draw(glow, new Vector2(i * 16, j * 16 + 2) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX - 108, 0, 16, 16), colour);
		}
	}
}