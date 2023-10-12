using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Collections.Generic;
using System;
using SpiritMod.Items.Armor.DiverSet;
using SpiritMod.Items.Armor.AstronautVanity;
using SpiritMod.Items.Armor.BeekeeperSet;
using SpiritMod.Items.Armor.CapacitorSet;
using SpiritMod.Items.Armor.CenturionSet;
using SpiritMod.Items.Armor.CommandoSet;
using SpiritMod.Items.Armor.CowboySet;
using SpiritMod.Items.Armor.JackSet;
using SpiritMod.Items.Armor.WitchSet;
using SpiritMod.Items.Armor.FreemanSet;
using SpiritMod.Items.Armor.GeodeArmor;
using SpiritMod.Items.Armor.HunterArmor;
using SpiritMod.Items.Armor.PlagueDoctor;
using SpiritMod.Items.Armor.ProtectorateSet;
using SpiritMod.Items.Armor.Masks;
using SpiritMod.Items.Sets.DonatorVanity;

namespace SpiritMod.Items.Sets.GamblerChestLoot.GamblerChestNPCs
{
	public abstract class GamblerChest : ModNPC
	{
		public struct LootInfo
		{
			public object Items;
			public Func<bool> Conditions;
			public Func<byte> Stack;
			public LootInfo(object Items, Func<bool> Conditions = null, Func<byte> Stack = null)
			{
				this.Items = Items;
				this.Conditions = Conditions;
				this.Stack = Stack;
			}
		}

		/// <summary>
		/// Dictates for how long the NPC will last after being activated
		/// </summary>
		public abstract int CounterMax { get; }
		public abstract int CoinRate { get; }
		public abstract int TotalValue { get; }
		public int maxValue;
		public int value;

		public int Counter
		{
			get => (int)NPC.ai[0];
			set => NPC.ai[0] = value;
		}

		public bool activated = false;

		public int[] genericVanityDrops = new int[] { ModContent.ItemType<DiverLegs>(), ModContent.ItemType<DiverBody>(), ModContent.ItemType<DiverHead>(), ModContent.ItemType<AstronautLegs>(), ModContent.ItemType<AstronautBody>(), ModContent.ItemType<AstronautHelm>(), ModContent.ItemType<BeekeeperLegs>(), ModContent.ItemType<BeekeeperBody>(), ModContent.ItemType<BeekeeperHead>(),
				ModContent.ItemType<CapacitorLegs>(), ModContent.ItemType<CapacitorBody>(), ModContent.ItemType<CapacitorHead>(), ModContent.ItemType<CenturionLegs>(), ModContent.ItemType<CenturionBody>(), ModContent.ItemType<CenturionHead>(), ModContent.ItemType<CommandoLegs>(), ModContent.ItemType<CommandoBody>(), ModContent.ItemType<CommandoHead>(),
				ModContent.ItemType<CowboyLegs>(), ModContent.ItemType<CowboyBody>(), ModContent.ItemType<CowboyHead>(), ModContent.ItemType<FreemanLegs>(), ModContent.ItemType<FreemanBody>(), ModContent.ItemType<FreemanHead>(), ModContent.ItemType<GeodeLeggings>(), ModContent.ItemType<GeodeChestplate>(), ModContent.ItemType<GeodeHelmet>(), ModContent.ItemType<SnowRangerLegs>(), ModContent.ItemType<SnowRangerBody>(), ModContent.ItemType<SnowRangerHead>(),
				ModContent.ItemType<JackLegs>(), ModContent.ItemType<JackBody>(), ModContent.ItemType<JackHead>(), ModContent.ItemType<PlagueDoctorLegs>(), ModContent.ItemType<PlagueDoctorRobe>(), ModContent.ItemType<PlagueDoctorCowl>(), ModContent.ItemType<ProtectorateLegs>(), ModContent.ItemType<ProtectorateBody>(), ModContent.ItemType<LeafPaddyHat>(), ModContent.ItemType<PsychoMask>(),
				ModContent.ItemType<WitchLegs>(), ModContent.ItemType<WitchBody>(), ModContent.ItemType<WitchHead>()};

		public int[] donatorVanityDrops = new int[] { ModContent.ItemType<WaasephiVanity>(), ModContent.ItemType<MeteorVanity>(), ModContent.ItemType<LightNovasVanity>(), ModContent.ItemType<PixelatedFireballVanity>() };

