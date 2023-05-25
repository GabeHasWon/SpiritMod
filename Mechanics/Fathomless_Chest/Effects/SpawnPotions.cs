using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace SpiritMod.Mechanics.Fathomless_Chest.Effects
{
	public class SpawnPotions : ChanceEffect
	{
		public override byte WhoAmI => 10;

		public override bool Unlucky => false;

		public override void Effects(Player player, Point16 tileCoords, IEntitySource source)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
				return;

			for (int i = 0; i < 4; i++)
			{
				int randomPotion = Utils.SelectRandom(Main.rand, new int[38] { 2344, 303, 300, 2325, 2324, 2356, 2329, 2346, 295, 2354, 2327, 291, 305, 2323, 304, 2348, 297, 292, 2345, 2352, 294, 293, 2322, 299, 288, 2347, 289, 298, 2355, 296, 2353, 2328, 290, 301, 2326, 2359, 302, 2349 });

				int number = Item.NewItem(source, (int)(tileCoords.X * 16) + 8, (int)(tileCoords.Y * 16) + 12, 16, 18, randomPotion, 1);
				Main.item[number].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
				Main.item[number].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
			}
		}
	}
}