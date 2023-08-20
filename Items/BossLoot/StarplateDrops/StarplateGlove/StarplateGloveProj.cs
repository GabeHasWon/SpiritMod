using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.StarplateDrops.StarplateGlove
{
	public class StarplateGloveProj : ModProjectile
	{
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		private int State
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		private const int RELEASED = 1;
		private const int RETURNING = 2;

		private Vector2 direction;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Hundred-Crack Fist");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.aiStyle = 1;
			AIType = ProjectileID.Bullet;
			Projectile.scale = 1f;
			Projectile.tileCollide = false;
			Projectile.hide = false;
			Projectile.friendly = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.timeLeft = 2;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			Projectile.timeLeft = 2;

			if (player.HeldItem.type != ModContent.ItemType<StarplateGlove>())
				State = RETURNING;

			if (State != RETURNING)
			{
				if (player.whoAmI == Main.myPlayer)
				{
					direction = Projectile.DirectionTo(Main.MouseWorld);

					Projectile.netUpdate = true;
				}
				Projectile.velocity *= 0.9f;

				if (!player.controlUseTile)
					State = RELEASED;

				if (player.controlUseTile && State == RELEASED)
				{
					State = RETURNING;
					Projectile.netUpdate = true;
				}

				if (player.controlUseItem && ++Counter % 7 == 0)
				{
					Counter = 0;

					if (player.statMana > 0)
					{
						player.statMana -= Math.Min(player.statMana, 6);
						player.manaRegenDelay = 60;

						if (player.whoAmI == Main.myPlayer)
						{
							Vector2 speed = (direction * 10).RotatedByRandom(0.7f);

							int type = Main.rand.NextBool(2) ? ModContent.ProjectileType<StargloveChargeOrange>() : ModContent.ProjectileType<StargloveChargePurple>();
							int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + (speed * 8), speed, type, Projectile.damage, Projectile.knockBack, player.whoAmI);

							if (Main.netMode != NetmodeID.SinglePlayer)
								NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);

							if (type == ModContent.ProjectileType<StargloveChargePurple>())
							{
								for (float num2 = 0.0f; (double)num2 < 10; ++num2)
								{
									int dustIndex = Dust.NewDust(Projectile.Center + (speed * 5), 2, 2, DustID.Clentaminator_Cyan, 0f, 0f, 0, default, 1.5f);
									Main.dust[dustIndex].noGravity = true;
									Main.dust[dustIndex].velocity = Vector2.Normalize((speed * 5).RotatedBy(Main.rand.NextFloat(6.28f))) * 2.5f;
								}
							}
							else
							{
								for (float num2 = 0.0f; (double)num2 < 10; ++num2)
								{
									int dustIndex = Dust.NewDust(Projectile.Center + (speed * 5), 2, 2, DustID.Torch, 0f, 0f, 0, default, 2f);
									Main.dust[dustIndex].noGravity = true;
									Main.dust[dustIndex].velocity = Vector2.Normalize((speed * 8).RotatedBy(Main.rand.NextFloat(6.28f))) * 2.5f;
								}
							}

							for (int j = 0; j < 5; j++)
							{
								int proj2 = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + (speed * 8), speed, type, 0, 0, player.whoAmI, proj);

								if (Main.netMode != NetmodeID.SinglePlayer)
									NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj2);
							}

							Projectile.netUpdate = true;
						}
					}
				}

				Projectile.rotation = direction.ToRotation() + 1.57f;
			}
			else
			{
				Vector2 vel = player.Center - Projectile.position;

				if (vel.Length() < 40 || vel.Length() > 1500 || Projectile.position.HasNaNs())
					Projectile.active = false;

				Projectile.velocity = Vector2.Normalize(vel) * 20;
				Projectile.rotation = vel.ToRotation() - 1.57f;
			}
		}

		public override Color? GetAlpha(Color lightColor) => Color.White;

		public override void SendExtraAI(BinaryWriter writer) => writer.WriteVector2(direction);

		public override void ReceiveExtraAI(BinaryReader reader) => direction = reader.ReadVector2();
	}
}