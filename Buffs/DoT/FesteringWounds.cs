using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace SpiritMod.Buffs.DoT
{
	public class FesteringWounds : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Festering Wounds");
			// Description.SetDefault("Deals more damage if the victim is under half health");
			Main.buffNoTimeDisplay[Type] = false;
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			Terraria.ID.BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			if (!npc.friendly)
			{
				if (npc.lifeRegen > 0)
					npc.lifeRegen = 0;

				if (npc.life <= npc.lifeMax / 2)
					npc.lifeRegen -= 8;
				else
					npc.lifeRegen -= 4;

				if (Main.rand.NextBool(3))
				{
					int d = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Plantera_Green);
					Main.dust[d].velocity.X *= 0f;
					Main.dust[d].velocity.Y *= 0.5f;
				}
			}
		}

		public override void Update(Player player, ref int buffIndex)
		{
			if (player.lifeRegen > 0)
				player.lifeRegen = 0;

			if (player.statLife <= player.statLife / 2)
				player.lifeRegen -= 5;
			else
				player.lifeRegen -= 2;

			if (Main.rand.NextBool(3))
			{
				int d = Dust.NewDust(player.position, player.width, player.height, DustID.Plantera_Green);
				Main.dust[d].velocity.X *= 0f;
				Main.dust[d].velocity.Y *= 0.5f;
			}
		}
	}
}