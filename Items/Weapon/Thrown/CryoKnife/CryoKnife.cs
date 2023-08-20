using Microsoft.Xna.Framework;
using SpiritMod.Buffs.DoT;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Weapon.Thrown.CryoKnife
{
	public class CryoKnife : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Cryolite Bomb");
			// Tooltip.SetDefault("Occasionally inflicts 'Cryo Crush'\nCryo Crush deals increased damage to weakened enemies");
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Shuriken);
			Item.width = 30;
			Item.height = 30;
			Item.shoot = ModContent.ProjectileType<CryoKnifeProj>();
			Item.useAnimation = 55;
			Item.useTime = 55;
			Item.shootSpeed = 10f;
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 16;
			Item.autoReuse = true;
			Item.knockBack = 1f;
			Item.value = Item.buyPrice(0, 0, 1, 0);
			Item.rare = ItemRarityID.LightRed;
		}
	}

	public class CryoKnifeProj : ModProjectile
	{
		public override string Texture => "SpiritMod/Items/Weapon/Thrown/CryoKnife/CryoKnife";

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Cryolite Bomb");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16;
			Projectile.height = 14;
			Projectile.aiStyle = 2;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
		}

		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item69, Projectile.Center);
			SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);

			Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<CryoExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
			
			for (int i = 0; i < 5; i++)
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, 0, 0, 0, default, .5f);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Main.rand.NextBool(4))
				target.AddBuff(ModContent.BuffType<CryoCrush>(), 240);
		}
	}

	public class CryoExplosion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Cryolite Bomb");
			Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.width = 80;
			Projectile.height = 80;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.hide = true;
			Projectile.penetrate = 3;
			Projectile.alpha = 0;
			Projectile.timeLeft = 120;
			Projectile.tileCollide = false;
		}

		public override void Kill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);

			for (int i = 0; i < 40; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.BlueCrystalShard, 0f, -2f, 0, default, 2f);
				dust.noGravity = true;

				dust.position += new Vector2((Main.rand.Next(-50, 51) / 20) - 1.5f, (Main.rand.Next(-50, 51) / 20) - 1.5f);

				if (dust.position != Projectile.Center)
					dust.velocity = Projectile.DirectionTo(dust.position) * 6f;
			}

			for (int i = 0; i < 3; i++)
				Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, Mod.Find<ModGore>("CryoShard1").Type, 1f);
			for (int i = 0; i < 3; i++)
				Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, Mod.Find<ModGore>("CryoShard2").Type, 1f);
			for (int i = 0; i < 3; i++)
				Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, Mod.Find<ModGore>("CryoShard3").Type, 1f);
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCsAndTiles.Add(index);
		public override Color? GetAlpha(Color lightColor) => new Color(220, 220, 220, 100);

		public override void AI()
		{
			if (++Projectile.frameCounter >= 2)
			{
				Projectile.frameCounter = 0;
				Projectile.frame = Math.Min(Main.projFrames[Type] - 1, Projectile.frame + 1);
			}

			Projectile.scale -= .0025f;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Main.rand.NextBool(4))
				target.AddBuff(ModContent.BuffType<CryoCrush>(), 240);
		}
	}
}
