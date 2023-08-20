using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.FrigidSet.Frostbite
{
	public class HowlingScepter : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frostbite");
			// Tooltip.SetDefault("Creates a slow-moving blizzard that follows your cursor");
		}

		public override void SetDefaults()
		{
			Item.damage = 7;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Magic;
			Item.width = 64;
			Item.height = 64;
			Item.useTime = 30;
			Item.mana = 4;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0;
			Item.value = Item.sellPrice(0, 0, 5, 0);
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.channel = true;
			Item.UseSound = SoundID.Item20;
			Item.shoot = ModContent.ProjectileType<FrostbiteProj>();
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) => position = Main.MouseWorld;

		public override void AddRecipes()
		{
			Recipe modRecipe = CreateRecipe();
			modRecipe.AddIngredient(ModContent.ItemType<FrigidFragment>(), 9);
			modRecipe.AddTile(TileID.Anvils);
			modRecipe.Register();
		}
	}
}
