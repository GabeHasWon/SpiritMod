using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.DonatorItems;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.DonatorItems
{
	class DuskfeatherDagger : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Duskfeather Dagger");
			// Tooltip.SetDefault("Can throw up to eight Duskfeather blades\nRight-click to recall all deployed blades");
		}

		public override void SetDefaults()
		{
			Item.width = 42;
			Item.height = 42;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Orange;
			Item.damage = 24;
			Item.crit = 16;
			Item.knockBack = 3f;
			Item.DamageType = DamageClass.Ranged;
			Item.autoReuse = true;
			Item.shootSpeed = 16f;
			Item.UseSound = SoundID.Item1;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.useTime = 18;
			Item.useAnimation = 18;
			Item.shoot = ModContent.ProjectileType<DuskfeatherBlade>();
		}

		public override bool AltFunctionUse(Player player) => true;

		public override bool CanUseItem(Player player)
		{
			//Don't put this line into SetDefaults, or the item will break.
			Item.shoot = ModContent.ProjectileType<DuskfeatherBlade>();

			if (player.altFunctionUse == 2)
			{
				if (Item.useStyle == ItemUseStyleID.Swing)
				{
					Item.useStyle = ItemUseStyleID.HoldUp;
					Item.UseSound = null;
				}
				else return false;
			}
			else
			{
				Item.useStyle = ItemUseStyleID.Swing;
				Item.UseSound = SoundID.Item1;
			}
			return true;
		}

		public override bool? UseItem(Player player)
		{
			if (player.altFunctionUse == 2)
				DuskfeatherBlade.AttractBlades(player);
			else if (player.ownedProjectileCounts[Item.shoot] >= 8)
				DuskfeatherBlade.AttractOldestBlade(player);

			return base.UseItem(player);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
			=> player.altFunctionUse != 2;

		public override void AddRecipes()
		{
			var recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Muramasa);
			recipe.AddIngredient(ItemID.Feather, 8);
			recipe.AddIngredient(ItemID.FossilOre, 25);
			recipe.AddIngredient(ItemID.Amber, 8);
			recipe.AddTile(TileID.DemonAltar);
			recipe.Register();
		}
	}
}