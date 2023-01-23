using Microsoft.Xna.Framework;
using SpiritMod.Items;
using SpiritMod.Items.Accessory.BowSummonItem;
using SpiritMod.Utilities;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Projectiles.Summon.BowSummon
{
	public class BowSummon : ModProjectile
	{
		private int Timer
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private int TargetIndex
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Jinxbow");

			Main.projPet[Projectile.type] = true;
			Main.projFrames[Projectile.type] = 4;

			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 1;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.netImportant = true;
			Projectile.width = 20;
			Projectile.height = 40;
			Projectile.friendly = true;
			Projectile.minion = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 18000;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			if (player.HasAccessory<BowSummonItem>())
				Projectile.timeLeft = 2;

			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				if (i != Projectile.whoAmI && Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].type == Projectile.type && Math.Abs(Projectile.position.X - Main.projectile[i].position.X) + Math.Abs(Projectile.position.Y - Main.projectile[i].position.Y) < (float)Projectile.width)
				{
					if (Projectile.position.X < Main.projectile[i].position.X)
						Projectile.velocity.X = Projectile.velocity.X - 0.05f;
					else
						Projectile.velocity.X = Projectile.velocity.X + 0.05f;

					if (Projectile.position.Y < Main.projectile[i].position.Y)
						Projectile.velocity.Y = Projectile.velocity.Y - 0.05f;
					else
						Projectile.velocity.Y = Projectile.velocity.Y + 0.05f;
				}
			}

			float minDist = 900f;
			bool foundTarget = false;

			for (int n = 0; n < 200; n++)
			{
				if (Main.npc[n].CanBeChasedBy(Projectile, false))
				{
					float num532 = Main.npc[n].position.X + 40 + (Main.npc[n].width / 2);
					float num533 = Main.npc[n].position.Y - 90 + (Main.npc[n].height / 2);
					float num534 = Math.Abs(Projectile.position.X + (Projectile.width / 2) - num532) + Math.Abs(Projectile.position.Y + (Projectile.height / 2) - num533);

					if (num534 < minDist && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[n].position, Main.npc[n].width, Main.npc[n].height))
					{
						minDist = num534;
						foundTarget = true;
					}
				}
			}

			if (!foundTarget)
			{
				Projectile.friendly = true;
				Projectile.position.X = Main.player[Projectile.owner].Center.X - (Projectile.width * .5f);
				Projectile.position.Y = Main.player[Projectile.owner].Center.Y - (Projectile.width * .5f) - 40;
				Projectile.rotation = Main.player[Projectile.owner].velocity.X * 0.085f;
				Projectile.spriteDirection = Main.player[Projectile.owner].direction;
				Projectile.frame = 0;
			}
			else
			{
				Projectile.spriteDirection = 1;
				Timer++;
				if (Timer >= 70)
				{
					int range = 30;   //How many tiles away the projectile targets NPCs
					float shootVelocity = 16f; //magnitude of the shoot vector (speed of arrows shot)

					//TARGET NEAREST NPC WITHIN RANGE
					float lowestDist = float.MaxValue;
					for (int i = 0; i < 200; ++i)
					{
						NPC npc = Main.npc[i];
						//if npc is a valid target (active, not friendly, and not a critter)
						if (npc.active && npc.CanBeChasedBy(Projectile) && !npc.friendly)
						{
							//if npc is within 50 blocks
							float dist = Projectile.Distance(npc.Center);
							if (dist / 16 < range)
							{
								//if npc is closer than closest found npc
								if (dist < lowestDist)
								{
									lowestDist = dist;

									//target this npc
									TargetIndex = npc.whoAmI;
									Projectile.netUpdate = true;
								}
							}
						}
					}

					NPC target = Main.npc[TargetIndex];

					if (target.CanBeChasedBy(Projectile, false))
						Projectile.rotation = Projectile.DirectionTo(target.Center).ToRotation();

					if (++Projectile.frameCounter >= 5)
					{
						Projectile.frameCounter = 0;
						Projectile.frame++;
					}

					if (Projectile.frame > 3)
					{
						Vector2 ShootArea = new Vector2(Projectile.Center.X, Projectile.Center.Y);
						Vector2 direction = Vector2.Normalize(target.Center - ShootArea) * shootVelocity;

						int selectedIndex = 0;
						bool itemChosen = false;
						for (int i = 0; i < player.inventory.Length; ++i)
						{
							Item item = player.inventory[i];
							Item selItem = player.inventory[selectedIndex];

							if (!item.IsAir && item.ammo == AmmoID.Arrow && (selItem.ammo == AmmoID.None || item.damage > selItem.damage))
							{
								selectedIndex = i;
								itemChosen = true;
							}
						}

						Item selectedItem = player.inventory[selectedIndex];
						int shootType = selectedItem.shoot;
						int damage = (int)player.GetDamage(DamageClass.Summon).ApplyTo(16);

						if (!itemChosen)
						{
							shootType = ProjectileID.JestersArrow;
							damage = (int)(damage * 0.6f);
							direction *= 0.5f;
						}

						if (Main.myPlayer == Projectile.owner)
						{
							int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, direction, shootType, damage, Projectile.knockBack, player.whoAmI);

							Projectile newProj = Main.projectile[proj];
							newProj.DamageType = DamageClass.Summon;
							newProj.friendly = true;
							newProj.netUpdate = true;

							if (Main.netMode != NetmodeID.SinglePlayer)
								NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, proj);

							GItem.UseAmmoDirect(player, selectedIndex);
						}

						Projectile.frame = 0;
						Timer = 0;

						Projectile.netUpdate = true;
					}
				}

				Projectile.position.X = player.Center.X - (Projectile.width * .5f);
				Projectile.position.Y = player.Center.Y - (Projectile.width * .5f) - 40;
			}
		}

		public override bool MinionContactDamage() => false;
	}
}