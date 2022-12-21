using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Items.Armor.WayfarerSet
{
	internal class WayfarerPlayer : ModPlayer
	{
		public override void PostUpdateEquips()
		{
			Item bodyVanitySlot = Player.armor[11];
			Item bodyArmorSlot = Player.armor[1];
			if (bodyVanitySlot.type == ModContent.ItemType<WayfarerBody>() || (bodyArmorSlot.type == ModContent.ItemType<WayfarerBody>() && bodyVanitySlot.IsAir))
				Player.back = (sbyte)EquipLoader.GetEquipSlot(Mod, nameof(WayfarerBody), EquipType.Back);
		}
	}
}
