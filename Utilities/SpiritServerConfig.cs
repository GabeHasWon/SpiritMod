using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SpiritMod.Utilities;

class SpiritServerConfig : ModConfig
{
	public override ConfigScope Mode => ConfigScope.ServerSide;

	[DefaultValue(true)]
	public bool LuminousOceanEnabled { get; set; }
}