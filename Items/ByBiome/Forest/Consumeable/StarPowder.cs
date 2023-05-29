using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using SpiritMod.Tiles.Block;
using Terraria.DataStructures;

namespace SpiritMod.Items.ByBiome.Forest.Consumeable
{
	internal class StarPowder : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Starpowder");
			Tooltip.SetDefault("Throw onto grass to invigorate it with starlight!");
		}

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 26;
			Item.rare = ItemRarityID.White;
			Item.maxStack = 99;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.noMelee = true;
			Item.consumable = true;
			Item.autoReuse = false;
			Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<StarPowderProj>();
			Item.shootSpeed = 6f;
		}

		public override void AddRecipes()
		{
			CreateRecipe(5)
				.AddIngredient(ItemID.FallenStar, 1)
				.Register();
		}
	}

	internal class StarPowderProj : ModProjectile
	{
		public override string Texture => base.Texture[..^"Proj".Length];

		public override void SetDefaults() => Projectile.CloneDefaults(ProjectileID.PurificationPowder);

		public override void OnSpawn(IEntitySource source)
		{
			for (int i = 0; i < 20; i++)
			{
				Vector2 rectDims = new Vector2(50, 50);
				Vector2 position = new Vector2(Projectile.Center.X - (rectDims.X / 2), Projectile.Center.Y - (rectDims.Y / 2)) + (Projectile.velocity * 2);
				Vector2 velocity = (new Vector2(Projectile.velocity.X, Projectile.velocity.Y) * Main.rand.NextFloat(0.8f, 1.2f)).RotatedByRandom(1f);
				Dust dust = Dust.NewDustDirect(position, (int)rectDims.X, (int)rectDims.Y, Main.rand.NextBool(2) ? DustID.BlueTorch : DustID.PurificationPowder, 
					velocity.X, velocity.Y, 0, default, Main.rand.NextFloat(0.7f, 1.1f));
				dust.noGravity = true;
				dust.fadeIn = 1.1f;
				if (dust.type == DustID.PurificationPowder && Main.rand.NextBool(2))
					dust.color = Color.Goldenrod;
			}
		}

		public override void AI() => ConvertTiles();

		private void ConvertTiles()
		{
			Point pos = Projectile.position.ToTileCoordinates();
			Point end = Projectile.BottomRight.ToTileCoordinates();

			for (int i = pos.X; i < end.X; ++i)
			{
				for (int j = pos.Y; j < end.Y; ++j)
				{
					if (!WorldGen.InWorld(i, j))
						continue;

					Tile tile = Main.tile[i, j];

					if (tile.TileType == TileID.Grass)
						tile.TileType = (ushort)ModContent.TileType<Stargrass>();
				}
			}
		}
	}
}
