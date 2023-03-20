using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SpiritMod.Items.Books.UI.MaterialUI;

public abstract class UIPageState : UIState
{
	internal static float offsetX = -1;
	internal static float offsetY = -1;

	protected abstract string BackgroundTexturePath { get; }

	private UIDragableElement mainPanel;

	public override void OnInitialize()
	{
		mainPanel = new UIDragableElement
		{
			HAlign = 0.5f,
			VAlign = 0.5f,
			Width = StyleDimension.FromPixels(756f),
			Height = StyleDimension.FromPixels(323f)
		};
		mainPanel.OnScrollWheel += OnScrollWheel_FixHotbarScroll;
		Append(mainPanel);

		if (offsetX != -1)
		{
			mainPanel.Left.Set(offsetX, 0f);
			mainPanel.Top.Set(offsetY, 0f);
		}

		var panelBackground = new UIImage(ModContent.Request<Texture2D>(BackgroundTexturePath))
		{
			Width = StyleDimension.FromPercent(1f),
			Height = StyleDimension.FromPercent(1f)
		};
		panelBackground.SetPadding(12);
		mainPanel.Append(panelBackground);
		mainPanel.AddDragTarget(panelBackground);

		var closeTexture = ModContent.Request<Texture2D>("SpiritMod/Items/Books/UI/closeButton");
		UIImageButton closeButton = new UIImageButton(closeTexture)
		{
			Left = StyleDimension.FromPixelsAndPercent(-20, 1f),
			Top = StyleDimension.FromPixels(5),
			Width = StyleDimension.FromPixels(15),
			Height = StyleDimension.FromPixels(15)
		};
		closeButton.OnClick += CloseButton_OnClick;
		panelBackground.Append(closeButton);

		UIElement messageBoxPanel = new UIElement
		{
			Width = StyleDimension.FromPercent(1),
			Height = StyleDimension.FromPixelsAndPercent(-50, 1),
			Top = StyleDimension.FromPixels(50)
		};
		panelBackground.Append(messageBoxPanel);
		mainPanel.AddDragTarget(messageBoxPanel);
	}

	private static void OnScrollWheel_FixHotbarScroll(UIScrollWheelEvent evt, UIElement listeningElement) => Main.LocalPlayer.ScrollHotbar(Terraria.GameInput.PlayerInput.ScrollWheelDelta / 120);

	private static void CloseButton_OnClick(UIMouseEvent evt, UIElement listeningElement)
	{
		SoundEngine.PlaySound(SoundID.MenuClose);
		ModContent.GetInstance<SpiritMod>().BookUserInterface.SetState(null);
	}

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
}

class UIGraniteMaterialPageState : UIPageState
{
	protected override string BackgroundTexturePath => "SpiritMod/Items/Books/UI/MaterialUI/GraniteMaterialPage";
}

class UIMarbleMaterialPageState : UIPageState
{
	protected override string BackgroundTexturePath => "SpiritMod/Items/Books/UI/MaterialUI/MarbleMaterialPage";
}

class UIGlowrootPageState : UIPageState
{
	protected override string BackgroundTexturePath => "SpiritMod/Items/Books/UI/MaterialUI/GlowrootPage";
}

class UIBismitePageStsate : UIPageState
{
	protected override string BackgroundTexturePath => "SpiritMod/Items/Books/UI/MaterialUI/BismiteMaterialPage";
}

class UIFrigidFragmentPageState : UIPageState
{
	protected override string BackgroundTexturePath => "SpiritMod/Items/Books/UI/MaterialUI/FrigidFragmentMaterialPage";
}

class UIHeartScalePageState : UIPageState
{
	protected override string BackgroundTexturePath => "SpiritMod/Items/Books/UI/MaterialUI/HeartScaleMaterialPage";
}

class UIEnchantedLeafPageState : UIPageState
{
	protected override string BackgroundTexturePath => "SpiritMod/Items/Books/UI/MaterialUI/EnchantedLeafMaterialPage";
}