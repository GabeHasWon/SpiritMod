using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items;
using SpiritMod.Items.Armor.BotanistSet;
using SpiritMod.Items.Material;
using SpiritMod.Items.Placeable;
using SpiritMod.Systems;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles
{
	[TileTag(TileTags.HarvestableHerb)]
	public class SoulBloomTile : ModTile, IHarvestableHerb
	{
		private const int FrameWidth = 18; // A constant for readability and to kick out those magic numbers

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileObsidianKill[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileNoFail[Type] = true;

			TileID.Sets.ReplaceTileBreakUp[Type] = true;
			TileID.Sets.IgnoredInHouseScore[Type] = true;
			TileID.Sets.IgnoredByGrowingSaplings[Type] = true;

			TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]);

			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Soulbloom");
			AddMapEntry(new Color(110, 158, 234), name);

			TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
			TileObjectData.newTile.AnchorValidTiles = new int[] { ModContent.TileType<Block.SpiritGrass>() };
			TileObjectData.newTile.AnchorAlternateTiles = new int[] { TileID.ClayPot, TileID.PlanterBox };
			TileObjectData.addTile(Type);

			HitSound = SoundID.Grass;
			DustType = DustID.Flare_Blue;
		}

		public bool CanBeHarvested(int i, int j) => Main.tile[i, j].HasTile && GetStage(i, j) == PlantStage.Grown;

		public override bool CanPlace(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			if (!tile.HasTile)
				return true;

			int tileType = tile.TileType;
			if (tileType == Type)
			{
				PlantStage stage = GetStage(i, j);
				return stage == PlantStage.Grown;
			}
			else
			{
				if (Main.tileCut[tileType] || TileID.Sets.BreakableWhenPlacing[tileType] || tileType == TileID.WaterDrip || tileType == TileID.LavaDrip || tileType == TileID.HoneyDrip || tileType == TileID.SandDrip)
				{
					bool foliageGrass = tileType == TileID.Plants || tileType == TileID.Plants2;
					bool moddedFoliage = tileType >= TileID.Count && (Main.tileCut[tileType] || TileID.Sets.BreakableWhenPlacing[tileType]);
					bool harvestableVanillaHerb = Main.tileAlch[tileType] && WorldGen.IsHarvestableHerbWithSeed(tileType, tile.TileFrameX / 18);

					if (foliageGrass || moddedFoliage || harvestableVanillaHerb)
					{
						WorldGen.KillTile(i, j);

						if (!tile.HasTile && Main.netMode == NetmodeID.MultiplayerClient)
							NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);

						return true;
					}
				}
				return false;
			}
		}

		private static SpriteEffects GetEffects(int i) => (i % 2 == 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) => GetEffects(i);
		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = -2;

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			TileSwaySystem.DrawGrassSway(spriteBatch, TextureAssets.Tile[Type].Value, i, j, Lighting.GetColor(i, j), GetEffects(i));
			return false;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch) => TileSwaySystem.DrawGrassSway(spriteBatch, Texture + "_Glow", i, j, Color.White * .5f, GetEffects(i));

		public override IEnumerable<Item> GetItemDrops(int i, int j)
		{
			PlantStage stage = GetStage(i, j);

			if (stage == PlantStage.Planted)
				yield break;

			Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();
			Player nearestPlayer = Main.player[Player.FindClosest(worldPosition, 16, 16)];

			int herbItemStack = 0;
			int seedItemStack = 0;
			Item item = nearestPlayer.HeldItem;

			if (nearestPlayer.active && item.type == ItemID.StaffofRegrowth || item.type == ItemID.AcornAxe) // Increased yields with Staff of Regrowth, even when not fully grown
			{
				if (stage == PlantStage.Grown)
					(herbItemStack, seedItemStack) = (2, Main.rand.Next(2, 5));
				else if (stage == PlantStage.Growing)
					seedItemStack = Main.rand.Next(1, 3);
			}
			else if (stage == PlantStage.Grown)
				(herbItemStack, seedItemStack) = (1, Main.rand.Next(1, 4));
			else if (stage == PlantStage.Growing)
				herbItemStack = 1;

			if (nearestPlayer.GetModPlayer<BotanistPlayer>().active && stage != PlantStage.Planted)
			{
				seedItemStack += 2;
				herbItemStack++;
			}

			if (herbItemStack > 0)
				yield return new Item(ModContent.ItemType<SoulBloom>()) { stack = herbItemStack };
			if (seedItemStack > 0)
				yield return new Item(ModContent.ItemType<SoulSeeds>()) { stack = seedItemStack };
		}

		public override bool IsTileSpelunkable(int i, int j) => GetStage(i, j) == PlantStage.Grown;

		public override void RandomUpdate(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			PlantStage stage = GetStage(i, j);

			if (stage == PlantStage.Planted) //Grow only if just planted
			{
				tile.TileFrameX += FrameWidth;

				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendTileSquare(-1, i, j, 1);
			}
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			bool shouldBloom = (Main.moonPhase == (int)MoonPhase.Empty && !Main.dayTime) || MyWorld.blueMoon;

			if (shouldBloom && GetStage(i, j) == PlantStage.Growing)
				SetStage(i, j, PlantStage.Grown);
			else if (!shouldBloom && GetStage(i, j) == PlantStage.Grown)
				SetStage(i, j, PlantStage.Growing);
		}

		// A helper method to quickly get the current stage of the herb (assuming the tile at the coordinates is our herb)
		private static PlantStage GetStage(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			return (PlantStage)(tile.TileFrameX / FrameWidth);
		}

		// A helper method to quickly set the current stage of the herb (assuming the tile at the coordinates is our herb)
		private static void SetStage(int i, int j, PlantStage stage)
		{
			Tile tile = Framing.GetTileSafely(i, j);
			tile.TileFrameX = (short)(FrameWidth * (int)stage);
		}
	}
}
