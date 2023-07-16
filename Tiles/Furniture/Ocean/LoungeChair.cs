using Microsoft.Xna.Framework;
using SpiritMod.Items.ByBiome.Ocean.Placeable;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Furniture.Ocean;

public class LoungeChair : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;

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

		AddMapEntry(new Color(71, 185, 238));
		DustType = DustID.BlueCrystalShard;
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY) 
		=> Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 32, ModContent.ItemType<LoungeChairItem>());
	public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

	public override bool RightClick(int i, int j)
	{
		Player player = Main.LocalPlayer;

		if (!Player.IsHoveringOverABottomSideOfABed(i, j))
		{
			if (player.IsWithinSnappngRangeToTile(i, j, PlayerSleepingHelper.BedSleepingMaxDistance))
			{
				player.GamepadEnableGrappleCooldown();
				player.sleeping.StartSleeping(player, i, j);
			}
		}
		return true;
	}

	public override void MouseOver(int i, int j)
	{
		Player player = Main.LocalPlayer;
		player.noThrow = 2;
		player.cursorItemIconEnabled = true;
		player.cursorItemIconID = ModContent.ItemType<LoungeChairItem>();
	}
}
