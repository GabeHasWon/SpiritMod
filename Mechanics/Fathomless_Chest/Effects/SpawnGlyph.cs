using SpiritMod.Items.Glyphs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.Fathomless_Chest.Effects
{
	public class SpawnGlyph : ChanceEffect
	{
		public override bool Unlucky => false;

		public override void Effects(Player player, Point16 tileCoords, IEntitySource source)
		{
			int item = Item.NewItem(source, (tileCoords.X * 16) + 8, (tileCoords.Y * 16) + 12, 16, 18, ModContent.ItemType<Glyph>(), 1);

			if (Main.netMode != NetmodeID.SinglePlayer && item >= 0)
				NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item, 1f);
		}
	}
}