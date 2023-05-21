using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles.Magic;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.MarbleSet
{
	public class MarbleStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gilded Tome");
			Tooltip.SetDefault("Rains down gilded stalactites from the sky");
		}

		public override void SetDefaults()
		{
			Item.damage = 30;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 8;
			Item.width = Item.height = 50;
			Item.useTime = 17;
			Item.useAnimation = 34;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.knockBack = 1;
			Item.useTurn = false;
			Item.value = Item.sellPrice(0, 0, 50, 0);
			Item.rare = ItemRarityID.Green;
			Item.autoReuse = true;
			Item.channel = true;
			Item.shoot = ModContent.ProjectileType<MarbleStalactite>();
			Item.shootSpeed = 10f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (Main.myPlayer == player.whoAmI)
			{
				position = new Vector2(Main.MouseWorld.X + (15 * Main.rand.NextFloat(-1.0f, 1.0f)), player.Center.Y - (150 + (100 * Main.rand.NextFloat())));
				velocity = (Vector2.UnitY * Main.rand.Next(12, 18)).RotatedByRandom(.15f);

				Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
				Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<MarblePortal>(), damage, knockback, player.whoAmI);
			}

			if (player.ItemAnimationJustStarted)
			{
				int heldType = ModContent.ProjectileType<MarbleStaffProj>();

				if (player.ownedProjectileCounts[heldType] < 1)
					Projectile.NewProjectile(source, player.MountedCenter, Vector2.Zero, heldType, damage, knockback, player.whoAmI);
			}

			return false;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<MarbleChunk>(), 13);
			recipe.AddIngredient(ItemID.Book, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}

	public class MarbleStaffProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gilded Tome");
			Main.projFrames[Type] = 13;
		}

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(20);
			Projectile.penetrate = -1;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
		}

		public Player Owner => Main.player[Projectile.owner];

		public override void OnSpawn(IEntitySource source)
		{
			if (!Main.dedServ)
				SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
		}

		public override void AI()
		{
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, new Vector2(Owner.direction * 18, -6 * Owner.gravDir), 0.1f);

			Owner.itemRotation = MathHelper.WrapAngle(Projectile.velocity.ToRotation() + (Projectile.direction < 0 ? MathHelper.Pi : 0));
			Owner.heldProj = Projectile.whoAmI;

			Projectile.direction = Projectile.spriteDirection = Owner.direction;
			Projectile.rotation = 0f;
			Projectile.Center = Owner.MountedCenter + Projectile.velocity;

			if (++Projectile.frameCounter >= 4)
			{
				Projectile.frameCounter = 0;

				if (++Projectile.frame >= Main.projFrames[Type])
					Projectile.frame = 8;
			}
			if (Main.rand.NextBool(10))
			{
				Dust.NewDustPerfect(Projectile.Center + (Main.rand.NextVector2Unit() * Main.rand.NextFloat() * 18) + (Vector2.UnitX * 8 * Projectile.direction), DustID.FireworkFountain_Yellow, Vector2.UnitY * -1.5f, 0, default, .5f).noGravity = true;
			}

			if (Owner.channel || Owner.itemAnimation > (Owner.itemAnimationMax / 2))
				Projectile.timeLeft = 2;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle drawFrame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
			SpriteEffects effects = (Projectile.spriteDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			for (int i = 3; i > 0; i--)
			{
				Color drawColor = (i == 1) ? lightColor : (Color.Goldenrod * .5f) with { A = 0 };
				float scale = Projectile.scale + ((float)Math.Sin(Main.timeForVisualEffects / 30) * .2f * (i - 1));

				Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, drawFrame, Projectile.GetAlpha(drawColor), Projectile.rotation, drawFrame.Size() / 2, scale, effects, 0);
			}

			return false;
		}

		public override bool? CanDamage() => false;
	}
}
