using Microsoft.Xna.Framework;
using SpiritMod.Items.Consumable.Food;
using SpiritMod.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Ambient.SpaceCrystals
{
	public class BlueShardBig : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Width = 2;
			Main.tileLighted[Type] = true;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.RandomStyleRange = 3;
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Giant Crystal");
			DustType = DustID.DungeonSpirit;
			AddMapEntry(new Color(110, 120, 255), name);
		}

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) => offsetY = 2;
		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.3f / 4;
			g = 0.3f / 4;
			b = 0.9f / 4;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(new Terraria.DataStructures.EntitySource_TileBreak(i, j), i * 16, j * 16, 64, 32, ModContent.ItemType<RockCandy>());
			for (int e = 1; e < 8; e++)
			{
				Vector2 position = new Vector2(i, j).ToWorldCoordinates();
				Vector2 velocity = new Vector2(Main.rand.NextFloat(-1.0f, 1.0f) * 3f, Main.rand.NextFloat(-2.0f, 0.0f));

				ParticleHandler.SpawnParticle(new GlowParticle(position, velocity * .75f, Color.Lerp(Color.White, Color.Blue, Main.rand.NextFloat(0.0f, 1.0f)), Main.rand.NextFloat(0.03f, 0.10f), 60));
				Dust dust = Dust.NewDustDirect(position, 16, 16, DustID.Electric, Main.rand.NextFloat(-1.0f, 1.0f) * 5f, Main.rand.NextFloat(-1.0f, 0.0f));
				dust.noGravity = true;
				if (e <= 4)
					Gore.NewGore(new Terraria.DataStructures.EntitySource_TileBreak(i, j), position, velocity, Mod.Find<ModGore>("BlueShardGore").Type);
			}
			SoundEngine.PlaySound(SoundID.Item27);
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			if (!fail)
				num = 4;
		}
	}
}