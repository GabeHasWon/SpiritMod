using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace SpiritMod.Utilities.Wiki;

internal static class ItemWikiWriter
{
	internal static void WriteItems()
	{
		using var stream = new StreamWriter(WikiWriter.Path + "Items.txt");
		var items = WikiWriter.GetAllOf<ModItem>();

		foreach (var item in items)
		{

			try
			{
				string name = item.Name;
				int type = SpiritMod.Instance.Find<ModItem>(name).Type;
				Item itemInst = ContentSamples.ItemsByType[type];

				WriteSingleItem(stream, itemInst);
				stream.Write("\n");
			}
			catch
			{
				stream.Write("\n"); //Move to the next entry and continue
				continue;
			}
		}
	}

	private static void WriteSingleItem(StreamWriter writer, Item item)
	{
		string tooltip = "";
		for (int line = 0; line < item.ToolTip.Lines; ++line)
		{
			string text = item.ToolTip.GetLine(line);
			if (text != "")
				tooltip += text;
		}

		string damage = GetItemClass(item);

		var lines = new List<string>
			{
				$"\"{item.Name}\"", //"Name"
				tooltip != "" ? $"Tooltip: {tooltip}" : "", //Tooltip(s)
				damage != "None" && item.damage != 0 ? $"{item.damage} {GetItemClass(item)} damage" : "", //X Class damage
				item.knockBack != 0 ? $"{item.knockBack} knockback" : "", //X knockback
				item.crit != 0 ? $"{item.crit} critical strike chance" : item.damage == 0 ? "" : "4 critical strike chance", //X critical strike chance
				$"Use time/animation {item.useTime}/{item.useAnimation}",
				GetRarity(item)
			};

		foreach (string line in lines)
			if (line != "")
				writer.Write(line + "\t");
	}

	private static string GetRarity(Item item)
	{
		foreach (var field in typeof(ItemRarityID).GetFields())
		{
			if (field.FieldType == typeof(int) && (int)field.GetRawConstantValue() == item.rare)
				return field.Name;
		}
		return "";
	}

	private static string GetItemClass(Item item)
	{
		if (item.IsMelee())
			return "melee";
		else if (item.IsRanged())
			return "ranged";
		else if (item.IsSummon())
			return "summon";
		else if (item.IsMagic())
			return "magic";
		else if (item.IsThrown())
			return "thrown";
		return "None";
	}
}
