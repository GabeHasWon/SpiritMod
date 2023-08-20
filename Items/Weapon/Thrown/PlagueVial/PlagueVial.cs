using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Weapon.Thrown.PlagueVial
{
	public class PlagueVial : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Plague Vial");
			/* Tooltip.SetDefault("A noxious mixture of flammable toxins\n" +
				"Explodes into cursed embers upon hitting foes\n" +
				"'We could make a class out of this!'"); */
		}

		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.width = 16;
			Item.height = 16;
			Item.noUseGraphic = true;
			Item.UseSound = SoundID.Item106;
			Item.DamageType = DamageClass.Ranged;
			Item.channel = true;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<PlagueVialProj>();
			Item.useAnimation = 24;
			Item.useTime = 24;
			Item.consumable = true;
			Item.maxStack = 999;
			Item.shootSpeed = 10.0f;
			Item.damage = 25;
			Item.knockBack = 4.5f;
			Item.value = Item.sellPrice(0, 0, 1, 0);
			Item.rare = ItemRarityID.Green;
			Item.autoReuse = false;
			Item.consumable = true;
		}
	}

	public class PlagueVialProj : ModProjectile
	{
		public override string Texture => "SpiritMod/Items/Weapon/Thrown/PlagueVial/PlagueVial";

		// public override void SetStaticDefaults() => DisplayName.SetDefault("Plague Vial");

		public override void SetDefaults()
		{
			Projectile.width = Projectile.height = 16;
			Projectile.aiStyle = 2;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
		}

		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item107, Projectile.Center);

			Projectile proj = Projectile.NewProjectileDirect(Entity.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PlagueVialExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
			proj.position.Y -= (proj.height / 2) - 20;

			for (int i = 0; i < 30; i++)
			{
				float magnitude = Main.rand.NextFloat();
				Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, (Vector2.UnitY * magnitude * -3).RotatedByRandom(1.57f), 0, default, magnitude + 1.5f).noGravity = true;
			}
		}
	}

	public class PlagueVialExplosion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Plague Vial");
			Main.projFrames[Projectile.type] = 5;
		}

		public override void SetDefaults()
		{
			Projectile.width = 92;
			Projectile.height = 114;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;

			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
		}

		public override Color? GetAlpha(Color lightColor) => Color.White;

		public override void AI()
		{
			if (++Projectile.frameCounter >= 5)
			{
				Projectile.frameCounter = 0;

				if (++Projectile.frame >= Main.projFrames[Type])
					Projectile.Kill();
			}
		}
	}
}