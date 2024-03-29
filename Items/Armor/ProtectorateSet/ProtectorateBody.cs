using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Armor.ProtectorateSet
{
	[AutoloadEquip(EquipType.Body)]
	public class ProtectorateBody : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Protectorate Uniform");
		}
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.value = Item.sellPrice(0, 0, 20, 0);
			Item.rare = ItemRarityID.Green;

			Item.vanity = true;
		}
    }
}
