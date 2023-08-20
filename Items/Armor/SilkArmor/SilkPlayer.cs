using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Items.Armor.SilkArmor
{
	internal class SilkPlayer : ModPlayer
	{
		public override void Load() => Terraria.On_Player.SetArmorEffectVisuals += LoadArmorVisuals;
		public override void Unload() => Terraria.On_Player.SetArmorEffectVisuals -= LoadArmorVisuals;

		private static void LoadArmorVisuals(Terraria.On_Player.orig_SetArmorEffectVisuals orig, Player self, Player drawPlayer)
		{
			orig(self, drawPlayer);

			Player player = drawPlayer;

			Item bodyVanitySlot = player.armor[11];
			Item bodyArmorSlot = player.armor[1];
			if (!player.Male && (bodyVanitySlot.type == ModContent.ItemType<SilkTop>() || (bodyArmorSlot.type == ModContent.ItemType<SilkTop>() && bodyVanitySlot.IsAir)))
				player.body = EquipLoader.GetEquipSlot(SpiritMod.Instance, "AltTop", EquipType.Body);
		}

		public override void PostUpdateEquips()
		{
			Item headVanitySlot = Player.armor[10];
			Item headArmorSlot = Player.armor[0];
			if (headVanitySlot.type == ModContent.ItemType<Earrings>() || (headArmorSlot.type == ModContent.ItemType<Earrings>() && headVanitySlot.IsAir))
				Player.front = (sbyte)EquipLoader.GetEquipSlot(Mod, nameof(Earrings), EquipType.Front);
		}
	}
}
