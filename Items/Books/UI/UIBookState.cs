using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SpiritMod.Items.Books.UI
{
	class UIBookState : UIState
	{
		internal string title;
		private string author;
		private string bookContents;
		private UIDragableElement mainPanel;

		internal static float offsetX = -1;
		internal static float offsetY = -1;

		public UIBookState(string title, string author, string bookContents)
		{
			this.title = title;
			this.author = author;
			this.bookContents = bookContents;
		}

		public override void OnInitialize()
		{
			mainPanel = new UIDragableElement
			{
				HAlign = 0.5f,
				VAlign = 0.5f,
				Width = StyleDimension.FromPixels(400),
				Height = StyleDimension.FromPixels(600)
			};

			mainPanel.OnScrollWheel += OnScrollWheel_FixHotbarScroll;
			Append(mainPanel);

			if (offsetX != -1)
			{
				mainPanel.Left.Set(offsetX, 0f);
				mainPanel.Top.Set(offsetY, 0f);
			}

			var panelBackground = new UIImage(ModContent.Request<Texture2D>("SpiritMod/Items/Books/UI/BookBackground"))
			{ 
				Width = StyleDimension.FromPercent(1),
				Height = StyleDimension.FromPercent(1)
			};
			panelBackground.SetPadding(12);
			mainPanel.Append(panelBackground);
			mainPanel.AddDragTarget(panelBackground);

			var closeTexture = ModContent.Request<Texture2D>("SpiritMod/Items/Books/UI/closeButton", ReLogic.Content.AssetRequestMode.ImmediateLoad);
			UIImageButton closeButton = new UIImageButton(closeTexture);
			closeButton.Left = StyleDimension.FromPixelsAndPercent(-15, 1f);
			//closeButton.Top.Set(0, 0f);
			closeButton.Width = StyleDimension.FromPixels(15);
			closeButton.Height = StyleDimension.FromPixels(15);
			closeButton.OnLeftClick += CloseButton_OnClick;
			panelBackground.Append(closeButton);

			UIText titleLabel = new UIText(title, 1.2f);
			panelBackground.Append(titleLabel);
			mainPanel.AddDragTarget(titleLabel);

			UIText authorLabel = new UIText(author, 0.7f);
			authorLabel.Left = StyleDimension.FromPixels(12);
			authorLabel.Top = StyleDimension.FromPixels(24);
			panelBackground.Append(authorLabel);
			mainPanel.AddDragTarget(authorLabel);

			UIElement messageBoxPanel = new UIElement
			{
				Width = { Percent = 1f },
				Height = { Pixels = -50, Percent = 1f },
				Top = { Pixels = 50, },
			};
			panelBackground.Append(messageBoxPanel);
			mainPanel.AddDragTarget(messageBoxPanel);

			UIMessageBox messageBox = new UIMessageBox(bookContents)
			{
				Width = { Percent = 1f }, // no scrollbar option
				Height = { Percent = 1f },
			};
			messageBoxPanel.Append(messageBox);
			mainPanel.AddDragTarget(messageBox);
			
			var messageBoxScrollbar = new InvisibleUIScrollbar(ModContent.GetInstance<SpiritMod>().BookUserInterface)
			{
				Height = { Pixels = -20, Percent = 1f },
				VAlign = 0.5f,
				HAlign = 1f
			}.WithView(100f, 1000f);
			messageBoxPanel.Append(messageBoxScrollbar);
			messageBox.SetScrollbar(messageBoxScrollbar);

			mainPanel.Recalculate();
		}

		// A hack to fix scroll bar usage scrolling the item hotbar
		internal static void OnScrollWheel_FixHotbarScroll(UIScrollWheelEvent evt, UIElement listeningElement) => Main.LocalPlayer.ScrollHotbar(Terraria.GameInput.PlayerInput.ScrollWheelDelta / 120);

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			if (mainPanel.ContainsPoint(Main.MouseScreen))
				Main.LocalPlayer.mouseInterface = true;

			if (mainPanel.Left.Pixels != 0)
			{
				offsetX = mainPanel.Left.Pixels;
				offsetY = mainPanel.Top.Pixels;
			}
			base.DrawSelf(spriteBatch);
		}

		private void CloseButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
		{
			SoundEngine.PlaySound(SoundID.MenuClose);
			ModContent.GetInstance<SpiritMod>().BookUserInterface.SetState(null);
		}
	}
}