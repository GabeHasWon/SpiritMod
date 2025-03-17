using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Mechanics.Trails;
using SpiritMod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.VinewrathDrops
{
	public class ThornBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Thornshot");
			// Tooltip.SetDefault("Wooden arrows occasionally split into poisonous thorns");
		}

		public override void SetDefaults()
		{
			Item.damage = 21;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 22;
			Item.height = 56;
			Item.useTime = 26;
			Item.useAnimation = 26;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ProjectileID.Shuriken;
			Item.useAmmo = AmmoID.Arrow;
			Item.knockBack = 1.5f;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item5;
			Item.value = Item.sellPrice(gold: 2, silver: 30);
			Item.autoReuse = true;
			Item.shootSpeed = 16f;
		}

		public override Vector2? HoldoutOffset() => new Vector2(-8, 0);

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			type = ModContent.ProjectileType<ThornArrow>();
		}
	}

	public class ThornArrow : ModProjectile, ITrailProjectile
	{
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.WoodenArrowFriendly;

		// public override void SetStaticDefaults() => DisplayName.SetDefault("Thorn Arrow");

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);
			AIType = ProjectileID.WoodenArrowFriendly;
		}

		public void DoTrailCreation(TrailManager tManager)
		{
			tManager.CreateTrail(Projectile, new StandardColorTrail(new Color(77, 128, 79)), new RoundCap(), new DefaultTrailPosition(), 5f, 400f, new ImageShader(Mod.Assets.Request<Texture2D>("Textures/Trails/Trail_5").Value, 0.01f, 1f, 1f));
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			{
				int n = Main.rand.Next(5, 6);
				int deviation = Main.rand.Next(0, 300);
				for (int i = 0; i < n; i++)
				{
					float rotation = MathHelper.ToRadians(270 / n * i + deviation);
					Vector2 perturbedSpeed = Vector2.Normalize(new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(rotation)) * 3.5f;
					Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center.X, Projectile.Center.Y, perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<ThornBowThorn>(), Projectile.damage / 5 * 3, 0f, Projectile.owner);
				}
			}
		}
	}
}