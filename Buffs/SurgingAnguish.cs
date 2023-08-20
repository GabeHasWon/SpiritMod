using Microsoft.Xna.Framework;
using SpiritMod.Dusts;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Buffs
{
	public class SurgingAnguish : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Surging Anguish");
			// Description.SetDefault("'Your mind and body are in great pain'");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.lifeRegen -= 3;

			int loops = Math.Max(2, (npc.width + npc.height) / 50);
			for (int i = 0; i < loops; i++)
			{
				Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, Main.rand.NextBool(2) ? DustID.GemRuby : ModContent.DustType<NightmareDust>());

				if (Main.rand.NextBool(2) && dust.type == DustID.GemRuby)
				{
					dust.fadeIn = 1.5f;
					dust.velocity = Vector2.Zero;
				}
				else
				{
					dust.velocity = new Vector2(0, -2);
				}

				dust.scale = npc.scale * Main.rand.NextFloat(0.8f, 1.3f);
				dust.noGravity = true;
			}
		}
	}
}