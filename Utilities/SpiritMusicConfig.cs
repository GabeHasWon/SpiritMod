using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SpiritMod.Utilities
{
	class SpiritMusicConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[DefaultValue(true)]
		public bool BlizzardMusic { get; set; }

		[DefaultValue(true)]
		public bool SnowNightMusic { get; set; }

		[DefaultValue(true)]
		public bool DesertNightMusic { get; set; }

        [DefaultValue(true)]
        public bool HallowNightMusic { get; set; }

		[DefaultValue(true)]
		public bool CorruptNightMusic { get; set; }

		[DefaultValue(true)]
		public bool CrimsonNightMusic { get; set; }

		[DefaultValue(true)]
		public bool GraniteMusic { get; set; }

		[DefaultValue(true)]
		public bool MarbleMusic { get; set; }

		[DefaultValue(true)]
		public bool UnderwaterMusic { get; set; }

        [DefaultValue(true)]
        public bool SpiderCaveMusic { get; set; }

		[DefaultValue(true)]
		public bool MeteorMusic { get; set; }

        [DefaultValue(true)]
        public bool FrostLegionMusic { get; set; }

		[DefaultValue(true)]
		public bool SkeletronPrimeMusic { get; set; }

		[DefaultValue(true)]
		public bool AuroraMusic { get; set; }

		[DefaultValue(true)]
		public bool LuminousMusic { get; set; }

        [DefaultValue(true)]
        public bool CalmNightMusic { get; set; }

        [DefaultValue(true)]
        public bool NeonBiomeMusic { get; set; }

		[DefaultValue(true)]
		public bool AshfallMusic { get; set; }

		[DefaultValue(true)]
		public bool VictoryDayMusic { get; set; }
	}
}