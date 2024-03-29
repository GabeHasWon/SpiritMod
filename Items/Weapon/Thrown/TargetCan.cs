using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Weapon.Thrown
{
	[Sacrifice(99)]
	public class TargetCan : ModItem
	{
		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.width = 9;
			Item.height = 15;
			Item.noUseGraphic = true;
			Item.UseSound = SoundID.Item1;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.consumable = true;
			Item.maxStack = Item.CommonMaxStack;
			Item.shoot = ModContent.ProjectileType<Projectiles.Thrown.TargetCan>();
			Item.useTime = Item.useAnimation = 25;
			Item.shootSpeed = 8.5f;
			Item.damage = 0;
			Item.knockBack = 1.5f;
			Item.value = Item.sellPrice(0, 0, 0, 20);
			Item.crit = 8;
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.consumable = true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(5);
			recipe.AddIngredient(ItemID.TinCan, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
