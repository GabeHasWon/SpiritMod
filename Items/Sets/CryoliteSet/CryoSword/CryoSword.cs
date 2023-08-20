using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.CryoliteSet.CryoSword
{
	public class CryoSword : ModItem
	{
		private int comboCounter;
		private readonly int maxCombo = 3;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Rimehowl");
			// Tooltip.SetDefault("Every third swing creates a cryonic wave, which inflicts 'Cryo Crush'\nCryo Crush deals increased damage to weakened enemies");
		}

		public override void SetDefaults()
		{
			Item.damage = 40;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Rapier;
			Item.knockBack = 5.5f;
			Item.value = Item.sellPrice(0, 0, 70, 0);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<CryoSwordProj>();
			Item.shootSpeed = 4f;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.06f, .16f, .22f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			if ((comboCounter + 1) == maxCombo)
			{
				bool tileBelow = false;
				for (int i = 0; i < 6; i++)
				{
					Point16 samplePos = (position / 16 + (Vector2.UnitY * i)).ToPoint16();

					if (WorldGen.SolidOrSlopedTile(Main.tile[samplePos.X, samplePos.Y]))
					{
						position = (samplePos.ToVector2() * 16) - new Vector2(-8, 18);
						tileBelow = true;
						break;
					}
				}

				if (player.velocity.Y == 0f || tileBelow)
				{
					velocity.Y = 0;

					for (int i = 0; i < 20; i++)
					{
						Dust.NewDustPerfect(position, Main.rand.NextBool(2) ? DustID.GemDiamond : DustID.DungeonSpirit, (velocity * Main.rand.NextFloat(0.2f, 2.0f)).RotatedByRandom(0.9f), 80);

						if (i < 10 && !Main.dedServ)
						{
							if (i < 5)
								ParticleHandler.SpawnParticle(new SmokeParticle(position + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.0f, 5.0f)), (velocity * Main.rand.NextFloat(1.0f, 2.2f)).RotatedByRandom(0.5f),
									Color.Lerp(Color.White, Color.LightBlue, Main.rand.NextFloat(1.0f)), Main.rand.NextFloat(0.55f, 1.25f), 30));

							ParticleHandler.SpawnParticle(new FireParticle(position, (velocity * Main.rand.NextFloat(1.0f, 1.8f)).RotatedByRandom(0.5f), Color.White, Color.Blue, Main.rand.NextFloat(0.15f, 0.45f), 30));
						}
					}

					Projectile.NewProjectile(player.GetSource_ItemUse(Item), position - new Vector2(0, 20) + (Vector2.UnitX * 50 * player.direction), velocity, ModContent.ProjectileType<IceWall>(), damage * 2, knockback * 2, player.whoAmI);

					if (Main.netMode != NetmodeID.Server)
						SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, position);
				}
				else
				{
					int loops = 20;
					for (int i = 0; i < 20; i++)
					{
						Vector2 newPos = position + (Vector2.UnitX * velocity.Length()).RotatedBy((1.57f - ((float)((float)i / loops) * 3.14f)) * player.direction);
						Vector2 newVel = velocity.RotatedBy(-1.57f * player.direction);

						Dust.NewDustPerfect(newPos, Main.rand.NextBool(2) ? DustID.GemDiamond : DustID.DungeonSpirit, (newVel * Main.rand.NextFloat(0.2f, 2.0f)).RotatedByRandom(0.35f), 80);

						if (i < 10 && !Main.dedServ)
						{
							if (i < 5)
								ParticleHandler.SpawnParticle(new SmokeParticle(newPos + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.0f, 5.0f)), (newVel * Main.rand.NextFloat(1.0f, 2.2f)).RotatedByRandom(0.2f),
									Color.Lerp(Color.White, Color.LightBlue, Main.rand.NextFloat(1.0f)), Main.rand.NextFloat(0.55f, 1.25f), 30));

							ParticleHandler.SpawnParticle(new FireParticle(newPos, (newVel * Main.rand.NextFloat(1.0f, 1.8f)).RotatedByRandom(0.2f), Color.White, Color.Blue, Main.rand.NextFloat(0.15f, 0.45f), 30));
						}
					}

					velocity.Y = 0;

					int numLoops = 5;
					for (int i = 0; i < numLoops; i++)
					{
						float increment = 5;
						Vector2 newVel = velocity.RotatedBy(MathHelper.ToRadians(-(increment * (numLoops / 2f)) + (float)((float)increment * i)));

						Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), position + (newVel * 10) + player.velocity, newVel, ModContent.ProjectileType<IceWave>(), damage * 2, knockback * 2, player.whoAmI);
						proj.frame = i;
						proj.ai[1] = (i == 2) ? 0 : 1; //This determines whether the projectile deals damage
						proj.netUpdate = true;
					}

					if (Main.netMode != NetmodeID.Server)
						SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss, position);
				}
			}

			velocity = new Vector2((comboCounter % 2 == 1) ? -1 : 1, 0);
			if ((comboCounter + 1) == maxCombo)
				velocity *= 2;

			comboCounter = ++comboCounter % maxCombo;
		}

		public override float UseSpeedMultiplier(Player player) => ((comboCounter + 1) == maxCombo) ? 0.85f : 1f;

		public override void AddRecipes()
		{
			Recipe modRecipe = CreateRecipe();
			modRecipe.AddIngredient(ModContent.ItemType<CryoliteBar>(), 15);
			modRecipe.AddTile(TileID.Anvils);
			modRecipe.Register();
		}
	}
}