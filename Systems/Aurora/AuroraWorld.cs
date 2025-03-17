using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using System.IO;

namespace SpiritMod.Systems.Aurora;

public class AuroraWorld : ModSystem
{
	public const byte Default = byte.MaxValue;

	/// <summary> Checks whether a natural aurora is selected (per <see cref="AuroraType"/>).<para/>
	/// <see cref="AuroraPlayer.AuroraActive"/> is more in-depth and should be used instead when checking for active auroras. </summary>
	public static bool AuroraActive => AuroraType != Default;

	/// <summary> A randomly-selected aurora localized to specific biomes or locations. </summary>
	public static byte AuroraType { get; private set; } = Default;

	public override void Load() => TimeSystem.TimeChanged += UpdateAurora;

	private static void UpdateAurora(bool day)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
			return;

		AuroraType = Default;

		if (!day && Main.rand.NextBool(3))
			AuroraType = (byte)Main.rand.Next([1, 2, 3, 5]);
	}

	public override void ResetNearbyTileEffects() => Main.LocalPlayer.GetModPlayer<AuroraPlayer>().Reset();

	public override void NetSend(BinaryWriter writer) => writer.Write(AuroraType);
	public override void NetReceive(BinaryReader reader) => AuroraType = reader.ReadByte();
}
