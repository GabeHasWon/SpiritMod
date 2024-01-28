using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod
{
	public static class NPCUtils
	{
		public static bool CanDamage(this NPC npc) => npc.lifeMax > 5 && !npc.friendly;

		public static bool CanLeech(this NPC npc) => npc.lifeMax > 5 && !npc.friendly && !npc.dontTakeDamage && !npc.immortal;

		public static bool CanDropLoot(this NPC npc) => npc.lifeMax > 5 && npc.value > 0 && !npc.friendly && !npc.SpawnedFromStatue;

		/*public static void AddItem(ref Chest shop, ref int nextSlot, int item, int price = -1, bool check = true)
		{
			if (check)
			{
				shop.item[nextSlot].SetDefaults(item);
				if (price >= 0)
					shop.item[nextSlot].shopCustomPrice = price;

				nextSlot++;
			}
		}*/

		public static int ToActualDamage(float damageValue, float expertScaling = 1, float masterScaling = 1)
		{
			if (Main.masterMode)
				damageValue = damageValue / 6 * masterScaling;
			else if (Main.expertMode)
				damageValue = damageValue / 4 * expertScaling;
			else
				damageValue /= 2;

			return (int)damageValue;
		}

		public static void PlayDeathSound(this NPC npc, string Sound)
		{
			if (Main.netMode != NetmodeID.Server && npc.ModNPC != null)
			{
				Main.musicFade[npc.ModNPC.Music] = 0.3f;
				//float temp = Main.soundVolume; //temporarily store main.soundvolume, since sounds dont play at all if sound volume is at 0, regardless of actual volume of the sound
				//Main.soundVolume = (temp == 0) ? 1 : Main.soundVolume;
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/DeathSounds/" + Sound), npc.Center);
				//Main.soundVolume = temp;
			}
			if (Main.netMode == NetmodeID.Server && npc.ModNPC != null)
			{
				ModPacket packet = SpiritMod.Instance.GetPacket(MessageType.PlayDeathSoundFromServer, 2);
				packet.Write((ushort)npc.whoAmI);
				packet.Write(Sound);
				packet.Send();
			}
		}
	}
}
