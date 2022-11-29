using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Items.Armor.AstronautVanity
{
	internal class AstronautPlayer : ModPlayer
	{
		public override void PostUpdateEquips()
		{
			Item bodyVanitySlot = Player.armor[11];
			Item bodyArmorSlot = Player.armor[1];
			if (bodyVanitySlot.type == ModContent.ItemType<AstronautBody>() || (bodyArmorSlot.type == ModContent.ItemType<AstronautBody>() && bodyVanitySlot.IsAir))
				Player.back = (sbyte)EquipLoader.GetEquipSlot(Mod, nameof(AstronautBody), EquipType.Back);
		}
	}
}
