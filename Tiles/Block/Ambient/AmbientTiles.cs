using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpiritMod.Tiles.Block.Ambient
{
	public class MagmastoneItem : AmbientStoneItem<Magmastone_Tile>
	{
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(50);
			recipe.AddIngredient(ItemID.StoneBlock, 50);
			recipe.AddIngredient(ItemID.AshBlock, 1);
			recipe.AddTile(ModContent.TileType<Furniture.ForagerTableTile>());
			recipe.Register();

			Recipe recipe1 = Recipe.Create(ItemID.StoneBlock, 50);
			recipe1.AddIngredient(this, 50);
			recipe1.AddTile(ModContent.TileType<Furniture.ForagerTableTile>());
			recipe1.Register();
		}
	}

	public class Magmastone_Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type, true);
			DustType = DustID.Torch;
			AddMapEntry(new Color(79, 55, 59));
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.222f * 1.5f;
			g = .133f * 1.5f;
			b = .073f * 1.5f;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			int height = tile.TileFrameY == 36 ? 18 : 16;
			if (tile.Slope == 0 && !tile.IsHalfBlock)
				Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Tiles/Block/Ambient/Magmastone_Glow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), new Color(100, 100, 100, 100), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}
	}

	public class SmolderingRockItem : AmbientStoneItem<SmolderingRock_Tile>
	{
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(50);
			recipe.AddIngredient(ItemID.StoneBlock, 50);
			recipe.AddIngredient(ItemID.AshBlock, 1);
			recipe.AddTile(ModContent.TileType<Furniture.ForagerTableTile>());
			recipe.Register();

			Recipe recipe1 = Recipe.Create(ItemID.StoneBlock, 50);
			recipe1.AddIngredient(this, 50);
			recipe1.AddTile(ModContent.TileType<Furniture.ForagerTableTile>());
			recipe1.Register();
		}
	}

	public class SmolderingRock_Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type, true);
			HitSound = SoundID.Tink;
			AddMapEntry(new Color(79, 55, 59));
			DustType = DustID.Torch;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.222f * 1.5f;
			g = .133f * 1.5f;
			b = .073f * 1.5f;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			int height = tile.TileFrameY == 36 ? 18 : 16;
			if (tile.Slope == 0 && !tile.IsHalfBlock)
				Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Tiles/Block/Ambient/SmolderingRock_Glow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), new Color(100, 100, 100, 100), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}
	}

	public class CinderstoneItem : AmbientStoneItem<CinderstoneTile>
	{
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(50);
			recipe.AddIngredient(ItemID.StoneBlock, 50);
			recipe.AddIngredient(ItemID.AshBlock, 1);
			recipe.AddTile(ModContent.TileType<Furniture.ForagerTableTile>());
			recipe.Register();

			Recipe recipe1 = Recipe.Create(ItemID.StoneBlock, 50);
			recipe1.AddIngredient(this, 50);
			recipe1.AddTile(ModContent.TileType<Furniture.ForagerTableTile>());
			recipe1.Register();
		}
	}
	public class CinderstoneTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type, true);
			HitSound = SoundID.Tink;
			AddMapEntry(new Color(79, 55, 59));
			DustType = DustID.Torch;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.222f * 1.5f;
			g = .133f * 1.5f;
			b = .073f * 1.5f;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			int height = tile.TileFrameY == 36 ? 18 : 16;
			if (tile.Slope == 0 && !tile.IsHalfBlock)
				Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Tiles/Block/Ambient/Cinderstone_Glow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), new Color(100, 100, 100, 100), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}
	}

	public class MottledStoneItem : AmbientStoneItem<MottledStone_Tile>
	{
	}

	public class MottledStone_Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			HitSound = SoundID.Tink;
			AddMapEntry(new Color(60, 60, 60));
			DustType = DustID.Wraith;
		}
	}

	public class AzureGemBlockItem : AzureGemItem<AzureBlock_Tile>
	{
	}

	public class AzureBlock_Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type, true);
			HitSound = SoundID.Tink;
			AddMapEntry(new Color(79, 55, 59));
			DustType = DustID.Flare_Blue;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.052f * 1.5f;
			g = .128f * 1.5f;
			b = .235f * 1.5f;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			int height = tile.TileFrameY == 36 ? 18 : 16;
			if (tile.Slope == 0 && !tile.IsHalfBlock)
				Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Tiles/Block/Ambient/AzureBlock_Glow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), new Color(100, 100, 100, 100), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}
	}

	public class ObsidianBlockItem : AmbientStoneItem<ObsidianBlockTile>
	{
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(50);
			recipe.AddIngredient(ItemID.StoneBlock, 50);
			recipe.AddIngredient(ItemID.Obsidian, 1);
			recipe.AddTile(ModContent.TileType<Furniture.ForagerTableTile>());
			recipe.Register();

			Recipe recipe1 = Recipe.Create(ItemID.StoneBlock, 50);
			recipe1.AddIngredient(this, 50);
			recipe1.AddTile(ModContent.TileType<Furniture.ForagerTableTile>());
			recipe1.Register();
		}
	}
	public class ObsidianBlockTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type, true);
			HitSound = SoundID.Tink;
			AddMapEntry(new Color(60, 60, 60));
			DustType = DustID.Wraith;
		}
	}

	public class OldStoneItem : AmbientStoneItem<OldStone_Tile>
	{
	}

	public class OldStone_Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			HitSound = SoundID.Tink;
			AddMapEntry(new Color(74, 60, 50));
			DustType = DustID.Iron;
		}
	}

	public class OutlandStoneItem : AmbientStoneItem<OutlandStoneTile>
	{
	}

	public class OutlandStoneTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			HitSound = SoundID.Tink;
			AddMapEntry(new Color(74, 60, 50));
			DustType = DustID.Wraith;
		}
	}

	public class RuinstoneItem : AmbientStoneItem<Ruinstone_Tile>
	{
	}

	public class Ruinstone_Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			Main.tileBlendAll[Type] = true;
			HitSound = SoundID.Tink;
			AddMapEntry(new Color(115, 76, 48));
			DustType = DustID.Mud;
		}
	}

	public class VinestoneItem : AmbientStoneItem<Vinestone_Tile>
	{
	}

	public class Vinestone_Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			HitSound = SoundID.Tink;
			AddMapEntry(new Color(69, 74, 60));
			DustType = DustID.Mud;
		}
	}

	public class WornStoneItem : AmbientStoneItem<WornStone_Tile>
	{
	}

	public class WornStone_Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			HitSound = SoundID.Tink;
			AddMapEntry(new Color(100, 100, 100));
			DustType = DustID.Stone;
		}
	}

	public class IvyStoneItem : AmbientStoneItem<IvyStoneTile>
	{
	}

	public class IvyStoneTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			HitSound = SoundID.Tink;
			AddMapEntry(new Color(100, 100, 100));
			DustType = DustID.Stone;
		}
	}

	public class CorruptPustuleItem : AmbientCorruptItem<CorruptPustule_Tile>
	{
	}

	public class CorruptPustule_Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			AddMapEntry(new Color(80, 55, 82));
			DustType = DustID.CorruptPlants;
			Main.tileLighted[Type] = true;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.074f * 1.5f;
			g = .143f * 1.5f;
			b = .040f * 1.5f;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			Vector2 zero = new Vector2(Main.offScreenRange);

			if (Main.drawToScreen)
				zero = Vector2.Zero;

			int height = tile.TileFrameY == 36 ? 18 : 16;

			if (tile.Slope == 0 && !tile.IsHalfBlock)
				Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Tiles/Block/Ambient/CorruptPustule_Glow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), new Color(100, 100, 100, 60), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}
	}

	public class DarkFoliageItem : AmbientCorruptItem<DarkFoliageTile>
	{
	}

	public class DarkFoliageTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			HitSound = SoundID.Grass;
			AddMapEntry(new Color(80, 55, 82));
			DustType = DustID.CorruptPlants;
		}
	}

	public class CorruptOvergrowthItem : AmbientCorruptItem<CorruptOvergrowthTile>
	{
	}

	public class CorruptOvergrowthTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			AddMapEntry(new Color(80, 55, 82));
			DustType = DustID.CorruptPlants;
		}
	}

	public class CorruptTendrilItem : AmbientCorruptItem<CorruptTendrilTile>
	{
	}

	public class CorruptTendrilTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			AddMapEntry(new Color(80, 55, 82));
			DustType = DustID.CorruptPlants;
		}
	}

	public class CorruptMassItem : AmbientCorruptItem<CorruptMassTile>
	{
	}

	public class CorruptMassTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			AddMapEntry(new Color(80, 55, 82));
			DustType = DustID.CorruptPlants;
		}
	}

	public class StalactiteStoneItem : AmbientStoneItem<StalactiteStone_Tile>
	{
	}

	public class StalactiteStone_Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			HitSound = SoundID.Tink;
			AddMapEntry(new Color(100, 100, 100));
			DustType = DustID.Stone;
		}
	}

	public class CragstoneItem : AmbientStoneItem<CragstoneTile>
	{
	}

	public class CragstoneTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			HitSound = SoundID.Tink;
			AddMapEntry(new Color(100, 100, 100));
			DustType = DustID.Stone;
		}
	}

	public class FracturedStoneItem : AmbientStoneItem<FracturedStoneTile>
	{
	}

	public class FracturedStoneTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			HitSound = SoundID.Tink;
			AddMapEntry(new Color(100, 100, 100));
			DustType = DustID.Stone;
		}
	}

	public class LeafyDirtItem : AmbientLeafItem<LeafyDirtTile>
	{
	}

	public class LeafyDirtTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			AddMapEntry(new Color(115, 87, 62));
			DustType = DustID.Dirt;
		}
	}

	public class ForestFoliageItem : AmbientLeafItem<ForestFoliageTile>
	{
	}

	public class ForestFoliageTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			AddMapEntry(new Color(57, 128, 78));
			DustType = DustID.Grass;
		}
	}
	public class FloweryFoliageItem : AmbientLeafItem<FloweryFoliageTile>
	{
	}

	public class FloweryFoliageTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			AddMapEntry(new Color(57, 128, 78));
			DustType = DustID.Grass;
		}
	}

	public class JungleFoliageItem : AmbientLeafItem<JungleFoliageTile>
	{
	}

	public class JungleFoliageTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			AddMapEntry(new Color(66, 122, 49));
			DustType = DustID.CorruptGibs;
		}
	}

	public class CrumblingDirtItem : AmbientDirtItem<CrumblingDirtTile>
	{
	}

	public class CrumblingDirtTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			TileID.Sets.CanBeDugByShovel[Type] = true;
			AddMapEntry(new Color(115, 87, 62));
			DustType = DustID.Dirt;
		}
	}

	public class CrackedDirtItem : AmbientDirtItem<CrackedDirtTile>
	{
	}

	public class CrackedDirtTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			TileID.Sets.CanBeDugByShovel[Type] = true;
			AddMapEntry(new Color(115, 87, 62));
			DustType = DustID.Dirt;
		}
	}

	public class RoughDirtItem : AmbientDirtItem<RoughDirtTile>
	{
	}

	public class RoughDirtTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			TileID.Sets.CanBeDugByShovel[Type] = true;
			AddMapEntry(new Color(115, 87, 62));
			DustType = DustID.Dirt;
		}
	}

	public class RockyDirtItem : AmbientDirtItem<RockyDirtTile>
	{
	}

	public class RockyDirtTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			TileID.Sets.CanBeDugByShovel[Type] = true;
			AddMapEntry(new Color(115, 87, 62));
			DustType = DustID.Dirt;
		}
	}

	public class LayeredDirtItem : AmbientDirtItem<LayeredDirtTile>
	{
	}

	public class LayeredDirtTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			TileID.Sets.CanBeDugByShovel[Type] = true;
			AddMapEntry(new Color(115, 87, 62));
			DustType = DustID.Dirt;
		}
	}

	public class CaveDirtItem : AmbientDirtItem<CaveDirtTile>
	{
	}

	public class CaveDirtTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			TileID.Sets.CanBeDugByShovel[Type] = true;
			AddMapEntry(new Color(115, 87, 62));
			DustType = DustID.Dirt;
		}
	}

	public class WavyDirtItem : AmbientDirtItem<WavyDirtTile>
	{
	}

	public class WavyDirtTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			TileID.Sets.CanBeDugByShovel[Type] = true;
			AddMapEntry(new Color(115, 87, 62));
			DustType = DustID.Dirt;
		}
	}

	public class CrimsonPustuleItem : AmbientCrimsonItem<CrimsonPustuleBlockTile>
	{
	}

	public class CrimsonPustuleBlockTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			AddMapEntry(new Color(92, 36, 49));
			DustType = DustID.Blood;
		}
	}

	public class CrimsonScabItem : AmbientCrimsonItem<CrimsonScabTile>
	{
	}

	public class CrimsonScabTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			AddMapEntry(new Color(92, 36, 49));
			DustType = DustID.Blood;
		}
	}

	public class BloodyFoliageItem : AmbientLeafItem<BloodyFoliageTile>
	{
		// public override void SetStaticDefaults() => DisplayName.SetDefault("Bloody Foliage");
	}

	public class BloodyFoliageTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			HitSound = SoundID.Grass;
			AddMapEntry(new Color(92, 36, 49));
			DustType = DustID.Blood;
		}
	}

	public class CrimsonBlisterItem : AmbientCrimsonItem<CrimsonBlisterTile>
	{
	}

	public class CrimsonBlisterTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type, true);
			AddMapEntry(new Color(92, 36, 49));
			DustType = DustID.Blood;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.212f;
			g = .146f;
			b = .066f;
		}
		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

			int height = tile.TileFrameY == 36 ? 18 : 16;
			if (tile.Slope == 0 && !tile.IsHalfBlock)
				Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Tiles/Block/Ambient/CrimsonBlister_Glow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), new Color(100, 100, 100, 60), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
		}
	}

	public class HallowPrismstoneItem : AmbientHallowItem<HallowPrismstoneTile>
	{
	}

	public class HallowPrismstoneTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type, true);
			AddMapEntry(new Color(94, 40, 117));
			DustType = DustID.PinkCrystalShard;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.152f * 1.5f;
			g = .039f * 1.5f;
			b = .168f * 1.5f;
		}
	}

	public class HallowCavernstoneItem : AmbientHallowItem<HallowCavernstoneTile>
	{
	}

	public class HallowCavernstoneTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			AddMapEntry(new Color(94, 40, 117));
			DustType = DustID.SnowBlock;
		}
	}

	public class HallowFoliageItem : AmbientLeafItem<HallowFoliageTile>
	{
	}

	public class HallowFoliageTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			HitSound = SoundID.Grass;
			AddMapEntry(new Color(39, 132, 168));
			DustType = DustID.GreenMoss;
		}
	}

	public class HallowShardstoneItem : AmbientHallowItem<HallowShardstoneTile>
	{
	}

	public class HallowShardstoneTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			AddMapEntry(new Color(94, 40, 117));
			DustType = DustID.PinkCrystalShard;
		}
	}

	public class HallowCrystallineItem : AmbientHallowItem<HallowCrystallineTile>
	{
	}

	public class HallowCrystallineTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			AddMapEntry(new Color(94, 40, 117));
			DustType = DustID.PinkCrystalShard;
		}
	}

	public class HiveBlockAltItem : AmbientBlockItem<HiveBlockAltTile>
	{
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.Hive, 1);
			recipe.AddTile(ModContent.TileType<Furniture.ForagerTableTile>());
			recipe.Register();

			Recipe recipe1 = Recipe.Create(ItemID.Hive, 1);
			recipe1.AddIngredient(this, 1);
			recipe1.AddTile(ModContent.TileType<Furniture.ForagerTableTile>());
			recipe1.Register();
		}
	}

	public class HiveBlockAltTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			AddMapEntry(new Color(94, 40, 117));
			DustType = 7;
		}
	}

	public class RuneBlockItem : AmbientBlockItem<RuneBlockTile>
	{
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.ArcaneRuneWall, 4);
			recipe.AddTile(ModContent.TileType<Furniture.ForagerTableTile>());
			recipe.Register();

			Recipe recipe1 = Recipe.Create(ItemID.ArcaneRuneWall, 1);
			recipe1.AddIngredient(this, 1);
			recipe1.AddTile(ModContent.TileType<Furniture.ForagerTableTile>());
			recipe1.Register();
		}
	}

	public class RuneBlockTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			AddMapEntry(new Color(99, 45, 117));
			DustType = DustID.CorruptionThorns;
		}
	}

	public class KrampusHornBlockItem : AmbientBlockItem<KrampusHornBlockTile>
	{
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.KrampusHornWallpaper, 4);
			recipe.AddTile(ModContent.TileType<Furniture.ForagerTableTile>());
			recipe.Register();

			Recipe recipe1 = Recipe.Create(ItemID.KrampusHornWallpaper, 1);
			recipe1.AddIngredient(this, 1);
			recipe1.AddTile(ModContent.TileType<Furniture.ForagerTableTile>());
			recipe1.Register();
		}
	}

	public class KrampusHornBlockTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			AmbientTileDefaults.SetTileData(Type);
			AddMapEntry(new Color(200, 200, 200));
			DustType = DustID.Sand;
		}
	}
}