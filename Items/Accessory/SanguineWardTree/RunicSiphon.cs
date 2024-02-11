using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.Clubs;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.SanguineWardTree
{
	public class RunicSiphon : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = false;
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			if (npc.lifeRegen > 0) 
				npc.lifeRegen = 0;

			npc.lifeRegen -= 8;
		}
	}

	public class RunicSiphonGNPC : GlobalNPC
	{
		private float _runeGlow;
		protected override bool CloneNewInstances => true;
		public override bool InstancePerEntity => true;
		public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
		{
			if (npc.HasBuff(ModContent.BuffType<RunicSiphon>()))
				modifiers.FinalDamage *= 1.15f;
		}

		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
		{
			if (npc.HasBuff(ModContent.BuffType<RunicSiphon>()))
				modifiers.FinalDamage *= 1.15f;
		}

		public override void PostAI(NPC npc) => _runeGlow = (npc.HasBuff(ModContent.BuffType<RunicSiphon>())) ? Math.Min(_runeGlow + 0.05f, 0.5f) : Math.Max(_runeGlow - 0.05f, 0);

		public override Color? GetAlpha(NPC npc, Color drawColor)
		{
			if (_runeGlow > 0)
				return Color.Lerp(drawColor, new Color(250, 85, 167), _runeGlow) * npc.Opacity;
			return null;
		}
	}
}
