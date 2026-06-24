using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace SpiritMod.Tiles.Furniture.Acid
{
	public class AcidClockTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 5;
			TileObjectData.newTile.CoordinateHeights = new[]
			{
				16,
				16,
				16,
				16,
				16
			};
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(100, 122, 111), Language.GetText("ItemName.GrandfatherClock"));
			DustType = -1;
			AdjTiles = new int[] { TileID.GrandfatherClocks };
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