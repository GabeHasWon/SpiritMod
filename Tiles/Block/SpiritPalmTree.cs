using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SpiritMod.Items.Placeable.Tiles;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Block
{
	public class SpiritPalmTree : ModPalmTree
	{
		// This is a blind copy-paste from Vanilla's PurityPalmTree settings.
		//TODO: This needs some explanations
		public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings
		{
			UseSpecialGroups = true,
			SpecialGroupMinimalHueValue = 11f / 72f,
			SpecialGroupMaximumHueValue = 0.25f,
			SpecialGroupMinimumSaturationValue = 0.88f,
			SpecialGroupMaximumSaturationValue = 1f
		};

		public override void SetStaticDefaults() => GrowsOnTileId = new int[] { ModContent.TileType<Spiritsand>() };
		public override int CreateDust() => DustID.WoodFurniture;
		public override int DropWood() => ModContent.ItemType<SpiritWoodItem>();
		public override Asset<Texture2D> GetTexture() => ModContent.Request<Texture2D>("SpiritMod/Tiles/Block/SpiritPalmTree");
		public override Asset<Texture2D> GetTopTextures() => ModContent.Request<Texture2D>("SpiritMod/Tiles/Block/SpiritPalmTree_Tops");
		public override Asset<Texture2D> GetOasisTopTextures() => ModContent.Request<Texture2D>("SpiritMod/Tiles/Block/SpiritPalmTree_OasisTops");

		public override int SaplingGrowthType(ref int style)
		{
			style = 1;
			return ModContent.TileType<SpiritSapling>();
		}
	}
}