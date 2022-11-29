using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Items.Armor.BotanistSet
{
	internal class BotanistPlayer : ModPlayer
	{
		public bool active = false;

		public override void ResetEffects() => active = false;

		public override void PostUpdateEquips()
		{
			Item bodyVanitySlot = Player.armor[11];
			Item bodyArmorSlot = Player.armor[1];
			if (bodyVanitySlot.type == ModContent.ItemType<BotanistBody>() || (bodyArmorSlot.type == ModContent.ItemType<BotanistBody>() && bodyVanitySlot.IsAir))
				Player.waist = (sbyte)EquipLoader.GetEquipSlot(Mod, nameof(BotanistBody), EquipType.Waist);
		}
	}
}
