using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SpearsMisc.BambooHalberd
{
	public class BambooHalberd : ModItem
	{
		// public override void SetStaticDefaults() => DisplayName.SetDefault("Bamboo Halberd");

		public override void SetDefaults()
		{
			Item.damage = 7;
			Item.knockBack = 2f;
			Item.width = Item.height = 24;
			Item.value = Item.sellPrice(silver: 18);
			Item.rare = ItemRarityID.White;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = Item.useAnimation = 20;
			Item.DamageType = DamageClass.Melee;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<BambooHalberdProj>();
			Item.shootSpeed = 2f;
			Item.UseSound = SoundID.Item1;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) => velocity = velocity.RotatedByRandom(.1f);

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.BambooBlock, 20);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}