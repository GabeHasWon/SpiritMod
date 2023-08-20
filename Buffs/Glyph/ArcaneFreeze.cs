using Terraria;
using Terraria.ModLoader;
using SpiritMod.NPCs;
using Microsoft.Xna.Framework;
using SpiritMod.Projectiles;

namespace SpiritMod.Buffs.Glyph
{
	public class ArcaneFreeze : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Arcane Freeze");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = false;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<GNPC>().slowDegree = 1f; //100% average reduced speed
			
			if (Main.rand.NextBool(25))
			{
				Vector2 position = npc.position + new Vector2(npc.width * Main.rand.NextFloat(), npc.height * Main.rand.NextFloat());
				Projectile.NewProjectile(npc.GetSource_Buff(buffIndex), position, Vector2.UnitX.RotatedByRandom(5f), ModContent.ProjectileType<FrozenFragment>(), 0, 0, Main.myPlayer, npc.whoAmI);
			}
		}
	}
}