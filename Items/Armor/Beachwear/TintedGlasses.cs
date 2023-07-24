using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Armor.Beachwear
{
	[Sacrifice(1)]
	[AutoloadEquip(EquipType.Head)]
	public class TintedGlasses : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("'They help sell the look, so it's best to keep them on even when it's dark.'");
			ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 24;
			Item.value = Item.buyPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Blue;
			Item.vanity = true;
		}
	}
}