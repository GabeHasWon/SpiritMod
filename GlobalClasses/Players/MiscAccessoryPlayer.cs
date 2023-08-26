using SpiritMod.Items.Accessory;
using SpiritMod.Projectiles.Clubs;
using SpiritMod.Utilities;
using SpiritMod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using SpiritMod.Items.Accessory.MeleeCharmTree;
using SpiritMod.Items.Accessory.MageTree;
using SpiritMod.Items.Sets.CascadeSet.Armor;
using System.Collections.Generic;
using SpiritMod.Items;
using System;
using System.Linq;
using SpiritMod.Projectiles.Summon.BowSummon;
using SpiritMod.Projectiles.Summon;
using SpiritMod.Items.Accessory.BowSummonItem;
using SpiritMod.Projectiles.Summon.CimmerianStaff;
using SpiritMod.Items.Accessory.GranitechDrones;
using SpiritMod.Items.Accessory.Bauble;
using SpiritMod.Items.Accessory.AceCardsSet;
using SpiritMod.Projectiles;

namespace SpiritMod.GlobalClasses.Players
{
	public class MiscAccessoryPlayer : ModPlayer
	{
		public readonly Dictionary<string, bool> accessory = new Dictionary<string, bool>();
		public readonly Dictionary<string, int> timers = new Dictionary<string, int>();

		public override void Initialize()
		{
			accessory.Clear();
			timers.Clear();

			var types = typeof(SpiritMod).Assembly.GetTypes(); //Add every accessory & timered item to this dict
			foreach (var type in types)
			{
				if (type.IsSubclassOf(typeof(AccessoryItem)) && !type.IsAbstract)
				{
					var item = Activator.CreateInstance(type) as AccessoryItem;
					accessory.Add(item.AccName, false);
				}

				if (typeof(ITimerItem).IsAssignableFrom(type) && !type.IsAbstract)
				{
					var item = Activator.CreateInstance(type) as ITimerItem;

					if (item.TimerCount() == 1)
						timers.Add(type.Name, 0);
					else
						for (int i = 0; i < item.TimerCount(); ++i)
							timers.Add(type.Name + i, 0);
				}
			}
		}

		public override void ResetEffects()
		{
			var accColl = accessory.Keys.ToList(); //Reset every acc
			foreach (string item in accColl)
				accessory[item] = false;

			var timerColl = timers.Keys.ToList(); //Decrement every timer
			foreach (string item in timerColl)
				timers[item]--;
		}

		public override void UpdateDead() => ResetEffects();

		/// <summary>Allows you to modify the knockback given ANY damage source. NOTE: This is an IL hook, which is why it needs a Player instance and is static.</summary>
		/// <param name="player">The specific player to change.</param>
		/// <param name="horizontal">Whether this is a horizontal (velocity.X) change or a vertical (velocity.Y) change.</param>
		public static float KnockbackMultiplier(Player player, bool horizontal)
		{
			float totalKb = 1f;

			// Frost Giant Belt
			if (player.HasAccessory<FrostGiantBelt>() && player.channel && HeldItemIsClub(player))
				totalKb *= 0.5f;

			// Cascade Chestplate
			if (player.ChestplateEquipped<CascadeChestplate>() && horizontal)
				totalKb *= 0.25f;

			if (totalKb < 0.001f) //Throws NullReferenceException if it's 0 for some reason
				totalKb = 0.001f;
			return totalKb;
		}

