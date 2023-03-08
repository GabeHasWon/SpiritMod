using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.BismiteSet
{
	public class BismiteStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bismite Staff");
			Tooltip.SetDefault("Shoots a bolt of energy\nOccasionally causes foes to receive 'Festering Wounds,' which deal more damage to foes under half health");
			Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 10;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 6;
			Item.width = 34;
			Item.height = 34;
			Item.useTime = 24;
			Item.useAnimation = 24;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 0;
			Item.value = Item.sellPrice(0, 0, 20, 0);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item20;
			Item.autoReuse = false;
			Item.shoot = ModContent.ProjectileType<BismiteShard>();
			Item.shootSpeed = 8f;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * Item.Size.Length();
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
				position += muzzleOffset;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<BismiteCrystal>(), 8);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}