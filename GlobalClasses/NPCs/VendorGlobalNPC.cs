using SpiritMod.Items.Ammo.Rocket.Warhead;
using SpiritMod.Items.BossLoot.MoonWizardDrops;
using SpiritMod.Items.Consumable;
using SpiritMod.Items.DonatorItems;
using SpiritMod.Items.Halloween;
using System.Linq;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Utilities;
using SpiritMod.Items.Pins;
using SpiritMod.Items.Placeable.Furniture;
using SpiritMod.Items.Material;
using SpiritTiles = SpiritMod.Tiles;
using SpiritItems = SpiritMod.Items;
using SpiritNPCs = SpiritMod.NPCs;
using SpiritMod.Items.Placeable.Furniture.Paintings;
using SpiritMod.Items.Sets.LaunchersMisc.Liberty;
using SpiritMod.Items.Accessory;
using SpiritMod.Items.Pets;
using SpiritMod.Items.Sets.FlailsMisc.JadeDao;
using SpiritMod.Items.ByBiome.Ocean.Misc.VolleyballContent;
using SpiritMod.Items.ByBiome.Ocean.Placeable;
using SpiritMod.Items.Sets.DashSwordSubclass.BladeOfTheDragon;
using SpiritMod.Items.Armor.FreemanSet;
using SpiritMod.Items.Armor.Beachwear;
using Terraria.GameContent.ItemDropRules;
using SpiritMod.Items.Armor.CommandoSet;
using Microsoft.CodeAnalysis;
using SpiritMod.Items.Ammo;
using SpiritMod.NPCs;

namespace SpiritMod.GlobalClasses.NPCs;

internal class VendorGlobalNPC : GlobalNPC
{
	public override void ModifyShop(NPCShop shop)
	{
		if (shop.NpcType == NPCID.Merchant)
		{
			shop.Add<CandyBowl>(Condition.Halloween);
			shop.Add<SpiritTiles.Furniture.FestivalLanternItem>(Condition.LanternNight);
			shop.Add<BeachUmbrellaItem>(Condition.InBeach);
			shop.Add<LoungeChairItem>(Condition.InBeach);
		}
		else if (shop.NpcType == NPCID.ArmsDealer)
		{
			shop.Add<SpiritItems.Ammo.Bullet.RubberBullet>();
			shop.Add<Warhead>();

			shop.Add<TinyLunazoaItem>(new Condition("Mods.SpiritMod.Conditions.HasMoonshot", () =>
			{
				for (int i = 0; i < Main.maxPlayers; ++i)
				{
					Player plr = Main.player[i];

					if (plr.active && plr.HasItemInAnyInventory(ModContent.ItemType<Moonshot>()))
						return true;
				}
				return false;
			}));
		}
		else if (shop.NpcType == NPCID.Cyborg)
		{
			shop.Add<FreemanHead>();
			shop.Add<FreemanBody>();
			shop.Add<FreemanLegs>();
		}
		else if (shop.NpcType == NPCID.Clothier)
		{
			shop.Add<TheCouch>();
			shop.Add(new Item(ItemID.MiningShirt) { shopCustomPrice = 200000 });
			shop.Add(new Item(ItemID.MiningPants) { shopCustomPrice = 200000 });

			shop.Add<TintedGlasses>(Condition.InBeach);
			shop.Add<BeachTowel>(Condition.InBeach);
			shop.Add<SwimmingTrunks>(Condition.InBeach);
			shop.Add<BikiniTop>(Condition.InBeach);
			shop.Add<BikiniBottom>(Condition.InBeach);

			shop.Add<CommandoHead>(SpiritConditions.VoyagerDown);
			shop.Add<CommandoBody>(SpiritConditions.VoyagerDown);
			shop.Add<CommandoLegs>(SpiritConditions.VoyagerDown);
		}
		else if (shop.NpcType == NPCID.Dryad)
		{
			shop.Add<SpiritItems.Placeable.MusicBox.TranquilWindsBox>();
			shop.Add<SpiritItems.Placeable.Tiles.HalloweenGrass>(Condition.DownedGolem, Condition.Halloween);
			shop.Add<SpiritItems.Placeable.Tiles.BriarGrassSeeds>(SpiritConditions.InBriar);
		}
		else if (shop.NpcType == NPCID.Wizard)
		{
			shop.Add<SurrenderBell>();
			shop.Add<PinStar>();
		}
		else if (shop.NpcType == NPCID.Steampunker)
		{
			shop.Add<SpiritSolution>();
			shop.Add<OliveSolution>();
		}
		else if (shop.NpcType == NPCID.PartyGirl)
		{
			Condition downedMechAny = Condition.DownedMechBossAny;
			shop.Add<SpiritItems.Sets.GunsMisc.Partystarter.PartyStarter>(downedMechAny);
			shop.Add(new Item(ModContent.ItemType<SpiritItems.Placeable.MusicBox.NeonMusicBox>()) { shopCustomPrice = 50000 }, downedMechAny);
			shop.Add<SpiritPainting>(downedMechAny);

			shop.Add<Volleyball>(Condition.InBeach);
		}
		else if (shop.NpcType == NPCID.WitchDoctor)
			shop.Add<SpiritItems.Sets.ClubSubclass.Macuahuitl>();
		else if (shop.NpcType == NPCID.Painter)
		{
			shop.Add<Canvas>();
			shop.Add<FloppaPainting>();
			shop.Add<SatchelReward>();

			shop.Add<ScrunklyPaintingItem>(new Condition("Mods.SpiritMod.Conditions.StarjinxDown", () =>
				ModContent.GetInstance<SpiritNPCs.StarjinxEvent.StarjinxEventWorld>().StarjinxDefeated));
		}
		else if (shop.NpcType == NPCID.Demolitionist)
		{
			shop.Add<LibertyItem>();
			shop.Add<Warhead>();
			shop.Add<ShortFuse>();
			shop.Add<LongFuse>();
		}
		else if (shop.NpcType == NPCID.BestiaryGirl)
		{
			shop.Add<CagedMoonlight>(new Condition("Mods.SpiritMod.Conditions.HalfCompleteBestiary", () =>
				Main.BestiaryDB.GetCompletedPercentByMod(Mod) >= 0.5f));

			shop.Add<SuspiciousLookingMeatballs>(new Condition("Mods.SpiritMod.Conditions.FullCompleteBestiary", () =>
				Main.BestiaryDB.GetCompletedPercentByMod(Mod) >= 1f));
		}
	}

	public override void SetupTravelShop(int[] shop, ref int nextSlot)
	{
		if (Main.rand.NextBool(8) && NPC.downedPlantBoss)
			shop[nextSlot++] = ModContent.ItemType<JadeDao>();

		if (Main.rand.NextBool(8) && NPC.downedPlantBoss)
			shop[nextSlot++] = ModContent.ItemType<BladeOfTheDragon>();
	}
}
