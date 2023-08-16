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

namespace SpiritMod.GlobalClasses.NPCs;

internal class VendorGlobalNPC : GlobalNPC
{
	public override void SetupShop(int type, Chest shop, ref int nextSlot)
	{
		if (type == NPCID.Merchant)
		{
			if (Main.halloween)
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<CandyBowl>(), false);

			if (LanternNight.LanternsUp)
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritTiles.Furniture.FestivalLanternItem>(), false);

			if (Main.LocalPlayer.ZoneBeach)
			{
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<BeachUmbrellaItem>(), false);
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LoungeChairItem>(), false);
			}
		}
		else if (type == NPCID.ArmsDealer)
		{
			shop.item[nextSlot].SetDefaults(ModContent.ItemType<SpiritItems.Ammo.Bullet.RubberBullet>(), false);
			shop.item[nextSlot].SetDefaults(ModContent.ItemType<Warhead>(), false);

			if (Main.player.Where(x => x.HasItem(ModContent.ItemType<Moonshot>())).Any())
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<TinyLunazoaItem>(), false);
		}
		else if (type == NPCID.Cyborg)
		{
			shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritItems.Armor.FreemanSet.FreemanHead>(), false);
			shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritItems.Armor.FreemanSet.FreemanBody>(), false);
			shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritItems.Armor.FreemanSet.FreemanLegs>(), false);
		}
		else if (type == NPCID.Clothier)
		{
			shop.item[nextSlot++].SetDefaults(ModContent.ItemType<TheCouch>(), false);
			shop.item[nextSlot].SetDefaults(410, false);
			shop.item[nextSlot++].shopCustomPrice = 200000;
			shop.item[nextSlot].SetDefaults(411, false);
			shop.item[nextSlot++].shopCustomPrice = 200000;

			if (MyWorld.downedRaider)
			{
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritItems.Armor.CommandoSet.CommandoHead>(), false);
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritItems.Armor.CommandoSet.CommandoBody>(), false);
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritItems.Armor.CommandoSet.CommandoLegs>(), false);
			}
			if (Main.LocalPlayer.ZoneBeach)
			{
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritItems.Armor.Beachwear.TintedGlasses>(), false);
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritItems.Armor.Beachwear.BeachTowel>(), false);
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritItems.Armor.Beachwear.SwimmingTrunks>(), false);
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritItems.Armor.Beachwear.BikiniTop>(), false);
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritItems.Armor.Beachwear.BikiniBottom>(), false);
			}
		}
		else if (type == NPCID.Dryad)
		{
			shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritItems.Placeable.MusicBox.TranquilWindsBox>(), false);

			if (NPC.downedGolemBoss && Main.halloween)
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritItems.Placeable.Tiles.HalloweenGrass>(), false);

			if (Main.LocalPlayer.ZoneBriar())
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritItems.Placeable.Tiles.BriarGrassSeeds>(), false);
		}
		else if (type == NPCID.Wizard)
		{
			shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SurrenderBell>(), false);
			shop.item[nextSlot++].SetDefaults(ModContent.ItemType<PinStar>(), false);
		}
		else if (type == NPCID.Steampunker)
		{
			shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritItems.Ammo.SpiritSolution>());
			shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritItems.Ammo.OliveSolution>());
		}
		else if (type == NPCID.PartyGirl)
		{
			if (NPC.downedMechBossAny)
			{
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritItems.Sets.GunsMisc.Partystarter.PartyStarter>(), false);
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<SpiritItems.Placeable.MusicBox.NeonMusicBox>(), false);
				shop.item[nextSlot++].shopCustomPrice = 50000;
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritPainting>(), false);
			}

			if (Main.LocalPlayer.ZoneBeach)
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Volleyball>(), false);
		}
		else if (type == NPCID.WitchDoctor)
			shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SpiritItems.Sets.ClubSubclass.Macuahuitl>());
		else if (type == NPCID.Painter)
		{
			shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Canvas>(), false);
			shop.item[nextSlot++].SetDefaults(ModContent.ItemType<FloppaPainting>(), false);
			shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SatchelReward>(), false);

			if (ModContent.GetInstance<SpiritNPCs.StarjinxEvent.StarjinxEventWorld>().StarjinxDefeated)
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ScrunklyPaintingItem>(), false);
		}
		else if (type == NPCID.Demolitionist)
		{
			shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LibertyItem>(), false);
			shop.item[nextSlot++].SetDefaults(ModContent.ItemType<Warhead>(), false);
			shop.item[nextSlot++].SetDefaults(ModContent.ItemType<ShortFuse>(), false);
			shop.item[nextSlot++].SetDefaults(ModContent.ItemType<LongFuse>(), false);
		}
		else if (type == NPCID.BestiaryGirl)
		{
			if (Main.BestiaryDB.GetCompletedPercentByMod(Mod) >= 0.5f)
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<CagedMoonlight>(), false);
			if (Main.BestiaryDB.GetCompletedPercentByMod(Mod) == 1)
				shop.item[nextSlot++].SetDefaults(ModContent.ItemType<SuspiciousLookingMeatballs>(), false);
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
