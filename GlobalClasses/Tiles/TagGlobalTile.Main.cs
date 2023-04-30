using SpiritMod.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace SpiritMod.GlobalClasses.Tiles;

public partial class TagGlobalTile : GlobalTile
{
	public static List<int> Indestructibles => Instance._indestructibles;
	public static List<int> IndestructiblesUngrounded => Instance._indestructiblesUngrounded;
	public static List<int> HarvestableHerbs => Instance._harvestableHerbs;

	private static TagGlobalTile Instance => ModContent.GetInstance<TagGlobalTile>();

	private List<int> _indestructibles = new();
	private List<int> _indestructiblesUngrounded = new();
	private List<int> _harvestableHerbs = new();

	public void Load(Mod mod)
	{
		var types = typeof(TagGlobalTile).Assembly.GetTypes();
		foreach (var type in types)
		{
			if (typeof(ModTile).IsAssignableFrom(type))
			{
				var tag = (TileTagAttribute)Attribute.GetCustomAttribute(type, typeof(TileTagAttribute));

				if (tag == null || tag.Tags.Length == 0)
					continue;

				int id = mod.Find<ModTile>(type.Name).Type;

				if (tag.Tags.Contains(TileTags.Indestructible))
					_indestructibles.Add(id);

				if (tag.Tags.Contains(TileTags.IndestructibleNoGround))
					_indestructiblesUngrounded.Add(id);

				if (tag.Tags.Contains(TileTags.HarvestableHerb))
				{
					if (!typeof(IHarvestableHerb).IsAssignableFrom(type))
						throw new InvalidCastException($"Tile with {nameof(TileTagAttribute)} does not inherit {nameof(IHarvestableHerb)}!");

					_harvestableHerbs.Add(id);
				}
			}
		}
	}
}
