using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Microsoft.Xna.Framework;

namespace SpiritMod.Tiles.Ambient.IceSculpture.Hostile;

internal class FrozenSpawner
{
	public static void SpawnFrozenEnemy(int i, int j, int type, float distance = 72)
	{
		Player player = Main.LocalPlayer;
		float dist = Vector2.Distance(new Vector2(i * 16, j * 16), player.Center);

		if (dist >= distance)
			return;

		SoundEngine.PlaySound(SoundID.Item27);

		int n = NPC.NewNPC(new Terraria.DataStructures.EntitySource_TileUpdate(i, j), i * 16 + Main.rand.Next(-10, 10), j * 16, type, 0, 0, 0, 0, 0, Main.myPlayer);
		Main.npc[n].GivenName = Language.GetTextValue("Mods.SpiritMod.Frozen") + " " + Lang.GetNPCNameValue(type);
		Main.npc[n].lifeMax = (int)(Main.npc[n].lifeMax * 1.5f);
		Main.npc[n].life = Main.npc[n].lifeMax;
		Main.npc[n].knockBackResist /= 2;
		Main.npc[n].netUpdate = true;
		WorldGen.KillTile(i, j);
	}
}
