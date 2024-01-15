using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Placeable.Furniture.Bamboo;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Furniture.Bamboo
{
	public class BambooBarrel : ModTile
	{
		private static Point GetMultiTilePos(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			if (tile.TileFrameX % 36 != 0)
				i--;
			if (tile.TileFrameY != 0)
				j--;

			return new Point(i, j);
		}

		public override void SetStaticDefaults()
        {
			Main.tileSpelunker[Type] = true;
			Main.tileContainer[Type] = true;
			Main.tileShine2[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileOreFinderPriority[Type] = 500;

			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.BasicChest[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.RandomStyleRange = 3;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
			TileObjectData.newTile.AnchorInvalidTiles = new int[] { 127 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);

			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(100, 100, 60), name, MapChestName);
			DustType = DustID.PalmWood;
			AdjTiles = new int[] { TileID.Containers };
			RegisterItemDrop(ModContent.ItemType<BambooBarrelItem>());
		}

		public override LocalizedText DefaultContainerName(int frameX, int frameY) => this.GetLocalization("MapEntry");

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

		private static string MapChestName(string name, int i, int j)
		{
			(i, j) = (GetMultiTilePos(i, j).X, GetMultiTilePos(i, j).Y);

			int chest = Chest.FindChest(i, j);
			if (chest < 0)
				return Language.GetTextValue("LegacyChestType.0");

			if (Main.chest[chest].name == string.Empty)
				return name;

			return name + ": " + Main.chest[chest].name;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) => Chest.DestroyChest(i, j);

		public override void PlaceInWorld(int i, int j, Item item)
		{
			if (Main.netMode != NetmodeID.SinglePlayer)
			{
				Point tilePos = GetMultiTilePos(i, j);
				NetMessage.SendTileSquare(-1, tilePos.X, tilePos.Y, 2, TileChangeType.None);
			} //Sync random style for the chest
		}

		public override bool RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Main.mouseRightRelease = false;

			Point tilePos = GetMultiTilePos(i, j);

			if (player.sign >= 0)
			{
				SoundEngine.PlaySound(SoundID.MenuClose);
				player.sign = -1;
				Main.editSign = false;
				Main.npcChatText = string.Empty;
			}
			if (Main.editChest)
			{
				SoundEngine.PlaySound(SoundID.MenuTick);
				Main.editChest = false;
				Main.npcChatText = string.Empty;
			}
			if (player.editedChestName)
			{
				NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
				player.editedChestName = false;
			}
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				if (tilePos.X == player.chestX && tilePos.Y == player.chestY && player.chest >= 0)
				{
					player.chest = -1;
					Recipe.FindRecipes();
					SoundEngine.PlaySound(SoundID.MenuClose);
				}
				else
				{
					NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, tilePos.X, tilePos.Y, 0f, 0f, 0, 0, 0);
					Main.stackSplit = 600;
				}
			}
			else
			{
				int chest = Chest.FindChest(tilePos.X, tilePos.Y);
				if (chest >= 0)
				{
					Main.stackSplit = 600;
					if (chest == player.chest)
					{
						player.chest = -1;
						SoundEngine.PlaySound(SoundID.MenuClose);
					}
					else
					{
						player.chest = chest;
						Main.playerInventory = true;
						Main.recBigList = false;
						player.chestX = tilePos.X;
						player.chestY = tilePos.Y;
						SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
					}
					Recipe.FindRecipes();
				}
			}
			return true;
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.cursorItemIconID = -1;

			Point tilePos = GetMultiTilePos(i, j);
			int chest = Chest.FindChest(tilePos.X, tilePos.Y);

			if (chest < 0)
				player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
			else
			{
				Tile tile = Framing.GetTileSafely(i, j);
				string defaultName = TileLoader.DefaultContainerName(tile.TileType, tile.TileFrameX, tile.TileFrameY);

				player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : defaultName;
				if (player.cursorItemIconText == defaultName)
				{
					player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type);
					player.cursorItemIconText = string.Empty;
				}
			}
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
		}

		public override void MouseOverFar(int i, int j)
		{
			MouseOver(i, j);
			Player player = Main.LocalPlayer;
			if (player.cursorItemIconText == string.Empty)
			{
				player.cursorItemIconEnabled = false;
				player.cursorItemIconID = 0;
			}
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			Texture2D texture = TextureAssets.Tile[Type].Value;
			Rectangle source = new Rectangle(tile.TileFrameX, tile.TileFrameY % 36, 16, (tile.TileFrameY > 0) ? 18 : 16);

			Vector2 offset = Lighting.LegacyEngine.Mode > 1 ? Vector2.Zero : Vector2.One * 12;
			Vector2 drawPos = ((new Vector2(i, j) + offset) * 16) - Main.screenPosition;

			spriteBatch.Draw(texture, drawPos, source, Lighting.GetColor(i, j), 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);

			if (Main.InSmartCursorHighlightArea(i, j, out bool actuallySelected))
				spriteBatch.Draw(ModContent.Request<Texture2D>(HighlightTexture).Value, drawPos, source, actuallySelected ? Color.Yellow : Color.Gray, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);

			return false;
		}
	}
}