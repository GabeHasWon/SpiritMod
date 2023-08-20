using System.Collections.Generic;
using Terraria.GameContent.Generation;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace SpiritMod.World.Micropasses;

internal class MicropassSystem : ModSystem
{
	public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
	{
		foreach (var micropass in ModContent.GetContent<Micropass>())
		{
			bool afterIndex = true;
			int index = micropass.GetWorldGenIndexInsert(tasks, ref afterIndex);

			if (index <= -1)
				continue;

			if (afterIndex)
				index++;

			tasks.Insert(index, new PassLegacy(micropass.WorldGenName, micropass.Run));
		}
	}
}
