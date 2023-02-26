using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace SpiritMod.Mechanics.Fathomless_Chest.Effects
{
	public class BuffWeak : ChanceEffect
	{
		public override bool Unlucky => true;

		public override void Effects(Player player, Point16 tileCoords, IEntitySource source)
		{
			player.AddBuff(BuffID.Darkness, 3600);
			player.AddBuff(BuffID.Weak, 3600);
		}
	}
}