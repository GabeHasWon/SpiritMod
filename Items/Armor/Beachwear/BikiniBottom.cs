using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Armor.Beachwear
{
	[Sacrifice(1)]
	[AutoloadEquip(EquipType.Legs)]
	public class BikiniBottom : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => false;

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 12;
			Item.value = Item.buyPrice(0, 5, 0, 0);
			Item.rare = ItemRarityID.Blue;
			Item.vanity = true;
		}
	}
}