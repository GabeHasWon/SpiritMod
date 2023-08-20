using Microsoft.Xna.Framework;
using SpiritMod.Items.Placeable.Furniture;
using Terraria;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;
using Terraria.DataStructures;
using SpiritMod.NPCs.Critters;

namespace SpiritMod.Tiles.Furniture;

public class ForgottenKingStatue : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileTable[Type] = true;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
		TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
		TileObjectData.newTile.StyleWrapLimit = 2;
		TileObjectData.newTile.StyleMultiplier = 2;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
		TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
		TileObjectData.addAlternate(1);
		TileObjectData.addTile(Type);

		LocalizedText name = CreateMapEntryName();
		// name.SetDefault("Forgotten King Statue");
		AddMapEntry(new Color(140, 140, 140), name);

		DustType = DustID.Stone;
	}

	public override void HitWire(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		int topX = i - tile.TileFrameX % 36 / 18;
		int topY = j - tile.TileFrameY % 36 / 18;

		for (int x = topX; x < topX + 2; x++)
		{
			for (int y = topY; y < topY + 2; y++)
			{
				if (Wiring.running)
					Wiring.SkipWire(x, y);
			}
		}

		int type = Main.rand.NextBool() ? ModContent.NPCType<Hemoglob>() : ModContent.NPCType<Hemoglorb>();
		int npc = NPC.NewNPC(new EntitySource_TileBreak(i, j), (int)((i + 1.5f) * 16f), (int)((j + 0.5f) * 16f), type);
		Main.npc[npc].SpawnedFromStatue = true;
		Main.npc[npc].catchItem = ItemID.None;

		if (Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc);
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY) 
		=> Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 48, 48, ModContent.ItemType<ForgottenKingStatueItem>());
}