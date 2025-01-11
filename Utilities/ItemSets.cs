using SpiritMod.Items.Consumable.Fish;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Utilities;

public class ItemSets : ModSystem
{
	public readonly static HashSet<int> Fish = [ ItemID.ArmoredCavefish, ItemID.AtlanticCod, ItemID.Bass, ItemID.BlueJellyfish, ItemID.ChaosFish, ItemID.CrimsonTigerfish, 
		ItemID.Damselfish, ItemID.DoubleCod, ItemID.Ebonkoi, ItemID.FlarefinKoi, ItemID.FrostMinnow, ItemID.GoldenCarp, ItemID.GreenJellyfish, ItemID.Hemopiranha, 
		ItemID.Honeyfin, ItemID.NeonTetra, ItemID.Obsidifish, ItemID.PrincessFish, ItemID.PinkJellyfish, ItemID.Prismite, ItemID.RedSnapper, ItemID.Salmon, ItemID.Shrimp, 
		ItemID.SpecularFish, ItemID.Stinkfish, ItemID.Trout, ItemID.Tuna, ItemID.VariegatedLardfish ];

	public readonly static HashSet<int> AllFish = new(Fish);

	public override void PostSetupContent()
	{
		AllFish.Add(ModContent.ItemType<RawFish>());
		AllFish.Add(ModContent.ItemType<CrystalFish>());
	}
}
