using Microsoft.Xna.Framework;
using SpiritMod.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.DonatorItems.FrostTroll
{
	public class BlizzardEdge : ModItem
	{
		private readonly Color Blue = new(0, 114, 201);
		private readonly Color White = new(255, 255, 255);
		int counter = 5;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blizzard's Edge");
			Tooltip.SetDefault("Right-click after five swings to summon a blizzard");
			Item.staff[Type] = true; //This will only take effect when using right-click
		}

		public override void SetDefaults()
		{
			Item.damage = 52;
			Item.useTime = 24;
			Item.useAnimation = 24;
			Item.DamageType = DamageClass.Melee;
			Item.width = 40;
			Item.height = 50;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5;
			Item.value = 96700;
			Item.rare = ItemRarityID.LightPurple;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.crit = 6;
			Item.shootSpeed = 1f;
			Item.shoot = ModContent.ProjectileType<BlizzardProjectile>();
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
		{
			if (Main.rand.NextBool(4))
				target.AddBuff(BuffID.Frostburn, 400, true);
		}

		public override bool AltFunctionUse(Player player) => counter <= 0;

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			counter--;

			if (player.altFunctionUse == 2 && player.whoAmI == Main.myPlayer)
			{
				DrawDust(player);

				player.GetModPlayer<MyPlayer>().Shake += 8;
				SoundEngine.PlaySound(SoundID.Item45);

				if (Main.netMode != NetmodeID.Server)
					SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/BlizzardLoop") with { Volume = 0.65f, PitchVariance = 0.54f }, player.Center);

				int p = Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<BlizzardProjectile>(), damage / 3, knockback / 4, player.whoAmI);

				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p);

				counter = 5;
			}

			if (counter == 0 && Main.netMode != NetmodeID.Server)
			{
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/MagicCast1") with { Volume = 0.5f, PitchVariance = 0.54f }, player.Center);
				SoundEngine.PlaySound(SoundID.Item46);
			}
			return false;
		}

		public void DrawDust(Player player)
		{
			float cosRot = (float)Math.Cos(player.itemRotation * player.direction * player.gravDir);
			float sinRot = (float)Math.Sin(player.itemRotation * player.direction * player.gravDir);

			if (!Main.dedServ)
			{
				for (int i = 0; i < 13; i++)
				{
					float length = (Item.width * 1.2f - i * Item.width / 9) * Item.scale + 32;
					ParticleHandler.SpawnParticle(new FireParticle(new Vector2((float)(player.itemLocation.X + length * cosRot * player.direction), (float)(player.itemLocation.Y + length * sinRot)), new Vector2(0, Main.rand.NextFloat(-.8f, -.2f)), Blue, White, Main.rand.NextFloat(0.15f, 0.45f), 30));
				}
			}
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				if (counter > 0)
					return false;

				Item.useStyle = ItemUseStyleID.Shoot;
			}
			else
			{
				Item.useStyle = ItemUseStyleID.Swing;
			}
			return true;
		}
	}

	class BlizzardProjectile : ModProjectile
	{
		private readonly Color Blue = new(0, 114, 201);
		private readonly Color White = new(255, 255, 255);

		public ref float Timer => ref Projectile.ai[0];
		private readonly int timerMax = 400;

		public override string Texture => SpiritMod.EMPTY_TEXTURE;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Blizzard");

		public override void SetDefaults()
		{
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = 25;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = false;
			Projectile.width = 60;
			Projectile.height = 60;
			Projectile.alpha = 255;
		}

		public override void AI()
		{
			Projectile.position -= Projectile.velocity;
			Projectile.rotation = Projectile.velocity.ToRotation();
			Vector2 dirFactor = Vector2.Normalize(Projectile.velocity);

			if (Timer < timerMax)
				Timer += timerMax / 20;

			Vector2 position = Projectile.Center + (Vector2.UnitY * Main.rand.Next(-25, 25)).RotatedBy(Projectile.rotation);

			float scale = Main.rand.NextFloat(1.125f, 1.775f);
			Dust.NewDustPerfect(position - (Vector2.UnitY * 18 * scale), ModContent.DustType<Dusts.BlizzardDust>(), dirFactor * Main.rand.NextFloat(13f, 16f), 100, new Color(), Main.rand.NextFloat(1.125f, 1.775f));

			if (Main.rand.NextBool(3))
			{
				ParticleHandler.SpawnParticle(new SmokeParticle(position, new Vector2(Main.rand.NextFloat(-1.5f, 1.5f)), Color.Lerp(Color.LightBlue * .8f, White * .8f, Projectile.timeLeft / 120f), Main.rand.NextFloat(0.55f, 0.75f), 30, delegate (Particle p)
				{
					p.Velocity *= 0.93f;
					p.Velocity = dirFactor * Main.rand.NextFloat(11f, 14f);
				}));
			}
			if (Main.rand.NextBool(5))
			{
				ParticleHandler.SpawnParticle(new SnowflakeParticle(position, new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-4.5f, -2.5f)), Blue, White, Main.rand.NextFloat(.45f, .95f), 45, .5f, Main.rand.Next(3), delegate (Particle p)
				{
					p.Velocity *= 0.93f;
					p.Velocity = dirFactor * Main.rand.NextFloat(12f, 15f);
				}));
			}
			ParticleHandler.SpawnParticle(new FireParticle(position, new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-4.5f, -2.5f)), Blue, White, Main.rand.NextFloat(0.15f, 0.45f), 30, delegate (Particle p)
			{
				p.Velocity *= 0.93f;
				p.Velocity = dirFactor * Main.rand.NextFloat(13f, 16f);
			}));
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Vector2 dirFactor = Vector2.Normalize(Projectile.velocity);

			if (Main.rand.NextBool(3))
				target.AddBuff(BuffID.Frostburn, 240);

			for (int i = 0; i < 2; i++)
			{
				ParticleHandler.SpawnParticle(new ImpactLine(target.Center, (dirFactor * .65f).RotatedByRandom(.5f), Color.Lerp(White, Blue, Main.rand.NextFloat()), new Vector2(0.25f, Main.rand.NextFloat(0.4f, .55f) * 1.5f), 70) 
				{ TimeActive = 30 });
			}
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			Vector2 dirFactor = Vector2.Normalize(Projectile.velocity);
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + (dirFactor * Timer));
		}
	}
}