		public abstract List<LootInfo> LootDrops();

		public sealed override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

			StaticDefaults();
		}
		public virtual void StaticDefaults() { }

		public sealed override void SetDefaults()
		{
			NPC.knockBackResist = 0;
			NPC.aiStyle = -1;
			NPC.lifeMax = 1;
			NPC.immortal = true;
			NPC.noTileCollide = false;
			NPC.dontCountMe = true;

			Defaults();
		}
		public virtual void Defaults() { }

		public sealed override void AI()
		{
			Counter++;
			if (!activated)
			{
				if (Counter >= 70 && NPC.velocity.Y == 0)
				{
					Counter = 0;
					activated = true;
					maxValue = value = TotalValue;

					OnActivate();
				}
				return;
			}

			if (Counter >= CounterMax)
			{
				if (Main.netMode != NetmodeID.Server)
					DeathEffects();

				NPC.active = false;
			}

			if (Main.netMode != NetmodeID.MultiplayerClient)
				SpawnLoot();

			if (NPC.velocity.Y != 0)
				NPC.rotation += Main.rand.NextFloat(-0.1f, 0.1f);
			else
				NPC.rotation = 0;

			int fadeoutTime = 14;
			if (Counter > (CounterMax - fadeoutTime))
				NPC.scale = MathHelper.Max(NPC.scale - (1f / fadeoutTime), 0);
		}

		public virtual void OnActivate() { }
		public virtual void DeathEffects() { }

		private void SpawnLoot()
		{
			if (Counter > (CounterMax / 2) && Counter % CoinRate == 0) //Spawn coins
			{
				static int GetCoinValue(int type) => type switch
				{
					ItemID.SilverCoin => 100,
					ItemID.GoldCoin => 10000,
					ItemID.PlatinumCoin => 1000000,
					_ => 1
				};

				int segment = (Counter == CounterMax) ? 0 : (int)(maxValue / (float)(CounterMax / 2f / CoinRate) * ((1f - ((float)Counter / CounterMax)) * CoinRate));
				List<int> options = new() { ItemID.CopperCoin, ItemID.SilverCoin, ItemID.GoldCoin, ItemID.PlatinumCoin };

				while (value > segment)
				{
					if (value < GetCoinValue(ItemID.PlatinumCoin))
						options.Remove(ItemID.PlatinumCoin);
					if (value < GetCoinValue(ItemID.GoldCoin))
						options.Remove(ItemID.GoldCoin);
					if (value < GetCoinValue(ItemID.SilverCoin))
						options.Remove(ItemID.SilverCoin);

					int randomPick = Main.rand.Next(options.Count);

					int randomStack = Main.rand.Next(1, 8) + 1;
					int stack = ((randomStack * GetCoinValue(options[randomPick])) <= value) ? randomStack : 1;

					value -= randomPick switch
					{
						1 => 100,
						2 => 10000,
						3 => 1000000,
						_ => 1
					} * stack;

					int item = Item.NewItem(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, options[randomPick], stack);
					Main.item[item].velocity = Vector2.UnitY.RotatedByRandom(1.2f) * -Main.rand.NextFloat(3.0f, 4.0f);
				}
			}

			var drops = LootDrops();
			foreach (LootInfo item in drops) //Spawn other item drops
			{
				if (!item.Conditions())
					continue;

				switch (item.Items)
				{
					case Array a:
						int[] itemArray = (int[])item.Items;
						int itemChoice = itemArray[Main.rand.Next(itemArray.Length)];

						if (itemChoice != ItemID.None)
							Item.NewItem(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, itemArray[Main.rand.Next(itemArray.Length)], (item.Stack == null) ? 1 : item.Stack());
						break;
					case int i:
						int itemint = (int)item.Items;

						if (itemint != ItemID.None)
							Item.NewItem(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, itemint, (item.Stack == null) ? 1 : item.Stack());
						break;
				}
			}
		}

		public override void FindFrame(int frameHeight)
		{
			if (activated)
			{
				NPC.frameCounter = MathHelper.Min((float)NPC.frameCounter + .2f, (Main.npcFrameCount[Type] - 1));
				NPC.frame.Y = (int)NPC.frameCounter * frameHeight;
			}
		}
	}
}