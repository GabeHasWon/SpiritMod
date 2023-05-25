using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace SpiritMod.Mechanics.Fathomless_Chest.Effects
{
	public class SpawnButterflies : ChanceEffect
	{
		public override byte WhoAmI => 7;

		public override bool Unlucky => false;

		public override void Effects(Player player, Point16 tileCoords, IEntitySource source)
		{
			for (int g = 0; g < 8 + Main.rand.Next(6); g++)
			{
				float npcposX = (tileCoords.X * 16) + Main.rand.Next(-60, 60);
				float npcposY = (tileCoords.Y * 16) + Main.rand.Next(-60, 60);

				int npcType = 356;

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					if (Main.rand.NextBool(6))
						npcType = NPCID.GoldButterfly;

					int id = NPC.NewNPC(source, (int)npcposX, (int)npcposY, npcType);
					Main.npc[id].netUpdate2 = true;
				}
				if (Main.netMode != NetmodeID.Server)
				{
					SoundEngine.PlaySound(SoundID.Item6, new Vector2(npcposX, npcposY));
				}

				Vector2 spinningpoint = (Vector2.UnitX * -3f).RotatedByRandom(MathHelper.Pi);
				Vector2 vector2 = new Vector2(1.1f, 1f);

				int loops = 28;
				for (float i = 0f; i < loops; ++i)
				{
					Dust dust = Dust.NewDustPerfect(new Vector2(npcposX, npcposY), (npcType == NPCID.GoldButterfly) ? DustID.GoldCoin : DustID.MagicMirror, Vector2.Zero, 0, default, 1f);
					dust.velocity = spinningpoint.RotatedBy(6.28318548202515 * (double)i / (double)loops, new Vector2()) * vector2 * (float)(.8 + (double)Main.rand.NextFloat() * .4);
					dust.noGravity = true;
					dust.fadeIn = Main.rand.NextFloat();
				}
			}
		}
	}
}