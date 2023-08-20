using Terraria;
using Terraria.ModLoader;
using SpiritMod.NPCs;
using Microsoft.Xna.Framework;
using SpiritMod.Projectiles;

namespace SpiritMod.Buffs
{
	public class Frozen : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frozen");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = false;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			if (npc.knockBackResist <= 0f)
				return;

			npc.GetGlobalNPC<GNPC>().slowDegree = .3f; //30% average reduced speed
			
			if (Main.rand.NextBool(25))
			{
				Vector2 position = npc.position + new Vector2(npc.width * Main.rand.NextFloat(), npc.height * Main.rand.NextFloat());
				Projectile.NewProjectile(npc.GetSource_Buff(buffIndex), position, Vector2.UnitX.RotatedByRandom(5f), ModContent.ProjectileType<FrozenFragment>(), 0, 0, Main.myPlayer, npc.whoAmI);
			}
		}
	}
}