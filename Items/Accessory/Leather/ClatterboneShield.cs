using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.Leather
{
	[AutoloadEquip(EquipType.Shield)]
	public class ClatterboneShield : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Fervent Protector");
			// Tooltip.SetDefault("Increases defense by 2 and slows movement speed by 3% for every nearby foe\nThis effect stacks up to five times\n'There is something special between us'");
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.rare = ItemRarityID.Green;
			Item.defense = 2;
			Item.accessory = true;
			Item.value = Item.buyPrice(0, 0, 5, 0);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetSpiritPlayer().clatterboneShield = true;

			player.statDefense += 2 * player.GetSpiritPlayer().clatterStacks;
			player.moveSpeed -= .03f * player.GetSpiritPlayer().clatterStacks;
			player.maxRunSpeed -= .03f * player.GetSpiritPlayer().clatterStacks;
		}
	}
}
