using Microsoft.Xna.Framework;
using SpiritMod.Items.Placeable.Furniture.Bamboo;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Furniture.Bamboo
{
	public class BambooBirdCage : ModTile
	{
		private readonly int[] birdList = new int[] { ItemID.Cardinal, ItemID.BlueJay, ItemID.GoldBird, ItemID.Bird };
		private bool ContainsBird(int i, int j, out int itemType, int tileFrameX = -1)
		{
			if (tileFrameX == -1)
				tileFrameX = Framing.GetTileSafely(i, j).TileFrameX;

			if (tileFrameX >= 36)
			{
				itemType = birdList[(int)MathHelper.Clamp((tileFrameX / 36) - 1, 0, birdList.Length - 1)];
				return true;
			}
			itemType = 0;
			return false;
		}

		public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(1, 2);
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 18 };
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 2, 0);

			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 2, 0);
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);

			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(100, 100, 60), name);
			DustType = -1;
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j)
		{
			var drops = new List<Item>() { new Item(ModContent.ItemType<BambooBirdCageItem>()) };
			if (ContainsBird(i, j, out int birdType, Main.tile[i, j].TileFrameX))
				drops.Add(new Item(birdType));

			foreach (Item item in drops)
				yield return item;
		}

		public override bool RightClick(int i, int j)
		{
			int x = i - Framing.GetTileSafely(i, j).TileFrameX / 18 % 2;
			int y = j - Framing.GetTileSafely(i, j).TileFrameY / 18 % 3;

			int heldType = Main.LocalPlayer.HeldItem.type;
			bool containsBird = ContainsBird(i, j, out int birdType);

			if (birdList.Contains(heldType) && !containsBird)
			{
				//Set the frame, and consume the respective bird
				for (int l = x; l < x + 2; l++)
				{
					for (int m = y; m < y + 3; m++)
					{
						Tile tile = Framing.GetTileSafely(l, m);
						if (tile.HasTile)
						{
							int frame = Array.FindIndex(birdList, x => x == Main.LocalPlayer.HeldItem.type) + 1;

							if (frame == -1)
								frame = 0;

							tile.TileFrameX += (short)(36 * frame);
						}
					}
				}

				if (!Main.LocalPlayer.ConsumeItem(heldType))
				{
					if (Main.mouseItem.stack > 1)
						Main.mouseItem.stack--;
					else
						Main.mouseItem.TurnToAir();
				}
			}
			else
			{
				//Reset the frame, and return the respective bird
				for (int l = x; l < x + 2; l++)
				{
					for (int m = y; m < y + 3; m++)
					{
						Tile tile = Framing.GetTileSafely(l, m);
						int frame = tile.TileFrameX % 36 / 18;

						if (tile.HasTile)
							tile.TileFrameX = (short)(18 * frame);
					}
				}

				Main.LocalPlayer.QuickSpawnItem(new EntitySource_TileInteraction(Main.LocalPlayer, i, j), birdType);
			}
			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendTileSquare(-1, x, y + 1, 3);

			return true;
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;

			if (ContainsBird(i, j, out int birdType))
				player.cursorItemIconID = birdType;
			else
				player.cursorItemIconID = ModContent.ItemType<BambooBirdCageItem>();

			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			if (closer && ContainsBird(i, j, out _) && Main.rand.NextBool(100))
				SoundEngine.PlaySound(SoundID.Bird, new Vector2(i * 16, j * 16));
		}
	}
}