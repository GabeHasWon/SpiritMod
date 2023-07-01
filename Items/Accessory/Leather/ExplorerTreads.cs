using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.Leather
{
	[AutoloadEquip(EquipType.Shoes)]
	public class ExplorerTreads : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Explorer's Treads");
			Tooltip.SetDefault("+6% Movement speed for every 10% missing health\n" +
				"You can avoid most traps and hazards\n" +
				"'Makes exploring temples like a walk in the park'");
		}

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 20;
			Item.value = Item.buyPrice(0, 6, 75, 0);
			Item.rare = ItemRarityID.Green;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) => player.GetModPlayer<MyPlayer>().explorerTreads = true;
	}
}
