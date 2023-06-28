using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GamblerChestLoot.FunnyFirework
{
	public class FunnyFirework : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Funny Firework");
			// Tooltip.SetDefault("Does a funny");
		}

		public override void SetDefaults()
		{
			Item.width = 44;
			Item.height = 40;
			Item.useTime = 18;
			Item.useAnimation = 18;
			Item.noUseGraphic = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 0;
			Item.value = Item.sellPrice(0, 0, 0, 0);
			Item.rare = ItemRarityID.Blue;
			Item.shootSpeed = 10f;
			Item.shoot = ModContent.ProjectileType<FunnyFireworkProj>();
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.maxStack = 999;
			Item.noMelee = true; 
		}
	}

	public class FunnyFireworkProj : ModProjectile
	{
		private const int timeLeftMax = 90;

		//public override void SetStaticDefaults() => DisplayName.SetDefault("Funny Firework");

		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.friendly = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.damage = 10; 
			Projectile.penetrate = 1;
			Projectile.timeLeft = timeLeftMax;
		}

		public override void AI()
		{
			if (Projectile.timeLeft <= (timeLeftMax / 2))
			{
				if (Projectile.timeLeft == (timeLeftMax / 2) && Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.ai[0] = Main.rand.NextFloat(-3.0f, 3.0f);
					Projectile.netUpdate = true;
				}

				Projectile.velocity = Projectile.velocity.RotatedBy(.02f * Projectile.ai[0]);
			}
			else
				Projectile.velocity = Projectile.velocity.RotatedByRandom(Math.Sin(Main.GlobalTimeWrappedHourly / 30) * (1f - ((float)Projectile.timeLeft / timeLeftMax)) * .5f);
			
			Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

			if (Main.rand.NextBool(3))
				Dust.NewDustPerfect(Projectile.Center - Projectile.velocity, DustID.Torch, null, 0, default, Main.rand.NextFloat(.5f, 2.5f)).noGravity = true;
		}

		public override void Kill(int timeLeft)
		{
			string dustImage = "SpiritMod/Effects/DustImages/" + Main.rand.Next(4) switch
			{
				1 => "GladeFirework",
				2 => "TrollFirework",
				3 => "AmogusFirework",
				_ => "GarfieldFirework"
			};

			DustHelper.DrawDustImageRainbow(Projectile.Center, 0.125f, dustImage, 1f);
			SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
		}
	}
}