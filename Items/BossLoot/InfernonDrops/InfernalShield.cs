using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.InfernonDrops
{
	[AutoloadEquip(EquipType.Shield)]
	public class InfernalShield : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => false;

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.rare = ItemRarityID.Pink;
			Item.value = 80000;
			Item.damage = 36;
			Item.defense = 3;
			Item.DamageType = DamageClass.Melee;
			Item.accessory = true;

			Item.knockBack = 5f;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetSpiritPlayer().infernalShield = true;
			player.endurance += 0.05f;
		}
	}
}
