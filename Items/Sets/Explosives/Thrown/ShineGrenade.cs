using Microsoft.Xna.Framework;
using SpiritMod.Projectiles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.Explosives.Thrown
{
	//[ItemTag(ItemTags.Explosive)]
	public class ShineGrenade : ModItem
	{
		public override bool IsLoadingEnabled(Mod mod) => false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shine Grenade");
			Tooltip.SetDefault("Confuses enemies on detonation");
		}

		public override void SetDefaults()
		{
			Item.damage = 45;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Throwing;
			Item.width = 18;
			Item.height = 20;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.shoot = ModContent.ProjectileType<ShineGrenadeProj>();
			Item.knockBack = 4;
			Item.useTurn = false;
			Item.value = Item.sellPrice(0, 0, 1, 0);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = false;
			Item.shootSpeed = 7.5f;
			Item.noUseGraphic = true;
			Item.consumable = true;
			Item.maxStack = 999;
		}

		public override void AddRecipes()
		{
			var recipe = CreateRecipe(5);
			recipe.AddIngredient(ItemID.Grenade, 5);
			recipe.AddIngredient(ItemID.FallenStar, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}

	public class ShineGrenadeProj : ModProjectile
	{
		public override bool IsLoadingEnabled(Mod mod) => false;

		public override string Texture => Mod.Name + "/Items/Sets/Explosives/Thrown/ShineGrenade";

		public override void SetStaticDefaults() => DisplayName.SetDefault("Shine Grenade");

		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.timeLeft = 180;
			Projectile.penetrate = -1;
		}

		public override void AI()
		{
			Projectile.rotation += 0.06f * Projectile.velocity.X;
			Projectile.velocity.Y += 0.2f;

			if (Projectile.timeLeft < 2)
			{
				const int size = 200;

				ProjectileExtras.Explode(Projectile.type, size, size, delegate
				{ 

				});
				Projectile.active = false;
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.velocity.X != oldVelocity.X)
				Projectile.velocity.X = -oldVelocity.X;

			if (Projectile.velocity.Y != oldVelocity.Y)
				Projectile.velocity.Y = -oldVelocity.Y * 0.25f;

			Projectile.velocity.X *= 0.96f;
			return false;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Projectile.timeLeft = Math.Min(Projectile.timeLeft, 2);

			target.AddBuff(BuffID.Confused, 300);
		}

		public override bool CanHitPlayer(Player target) => target.whoAmI == Projectile.owner;

		public override bool? CanHitNPC(NPC target) => target.friendly ? false : null;
	}
}
