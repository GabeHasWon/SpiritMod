using Microsoft.Xna.Framework;
using SpiritMod.NPCs.DarkfeatherMage.Projectiles;
using SpiritMod.Projectiles;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.DarkfeatherVisage
{
    [AutoloadEquip(EquipType.Head)]
    public class DarkfeatherVisage : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Darkfeather Visage");
            // Tooltip.SetDefault("Increases magic and summon damage by 7%\nGrants a bonus when worn with a magic robe or fur coat");
			ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
		}

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 20;
            Item.value = Item.sellPrice(0, 1, 6, 0);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 2;
        }

		public override bool IsArmorSet(Item head, Item body, Item legs)
			=> body.type >= ItemID.AmethystRobe && body.type <= ItemID.DiamondRobe || body.type == ItemID.GypsyRobe || body.type == ItemID.AmberRobe || body.type == ItemID.FlinxFurCoat;
		
		public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += .07f;
			player.GetDamage(DamageClass.Summon) += .07f;
        }

		public override void UpdateArmorSet(Player player)
		{
			player.setBonus = "Generates exploding darkfeather bolts around the player";
			player.GetSpiritPlayer().darkfeatherVisage = true;
		}

		public static void SpawnDarkfeatherBombs(Player player)
		{
			if (!Main.rand.NextBool(80))
				return;

			int spawnProj = ModContent.ProjectileType<DarkfeatherBomb>();
			int bombAmt = player.ownedProjectileCounts[spawnProj];

			if (Main.rand.Next(100) < bombAmt || bombAmt >= 4)
				return;

			int dimension = 24;
			int dimension2 = 90;

			for (int j = 0; j < 50; j++)
			{
				int randValue = Main.rand.Next(200 - j * 2, 350 + j * 2);
				Vector2 center = player.Center;
				center.X += Main.rand.Next(-randValue, randValue + 1);
				center.Y += Main.rand.Next(-randValue, randValue + 1);

				if (!Collision.SolidCollision(center, dimension, dimension) && !Collision.WetCollision(center, dimension, dimension))
				{
					center -= new Vector2(dimension / 2);

					if (Collision.CanHit(new Vector2(player.Center.X, player.position.Y), 1, 1, center + new Vector2(dimension / 2), 1, 1) || Collision.CanHit(new Vector2(player.Center.X, player.position.Y - 50f), 1, 1, center + new Vector2(dimension / 2), 1, 1))
					{
						int x = (int)center.X / 16;
						int y = (int)center.Y / 16;

						bool flag = false;
						if (Main.rand.NextBool(4) && Main.tile[x, y] != null && Main.tile[x, y].WallType > 0)
							flag = true;
						else
						{
							center.X -= dimension2 / 2;
							center.Y -= dimension2 / 2;

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

							if (Main.myPlayer == player.whoAmI)
							{
								Projectile proj = Projectile.NewProjectileDirect(player.GetSource_FromThis(), center - (Vector2.UnitY * Main.rand.Next(40, 90)), Vector2.Zero, spawnProj, 23, 1.5f, player.whoAmI, 0f, 0f);

								proj.hostile = false;
								proj.friendly = true;
								proj.DamageType = DamageClass.Magic;

								for (int i = 0; i < 24; i++)
								{
									Vector2 vector2 = Vector2.UnitX * -proj.width / 2f;
									vector2 += -Utils.RotatedBy(Vector2.UnitY, i * 3.141591734f / 6f, default) * new Vector2(16f, 16f);
									vector2 = Utils.RotatedBy(vector2, proj.rotation - 1.57079637f, default) * 1.3f;

									var spawnPos = new Vector2(proj.Center.X + 21 * proj.spriteDirection, proj.Center.Y + 12);

									Dust dust = Dust.NewDustDirect(spawnPos, 0, 0, DustID.ChlorophyteWeapon, 0f, 0f, 160, new Color(209, 255, 0), .86f);
									dust.shader = GameShaders.Armor.GetSecondaryShader(27, Main.LocalPlayer);
									dust.position = new Vector2(proj.Center.X + 21 * proj.spriteDirection, proj.Center.Y + 12) + vector2;
									dust.velocity = Vector2.Normalize(proj.Center - proj.velocity * 3f - dust.position) * 1.25f;
									dust.noGravity = true;
								}
								return;
							}
						}
					}
				}
			}
		}
	}
}
