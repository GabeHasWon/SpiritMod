using SpiritMod.World;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader.Config;

namespace SpiritMod.Utilities
{
	class SpiritClientConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[Range(0f, 1f)]
		[Increment(.01f)]
		[DefaultValue(1f)]
		[Slider]
		public float ScreenShake { get; set; }

        [DefaultValue(true)]
        public bool DistortionConfig { get; set; }

		[DefaultValue(true)]
		public bool ForegroundParticles { get; set; }

		[DefaultValue(true)]
		public bool AutoReuse { get; set; }

		[DefaultValue(true)]
		public bool QuickSell { get; set; }

        [DefaultValue(true)]
        public bool AmbientSounds { get; set; }

		[DefaultValue(true)]
		public bool LeafFall { get; set; }

		[DefaultValue(QuestUtils.QuestInvLocation.Minimap)]
		[DrawTicks]
		public QuestUtils.QuestInvLocation QuestBookLocation { get; set; }

		[DefaultValue(true)]
		public bool QuestBookSwitching { get; set; }

		/*[Label("Town NPC Portraits")]
		[Tooltip("Enables the showing of NPC portraits when talking to a Town NPC")]
		[DefaultValue(true)]
		public bool ShowNPCPortraits { get; set; }*/

		[DefaultValue(true)]
		public bool ShowNPCQuestNotice { get; set; }

		[DefaultValue(false)]
		public bool DoubleHideoutGeneration { get; set; }

		[DefaultValue(OceanGeneration.OceanShape.Piecewise_V)]
		public OceanGeneration.OceanShape OceanShape { get; set; }

		[DefaultValue(true)]
		public bool VentCritters { get; set; }

		[ReloadRequired]
		[DefaultValue(true)]
		public bool SurfaceWaterTransparency { get; set; }

		[ReloadRequired]
		[DefaultValue(true)]
		public bool EnemyFishing { get; set; }

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context) => ScreenShake = Utils.Clamp(ScreenShake, 0f, 1f);
	}
}