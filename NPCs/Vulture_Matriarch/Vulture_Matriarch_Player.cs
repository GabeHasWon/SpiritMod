using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SpiritMod.NPCs.Vulture_Matriarch
{
	public class Vulture_Matriarch_Player : ModPlayer
	{
		public bool goldified = false;
		
		public override void ResetEffects()
		{
			goldified = false;
		}
		
		public override void UpdateDead()
		{
			goldified = false;
		}
		
		public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			if (goldified)
			{
				if (Main.rand.NextBool(5) && drawInfo.shadow == 0f)
				{
					int index = Dust.NewDust(Player.position, Player.getRect().Width, Player.getRect().Height, DustID.GoldFlame, 0.0f, 0.0f, 0, new Color(), 1f);
					Main.dust[index].scale = 1.5f;
					Main.dust[index].noGravity = true;
					Main.dust[index].velocity *= 1.1f;
					drawInfo.DustCache.Add(index);
				}
				fullBright = false;
			}
		}

		public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (goldified)
				damageSource = PlayerDeathReason.ByCustomReason(Player.name + Language.GetTextValue("Mods.SpiritMod.GoldifiedDeath"));
			return true;
		}

		public override void ModifyHurt(ref Player.HurtModifiers modifiers)
		{
			if (goldified)
			{
				modifiers.DisableSound();
				SoundEngine.PlaySound(SoundID.Item37, Player.position);
			}
		}
	}
}