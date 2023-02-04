using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.FlailsMisc.ClatterMace
{
	public class ClatterMace : BaseFlailItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Clattering Mace");
			Tooltip.SetDefault("Has a chance to lower enemy defense on hit");
			ItemID.Sets.ToolTipDamageMultiplier[Type] = 2;
		}

		public override void SafeSetDefaults()
		{
			Item.width = 38;
			Item.height = 34;
			Item.value = Item.sellPrice(0, 0, 60, 0);
			Item.rare = ItemRarityID.Green;
			Item.damage = 10;
			Item.knockBack = 5.5f;
			Item.useTime = Item.useAnimation = 30;
			Item.shoot = ModContent.ProjectileType<ClatterMaceProj>();
			Item.shootSpeed = 12f;
		}
	}
}