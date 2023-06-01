using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.SlagSet;
using SpiritMod.VerletChains;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;

namespace SpiritMod.NPCs.ChainedSinner
{
	internal class ChainedSinner : ModNPC
	{
		public Chain chain;

		public float angularMomentum = 0;

		private Vector2 arbitraryVelocity;
		private Vector2 cachedVel;
		private Vector2 spawnPos;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Furnace Maw");
			NPCHelper.ImmuneTo(this, BuffID.Poisoned);
		}

		public override void SetDefaults()
		{
			NPC.width = 56;
			NPC.height = 74;
			NPC.aiStyle = -1;
			NPC.knockBackResist = 0;
			NPC.lifeMax = 175;
			NPC.damage = 60;
			NPC.defense = 20;
			NPC.noTileCollide = true;
			NPC.lavaImmune = true;
			NPC.HitSound = SoundID.NPCHit41;
			NPC.DeathSound = SoundID.NPCDeath27;
		}

		public override void OnSpawn(IEntitySource source)
		{
			const int Distance = 8;

			int x = (int)(NPC.Center.X / 16f);
			int y = (int)(NPC.Center.Y / 16f);
			List<Point16> points = new();

			for (int i = x - Distance; i < x + Distance; ++i)
			{
				for (int j = y - Distance; j < y + Distance; ++j)
				{
					Tile tile = Main.tile[i, j];

					if (tile.HasTile && (tile.TileType == TileID.Ash || tile.TileType == TileID.ObsidianBrick || tile.TileType == TileID.HellstoneBrick))
						points.Add(new(i, j));
				}
			}

			if (points.Count == 0)
			{
				NPC.active = false;
				return;
			}

			NPC.Center = Main.rand.Next(points).ToVector2() * 16;
			NPC.netUpdate = true;
			InitializeChain(NPC.Center);
		}

		public override void AI()
		{
			const int LoopLength = 120;

			NPC.TargetClosest(true);
			NPC.ai[0]++;

			if (chain is null)
				InitializeChain(NPC.Center);

			chain.Update(spawnPos - new Vector2(0, 1), NPC.Center);

			float X = (float)Math.Sin(NPC.ai[0] / 50f);

			Player player = Main.player[NPC.target];

			if (NPC.ai[0] % LoopLength < 30)
			{
				if (NPC.ai[0] % LoopLength == 1)
					cachedVel = Vector2.Normalize(player.Center - NPC.Center);

				if (NPC.ai[0] % LoopLength > 1)
					arbitraryVelocity += cachedVel / 1.5f;
			}
			else if (NPC.ai[0] % LoopLength > LoopLength / 3)
			{
				float timeSince = NPC.ai[0] % LoopLength - (LoopLength / 3);
				arbitraryVelocity = new Vector2(X, 0) * timeSince / ((LoopLength / 3) * 0.75f);
			}
			else
				arbitraryVelocity *= 0.42f;

			chain.LastVertex.Position += arbitraryVelocity;
			NPC.Center = chain.LastVertex.Position;
		}

		public void InitializeChain(Vector2 position)
		{
			chain = new Chain(16, 16, position, new ChainPhysics(0.95f, 0.5f, 0.4f), true, false);
			spawnPos = position;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.AddCommon<CarvedRock>(1, 4, 9);
			npcLoot.AddCommon(ItemID.Chain, 8, 2, 4);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (!NPC.IsABestiaryIconDummy)
				chain.Draw(spriteBatch, Mod.Assets.Request<Texture2D>("NPCs/ChainedSinner/ChainedSinner_Chain").Value);
			return true;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
			{
				for (int i = 0; i < 3; ++i)
				Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, Mod.Find<ModGore>("ChainedSinner" + i).Type);

				foreach (var item in chain.VerticesArray())
					Gore.NewGoreDirect(NPC.GetSource_Death(), item, Vector2.Zero, Mod.Find<ModGore>("ChainedSinnerChain").Type);
			}
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
				new FlavorTextBestiaryInfoElement("We forge the chains we bear in life and in death."),
			});
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) 
		{
			if (spawnInfo.SpawnTileY < Main.maxTilesY - 160 && spawnInfo.SpawnTileType == TileID.Ash || spawnInfo.SpawnTileType == TileID.ObsidianBrick || spawnInfo.SpawnTileType == TileID.HellstoneBrick)
				return NPC.downedBoss3 ? SpawnCondition.Underworld.Chance * 0.9f : 0f;
			return 0;
		}
	}
}