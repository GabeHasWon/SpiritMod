using Microsoft.Xna.Framework;
using SpiritMod.Items.Placeable.Tiles;
using SpiritMod.NPCs.Wheezer;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.IceSculpture.Hostile
{
	public class IceWheezerHostile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Frozen Wheezer");
			DustType = DustID.SnowBlock;
			AddMapEntry(new Color(200, 200, 200), name);
		}

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;

		public override void KillMultiTile(int i, int j, int frameX, int frameY) => SoundEngine.PlaySound(SoundID.Item27, new Vector2(i, j) * 16);

		public override void NearbyEffects(int i, int j, bool closer)
		{
			if (closer)
				FrozenSpawner.SpawnFrozenEnemy(i, j, ModContent.NPCType<Wheezer>());
		}
	}
}