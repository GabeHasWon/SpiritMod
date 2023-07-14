using Terraria;
using Terraria.GameContent;
using Microsoft.Xna.Framework;

namespace SpiritMod.Mechanics.VolleyballSystem;

internal class CourtDrawer
{
	internal static void Draw(Court court)
	{
		var pixel = TextureAssets.MagicPixel.Value;
		int width = court.bounds.Width * 16;
		int height = court.bounds.Height * 16;

		//Vertical
		Main.spriteBatch.Draw(pixel, new Vector2(court.bounds.Left, court.bounds.Top) * 16 - Main.screenPosition, new Rectangle(0, 0, 2, height), Color.White);
		Main.spriteBatch.Draw(pixel, new Vector2(court.bounds.Right, court.bounds.Top) * 16 - Main.screenPosition, new Rectangle(0, 0, 2, height), Color.White);

		//Horizontal
		Main.spriteBatch.Draw(pixel, new Vector2(court.bounds.Left, court.bounds.Top) * 16 - Main.screenPosition, new Rectangle(0, 0, width, 2), Color.White);

		Main.spriteBatch.Draw(pixel, new Vector2(court.bounds.Left, court.bounds.Top) * 16 - Main.screenPosition, new Rectangle(0, 0, width, height), Color.Orange * 0.1f);
	}
}