using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Buffs;
using Terraria;
using SpiritMod.NPCs.AuroraStag;
using Microsoft.Xna.Framework;

namespace SpiritMod.Items.Consumable.Food
{
	[Sacrifice(5)]
	public class IceBerries : FoodItem
	{
		internal override Point Size => new(30, 42);

		public override bool AltFunctionUse(Player player) => true;

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI != Main.myPlayer)
				return false;

			MyPlayer myPlayer = player.GetModPlayer<MyPlayer>();
			AuroraStag auroraStag = myPlayer.hoveredStag;

			if (player.altFunctionUse == 2 && auroraStag != null && !auroraStag.Scared && !auroraStag.NPC.immortal && auroraStag.TameAnimationTimer == 0) 
			{
				auroraStag.TameAnimationTimer = AuroraStag.TameAnimationLength;
				myPlayer.hoveredStag = null;

				if (Main.netMode == NetmodeID.MultiplayerClient)
					SpiritMod.WriteToPacket(SpiritMod.Instance.GetPacket(4), (byte)MessageType.TameAuroraStag, auroraStag.NPC.whoAmI).Send();
			}
			else if (player.altFunctionUse != 2)
				player.AddBuff(ModContent.BuffType<IceBerryBuff>(), 19600);

			return true;
		}
	}
}
