﻿using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Utilities.ModCompatibility.PhaseIndicatorCompat;

internal class PhaseIndicatorLoader : ILoadable
{
	public void Load(Mod spirit)
	{
		if (!ModLoader.TryGetMod("PhaseIndicator", out Mod phase))
			return;

		var types = typeof(SpiritMod).Assembly.GetTypes(); //Add every accessory & timered item to this dict
		foreach (var type in types)
		{
			if (typeof(ModNPC).IsAssignableFrom(type) && Attribute.IsDefined(type, typeof(PhaseIndicatorAttribute)))
			{
				PhaseIndicatorAttribute att = (PhaseIndicatorAttribute)Attribute.GetCustomAttribute(type, typeof(PhaseIndicatorAttribute));

				int npcType = spirit.Find<ModNPC>(type.Name).Type;
				float[] phases = att.Phases;
				Texture2D tex = att.Indicator;

				for (int i = 0; i < phases.Length; ++i)
				{
					float curPhase = phases[i];

					if (tex is not null)
						phase.Call("PhaseIndicator", npcType, (NPC npc, float difficulty) => curPhase, tex);
					else
						phase.Call("PhaseIndicator", npcType, (NPC npc, float difficulty) => curPhase);
				}
			}
		}
	}

	public void Unload() { }
}
