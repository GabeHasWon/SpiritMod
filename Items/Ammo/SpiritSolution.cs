using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Ammo
{
	public class SpiritSolution : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Cyan Solution");
			// Tooltip.SetDefault("Used by the Clentaminator\nSpreads the Spirit");
		}

		public override void SetDefaults()
		{
			Item.shoot = ModContent.ProjectileType<Projectiles.Solutions.SpiritSolution>() - ProjectileID.PureSpray;
			Item.ammo = AmmoID.Solution;
			Item.width = 10;
			Item.height = 12;
			Item.value = Item.buyPrice(0, 0, 7, 0);
			Item.rare = ItemRarityID.Orange;
			Item.maxStack = 999;
			Item.consumable = true;
		}
	}
}
