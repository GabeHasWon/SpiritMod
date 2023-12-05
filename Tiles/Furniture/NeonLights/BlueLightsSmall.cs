using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using System;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Furniture.NeonLights
{
	public class BlueLightsSmall : ModTile
	{
		public static SpriteEffects GetEffect(int i) => (i % 2 == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileLighted[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorBottom = default(AnchorData);
			TileObjectData.addTile(Type);
			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
			DustType = DustID.Dirt;
			LocalizedText name = CreateMapEntryName();
			AdjTiles = new int[] { TileID.Torches };
            TileID.Sets.DisableSmartCursor[Type] = true;
            DustType = -1;

			StaticDefaults();
        }

		public virtual void StaticDefaults() => AddMapEntry(new Color(82, 125, 255), CreateMapEntryName());

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
			=> (r, g, b) = (.082f * 1.75f, .125f * 1.75f, .255f * 1.75f);

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
			=> spriteEffects = GetEffect(i);

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            Color colour = Color.White * MathHelper.Lerp(.2f, 1f, (float)((Math.Sin(SpiritMod.GlobalNoise.Noise(i * .2f, j * .2f) * 3f + Main.GlobalTimeWrappedHourly * 1.3f) + 1f) * .5f));
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

            spriteBatch.Draw(glow, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), colour, 0, Vector2.Zero, 1, GetEffect(i), 0);
        }
    }
}