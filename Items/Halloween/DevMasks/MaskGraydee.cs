using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Halloween.DevMasks
{
	[AutoloadEquip(EquipType.Head)]
	public class MaskGraydee : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Graydee's Mask");
			// Tooltip.SetDefault("Vanity item \n'Great for impersonating devs!'");

		}


		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 20;
			Item.value = 3000;
			Item.rare = ItemRarityID.Cyan;
		}
	}
}
