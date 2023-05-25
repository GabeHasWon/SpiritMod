using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace SpiritMod.Mechanics.Fathomless_Chest.Effects
{
	public class CoinPortal : ChanceEffect
	{
		public override byte WhoAmI => 2;

		public override bool Unlucky => false;

		public override void Effects(Player player, Point16 tileCoords, IEntitySource source) 
			=> Projectile.NewProjectile(source, (tileCoords.ToVector2() * 16) + new Vector2(8, -18), Vector2.Zero, ProjectileID.CoinPortal, 0, 0);
	}
}