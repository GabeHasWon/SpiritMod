using log4net;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SpiritMod.Items.Sets.ToolsMisc.Evergreen;
using SpiritMod.Utilities.Helpers;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Utilities.ILEdits
{
	class UnfellerTreeEdit : ILEdit
	{
		public static ushort LastAxedType = 0;

		public override void Load(Mod mod) => Terraria.IL_Player.ItemCheck_UseMiningTools_ActuallyUseMiningTool += Player_ItemCheck;

		private static void Player_ItemCheck(ILContext il)
		{
			static void SetLastAxe(Tile tile) => 
				LastAxedType = tile.TileType;

			// Get an ILCursor and a logger to report errors if we find any
			ILCursor cursor = new ILCursor(il);
			ILog logger = ModContent.GetInstance<SpiritMod>().Logger;

			if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStloc(2))) //Match tile stloc
			{
				logger.Error("Failed to patch Player.ItemCheck for Unfeller of Evergreens: First jump failed");
				return;
			}

			cursor.Emit(OpCodes.Ldloc_2);
			cursor.EmitDelegate(SetLastAxe); //Set the last axed type

			if (!cursor.TryGotoNext(i => i.MatchCall(typeof(LucyAxeMessage), nameof(LucyAxeMessage.Create)))) //Match LucyAxeMessage.Creation usage
			{
				logger.Error("Failed to patch Player.ItemCheck for Unfeller of Evergreens: First jump failed");
				return;
			}

			if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall<WorldGen>(nameof(WorldGen.KillTile)))) //WorldGen.KillTile call
			{
				logger.Error("Failed to patch Player.ItemCheck for Unfeller of Evergreens: First jump failed");
				return;
			}

			// time for the fun stuff b a y b e e
			// We still have the type of the tile that got destroyed earlier pushed onto the stack
			// Now we use it by emitting a delegate that consumes this value from the stack, and we execute our own code
			// (Mother nature is happy with you)
			cursor.EmitDelegate(ReplaceTile);

			ILHelper.CompleteLog(cursor, false);
		}

		public static void ReplaceTile()
		{
			// Item that was used to kill the tile
			Item item = Main.LocalPlayer.HeldItem;

			// If the tile that got destroyed isn't a tree, or if the item type isn't the unfeller of evergreens, we stop and return
			if ((LastAxedType != TileID.Trees && LastAxedType != TileID.PalmTree) || item.type != ModContent.ItemType<UnfellerOfEvergreens>())
				return;

			// We will keep moving this tile down until we hit the tile the tree was sitting on (which will be the first solid tile we find)
			int currentX = Player.tileTargetX;
			int currentY = Player.tileTargetY;
			Tile currentTile = Framing.GetTileSafely(currentX, currentY);

			while (!currentTile.HasTile || !Main.tileSolid[currentTile.TileType])
				currentTile = Framing.GetTileSafely(currentX, ++currentY);

			// Now we finish up and plant a sapling above the tile we hit
			if (currentTile.TileType != LastAxedType)
				WorldGen.PlaceTile(currentX, currentY - 1, TileID.Saplings);
		}
	}
}
