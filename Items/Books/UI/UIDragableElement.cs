using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SpiritMod.Items.Books.UI
{
	// This class adapted from RecipeBrowser with permission and is NOT open source.
	internal class UIDragableElement : UIElement
	{
		private static Texture2D dragTexture;

		private readonly bool dragable;
		private readonly bool resizeableX;
		private readonly bool resizeableY;

		private bool IsResizable => resizeableX || resizeableY;

		private Vector2 offset;
		private bool dragging;
		private bool resizeing;

		//private int minX, minY, maxX, maxY;
		private readonly List<UIElement> additionalDragTargets;

		// TODO, move panel back in if offscreen? prevent drag off screen?
		public UIDragableElement(bool dragable = true, bool resizeableX = false, bool resizeableY = false)
		{
			this.dragable = dragable;
			this.resizeableX = resizeableX;
			this.resizeableY = resizeableY;

			if (dragTexture == null)
				dragTexture = ModContent.Request<Texture2D>("Terraria/Images/UI/PanelBorder", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

			additionalDragTargets = new List<UIElement>();
		}

		public void AddDragTarget(UIElement element) => additionalDragTargets.Add(element);

		public override void LeftMouseDown(UIMouseEvent evt)
		{
			DragStart(evt);
			base.LeftMouseDown(evt);
		}

		public override void LeftMouseUp(UIMouseEvent evt)
		{
			DragEnd(evt);
			base.LeftMouseUp(evt);
		}

		private void DragStart(UIMouseEvent evt)
		{
			CalculatedStyle innerDimensions = GetInnerDimensions();

			if (evt.Target == this || additionalDragTargets.Contains(evt.Target))
			{
				if (IsResizable && new Rectangle((int)(innerDimensions.X + innerDimensions.Width - 18), (int)(innerDimensions.Y + innerDimensions.Height - 18), 18, 18).Contains(evt.MousePosition.ToPoint()))
				//if (resizeable && new Rectangle((int)(innerDimensions.X + innerDimensions.Width - 12), (int)(innerDimensions.Y + innerDimensions.Height - 12), 12 + 6, 12 + 6).Contains(evt.MousePosition.ToPoint()))
				{
					offset = new Vector2(evt.MousePosition.X - innerDimensions.X - innerDimensions.Width, evt.MousePosition.Y - innerDimensions.Y - innerDimensions.Height);
					//offset = new Vector2(evt.MousePosition.X - innerDimensions.X - innerDimensions.Width - 6, evt.MousePosition.Y - innerDimensions.Y - innerDimensions.Height - 6);
					resizeing = true;
				}
				else if (dragable)
				{
					offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
					dragging = true;
				}
			}
		}

		private void DragEnd(UIMouseEvent evt)
		{
			if (evt.Target == this || additionalDragTargets.Contains(evt.Target))
			{
				dragging = false;
				resizeing = false;
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			//Rectangle hitbox = GetInnerDimensions().ToRectangle();
			//Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Red * 0.6f);

			CalculatedStyle dimensions = GetOuterDimensions();
			if (ContainsPoint(Main.MouseScreen))
			{
				Main.LocalPlayer.mouseInterface = true;
				Main.LocalPlayer.cursorItemIconEnabled = false;
				Main.ItemIconCacheUpdate(0);
			}

			if (dragging)
			{
				Left.Set(Main.MouseScreen.X - offset.X, 0f);
				Top.Set(Main.MouseScreen.Y - offset.Y, 0f);
				Recalculate();
			}
			else
			{
				if (Parent != null && !dimensions.ToRectangle().Intersects(Parent.GetDimensions().ToRectangle()))
				{
					var parentSpace = Parent.GetDimensions().ToRectangle();
					Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
					Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
					Recalculate();
				}
			}

			if (resizeing)
			{
				if (resizeableX)
				{
					//Width.Pixels = Utils.Clamp(Main.MouseScreen.X - dimensions.X - offset.X, minX, maxX);
					Width.Pixels = Main.MouseScreen.X - dimensions.X - offset.X;
				}
				if (resizeableY)
				{
					//Height.Pixels = Utils.Clamp(Main.MouseScreen.Y - dimensions.Y - offset.Y, minY, maxY);
					Height.Pixels = Main.MouseScreen.Y - dimensions.Y - offset.Y;
				}
				Recalculate();
			}

			base.DrawSelf(spriteBatch);
		}

		protected override void DrawChildren(SpriteBatch spriteBatch)
		{
			base.DrawChildren(spriteBatch);

			if (IsResizable)
				DrawDragAnchor(spriteBatch, dragTexture, Color.Black/*this.BorderColor*/);
		}

		private void DrawDragAnchor(SpriteBatch spriteBatch, Texture2D texture, Color color)
		{
			CalculatedStyle dimensions = GetOuterDimensions();

			//Rectangle hitbox = GetInnerDimensions().ToRectangle();
			//hitbox = new Rectangle((int)(innerDimensions.X + innerDimensions.Width - 18), (int)(innerDimensions.Y + innerDimensions.Height - 18), 18, 18);
			//Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.LightBlue * 0.6f);

			Point point = new Point((int)(dimensions.X + dimensions.Width - 12), (int)(dimensions.Y + dimensions.Height - 12));
			spriteBatch.Draw(texture, new Rectangle(point.X - 2, point.Y - 2, 12 - 2, 12 - 2), new Rectangle(12 + 4, 12 + 4, 12, 12), color);
			spriteBatch.Draw(texture, new Rectangle(point.X - 4, point.Y - 4, 12 - 4, 12 - 4), new Rectangle(12 + 4, 12 + 4, 12, 12), color);
			spriteBatch.Draw(texture, new Rectangle(point.X - 6, point.Y - 6, 12 - 6, 12 - 6), new Rectangle(12 + 4, 12 + 4, 12, 12), color);
		}
	}
}