		public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)/* tModPorter If you don't need the Item, consider using ModifyHitNPC instead */
		{
			// Twilight Talisman
			if (Player.HasAccessory<Twilight1>() && Main.rand.NextBool(13))
				target.AddBuff(BuffID.ShadowFlame, 180);

			// Ace of Spades
			if (Player.HasAccessory<AceOfSpades>() || Player.HasAccessory<FourOfAKind>())
			{
				modifiers.CritDamage *= 1.2f;

				for (int i = 0; i < 3; i++)
					Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<SpadeDust>(), 0, -0.8f);
			}
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone) => OnHitNPCWithAnything(proj, target, hit);
		public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone) => OnHitNPCWithAnything(item, target, hit);

		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
		{
			// Twilight Talisman
			bool shadowFlameCondition = Player.HasAccessory<Twilight1>() && Main.rand.NextBool(15);
			AddBuffWithCondition(shadowFlameCondition, target, BuffID.ShadowFlame, 180);

			// Ace of Spades
			if ((Player.HasAccessory<AceOfSpades>() || Player.HasAccessory<FourOfAKind>()))
			{
				modifiers.CritDamage *= 1.2f;
				for (int i = 0; i < 3; i++)
					Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<SpadeDust>(), 0, -0.8f);
			}
		}

		public void OnHitNPCWithAnything(Entity weapon, NPC target, NPC.HitInfo hit)
		{
			// Hell Charm
			if (Player.HasAccessory<HCharm>() && Main.rand.NextBool(11))
			{
				if ((weapon is Item i && i.IsMelee()) || weapon is Projectile)
					target.AddBuff(BuffID.OnFire, 120);
			}

			// Amazon Charm
			if (Player.HasAccessory<YoyoCharm2>() && Main.rand.NextBool(11))
			{
				if ((weapon is Item i && i.IsMelee()) || weapon is Projectile)
					target.AddBuff(BuffID.Poisoned, 120);
			}

			if (hit.Crit && !target.friendly && target.lifeMax > 15 && !target.SpawnedFromStatue && target.type != NPCID.TargetDummy)
			{
				if (((weapon is Item item) && Main.rand.NextFloat() < (item.useTime / 100f)) || ((weapon is Projectile proj) && Main.rand.NextFloat() < (proj.GetGlobalProjectile<SpiritGlobalProjectile>().storedUseTime / 75f)))
				{
					//Ace of Hearts
					if (Player.HasAccessory<AceOfHearts>() || Player.HasAccessory<FourOfAKind>())
					{
						ItemUtils.NewItemWithSync(Player.GetSource_OnHit(target), Player.whoAmI, (int)target.position.X, (int)target.position.Y, target.width, target.height, Main.halloween ? ItemID.CandyApple : ItemID.Heart);

						for (int i = 0; i < 3; i++)
							Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<HeartDust>(), 0, -0.8f);
					}
					//Ace of Diamonds
					if (Player.HasAccessory<AceOfDiamonds>() || Player.HasAccessory<FourOfAKind>())
					{
						ItemUtils.NewItemWithSync(Player.GetSource_OnHit(target), Player.whoAmI, (int)target.position.X, (int)target.position.Y, target.width, target.height, ModContent.ItemType<DiamondAce>());

						for (int i = 0; i < 3; i++)
							Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<DiamondDust>(), 0, -0.8f);
					}
				}

				//Ace of Clubs
				if (Player.HasAccessory<AceOfClubs>() || Player.HasAccessory<FourOfAKind>())
				{
					for (int i = 0; i < 3; i++)
						Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<ClubDust>(), 0, -0.8f);

					int money = hit.Damage;

					if (money / 1000000 > 0)
						ItemUtils.NewItemWithSync(Player.GetSource_OnHit(target), Player.whoAmI, (int)target.position.X, (int)target.position.Y, target.width, target.height, ItemID.PlatinumCoin, money / 1000000);

					money %= 1000000;
					if (money / 10000 > 0)
						ItemUtils.NewItemWithSync(Player.GetSource_OnHit(target), Player.whoAmI, (int)target.position.X, (int)target.position.Y, target.width, target.height, ItemID.GoldCoin, money / 10000);

					money %= 10000;
					if (money / 100 > 0)
						ItemUtils.NewItemWithSync(Player.GetSource_OnHit(target), Player.whoAmI, (int)target.position.X, (int)target.position.Y, target.width, target.height, ItemID.SilverCoin, money / 100);

					money %= 100;
					if (money > 0)
						ItemUtils.NewItemWithSync(Player.GetSource_OnHit(target), Player.whoAmI, (int)target.position.X, (int)target.position.Y, target.width, target.height, ItemID.CopperCoin, money);
				}
			}
		}

		private static void AddBuffWithCondition(bool condition, NPC p, int id, int ticks) { if (condition) p.AddBuff(id, ticks); }
		
		public int addDef = 0;
		public int frostBeltTimer = 0;
		public override void UpdateEquips()
		{
	
			if (Player.HasAccessory<FrostGiantBelt>() && Player.channel)
				if (HeldItemIsClub(Player))
				{
					frostBeltTimer++;
					if ((frostBeltTimer % 6 == 0) && addDef < 15){
						addDef++; 
					}
					Player.statDefense += addDef;
				}
			if (!Player.channel)
			{
				frostBeltTimer = 0;
				addDef = 0; 
			}
		}

		public override void PostUpdateEquips()
		{
			if (!Player.dead && Player.active)
			{
				//	Projectile.NewProjectile(Terraria.Entity.GetSource_NaturalSpawn(), Player.Center, Vector2.Zero, ModContent.ProjectileType<UmbillicalEyeballProj>(), (int)(Player.GetDamage(DamageClass.Summon).ApplyTo(55)), 1.5f, Player.whoAmI, Player.ownedProjectileCounts[ModContent.ProjectileType<UmbillicalEyeballProj>()], 0);
							//if (Player.HasAccessory<UmbillicalEyeball>() && Player.ownedProjectileCounts[ModContent.ProjectileType<UmbillicalEyeballProj>()] < 3)

				if (Player.HasAccessory<RogueCrest>() && Player.ownedProjectileCounts[ModContent.ProjectileType<RogueKnifeMinion>()] < 1)
					Projectile.NewProjectile(Terraria.Entity.GetSource_NaturalSpawn(), Player.Center, Vector2.Zero, ModContent.ProjectileType<RogueKnifeMinion>(), (int)Player.GetDamage(DamageClass.Summon).ApplyTo(5), .5f, Player.whoAmI);

				if (Player.HasAccessory<BowSummonItem>() && Player.ownedProjectileCounts[ModContent.ProjectileType<BowSummon>()] < 1)
					Projectile.NewProjectile(Terraria.Entity.GetSource_NaturalSpawn(), Player.Center, Vector2.Zero, ModContent.ProjectileType<BowSummon>(), (int)Player.GetDamage(DamageClass.Summon).ApplyTo(22), 1.5f, Player.whoAmI);

				if (Player.HasAccessory<SpellswordCrest>() && Player.ownedProjectileCounts[ModContent.ProjectileType<HolyKnifeMinion>()] < 1)
					Projectile.NewProjectile(Terraria.Entity.GetSource_NaturalSpawn(), Player.Center, Vector2.Zero, ModContent.ProjectileType<HolyKnifeMinion>(), (int)Player.GetDamage(DamageClass.Summon).ApplyTo(32), 1.25f, Player.whoAmI);

				if (Player.HasAccessory<CimmerianScepter>() && Player.ownedProjectileCounts[ModContent.ProjectileType<CimmerianScepterProjectile>()] < 1)
					Projectile.NewProjectile(Terraria.Entity.GetSource_NaturalSpawn(), Player.Center, Vector2.Zero, ModContent.ProjectileType<CimmerianScepterProjectile>(), (int)Player.GetDamage(DamageClass.Summon).ApplyTo(22), 1.5f, Player.whoAmI);

				if (Player.HasAccessory<GranitechDroneBox>())
				{
					if (Player.ownedProjectileCounts[ModContent.ProjectileType<GranitechDrone>()] < 3)
					{
						int dmg = (int)Player.GetDamage(DamageClass.Summon).ApplyTo(ContentSamples.ItemsByType[ModContent.ItemType<GranitechDroneBox>()].damage);
						int newProj = Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<GranitechDrone>(), dmg, 1.5f, Player.whoAmI);
						Main.projectile[newProj].ai[1] = Player.ownedProjectileCounts[ModContent.ProjectileType<GranitechDrone>()];
						Main.projectile[newProj].originalDamage = dmg;
					}
				}
			}
		}

		public override void OnHurt(Player.HurtInfo info)
		{
			// Spectre Ring
			if (Player.HasAccessory<SpectreRing>())
			{
				for (int h = 0; h < 3; h++)
				{
					Vector2 vel = new Vector2(0, -1);
					float rand = Main.rand.NextFloat() * MathHelper.TwoPi;
					vel = vel.RotatedBy(rand);
					vel *= 2f;
					Projectile.NewProjectile(Player.GetSource_OnHurt(null), Player.Center, vel, ProjectileID.LostSoulFriendly, 45, 0, Main.myPlayer);
				}
			}

			// Mana Shield & Seraphim Bulwark
			if (Player.HasAccessory<ManaShield>() || Player.HasAccessory<SeraphimBulwark>())
			{
				if (Player.statMana > info.Damage / 10 * 4)
				{
					if ((Player.statMana - info.Damage / 10 * 4) > 0)
						Player.statMana -= info.Damage / 10 * 4;
					else
						Player.statMana = 0;
				}
			}

			//Bauble
			if (Player.HasAccessory<Bauble>())
			{
				NPC npc = (info.DamageSource.SourceNPCIndex > -1) ? Main.npc[info.DamageSource.SourceNPCIndex] : null;
				bool destructableNPC = npc != null && npc.value == 0 && npc.lifeMax <= 1;

				if ((Player.ownedProjectileCounts[ModContent.ProjectileType<IceReflector>()] > 0) && (info.DamageSource.SourceProjectileLocalIndex > -1 || destructableNPC))
				{
					if (destructableNPC)
						npc.life = 0;

					return;
				}

				bool hitBelowHalf = Player.statLife > (Player.statLifeMax2 / 2) && (Player.statLife - info.Damage) < (Player.statLifeMax2 / 2);

				if (Player.ItemTimer<Bauble>() <= 0 && hitBelowHalf)
				{
					Projectile.NewProjectileDirect(Player.GetSource_OnHurt(null), Player.Center, Vector2.Zero, ModContent.ProjectileType<IceReflector>(), 0, 0, Player.whoAmI);

					Player.AddBuff(ModContent.BuffType<BaubleResistance>(), Bauble.shieldTime);
					Player.SetItemTimer<Bauble>(Bauble.cooldown);
				}
			}
		}

		public override void ModifyHurt(ref Player.HurtModifiers modifiers)
		{
			// Mana Shield & Seraphim Bulwark damage reduction
			if (Player.HasAccessory<ManaShield>() || Player.HasAccessory<SeraphimBulwark>())
				modifiers.FinalDamage *= 0.1f;
		}

		public override void PostHurt(Player.HurtInfo info)
		{
			// Spectre Ring
			if (Player.HasAccessory<SpectreRing>())
			{
				int newProj = Projectile.NewProjectile(Player.GetSource_OnHurt(null), Player.Center, new Vector2(6, 6), ProjectileID.SpectreWrath, 40, 0f, Main.myPlayer);

				int dist = 800;
				int target = -1;
				for (int i = 0; i < 200; ++i)
				{
					if (Main.npc[i].active && Main.npc[i].CanBeChasedBy(Main.projectile[newProj], false))
					{
						if ((Main.npc[i].Center - Main.projectile[newProj].Center).LengthSquared() < dist * dist)
						{
							target = i;
							break;
						}
					}
				}
				Main.projectile[newProj].ai[0] = target;
			}
		}

		/// <summary>A bit of a wacky way of checking if a held item is a club, but it works.</summary>
		/// <param name="player">Player to check held item of.</param>
		public static bool HeldItemIsClub(Player player)
		{
			Item heldItem = player.HeldItem;
			if (heldItem.shoot > ProjectileID.None && heldItem.ModItem != null && heldItem.ModItem.Mod == ModContent.GetInstance<SpiritMod>())
			{
				var p = ContentSamples.ProjectilesByType[heldItem.shoot];

				if (p.ModProjectile is ClubProj)
					return true;
			}
			return false;
		}
	}
}
