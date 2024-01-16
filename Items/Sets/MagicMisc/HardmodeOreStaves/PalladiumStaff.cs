using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.MagicMisc.HardmodeOreStaves
{
	public class PalladiumStaff : ModItem
	{
		public override void SetStaticDefaults() => Item.staff[Type] = true;

		public override void SetDefaults()
		{
			Item.damage = 40;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 15;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 28;
			Item.useAnimation = 28;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 5;
			Item.useTurn = false;
			Item.value = Item.sellPrice(0, 3, 0, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item83;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<PalladiumStaffProj>();
			Item.shootSpeed = 8f;
		}

		public override bool CanUseItem(Player player) => !Collision.SolidCollision(Main.MouseWorld, 16, 16);

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			velocity = Vector2.Zero;
			position = Main.MouseWorld.ToTileCoordinates().ToWorldCoordinates(8, 4);

			while (!WorldGen.SolidTile(position.ToTileCoordinates()) && !Main.tileSolidTop[Framing.GetTileSafely(position.ToTileCoordinates()).TileType])
				position.Y += 8;

			position.Y -= 32;

			foreach (Projectile proj in Main.projectile)
				if (proj.active && proj.type == Item.shoot && proj.owner == player.whoAmI)
					proj.active = false;

			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
			return false;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.PalladiumBar, 12);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
