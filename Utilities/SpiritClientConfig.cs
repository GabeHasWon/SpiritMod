using SpiritMod.World;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader.Config;

namespace SpiritMod.Utilities;

class SpiritClientConfig : ModConfig
{
	public override ConfigScope Mode => ConfigScope.ClientSide;

	[Range(0f, 1f)]
	[Increment(.01f)]
	[DefaultValue(1f)]
	[Slider]
	[Header("Visuals")]
	public float ScreenShake { get; set; }

    [DefaultValue(true)]
    public bool DistortionConfig { get; set; }

	[DefaultValue(true)]
	public bool ForegroundParticles { get; set; }

	[DefaultValue(true)]
	public bool AuroraEnabled { get; set; }

	[Header("QoL")]
	[DefaultValue(true)]
	public bool AutoReuse { get; set; }

	[DefaultValue(true)]
	public bool QuickSell { get; set; }

	[Header("Ambience")]
	[DefaultValue(true)]
    public bool AmbientSounds { get; set; }

	[DefaultValue(true)]
	public bool LeafFall { get; set; }

	[Header("Quests")]
	[DefaultValue(QuestUtils.QuestInvLocation.Minimap)]
	[DrawTicks]
	public QuestUtils.QuestInvLocation QuestBookLocation { get; set; }

	[DefaultValue(true)]
	public bool QuestBookSwitching { get; set; }

	[DefaultValue(true)]
	public bool ShowNPCQuestNotice { get; set; }

	[Header("Generation")]
	[DefaultValue(false)]
	public bool DoubleHideoutGeneration { get; set; }

	[DefaultValue(true)]
	public bool EnableSepulchres { get; set; }

	[DefaultValue(false)]
	public bool ForceClassicZiggurat { get; set; }


	[Header("Oceans")]
	[DefaultValue(OceanGeneration.OceanShape.Piecewise_V)]
	public OceanGeneration.OceanShape OceanShape { get; set; }

	[DefaultValue(true)]
	public bool VentCritters { get; set; }

	[ReloadRequired]
	[DefaultValue(true)]
	public bool SurfaceWaterTransparency { get; set; }

	[Header("Fishing")]
	[ReloadRequired]
	[DefaultValue(true)]
	public bool EnemyFishing { get; set; }

	[OnDeserialized]
	internal void OnDeserializedMethod(StreamingContext context) => ScreenShake = Utils.Clamp(ScreenShake, 0f, 1f);
}