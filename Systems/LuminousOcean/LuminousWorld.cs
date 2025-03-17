using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.IO;

namespace SpiritMod.Systems.LuminousOcean;

public class LuminousWorld : ModSystem
{
	public const byte Default = byte.MaxValue;

	public const int GREEN = 0;
	public const int BLUE = 1;
	public const int PURPLE = 2;

	public static bool LuminousOceanActive => LuminousType != Default;
	public static byte LuminousType { get; private set; } = Default;

	public override void Load() => TimeSystem.TimeChanged += UpdateLuminousOcean;

	private static void UpdateLuminousOcean(bool day)
	{
		if (Main.netMode == NetmodeID.MultiplayerClient)
			return;

		LuminousType = Default;

		if (!day && Main.rand.NextBool(6))
		{
			var color = new Color(251, 255, 230);
			LuminousType = (byte)Main.rand.Next(0, 3);

			if (Main.netMode == NetmodeID.SinglePlayer)
				Main.NewText(Language.GetTextValue("Mods.SpiritMod.Events.LuminousOcean.OnStart"), color.R, color.G, color.B);
			else if (Main.netMode == NetmodeID.Server)
				ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.SpiritMod.Events.LuminousOcean.OnStart"), color);
		}
	}

	public override void NetSend(BinaryWriter writer) => writer.Write(LuminousType);
	public override void NetReceive(BinaryReader reader) => LuminousType = reader.ReadByte();
}