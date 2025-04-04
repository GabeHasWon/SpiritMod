using SpiritMod.NPCs.Tides.Tide;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SpiritMod.Mechanics.QuestSystem;
using SpiritMod.Mechanics.PortraitSystem;
using SpiritMod.GlobalClasses.Items;
using SpiritMod.Utilities;

namespace SpiritMod;

public partial class SpiritMod : Mod
{
	public override object Call(params object[] args)
	{
		if (args.Length < 1)
		{
			var stack = new System.Diagnostics.StackTrace(true);
			Logger.Error("Call Error: No arguments given:\n" + stack.ToString());
			return null;
		}

		CallContext context;
		int? contextNum = args[0] as int?;
		if (contextNum.HasValue)
			context = (CallContext)contextNum.Value;
		else
			context = ParseCallName(args[0] as string);

		if (context == CallContext.Invalid && !contextNum.HasValue)
		{ //Check if it has a valid value
			var stack = new System.Diagnostics.StackTrace(true);
			Logger.Error("Call Error: Context invalid or null:\n" + stack.ToString());
			return null;
		}

		if (context <= CallContext.Invalid || context >= CallContext.Limit)
		{ //Check if value is in-bounds
			var stack = new System.Diagnostics.StackTrace(true);
			Logger.Error("Call Error: Context invalid:\n" + stack.ToString());
			return null;
		}

		try
		{
			if (context == CallContext.Downed) //Gets if a boss has been downed
				return BossDowned(args);
			else if (context == CallContext.GlyphGet) //Gets an item's glyph
				return GetGlyph(args);
			else if (context == CallContext.GlyphSet) //Sets an item's glyph
			{
				SetGlyph(args);
				return null;
			}
			else if (context == CallContext.AddQuest) //Adds a quest
				//return QuestManager.ModCallAddQuest(args);
				throw new InvalidOperationException("Crossmod quests have been removed for the time being. Sorry! Will be replaced or fixed in the future.");
			else if (context == CallContext.UnlockQuest) //Unlocks a quest
			{
				QuestManager.ModCallUnlockQuest(args);
				return null;
			}
			else if (context == CallContext.GetQuestIsUnlocked) //Self explanatory until...
				return QuestManager.ModCallGetQuestValueFromContext(args, 0);
			else if (context == CallContext.GetQuestIsCompleted)
				return QuestManager.ModCallGetQuestValueFromContext(args, 2);
			else if (context == CallContext.GetQuestIsActive)
				return QuestManager.ModCallGetQuestValueFromContext(args, 1);
			else if (context == CallContext.GetQuestRewardsGiven) //...here
				return QuestManager.ModCallGetQuestValueFromContext(args, 3);
			else if (context == CallContext.Portrait) //Adds a new portrait from another mod
			{
				PortraitManager.ModCallAddPortrait(args);
				return null;
			}
			else if (context == CallContext.Events) //Gets or sets event bools
				return EventCall(args);
			else if (context == CallContext.AddItemDefinition)
			{
				if (args[1] is int from && args[2] is int to)
					ContentDefinition.ItemDefinitions.Add(from, to);
			}
		}
		catch (Exception e)
		{
			Logger.Error("Call Error: " + e.Message + "\n" + e.StackTrace);
		}
		return null;
	}

	private object EventCall(object[] args)
	{
		if (args[1] is not bool get)
		{
			var stack = new System.Diagnostics.StackTrace(true);
			Logger.Error("Call Error: Invalid argument for Event call:\n" + stack.ToString());
			return null;
		}

		if (get)
			return GetEventFromCall(args[2]);
		else
			return SetEventFromCall(args[2], args[3], args.Length > 4 ? args[4] : null);
	}


	private static bool? GetEventFromCall(object nameVal)
	{
		if (nameVal is not string name)
			return null;

		return name.ToUpper() switch
		{
			"THETIDE" => TideWorld.TheTide,
			"TIDE" => TideWorld.TheTide,
			"CALMNIGHT" => MyWorld.calmNight,
			"BLUEMOON" => MyWorld.blueMoon,
			_ => null,
		};
	}

