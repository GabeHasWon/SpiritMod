using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using SpiritMod.Biomes;
using SpiritMod.Tiles.Walls.Natural;
using SpiritMod.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Sounds.GameContent
{
	/// <summary>
	/// Handles ambient sound loops, both starting, maintaining and stopping them.
	/// </summary>
	internal class AmbientSound : ModSystem
	{
		Dictionary<string, SlotId> soundSlots = new();

		public override void PostUpdateEverything()
		{
			if (Main.dedServ || Main.netMode == NetmodeID.Server)
				return;

			Player player = Main.LocalPlayer;
			MyPlayer spirit = player.GetSpiritPlayer();

			var config = ModContent.GetInstance<SpiritClientConfig>();

			if (config.AmbientSounds)
			{
				//Nighttime Ambience
				bool nightTimeCondition = !Main.dayTime && !player.ZoneSnow && player.ZoneOverworldHeight && !Main.dayTime
					&& !player.ZoneCorrupt && !player.ZoneCrimson && !player.ZoneJungle && !player.ZoneBeach
					&& !player.ZoneHallow && !player.ZoneDesert && !Main.raining
					&& !Main.bloodMoon && !BiomeTileCounts.InBriar && !BiomeTileCounts.InSpirit;

				UpdateSingleSound("nightAmbience", "SpiritMod/Sounds/NighttimeAmbience", 8f, 0.005f, nightTimeCondition);

				bool desertWind = player.ZoneDesert && player.ZoneOverworldHeight && !Terraria.GameContent.Events.Sandstorm.Happening && !Main.raining && !player.ZoneBeach;
				UpdateSingleSound("desertWind", "SpiritMod/Sounds/DesertWind", 5f, 0.005f, desertWind);

				bool lightWind = (spirit.ZoneReach || player.ZoneJungle) && player.ZoneOverworldHeight && !Main.raining;
				UpdateSingleSound("lightWind", "SpiritMod/Sounds/LightWind", 5f, 0.005f, lightWind);

				UpdateSingleSound("caveAmbience", "SpiritMod/Sounds/CaveAmbience", 5f, 0.0005f, player.ZoneRockLayerHeight);

				bool spookyAmbience = player.ZoneDungeon || (player.ZoneRockLayerHeight && Framing.GetTileSafely(player.Center.ToTileCoordinates()).WallType == ModContent.WallType<SepulchreWallTile>() && 
					Framing.GetTileSafely((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f) + 1).WallType == ModContent.WallType<SepulchreWallTile>());
				UpdateSingleSound("spookyAmbience", "SpiritMod/Sounds/SpookyAmbience", 1.5f, 0.02f, spookyAmbience); //Cave ambience

				bool submerged = spirit.Submerged(20, out int realDepth);
				UpdateSingleSound("underwaterAmbience", "SpiritMod/Sounds/UnderwaterAmbience", 1f, 0.08f, realDepth > 10);

				bool beachCondition = player.ZoneBeach && player.ZoneOverworldHeight;
				UpdateSingleSound("wavesAmbience", "SpiritMod/Sounds/WavesAmbience", 1f * (submerged ? 1 - (realDepth / 20f) : 1f), 0.01f, beachCondition);
			}
		}

		private void UpdateSingleSound(string key, string path, float maxVol, float lerpFactor, bool condition)
		{
			if (condition)
			{
				if (!soundSlots.ContainsKey(key))
					soundSlots.Add(key, SoundEngine.PlaySound(new SoundStyle(path) with { IsLooped = true, Volume = 0.1f, Type = SoundType.Ambient }));

				if (SoundEngine.TryGetActiveSound(soundSlots[key], out ActiveSound sound) && sound is not null)
					sound.Volume = MathHelper.Lerp(sound.Volume, maxVol, lerpFactor);
				else
					soundSlots[key] = SoundEngine.PlaySound(new SoundStyle(path) with { IsLooped = true, Volume = 0.1f, Type = SoundType.Ambient });
			}
			else
			{
				if (soundSlots.ContainsKey(key))
				{
					if (SoundEngine.TryGetActiveSound(soundSlots[key], out ActiveSound sound) && sound is not null)
					{
						sound.Volume = MathHelper.Lerp(sound.Volume, 0, lerpFactor);

						if (sound.Volume <= 0.001f)
						{
							sound.Stop();
							soundSlots.Remove(key);
						}
					}
				}
			}
		}
	}
}
