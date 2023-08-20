using Terraria;
using Terraria.ModLoader;
using SpiritMod.NPCs;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Audio;
using SpiritMod.Tiles.Furniture.Bamboo;

namespace SpiritMod.Buffs.Tiles
{
	public class Impaled : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Impaled");
			// Description.SetDefault("You've been impaled by a pike. Ouch!");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = false;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			if (npc.knockBackResist <= 0f)
				return;

			Tile tile = Framing.GetTileSafely((int)(npc.Center.X / 16), (int)((npc.Center.Y + ((npc.height / 2) - 8)) / 16));

			if (tile.HasTile && tile.TileType == ModContent.TileType<BambooPikeTile>()) //Severe effects
			{
				if (npc.buffTime[buffIndex] % 30 == 0)
					SoundEngine.PlaySound(SoundID.NPCHit1 with { PitchVariance = .2f, Pitch = -.5f }, npc.Center);

				npc.GetGlobalNPC<GNPC>().slowDegree = 1f; //100% reduced speed
				npc.GetGlobalNPC<GNPC>().impaled = true;
				npc.lifeRegen = -10;
			}
			else
			{
				npc.GetGlobalNPC<GNPC>().slowDegree = .8f; //80% reduced speed
				npc.lifeRegen = 0;
			}

			if (Main.rand.NextBool(7))
			{
				Vector2 position = npc.position + new Vector2(npc.width * Main.rand.NextFloat(), npc.height * Main.rand.NextFloat());
				Dust.NewDustPerfect(position, DustID.Blood, Vector2.Zero, 0, default, Main.rand.NextFloat(0.5f, 1.2f));
			}
		}

		public override void Update(Player player, ref int buffIndex)
		{
			Tile tile = Framing.GetTileSafely((int)(player.Center.X / 16), (int)((player.Center.Y + ((player.height / 2) - 8)) / 16));

			if (tile.HasTile && tile.TileType == ModContent.TileType<BambooPikeTile>()) //Severe effects
			{
				if (player.buffTime[buffIndex] % 30 == 25)
					SoundEngine.PlaySound(SoundID.NPCHit1 with { PitchVariance = .2f, Pitch = -.5f }, player.Center);

				player.lifeRegen = -15;
				player.velocity = new Vector2(0, .25f);
				player.gravity = 0f;
			}
			else
			{
				player.lifeRegen = 0;
				player.velocity = new Vector2(player.velocity.X * .8f, (player.velocity.Y > 0) ? player.velocity.Y : player.velocity.Y * .8f);
			}

			if (Main.rand.NextBool(7))
			{
				Vector2 position = player.position + new Vector2(player.width * Main.rand.NextFloat(), player.height * Main.rand.NextFloat());
				Dust.NewDustPerfect(position, DustID.Blood, Vector2.Zero, 0, default, Main.rand.NextFloat(0.5f, 1.2f));
			}
		}
	}
}