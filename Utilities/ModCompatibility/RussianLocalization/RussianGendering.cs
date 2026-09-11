using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritReforged.Common.ModCompat.LocalizationTools;

/// <summary>
/// Helper class for gendering support. Only works for Russian through tRU at the moment. Backported from Spirit Reforged.
/// </summary>
[ReinitializeDuringResizeArrays]
internal class RussianGendering : ModSystem
{
	internal static Dictionary<string, HashSet<int>> ItemGendersByGender = [];
	internal static string[] TypeMap = ItemID.Sets.Factory.CreateCustomSet("");

	/// <summary>
	/// Gets the gendering of an item ID. Options are "" (masculine/default), Feminine, Neuter (neutral), and Plural
	/// </summary>
	public static string GetGender(int id) => TypeMap[id];

	public override bool IsLoadingEnabled(Mod mod) => ModLoader.HasMod("CalamityRuTranslate");

	public override void PostSetupContent()
	{
		if (Language.ActiveCulture.Name != "ru-RU")
			return;

		// Get, find, remap and cache the gendering.
		ModLoader.TryGetMod("CalamityRuTranslate", out Mod russ);
		Type prefixOverhaul = russ.Code.GetType("CalamityRuTranslate.Core.ItemGenderPrefixes.PrefixOverhaul");
		FieldInfo genderCollections = prefixOverhaul.GetField("_genderCollections", BindingFlags.Instance | BindingFlags.NonPublic);
		PropertyInfo instanceProp = prefixOverhaul.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
		object instance = instanceProp.GetValue(null, null);
		dynamic dict = genderCollections.GetValue(instance);

		foreach (dynamic pair in dict)
		{
			string name = pair.Key.ToString();
			HashSet<int> value = pair.Value;

			ItemGendersByGender.Add(name, value);

			foreach (int val in value)
				TypeMap[val] = name;
		}
	}
}