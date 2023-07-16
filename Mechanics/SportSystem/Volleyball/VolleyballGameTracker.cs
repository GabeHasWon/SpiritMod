using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace SpiritMod.Mechanics.SportSystem.Volleyball;

internal class VolleyballGameTracker : CourtGameTracker
{
	public override Validator Validator => new VolleyballValidator();

	public PerSide<short> hits;

	public short volleyballWhoAmI = -1;

	protected override void InternalStart()
	{
		hits = new();
	}

	protected override void InternalEnd()
	{
		CheckWin();
	}

	public override void Draw(Court court)
	{
		var pixel = TextureAssets.MagicPixel.Value;
		int width = court.bounds.Width * 16;
		int height = court.bounds.Height * 16;

		if (Active)
		{
			const int PointXOffsets = 4;
			const float WinYOffset = 60;
			const int AllYOffset = 2;

			var font = FontAssets.DeathText.Value;
			var pos = new Point(court.center.X - PointXOffsets, court.center.Y + AllYOffset).ToWorldCoordinates() - Main.screenPosition;
			var size = font.MeasureString(points.left.ToString());
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, points.left.ToString(), pos, Color.White, 0f, size / 2f, Vector2.One);
			size = font.MeasureString(points.right.ToString());
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, wins.left.ToString(), pos + new Vector2(0, WinYOffset), Color.ForestGreen, 0f, size / 2f, Vector2.One * 0.75f);

			pos = new Point(court.center.X, court.center.Y + AllYOffset).ToWorldCoordinates() - Main.screenPosition;
			size = font.MeasureString("Points");
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, "Points", pos, Color.White, 0f, size / 2f, Vector2.One * 0.55f);
			size = font.MeasureString("Wins");
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, "Wins", pos + new Vector2(0, WinYOffset), Color.Green, 0f, size / 2f, Vector2.One * 0.5f);

			pos = new Point(court.center.X + PointXOffsets, court.center.Y + AllYOffset).ToWorldCoordinates() - Main.screenPosition;
			size = font.MeasureString(wins.left.ToString());
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, points.right.ToString(), pos, Color.White, 0f, size / 2f, Vector2.One);
			size = font.MeasureString(wins.right.ToString());
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, wins.right.ToString(), pos + new Vector2(0, WinYOffset), Color.ForestGreen, 0f, size / 2f, Vector2.One * 0.75f);
		}

		//Vertical
		Main.spriteBatch.Draw(pixel, new Vector2(court.bounds.Left, court.bounds.Top) * 16 - Main.screenPosition, new Rectangle(0, 0, 2, height), Color.White);
		Main.spriteBatch.Draw(pixel, new Vector2(court.bounds.Right, court.bounds.Top) * 16 - Main.screenPosition, new Rectangle(0, 0, 2, height), Color.White);

		//Horizontal
		Main.spriteBatch.Draw(pixel, new Vector2(court.bounds.Left, court.bounds.Top) * 16 - Main.screenPosition, new Rectangle(0, 0, width, 2), Color.White);

		//Flat
		Main.spriteBatch.Draw(pixel, new Vector2(court.bounds.Left, court.bounds.Top) * 16 - Main.screenPosition, new Rectangle(0, 0, width, height), Color.Orange * 0.1f);
	}
}
