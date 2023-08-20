using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.StarplateDrops.StarplateGlove
{
	public class StarplateGlove : ModItem
	{
		private readonly float charge = 1.33f;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Hundred-Crack Fist");
			// Tooltip.SetDefault("Right click launch multiple punches outward");
		}

		public override void SetDefaults()
		{
			Item.shootSpeed = 10f;
			Item.damage = 31;
			Item.knockBack = 3.3f;
			Item.DamageType = DamageClass.Magic;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useAnimation = 7;
			Item.useTime = 7;
			Item.channel = true;
			Item.width = 24;
			Item.height = 30;
			Item.mana = 6;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.value = Item.sellPrice(silver: 55);
			Item.rare = ItemRarityID.Orange;
			Item.shoot = ModContent.ProjectileType<StargloveChargeOrange>();
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.38f, .22f, .14f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}

		public override bool AltFunctionUse(Player player) => true;

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse != 2)
			{
				if (player.ownedProjectileCounts[ModContent.ProjectileType<StarplateGloveProj>()] > 0)
					return false;

				Item.useTime = 7;
				Item.useAnimation = 7;
			}
			else
			{
				Item.useTime = 40;
				Item.useAnimation = 40;
			}

			return true;
		}

		public override void HoldItem(Player player)
		{
			for (int i = 0; i < 1000; ++i)
				if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == ModContent.ProjectileType<StarplateGloveProj>())
					return;

			if (charge > 0)
			{
				int chosenDust = Main.rand.NextBool(2) ? DustID.Torch : DustID.BlueTorch;
				Vector2 vector2_1 = Main.OffsetsPlayerOnhand[player.bodyFrame.Y / 56] * 2f;
				if (player.direction != 1)
					vector2_1.X = (float)player.bodyFrame.Width - vector2_1.X;
				if ((double)player.gravDir != 1.0)
					vector2_1.Y = (float)player.bodyFrame.Height - vector2_1.Y;
				Vector2 vector2_2 = player.RotatedRelativePoint(player.position + vector2_1 - new Vector2((float)(player.bodyFrame.Width - player.width), (float)(player.bodyFrame.Height - 42)) / 2f, true) - player.velocity;
				for (int index = 0; index < 4; ++index)
				{
					Dust dust = Main.dust[Dust.NewDust(player.Center, 0, 0, chosenDust, (float)(player.direction * 2), 0.0f, 150, new Color(), 1.3f)];
					dust.position = vector2_2;
					dust.velocity *= 0.0f;
					dust.noGravity = true;
					dust.fadeIn = 1f;
					dust.velocity += player.velocity;
					dust.scale *= charge;
					if (Main.rand.NextBool(2))
					{
						dust.position += Utils.RandomVector2(Main.rand, -4f, 4f);
						dust.scale += Main.rand.NextFloat();
						if (Main.rand.NextBool(2))
							dust.customData = (object)player;
					}
				}
			}
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.ownedProjectileCounts[ModContent.ProjectileType<StarplateGloveProj>()] != 0)
				return false;

			if (player.altFunctionUse == 2)
			{
				velocity = new Vector2(position.Distance(Main.MouseWorld) / 10, 0).RotatedBy(velocity.ToRotation());
				int proj = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<StarplateGloveProj>(), damage, knockback, player.whoAmI);

				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);
				return false;
			}
			else
			{
				Vector2 speed = velocity.RotatedByRandom(0.5f);
				position += speed * 8;
				type = Main.rand.NextBool(2) ? ModContent.ProjectileType<StargloveChargeOrange>() : ModContent.ProjectileType<StargloveChargePurple>();
				
				int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

				if (type == ModContent.ProjectileType<StargloveChargePurple>())
				{
					for (float num2 = 0.0f; (double)num2 < 10; ++num2)
					{
						int dustIndex = Dust.NewDust(position - speed * 3, 2, 2, DustID.Clentaminator_Cyan, 0f, 0f, 0, default, 1.5f);
						Main.dust[dustIndex].noGravity = true;
						Main.dust[dustIndex].velocity = Vector2.Normalize((speed * 5).RotatedBy(Main.rand.NextFloat(6.28f))) * 2.5f;
					}
				}
				else
				{
					for (float num2 = 0.0f; (double)num2 < 10; ++num2)
					{
						int dustIndex = Dust.NewDust(position - speed * 3, 2, 2, DustID.Torch, 0f, 0f, 0, default, 2f);
						Main.dust[dustIndex].noGravity = true;
						Main.dust[dustIndex].velocity = Vector2.Normalize((speed * 8).RotatedBy(Main.rand.NextFloat(6.28f))) * 2.5f;
					}
				}

				for (int j = 0; j < 5; j++)
					Projectile.NewProjectile(source, position, speed, type, 0, 0, player.whoAmI, proj);
			}
			return false;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<CosmiliteShard>(), 17);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}