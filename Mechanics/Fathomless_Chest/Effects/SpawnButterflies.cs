using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace SpiritMod.Mechanics.Fathomless_Chest.Effects
{
	public class SpawnButterflies : ChanceEffect
	{
		public override bool Unlucky => false;

		public override void Effects(Player player, Point16 tileCoords, IEntitySource source)
		{
			for (int g = 0; g < 8 + Main.rand.Next(6); g++)
			{
				float npcposX = (tileCoords.X * 16) + Main.rand.Next(-60, 60);
				float npcposY = (tileCoords.Y * 16) + Main.rand.Next(-60, 60);
				int dustType;
				int npcType;
				if (Main.rand.NextBool(6))
				{
					npcType = NPC.NewNPC(source, (int)npcposX, (int)npcposY, NPCID.GoldButterfly);
					dustType = DustID.GoldCoin;
				}
				else
				{
					npcType = NPC.NewNPC(source, (int)npcposX, (int)npcposY, 356);
					dustType = DustID.MagicMirror;
				}
				Main.npc[npcType].netUpdate = true;
				Vector2 spinningpoint = new Vector2(0.0f, -3f).RotatedByRandom(MathHelper.Pi);
				float num1 = (float)28;
				Vector2 vector2 = new Vector2(1.1f, 1f);

				for (float num2 = 0.0f; (double)num2 < (double)num1; ++num2)
				{
					int dustIndex = Dust.NewDust(new Vector2(npcposX, npcposY), 0, 0, dustType, 0.0f, 0.0f, 0, new Color(), 1f);
					Main.dust[dustIndex].position = new Vector2(npcposX, npcposY);
					Main.dust[dustIndex].velocity = spinningpoint.RotatedBy(6.28318548202515 * (double)num2 / (double)num1, new Vector2()) * vector2 * (float)(0.800000011920929 + (double)Main.rand.NextFloat() * 0.400000005960464);
					Main.dust[dustIndex].noGravity = true;
					Main.dust[dustIndex].scale = 2f;
					Main.dust[dustIndex].fadeIn = Main.rand.NextFloat() * 2f;
					Dust dust = Dust.CloneDust(dustIndex);
					dust.scale /= 2f;
					dust.fadeIn /= 2f;
				}
				SoundEngine.PlaySound(SoundID.Item6, new Vector2(npcposX, npcposY));
			}
		}
	}
}