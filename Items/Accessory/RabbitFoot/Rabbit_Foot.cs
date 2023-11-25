using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.RabbitFoot
{
	[AutoloadEquip(EquipType.Waist)]
	public class Rabbit_Foot : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.value = Item.sellPrice(copper: 1);
			Item.rare = ItemRarityID.Blue;
			Item.accessory = true;
		}

		public override void UpdateEquip(Player player) => player.GetModPlayer<MyPlayer>().rabbitFoot = true;
	}
}