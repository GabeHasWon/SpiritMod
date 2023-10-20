using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Consumable.Food;
using SpiritMod.Items.Placeable.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.SpaceCrystals
{
	public class GreenShardBig : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileLighted[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.addTile(Type);

			DustType = DustID.GemEmerald;
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(200, 200, 200), name);
			RegisterItemDrop(ModContent.ItemType<RockCandy>());
		}
		
		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.5f / 4;
			g = 0.8f / 4;
			b = 0.4f / 4;
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) => drawData.tileLight *= 1.5f;

		public override void KillMultiTile(int i, int j, int frameX, int frameY) => SoundEngine.PlaySound(Terraria.ID.SoundID.Item27, new Vector2(i, j) * 16);
	}

	public class GreenShardBigRubble : GreenShardBig
	{
		public override string Texture => base.Texture.Replace("Rubble", "");

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileLighted[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.addTile(Type);
			DustType = DustID.GemEmerald;

			AddMapEntry(new Color(200, 200, 200), Language.GetText($"Mods.SpiritMod.Tiles.{Name}.MapEntry"));

			FlexibleTileWand.RubblePlacementMedium.AddVariation(ModContent.ItemType<AsteroidBlock>(), Type, 0);
			RegisterItemDrop(ModContent.ItemType<AsteroidBlock>());
		}
	}
}
