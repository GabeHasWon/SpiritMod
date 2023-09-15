using System.IO;
using System.Linq;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace SpiritMod.Utilities.Wiki;

internal static class NPCWikiWriter
{
	internal static void WriteNPCs()
	{
		using StreamWriter stream = new StreamWriter(WikiWriter.Path + "NPC.txt");
		var items = WikiWriter.GetAllOf<ModNPC>();

		stream.Write("\tMax Life\tDefense\tKnockback (Functional)\tCoins Dropped\tBestiary\n");

		foreach (var npc in items)
		{
			bool hasDoneAnything = false;

			try
			{
				string name = npc.Name;
				int type = SpiritMod.Instance.Find<ModNPC>(name).Type;
				NPC npcInstance = ContentSamples.NpcsByNetId[type];

				WriteSingleNPC(stream, npcInstance, ref hasDoneAnything);
				stream.Write("\n");
			}
			catch
			{
				if (hasDoneAnything)
					stream.Write("Broken Entry (Missing or Unloaded)\n"); //Move to the next entry and continue
				else
					stream.Write("\n");
				continue;
			}
		}
	}

	private static void WriteSingleNPC(StreamWriter stream, NPC npc, ref bool hasDoneAnything)
	{
		string name = Lang.GetNPCName(npc.type).Value + $" ({npc.ModNPC.Name})";
		stream.Write($"{name}\t{npc.lifeMax}\t{npc.defense}\t{npc.knockBackResist} ({npc.knockBackResist * 100:#0.#}%)\t");

		hasDoneAnything = true;

		if (npc.value > 0)
		{
			var coins = Utils.CoinsSplit((long)npc.value);

			if (coins[3] > 0)
				stream.Write($"{coins[3]}p");

			if (coins[2] > 0)
				stream.Write($"{coins[2]}g");

			if (coins[1] > 0)
				stream.Write($"{coins[1]}s");

			if (coins[0] > 0)
				stream.Write($"{coins[0]}s");
		}
		else
			stream.Write("n/a");

		stream.Write("\t");

		try
		{
			var flavorEntry = Main.BestiaryDB.FindEntryByNPCID(npc.type).Info.First(x => x is FlavorTextBestiaryInfoElement) as FlavorTextBestiaryInfoElement;
			stream.Write(flavorEntry.GetType().GetField("_key", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(flavorEntry));
		}
		catch
		{
			stream.Write("(No entry)");
		}
	}
}
