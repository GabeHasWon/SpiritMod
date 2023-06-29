using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Weapon.Magic
{
	public class CactusStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			//DisplayName.SetDefault("Cactus Staff");
			//Tooltip.SetDefault("Summons cactus walls to protect you");
		}

		public override void SetDefaults()
		{
			Item.damage = 6;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 7;
			Item.width = 42;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.knockBack = 4f;
			Item.value = 200;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item8;
			Item.shoot = ModContent.ProjectileType<CactusWallProj>();
			Item.shootSpeed = 8f;
			Item.autoReuse = false;
		}

		public override bool CanUseItem(Player player) => !Collision.SolidCollision(Main.MouseWorld, 16, 72);

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			velocity = Vector2.Zero;
			position = Main.MouseWorld.ToTileCoordinates().ToWorldCoordinates(8, 4);

			while (!WorldGen.SolidTile(position.ToTileCoordinates()))
				position.Y += 8;

			position.Y -= 32;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.ownedProjectileCounts[Item.shoot] >= 1)
			{
				for (int i = 0; i < Main.maxProjectiles; ++i)
				{
					Projectile p = Main.projectile[i];

					if (p.active && p.owner == player.whoAmI && p.type == Item.shoot)
						p.Kill();
				}
			}

			return true;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.Cactus, 22);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
