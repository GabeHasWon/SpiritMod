using Microsoft.Xna.Framework;
using SpiritMod.Mechanics.QuestSystem.Tasks;
using SpiritMod.Mechanics.QuestSystem;
using Terraria;
using Terraria.ModLoader;
using System.Linq;
using SpiritMod;

namespace SpiritMod.GlobalClasses
{
	public class InfoDisplayModifications : GlobalInfoDisplay
	{
		private static string _name = null;

		public override void ModifyDisplayValue(InfoDisplay currentDisplay, ref string displayValue)
		{
			if (currentDisplay == InfoDisplay.LifeformAnalyzer)
			{
				_name = null;

				for (int i = 0; i < Main.maxNPCs; ++i)
				{
					NPC npc = Main.npc[i];

					if (npc.CanBeChasedBy()) //This NPC is valid
					{
						foreach (var quest in QuestManager.ActiveQuests)
						{
							if (quest.CurrentTask is SlayTask slay && slay.MonsterIDs.Contains(npc.type)) //This NPC is part of a slay quest
								_name = npc.GivenOrTypeName;
							else if (quest.CurrentTask is BranchingTask branch && branch.Tasks.Any(x => x is SlayTask slayTask && slayTask.MonsterIDs.Contains(npc.type)))
								_name = npc.GivenOrTypeName;
						}
					}
				}

				if (_name is not null)
					displayValue = $"({QuestManager.LocalizationValue("Quest")}) " + _name;
			}
		}

		public override void ModifyDisplayColor(InfoDisplay currentDisplay, ref Color displayColor)
		{
			if (currentDisplay == InfoDisplay.LifeformAnalyzer && _name is not null)
				displayColor = Color.Orange;
		}
	}
}
