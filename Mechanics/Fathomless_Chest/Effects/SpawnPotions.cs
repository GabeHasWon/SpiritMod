using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace SpiritMod.Mechanics.Fathomless_Chest.Effects
{
	public class SpawnPotions : ChanceEffect
	{
		public override bool Unlucky => false;

		public override void Effects(Player player, Point16 tileCoords, IEntitySource source)
		{
			int randomPotion = Utils.SelectRandom(Main.rand, new int[38] { 2344, 303, 300, 2325, 2324, 2356, 2329, 2346, 295, 2354, 2327, 291, 305, 2323, 304, 2348, 297, 292, 2345, 2352, 294, 293, 2322, 299, 288, 2347, 289, 298, 2355, 296, 2353, 2328, 290, 301, 2326, 2359, 302, 2349 });
			int randomPotion2 = Utils.SelectRandom(Main.rand, new int[38] { 2344, 303, 300, 2325, 2324, 2356, 2329, 2346, 295, 2354, 2327, 291, 305, 2323, 304, 2348, 297, 292, 2345, 2352, 294, 293, 2322, 299, 288, 2347, 289, 298, 2355, 296, 2353, 2328, 290, 301, 2326, 2359, 302, 2349 });
			int randomPotion3 = Utils.SelectRandom(Main.rand, new int[38] { 2344, 303, 300, 2325, 2324, 2356, 2329, 2346, 295, 2354, 2327, 291, 305, 2323, 304, 2348, 297, 292, 2345, 2352, 294, 293, 2322, 299, 288, 2347, 289, 298, 2355, 296, 2353, 2328, 290, 301, 2326, 2359, 302, 2349 });
			int randomPotion4 = Utils.SelectRandom(Main.rand, new int[38] { 2344, 303, 300, 2325, 2324, 2356, 2329, 2346, 295, 2354, 2327, 291, 305, 2323, 304, 2348, 297, 292, 2345, 2352, 294, 293, 2322, 299, 288, 2347, 289, 298, 2355, 296, 2353, 2328, 290, 301, 2326, 2359, 302, 2349 });
			
			if (Main.netMode != NetmodeID.SinglePlayer)
			{
				randomPotion = 2344;
				randomPotion2 = 303;
				randomPotion3 = 300;
				randomPotion4 = 2356;
			}
			int number = Item.NewItem(source, (int)(tileCoords.X * 16) + 8, (int)(tileCoords.Y * 16) + 12, 16, 18, randomPotion, 1);
			Main.item[number].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
			Main.item[number].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
			if (Main.netMode != NetmodeID.SinglePlayer && number >= 0)
			{
				NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number, 1f);
			}
			int number2 = Item.NewItem(source, (int)(tileCoords.X * 16) + 8, (int)(tileCoords.Y * 16) + 12, 16, 18, randomPotion2, 1);
			Main.item[number2].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
			Main.item[number2].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
			if (Main.netMode != NetmodeID.SinglePlayer && number2 >= 0)
			{
				NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number2, 1f);
			}
			int number3 = Item.NewItem(source, (int)(tileCoords.X * 16) + 8, (int)(tileCoords.Y * 16) + 12, 16, 18, randomPotion3, 1);
			Main.item[number3].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
			Main.item[number3].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
			if (Main.netMode != NetmodeID.SinglePlayer && number3 >= 0)
			{
				NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number3, 1f);
			}
			int number4 = Item.NewItem(source, (int)(tileCoords.X * 16) + 8, (int)(tileCoords.Y * 16) + 12, 16, 18, randomPotion4, 1);
			Main.item[number4].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
			Main.item[number4].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
			if (Main.netMode != NetmodeID.SinglePlayer && number4 >= 0)
			{
				NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number4, 1f);
			}
		}
	}
}