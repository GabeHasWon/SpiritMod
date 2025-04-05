using System.Collections.Generic;
using Terraria.ModLoader;

namespace SpiritMod.Utilities;

/// <summary> Allows overriding various <see cref="ModType"/> definitions for cross-mod compatibility purposes. </summary>
public static class ContentDefinition
{
	public static readonly Dictionary<int, int> ItemDefinitions = [];

	public static int ItemType<T>() where T : ModItem => GetItemDefinition(ModContent.GetInstance<T>()?.Type ?? 0);

	/// <summary> Gets the variable definition of the given item type. </summary>
	public static int GetItemDefinition(int ofType)
	{
		if (ItemDefinitions.TryGetValue(ofType, out int newType))
			return newType;

		return ofType;
	}
}