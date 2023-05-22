using SpiritMod.Items.Glyphs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.Fathomless_Chest.Effects
{
	public class SpawnGlyph : ChanceEffect
	{
		public override byte WhoAmI => 8;

		public override bool Unlucky => false;

		public override void Effects(Player player, Point16 tileCoords, IEntitySource source)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
				return;

			Item.NewItem(source, (tileCoords.X * 16) + 8, (tileCoords.Y * 16) + 12, 16, 18, ModContent.ItemType<Glyph>());
		}
	}
}