using SpiritMod.Mechanics.Fathomless_Chest.Entities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Mechanics.Fathomless_Chest.Effects
{
	public class BuffMidas : ChanceEffect
	{
		public override bool Unlucky => false;

		public override void Effects(Player player, Point16 tileCoords, IEntitySource source)
		{
			player.AddBuff(ModContent.BuffType<Buffs.MidasTouch>(), 3600 * 5);

			for (int numi = 0; numi < 8; numi++)
			{
				float SpeedX = (float)(-1 * Main.rand.Next(40, 70) * 0.00999999977648258 + Main.rand.Next(-20, 21) * 0.4f);
				float SpeedY = (float)(-1 * Main.rand.Next(40, 70) * 0.00999999977648258 + Main.rand.Next(-20, 21) * 0.4f);
				int p = Projectile.NewProjectile(source, (float)(tileCoords.X * 16) + 8 + SpeedX, (float)(tileCoords.Y * 16) + 12 + SpeedY, SpeedX, SpeedY, ModContent.ProjectileType<MidasProjectile>(), 0, 0f, player.whoAmI, 0.0f, 0.0f);
				Main.projectile[p].scale = Main.rand.Next(60, 150) * 0.01f;
			}

			NPC.NewNPC(source, (tileCoords.X * 16) + 16, (tileCoords.Y * 16) + 47, ModContent.NPCType<Fathomless_Chest_Gold>());
		}

		public override void OnKillVase(Player player, Point16 tileCoords, IEntitySource source) => SoundEngine.PlaySound(SoundID.Item30, tileCoords.ToVector2() * 16);
	}
}