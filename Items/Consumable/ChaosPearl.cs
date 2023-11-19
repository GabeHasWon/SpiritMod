using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable
{
	public class ChaosPearl : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.value = Item.sellPrice(0, 0, 4, 0);
			Item.rare = ItemRarityID.Orange;
			Item.maxStack = Item.CommonMaxStack;
			Item.damage = 0;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = Item.useAnimation = 40;
			Item.noMelee = true;
			Item.autoReuse = false;
			Item.consumable = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.Thrown.ChaosPearl>();
			Item.shootSpeed = 12;
			Item.UseSound = SoundID.Item1;
		}
	}
}