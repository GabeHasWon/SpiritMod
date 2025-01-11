using Microsoft.Xna.Framework;
using SpiritMod.Mechanics.QuestSystem.Tasks;
using SpiritMod.Mechanics.QuestSystem;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace SpiritMod.GlobalClasses;

public class InfoDisplayModifications : GlobalInfoDisplay
{
	public override void ModifyDisplayParameters(InfoDisplay currentDisplay, ref string displayValue, ref string displayName, ref Color displayColor, ref Color displayShadowColor)
	{
		NPC storedNPC = null;

		if (currentDisplay == InfoDisplay.LifeformAnalyzer)
		{
			HashSet<int> validTypes = [];

			foreach (var quest in QuestManager.ActiveQuests)
			{
				if (quest.CurrentTask is SlayTask slay) //This NPC is part of a slay quest
				{
					foreach (int item in slay.MonsterIDs)
						validTypes.Add(item);
				}
				else if (quest.CurrentTask is BranchingTask branch)
				{
					foreach (var subTask in branch.Tasks) // Or a branched quest has a slay quest on it
						if (subTask is SlayTask slayTask)
							foreach (int item in slayTask.MonsterIDs)
								validTypes.Add(item);
				}
			}

			foreach (NPC npc in Main.ActiveNPCs) // Checks if any NPC fits any quest NPC - if there's more than one, take the rarer one
			{
				if (npc.CanBeChasedBy() && validTypes.Contains(npc.type) && (storedNPC is null || npc.rarity > storedNPC.rarity))
					storedNPC = npc;
			}

			if (storedNPC is not null)
				displayValue = $"({QuestManager.LocalizationValue("Quest")}) " + storedNPC.GivenOrTypeName;
		}

		if (currentDisplay == InfoDisplay.LifeformAnalyzer && storedNPC is not null)
			displayColor = Color.Orange;
	}
}
