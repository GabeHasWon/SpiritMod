using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.AvianDrops
{
	public class TalonPiercer : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Talon's Fury");
			Tooltip.SetDefault("Creates a flurry of feathers");
			Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.damage = 25;
			Item.DamageType = DamageClass.Magic;
			Item.mana = 10;
			Item.width = Item.height = 46;
			Item.useTime = 12;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.knockBack = 3.5f;
			Item.useTurn = true;
			Item.channel = true;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item20;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<GhostFeather>();
			Item.shootSpeed = 10f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			int heldProj = ModContent.ProjectileType<TalonPiercerProj>();
			if (player.ownedProjectileCounts[heldProj] < 1)
				Projectile.NewProjectile(Entity.GetSource_ItemUse(Item), position, Vector2.Zero, heldProj, damage, knockback, player.whoAmI);

			float randomRotation = Main.rand.NextFloat(-0.2f, 0.2f);
			velocity *= Main.rand.NextFloat(0.8f, 1.0f);
			position -= (Vector2.Normalize(velocity) * 800).RotatedBy(randomRotation);

			Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, randomRotation * -0.01f);
			return false;
		}
	}

	public class TalonPiercerProj : ModProjectile
	{
		private float Opacity
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private readonly int holdoutLength = 46;

		public override string Texture => "SpiritMod/Items/BossLoot/AvianDrops/TalonPiercer";

		public override void SetStaticDefaults() => DisplayName.SetDefault("Talon's Fury");

		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.aiStyle = -1;
			Projectile.timeLeft = 2;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
		}

		public override void AI()
		{
			Opacity = MathHelper.Min(1, Opacity + 0.05f);

			Player player = Main.player[Projectile.owner];
			player.heldProj = Projectile.whoAmI;
			Projectile.rotation = (-1.57f + (.2f * player.direction)) * player.gravDir;

			Projectile.Center = player.MountedCenter + (Vector2.UnitX * holdoutLength).RotatedBy(Projectile.rotation);

			if (player.channel)
			{
				player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, -1.57f * player.direction);
				player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, -1.57f * player.direction);

				Projectile.timeLeft = 2;
			}

			Projectile.alpha = player.channel ? 0 : 255;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Player player = Main.player[Projectile.owner];
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			SpriteEffects effects = (player.direction * player.gravDir < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			float rotation = Projectile.rotation + ((effects == SpriteEffects.FlipHorizontally) ? 2.355f : 0.785f);
			Vector2 origin = (effects == SpriteEffects.None) ? new Vector2(texture.Width - (Projectile.width / 2), Projectile.height / 2) : Projectile.Size / 2;

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), rotation, origin, Projectile.scale, effects, 0);

			Color glowColor = Color.White * Opacity;
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(glowColor), rotation, origin, Projectile.scale, effects, 0);

			return false;
		}

		public override bool? CanDamage() => false;

		public override bool? CanCutTiles() => false;
	}
}