	private static bool? SetEventFromCall(object nameVal, object valueVal, object optionalVal)
	{
		if (nameVal is not string name)
			return null;

		if (valueVal is not bool value)
			return null;

		name = name.ToUpper();

		if (name == "THETIDE" || name == "TIDE")
		{
			if (optionalVal is not bool additional)
				additional = false;

			if (!value && TideWorld.TheTide)
			{
				TideWorld.TideWave = 5;
				TideWorld.TideWaveIncrease(additional);
				return true;
			}
			else if (value && !TideWorld.TheTide)
			{
				if (Main.netMode == NetmodeID.MultiplayerClient)
					WriteToPacket(Instance.GetPacket(), (byte)MessageType.StartTide).Send();
				else
				{
					TideWorld.TheTide = true;
					TideWorld.TideWaveIncrease();
				}
				return true;
			}
			return false;
		}
		else if (name == "CALMNIGHT")
		{
			bool oldCalmNight = MyWorld.calmNight;
			MyWorld.calmNight = value;
			return oldCalmNight != value;
		}
		else if (name == "BLUEMOON")
		{
			bool oldBlueMoon = MyWorld.blueMoon;
			MyWorld.blueMoon = value;
			return oldBlueMoon != value;
		}
		return null;
	}

	private static CallContext ParseCallName(string context)
	{
		if (context == null)
			return CallContext.Invalid;

		return context switch
		{
			"downed" => CallContext.Downed,
			"getGlyph" => CallContext.GlyphGet,
			"setGlyph" => CallContext.GlyphSet,
			"AddQuest" => CallContext.AddQuest,
			"UnlockQuest" => CallContext.UnlockQuest,
			"IsQuestUnlocked" => CallContext.GetQuestIsUnlocked,
			"IsQuestActive" => CallContext.GetQuestIsActive,
			"IsQuestCompleted" => CallContext.GetQuestIsCompleted,
			"QuestRewardsGiven" => CallContext.GetQuestRewardsGiven,
			"Portrait" => CallContext.Portrait,
			"AddItemDefinition" => CallContext.AddItemDefinition,
			_ => CallContext.Invalid,
		};
	}

	private static bool BossDowned(object[] args)
	{
		if (args.Length < 2)
			throw new ArgumentException("No boss name specified");

		string name = args[1] as string;

		return name switch
		{
			"Scarabeus" => MyWorld.DownedScarabeus,
			"Moon Jelly Wizard" => MyWorld.DownedMoonWizard,
			"Vinewrath Bane" => MyWorld.DownedVinewrath,
			"Ancient Avian" => MyWorld.DownedAncientAvian,
			"Starplate Raider" => MyWorld.DownedStarplate,
			"Infernon" => MyWorld.DownedInfernon,
			"Dusking" => MyWorld.DownedDusking,
			"Atlas" => MyWorld.DownedAtlas,
			_ => throw new ArgumentException("Invalid boss name:" + name),
		};
	}

	private static void SetGlyph(object[] args)
	{
		if (args.Length < 2)
			throw new ArgumentException("Missing argument: Item");
		else if (args.Length < 3)
			throw new ArgumentException("Missing argument: Glyph");
		if (args[1] is not Item item)
			throw new ArgumentException("First argument must be of type Item");
		int? glyphID = args[2] as int?;
		if (!glyphID.HasValue)
			throw new ArgumentException("Second argument must be of type int");
		GlyphType glyph = (GlyphType)glyphID;
		if (glyph < GlyphType.None || glyph >= GlyphType.Count)
			throw new ArgumentException("Glyph must be in range [" +
				(int)GlyphType.None + "," + (int)GlyphType.Count + ")");
		item.GetGlobalItem<GlyphGlobalItem>().SetGlyph(item, glyph);
	}

	private static int GetGlyph(object[] args)
	{
		if (args.Length < 2)
			throw new ArgumentException("Missing argument: Item");
		if (args[1] is not Item item)
			throw new ArgumentException("First argument must be of type Item");
		return (int)item.GetGlobalItem<GlyphGlobalItem>().Glyph;
	}
}

internal enum CallContext
{
	Invalid = -1,
	Downed,
	GlyphGet,
	GlyphSet,
	AddQuest,
	UnlockQuest,
	GetQuestIsUnlocked,
	GetQuestIsActive,
	GetQuestIsCompleted,
	GetQuestRewardsGiven,
	Portrait,
	Events,
	AddItemDefinition,
	Limit
}