using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Armor.SilkArmor
{
	internal class SilkPlayer : ModPlayer
	{
		public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			Item earrings = ContentSamples.ItemsByType[ModContent.ItemType<Earrings>()];
			// Don't override existing front equips
			if (Player.head == earrings.headSlot && Player.front <= 0)
			{
				Player.front = earrings.frontSlot;
			}

			Item silkTop = ContentSamples.ItemsByType[ModContent.ItemType<SilkTop>()];
			if (!Player.Male && Player.body == silkTop.bodySlot)
			{
				Player.body = EquipLoader.GetEquipSlot(SpiritMod.Instance, "AltTop", EquipType.Body);
			}
		}
	}
}
