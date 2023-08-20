using SpiritMod.Items.Placeable.Furniture;
using SpiritMod.Projectiles.Yoyo;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Weapon.Yoyo
{
	public class Probe : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("The Probe");
			// Tooltip.SetDefault("Fires lasers at surrounding enemies");
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.WoodYoyo);
			Item.damage = 52;
			Item.value = Item.sellPrice(0, 10, 0, 0);
			Item.rare = ItemRarityID.LightPurple;
			Item.knockBack = 3f;
			Item.channel = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useAnimation = 25;
			Item.useTime = 24;
			Item.shoot = ModContent.ProjectileType<ProbeP>();
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.HallowedBar, 11);
			recipe.AddIngredient(ItemID.SoulofMight, 13);
			recipe.AddIngredient(ModContent.ItemType<PrintProbe>(), 1);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}
