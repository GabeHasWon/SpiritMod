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

namespace SpiritMod.NPCs.ChainedSinner
{
	internal class ChainedSinner : ModNPC
	{
		public Chain chain;

		public float angularMomentum = 0;

		private Vector2 arbitraryVelocity;
		private Vector2 CachedVel;
		private Vector2 spawnPos;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Chained Sinner");

		public override void SetDefaults()
		{
			NPC.width = 56;
			NPC.height = 74;
			NPC.aiStyle = -1;
			NPC.knockBackResist = 0;
			NPC.lifeMax = 220;
			NPC.damage = 60;
			NPC.defense = 20;
			NPC.noTileCollide = true;
			NPC.lavaImmune = true;
			NPC.buffImmune[BuffID.OnFire] = true;
		}

		public override void OnSpawn(IEntitySource source)
		{
			int x = (int)(NPC.Center.X / 16f);
			int y = (int)(NPC.Center.Y / 16f);
			List<Point16> points = new();

			for (int i = x - 4; i < x + 4; ++i)
			{
				for (int j = y - 4; j < y + 4; ++j)
				{
					Tile tile = Main.tile[i, j];

					if (tile.HasTile && (tile.TileType == TileID.Ash || tile.TileType == TileID.ObsidianBrick || tile.TileType == TileID.HellstoneBrick))
						points.Add(new(i, j));
				}
			}

			NPC.Center = Main.rand.Next(points).ToVector2() * 16;
			InitializeChain(NPC.Center);
			NPC.netUpdate = true;
		}

		public override void AI()
		{
			const int LoopLength = 120;

			NPC.TargetClosest(true);
			NPC.ai[0]++;

			chain.Update(spawnPos - new Vector2(0, 1), NPC.Center);

			float X = (float)Math.Sin(NPC.ai[0] / 50f);

			Player player = Main.player[NPC.target];

			if (NPC.ai[0] % LoopLength < 30)
			{
				if (NPC.ai[0] % LoopLength == 1)
					CachedVel = Vector2.Normalize(player.Center - NPC.Center);

				if (NPC.ai[0] % LoopLength > 1)
					arbitraryVelocity += CachedVel / 1.5f;
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

		public override float SpawnChance(NPCSpawnInfo spawnInfo) 
		{
			if (spawnInfo.SpawnTileY < Main.maxTilesY - 160 && spawnInfo.SpawnTileType == TileID.Ash || spawnInfo.SpawnTileType == TileID.ObsidianBrick || spawnInfo.SpawnTileType == TileID.HellstoneBrick)
				return NPC.downedBoss3 ? SpawnCondition.Underworld.Chance * 2f : 0f;
			return 0;
		}
	}
}