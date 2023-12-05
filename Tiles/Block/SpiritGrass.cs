using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;
using TileID = Terraria.ID.TileID;
using Terraria.ID;
using SpiritMod.Tiles.Ambient.Spirit;
using Terraria.Utilities;

namespace SpiritMod.Tiles.Block
{
	public class SpiritGrass : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileMerge[Type][ModContent.TileType<SpiritDirt>()] = true;
			Main.tileBlendAll[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileLighted[Type] = true;

			Main.tileMerge[Type][TileID.Grass] = true;
			Main.tileMerge[TileID.Grass][Type] = true;

			TileID.Sets.Grass[Type] = true;
			TileID.Sets.Conversion.Grass[Type] = true;
			TileID.Sets.CanBeDugByShovel[Type] = true;

			AddMapEntry(new Color(0, 191, 255));
			DustType = DustID.Flare_Blue;
		}

		public override bool CanExplode(int i, int j)
		{
			WorldGen.KillTile(i, j, false, false, true); //Makes the tile completely go away instead of reverting to dirt
			return true;
		}

		public static bool PlaceObject(int x, int y, int type, bool mute = false, int style = 0, int random = -1, int direction = -1)
		{
			if (!TileObject.CanPlace(x, y, type, style, direction, out TileObject toBePlaced, false))
				return false;

			toBePlaced.random = random;
			if (TileObject.Place(toBePlaced) && !mute)
				WorldGen.SquareTileFrame(x, y, true);

			return false;
		}

		public override void RandomUpdate(int i, int j)
		{
			Tile aboveTile = Framing.GetTileSafely(i, j - 1);

			if (!aboveTile.HasTile)
			{
				WeightedRandom<int> plant = new WeightedRandom<int>();
				plant.Add(-1, 1f);
				plant.Add(ModContent.TileType<SpiritFoliage>(), 0.25f);
				plant.Add(ModContent.TileType<SpiritTallgrass>(), 0.125f);
				plant.Add(ModContent.TileType<SoulBloomTile>(), 0.04f);

				int selection = plant;

				if (selection != -1)
				{
					int style = 0;

					if (selection == ModContent.TileType<SpiritFoliage>())
						style = Main.rand.Next(16);
					else if (selection == ModContent.TileType<SpiritFoliage>())
						style = Main.rand.Next(10);

					PlaceObject(i, j - 1, selection, true, style);
					NetMessage.SendObjectPlacement(-1, i, j - 1, selection, style, 0, -1, -1);
				}
			}

			if (SpreadHelper.Spread(i, j, Type, 4, TileID.Dirt) && Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendTileSquare(-1, i, j, 3, TileChangeType.None);
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile2 = Framing.GetTileSafely(i, j - 1);
			if (!Main.tileSolid[tile2.TileType] || !tile2.HasTile)
			{
				Color colour = Color.White * MathHelper.Lerp(0.2f, 1f, (float)((Math.Sin(SpiritMod.GlobalNoise.Noise(i * 0.2f, j * 0.2f) * 3f + Main.GlobalTimeWrappedHourly * 1.3f) + 1f) * 0.5f));
				Texture2D glow = ModContent.Request<Texture2D>("SpiritMod/Tiles/Block/SpiritGrass_Glow", ReLogic.Content.AssetRequestMode.AsyncLoad).Value;

				GTile.DrawSlopedGlowMask(i, j, glow, colour, Vector2.Zero);
			}
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			Tile tile = Framing.GetTileSafely(i, j - 1);
			if (Main.tileSolid[tile.TileType] && tile.HasTile)
				return;

			(r, g, b) = (.3f, .45f, 1.05f);
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			if (!fail) //Change self into dirt
			{
				fail = true;
				Framing.GetTileSafely(i, j).TileType = (ushort)ModContent.TileType<SpiritDirt>();
			}
		}
	}
}