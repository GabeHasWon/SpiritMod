using System;
using Terraria;
using Terraria.ModLoader; 

namespace SpiritMod.Mounts.Minecarts
{
	public class MinecartPlayer : ModPlayer
	{
		public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
		{
			if (Player.mount.Type == ModContent.MountType<MarbleMinecart.MarbleMinecart>() && Math.Abs(Player.velocity.X) > 3.5f) //reduces contact damage when ramming
			{
				modifiers.FinalDamage.Base -= (int)(Math.Abs(Player.velocity.X) - 5);
			}
		}
	}

	public class MinecartNPC : GlobalNPC
	{
		public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
		{
			if (target.mount.Type == ModContent.MountType<MarbleMinecart.MarbleMinecart>() && Math.Abs(target.velocity.X) > 3.5f) //does extra damage on hit
				npc.SimpleStrikeNPC((int)target.velocity.X, target.direction, true, 4f, null, false);
		}
	}
}
