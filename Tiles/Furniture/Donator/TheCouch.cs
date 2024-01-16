using Microsoft.Xna.Framework;
using SpiritMod.Buffs;
using Terraria;
using Terraria.GameContent.ObjectInteractions;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;

namespace SpiritMod.Tiles.Furniture.Donator
{
	public class TheCouch : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2); //this style already takes care of direction for us
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.newTile.Direction = Terraria.Enums.TileObjectDirection.None;
			TileObjectData.addTile(Type);
			TileID.Sets.CanBeSatOnForNPCs[Type] = true;
			TileID.Sets.CanBeSatOnForPlayers[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.HasOutlines[Type] = true;

			LocalizedText name = CreateMapEntryName();
			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
			AddMapEntry(new Color(200, 200, 200), name);
			DustType = -1;
		}

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
			=> settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance);

		public override bool RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;

			if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
			{
				player.GamepadEnableGrappleCooldown();
				player.sitting.SitDown(player, i, j);
			}
			return true;
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.cursorItemIconID = ModContent.ItemType<Items.DonatorItems.TheCouch>();
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
		}

		public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info) => info.VisualOffset.Y += 4;

		public override void NearbyEffects(int i, int j, bool closer)
		{
			if (closer && !Main.LocalPlayer.dead)
				Main.LocalPlayer.AddBuff(ModContent.BuffType<CouchPotato>(), 60);
		}
	}
}