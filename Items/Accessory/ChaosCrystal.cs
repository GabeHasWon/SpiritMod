using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory
{
	public class ChaosCrystal : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Chaos Crystal");
			// Tooltip.SetDefault("Getting hit has a chance to teleport you to somewhere nearby");
		}

		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.buyPrice(0, 5, 0, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) => player.GetSpiritPlayer().chaosCrystal = true;

		public static Vector2 TestTeleport(ref bool canSpawn, int teleportStartX, int teleportRangeX, int teleportStartY, int teleportRangeY)
		{
			Player player = Main.LocalPlayer;

			int repeats = 0;
			int num2 = 0;
			int num3 = 0;

			Vector2 Position = new Vector2(num2, num3) * 16f + new Vector2(-player.width / 2 + 8, -player.height);
			while (!canSpawn && repeats < 1000)
			{
				++repeats;

				int index1 = teleportStartX + Main.rand.Next(teleportRangeX);
				int index2 = teleportStartY + Main.rand.Next(teleportRangeY);
				Position = new Vector2(index1, index2) * 16f + new Vector2(-player.width / 2 + 8, -player.height);

				if (!Collision.SolidCollision(Position, player.width, player.height))
				{
					if ((Main.tile[index1, index2].WallType != 87 || index2 <= Main.worldSurface || NPC.downedPlantBoss) && (!Main.wallDungeon[Main.tile[index1, index2].WallType] || index2 <= Main.worldSurface || NPC.downedBoss3))
					{
						int num4 = 0;
						while (num4 < 100)
						{
							Tile tile = Main.tile[index1, index2 + num4];
							Position = new Vector2(index1, index2 + num4) * 16f + new Vector2(-player.width / 2 + 8, -player.height);
							Collision.SlopeCollision(Position, player.velocity, player.width, player.height, player.gravDir, false);

							bool flag = !Collision.SolidCollision(Position, player.width, player.height);

							if (flag)
								++num4;
							else if (!tile.HasTile || tile.IsActuated || !Main.tileSolid[tile.TileType])
								++num4;
							else
								break;
						}
						if (!Collision.LavaCollision(Position, player.width, player.height) && Collision.HurtTiles(Position, player.width, player.height, player).y <= 0.0)
						{
							Collision.SlopeCollision(Position, player.velocity, player.width, player.height, player.gravDir, false);
							if (Collision.SolidCollision(Position, player.width, player.height) && num4 < 99)
							{
								Vector2 Velocity1 = Vector2.UnitX * 16f;
								if (!(Collision.TileCollision(Position - Velocity1, Velocity1, player.width, player.height, false, false, (int)player.gravDir) != Velocity1))
								{
									Vector2 Velocity2 = -Vector2.UnitX * 16f;
									if (!(Collision.TileCollision(Position - Velocity2, Velocity2, player.width, player.height, false, false, (int)player.gravDir) != Velocity2))
									{
										Vector2 Velocity3 = Vector2.UnitY * 16f;
										if (!(Collision.TileCollision(Position - Velocity3, Velocity3, player.width, player.height, false, false, (int)player.gravDir) != Velocity3))
										{
											Vector2 Velocity4 = -Vector2.UnitY * 16f;
											if (!(Collision.TileCollision(Position - Velocity4, Velocity4, player.width, player.height, false, false, (int)player.gravDir) != Velocity4))
											{
												canSpawn = true;
												break;
											}
										}
									}
								}
							}
						}
					}
				}
			}

			return Position;
		}
	}
}
