using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
namespace SpiritMod.Tiles.Ambient
{
	public class Hourglass : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;

			Terraria.ID.TileID.Sets.FramesOnKillWall[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16,
				16
			};
			TileObjectData.newTile.AnchorBottom = default(AnchorData);
			TileObjectData.newTile.AnchorTop = default(AnchorData);
			TileObjectData.newTile.AnchorWall = true;
			TileObjectData.addTile(Type);
			Terraria.ID.TileID.Sets.DisableSmartCursor[Type] = true;
			DustType -= 1;
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Hourglass");
			AddMapEntry(new Color(150, 150, 150), name);
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
        public override bool RightClick(int x, int y)
        {
            double time = Main.time;

            if (!Main.dayTime)
                time += 54000.0;

                time = time / 86400.0 * 24.0;
                time = time - 7.5 - 12.0;

            if (time < 0.0)
                time += 24.0;
            if (time >= 24.0)
                time -= 24.0;

            int hours = (int)time;
            int minutes = (int)((time - hours) * 60.0);

            string timeText;
            string period = Language.GetTextValue(hours >= 12 ? "GameUI.TimePastMorning" : "GameUI.TimeAtMorning");
            int displayHours = hours % 12;
            if (displayHours == 0)
                displayHours = 12;

            timeText = $"{displayHours}:{minutes:00} {period}";

            Main.NewText(Language.GetTextValue("CLI.Time", timeText), 255, 240, 20);
            return true;
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (closer)
                Main.SceneMetrics.HasClock = true;
        }
	}
}