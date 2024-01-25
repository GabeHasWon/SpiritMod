using Microsoft.Xna.Framework;
using SpiritMod.Items.ByBiome.Ocean.Placeable;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Furniture.Ocean;

public class LoungeChair : ModTile
{
	private static bool Flipped(int i, int j) => Framing.GetTileSafely(i, j).TileFrameX > 36;

	private static bool HoveringOverBottomSide(int myX, int myY)
	{
		short frameX = Main.tile[myX, myY].TileFrameX;
		return !(frameX == 36 || frameX == 54);
	}

	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;

		TileID.Sets.CanBeSleptIn[Type] = true;
		TileID.Sets.InteractibleByNPCs[Type] = true;
		TileID.Sets.HasOutlines[Type] = true;
		TileID.Sets.IsValidSpawnPoint[Type] = true;
		AdjTiles = new int[] { TileID.Beds };
		AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
		TileObjectData.newTile.Origin = new Point16(1, 1);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidWithTop | AnchorType.SolidTile | AnchorType.Table, TileObjectData.newTile.Width, 0);
		TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
		TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
		TileObjectData.newTile.CoordinatePadding = 2;
		TileObjectData.newTile.StyleWrapLimit = 2;
		TileObjectData.newTile.StyleMultiplier = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
		TileObjectData.addAlternate(1);
		TileObjectData.addTile(Type);

		AddMapEntry(new Color(71, 185, 238), Language.GetText("ItemName.Bed"));
		DustType = -1;
	}

	public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

	public override bool RightClick(int i, int j)
	{
		Player player = Main.LocalPlayer;
		Tile tile = Main.tile[i, j];
		int spawnX = i - tile.TileFrameX / 18;
		int spawnY = j + 2;
		spawnX += tile.TileFrameX >= 54 ? 5 : 2;

		if (tile.TileFrameY % 38 != 0)
			spawnY--;

		if (!HoveringOverBottomSide(i, j))
		{
			if (player.IsWithinSnappngRangeToTile(i, j, PlayerSleepingHelper.BedSleepingMaxDistance))
			{
				player.GamepadEnableGrappleCooldown();

				//Workaround for the player being unable to sleep when the tile is flipped due to not using strict bed dimensions
				short oldFrameX = tile.TileFrameX;
				if (Flipped(i, j))
					tile.TileFrameX = 72;
				player.sleeping.StartSleeping(player, i, j);
				tile.TileFrameX = oldFrameX;
			}
		}
		else
		{
			player.FindSpawn();

			if (player.SpawnX == spawnX && player.SpawnY == spawnY)
			{
				player.RemoveSpawn();
				Main.NewText("Spawn point removed!", 255, 240, 20);
			}
			else if (Player.CheckSpawn(spawnX, spawnY))
			{
				player.ChangeSpawn(spawnX, spawnY);
				Main.NewText("Spawn point set!", 255, 240, 20);
			}
		}
		return true;
	}

	public override void ModifySleepingTargetInfo(int i, int j, ref TileRestingInfo info)
		=> info.VisualOffset += new Vector2(Flipped(i, j) ? 0f : 16f, 4f);

	public override void MouseOver(int i, int j)
	{
		Player player = Main.LocalPlayer;

		if (!HoveringOverBottomSide(i, j))
		{
			if (player.IsWithinSnappngRangeToTile(i, j, PlayerSleepingHelper.BedSleepingMaxDistance))
			{
				player.noThrow = 2;
				player.cursorItemIconEnabled = true;
				player.cursorItemIconID = ItemID.SleepingIcon;
			}
		}
		else
		{
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ModContent.ItemType<LoungeChairItem>();
		}
	}
}
