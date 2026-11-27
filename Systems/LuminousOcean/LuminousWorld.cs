using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria.ModLoader.IO;
using SpiritMod.Utilities;

namespace SpiritMod.Systems.LuminousOcean;

public class LuminousWorld : ModSystem
{
	public const byte DEFAULT_LUMINOUS_VALUE = byte.MaxValue;

	public const int GREEN = 0;
	public const int BLUE = 1;
	public const int PURPLE = 2;

	public static bool LuminousOceanActive => LuminousType != DEFAULT_LUMINOUS_VALUE;
	public static byte LuminousType { get; private set; } = DEFAULT_LUMINOUS_VALUE;

	public override void Load() => TimeSystem.TimeChanged += UpdateLuminousOcean;

	private static void UpdateLuminousOcean(bool day)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
			return;

		LuminousType = DEFAULT_LUMINOUS_VALUE;

		if (!day && Main.rand.NextBool(6) && ModContent.GetInstance<SpiritServerConfig>().LuminousOceanEnabled)
		{
			var color = new Color(251, 255, 230);
			LuminousType = (byte)Main.rand.Next(0, 3);

			if (Main.netMode == NetmodeID.SinglePlayer)
				Main.NewText(Language.GetTextValue("Mods.SpiritMod.Events.LuminousOcean.OnStart"), color);
			else if (Main.netMode == NetmodeID.Server)
				ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.SpiritMod.Events.LuminousOcean.OnStart"), color);
		}
	}

	public override void ClearWorld() => LuminousType = DEFAULT_LUMINOUS_VALUE;

	public override void NetSend(BinaryWriter writer) => writer.Write(LuminousType);
	public override void NetReceive(BinaryReader reader) => LuminousType = reader.ReadByte();

	public override void SaveWorldData(TagCompound tag) => tag[nameof(LuminousType)] = LuminousType;
	public override void LoadWorldData(TagCompound tag) => LuminousType = tag.GetByte(nameof(LuminousType));
}