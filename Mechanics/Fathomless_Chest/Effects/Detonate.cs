using Microsoft.Xna.Framework;
using SpiritMod.Mechanics.Fathomless_Chest.Entities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.Fathomless_Chest.Effects
{
	public class Detonate : ChanceEffect
	{
		public override byte WhoAmI => 5;

		public override bool Unlucky => true;

		public override void Effects(Player player, Point16 tileCoords, IEntitySource source)
			=> Projectile.NewProjectile(source, (tileCoords.ToVector2() * 16) + new Vector2(16, 24), Vector2.Zero, ModContent.ProjectileType<Fathomless_Chest_Bomb>(), 100, 11);

		public override void OnKillVase(Player player, Point16 tileCoords, IEntitySource source) { }
	}
}