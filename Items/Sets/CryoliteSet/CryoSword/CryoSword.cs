using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using Terraria;
using Terraria.Audio;
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
			DisplayName.SetDefault("Rimehowl");
			Tooltip.SetDefault("Every third swing creates a cryonic wave, which inflicts 'Cryo Crush'\nCryo Crush deals increased damage to weakened enemies");
		}

		public override void SetDefaults()
		{
			Item.damage = 33;
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
				for (int i = 0; i < 7; i++)
				{
					Vector2 samplePos = position / 16;
					samplePos.Y += i;

					Tile sample = Framing.GetTileSafely(samplePos);

					if (WorldGen.SolidOrSlopedTile(sample))
					{
						tileBelow = true;
						break;
					}
				}

				if (player.velocity.Y == 0f || tileBelow)
				{
					Vector2 newPos = position + new Vector2(0, player.height / 2);
					velocity.Y *= 0.08f;

					for (int i = 0; i < 20; i++)
					{
						Dust.NewDustPerfect(position, Main.rand.NextBool(2) ? DustID.GemDiamond : DustID.DungeonSpirit, (velocity * Main.rand.NextFloat(0.2f, 2.0f)).RotatedByRandom(0.9f), 80);

						if (i < 10)
						{
							if (i < 5)
								ParticleHandler.SpawnParticle(new SmokeParticle(newPos + (Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.0f, 5.0f)), (velocity * Main.rand.NextFloat(1.0f, 2.2f)).RotatedByRandom(0.5f),
									Color.Lerp(Color.White, Color.LightBlue, Main.rand.NextFloat(1.0f)), Main.rand.NextFloat(0.55f, 1.25f), 30));

							ParticleHandler.SpawnParticle(new FireParticle(newPos, (velocity * Main.rand.NextFloat(1.0f, 1.8f)).RotatedByRandom(0.5f), Color.White, Color.Blue, Main.rand.NextFloat(0.15f, 0.45f), 30));
						}
					}

					Projectile.NewProjectile(Entity.GetSource_ItemUse(Item), position + (velocity * 5), Vector2.Normalize(velocity) * 2.8f, ModContent.ProjectileType<CryoPillar>(), damage * 2, knockback * 2, player.whoAmI);

					if (Main.netMode != NetmodeID.Server)
						SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, position);
				}
				else if (Main.netMode != NetmodeID.Server)
				{
					SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss with { Volume = 0.6f }, position);
				}
			}

			velocity = new Vector2((comboCounter % 2 == 1) ? -1 : 1, 0);
			comboCounter = ++comboCounter % maxCombo;
		}

		public override float UseSpeedMultiplier(Player player) => ((comboCounter + 1) == maxCombo) ? 0.7f : 1f;

		public override void AddRecipes()
		{
			Recipe modRecipe = CreateRecipe();
			modRecipe.AddIngredient(ModContent.ItemType<CryoliteBar>(), 15);
			modRecipe.AddTile(TileID.Anvils);
			modRecipe.Register();
		}
	}
}