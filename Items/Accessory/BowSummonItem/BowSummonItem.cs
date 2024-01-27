using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.BowSummonItem
{
	public class BowSummonItem : MinionAccessory
	{
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 40;
			Item.value = Item.sellPrice(0, 0, 55, 0);
			Item.rare = ItemRarityID.Green;
			Item.damage = 12;
			Item.knockBack = 2;
			Item.DamageType = DamageClass.Summon;
			Item.accessory = true;
		}
	}
}
