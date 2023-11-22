using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory
{
	[AutoloadEquip(EquipType.Neck)]
	public class RogueCrest : MinionAccessory
	{
		public override void SetDefaults()
		{
            Item.damage = 5;
            Item.DamageType = DamageClass.Summon;
            Item.knockBack = .5f;
            Item.width = 48;
			Item.height = 49;
			Item.value = Item.buyPrice(0, 3, 0, 0);
			Item.rare = ItemRarityID.Green;
			Item.defense = 1;
			Item.accessory = true;
		}
	}
}
