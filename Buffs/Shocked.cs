using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Buffs
{
	public class Shocked : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shocked!");
			Description.SetDefault("");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.lifeRegen -= 3;

			int loops = (npc.width + npc.height) / 50;
			for (int i = 0; i < loops; i++)
			{
				Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, Main.rand.NextBool(2) ? DustID.GemSapphire : DustID.PurpleCrystalShard, Scale: Main.rand.NextFloat(0.2f, 0.5f));

				if (Main.rand.NextBool(2) && dust.type == DustID.GemRuby)
					dust.fadeIn = 1f;

				dust.velocity = Vector2.Zero;

				dust.scale = npc.scale * Main.rand.NextFloat(0.8f, 1.3f);
				dust.noGravity = true;
			}

			if (Main.rand.NextBool(10))
			{
				Vector2 position = npc.Center + Main.rand.NextVector2Unit() * (npc.Size.Length() * Main.rand.NextFloat());
				int numLoops = Main.rand.Next(4) + 1;

				for (int i = 0; i < numLoops; i++)
					DrawLine(position, Main.rand.NextFloat(MathHelper.TwoPi), (int)MathHelper.Max(Main.rand.Next(10, 30) - (i * 5), 5), out position);
			}
		}

		private static void DrawLine(Vector2 startPos, float angle, int length, out Vector2 endPos)
		{
			Vector2 position = startPos;

			for (int i = 0; i < length; i++)
			{
				position = startPos + (Vector2.UnitX.RotatedBy(angle) * i);

				Dust dust = Dust.NewDustPerfect(position, DustID.PinkTorch, Vector2.Zero);
				dust.noGravity = true;
			}

			endPos = position;
		}
	}
}