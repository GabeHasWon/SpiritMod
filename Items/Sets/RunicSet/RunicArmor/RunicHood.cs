using SpiritMod.Items.Sets.SpiritSet;
using SpiritMod.Items.Sets.RunicSet;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Projectiles;
using Microsoft.Xna.Framework;
using SpiritMod.NPCs.DarkfeatherMage.Projectiles;

namespace SpiritMod.Items.Sets.RunicSet.RunicArmor
{
	[AutoloadEquip(EquipType.Head)]
	public class RunicHood : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Runic Hood");
			// Tooltip.SetDefault("Increases magic damage by 12% and movement speed by 5%");
		}

		public override void SetDefaults()
		{
			Item.width = 34;
			Item.height = 30;
			Item.value = 70000;
			Item.rare = ItemRarityID.Pink;
			Item.defense = 12;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<RunicPlate>() && legs.type == ModContent.ItemType<RunicGreaves>();
		}
		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = "Leave behind dangerous explosive runes";
			player.GetSpiritPlayer().runicSet = true;
		}

		public override void UpdateEquip(Player player)
		{
			player.GetDamage(DamageClass.Magic) *= 1.12f;
			player.moveSpeed += 1.05f;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<Rune>(), 8);
			recipe.AddIngredient(ModContent.ItemType<SoulShred>(), 4);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}

		public static void SpawnRunicRunes(Player player)
		{
			if (!Main.rand.NextBool(15))
				return;

			int spawnProj = ModContent.ProjectileType<RunicRune>();
			int runeAmount = player.ownedProjectileCounts[spawnProj];

			if (Main.rand.Next(15) < runeAmount || runeAmount >= 10)
				return;

			int dimension = 24;
			int dimension2 = 90;

			for (int j = 0; j < 50; j++)
			{
				int randValue = Main.rand.Next(200 - j * 2, 400 + j * 2);
				Vector2 center = player.Center;
				center.X += Main.rand.Next(-randValue, randValue + 1);
				center.Y += Main.rand.Next(-randValue, randValue + 1);

				if (!Collision.SolidCollision(center, dimension, dimension) && !Collision.WetCollision(center, dimension, dimension))
				{
					center -= new Vector2(dimension / 2);

					if (Collision.CanHit(new Vector2(player.Center.X, player.position.Y), 1, 1, center, 1, 1) || Collision.CanHit(new Vector2(player.Center.X, player.position.Y - 50f), 1, 1, center, 1, 1))
					{
						int x = (int)center.X / 16;
						int y = (int)center.Y / 16;

						bool flag = false;
						if (Main.rand.NextBool(4) && Main.tile[x, y] != null && Main.tile[x, y].WallType > 0)
							flag = true;
						else
						{
							center -= new Vector2(dimension2 / 2);

							if (Collision.SolidCollision(center, dimension2, dimension2))
							{
								center.X += dimension2 / 2;
								center.Y += dimension2 / 2;
								flag = true;
							}
						}

						if (flag)
						{
							foreach (Projectile proj in Main.projectile)
							{
								if (proj.active && proj.owner == player.whoAmI && proj.type == spawnProj && center.Distance(proj.Center) < 48f)
								{
									flag = false;
									break;
								}
							}

							if (flag && Main.myPlayer == player.whoAmI)
							{
								Projectile.NewProjectile(player.GetSource_FromThis(), center.X, center.Y, 0f, 0f, ModContent.ProjectileType<RunicRune>(), 40, 1.5f, player.whoAmI, 0f, 0f);
								return;
							}
						}
					}
				}
			}
		}
	}
}