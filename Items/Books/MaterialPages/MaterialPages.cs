using SpiritMod.Items.Books.UI.MaterialUI;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Books.MaterialPages
{
	class MarblePage : MaterialPage<UIMarbleMaterialPageState>
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Notes on Ancient Marble Chunks");
			// Tooltip.SetDefault("by Professor Alex Tannis\nContains information on a strange ore found in Marble Caverns");
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI != Main.LocalPlayer.whoAmI)
				return false;

			SoundEngine.PlaySound(SoundID.MenuOpen);
			ModContent.GetInstance<SpiritMod>().BookUserInterface.SetState(new UIMarbleMaterialPageState());
			return true;
		}
	}

	class EnchantedLeafPage : MaterialPage<UIEnchantedLeafPageState>
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Notes on Enchanted Leaves");
			// Tooltip.SetDefault("by Professor Alex Tannis\nContains information on mystical leaves found in the Briar");
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI != Main.LocalPlayer.whoAmI)
				return false;

			SoundEngine.PlaySound(SoundID.MenuOpen);
			ModContent.GetInstance<SpiritMod>().BookUserInterface.SetState(new UIEnchantedLeafPageState());
			return true;
		}
	}

	class GranitePage : MaterialPage<UIGraniteMaterialPageState>
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Notes on Enchanted Granite Chunks");
			// Tooltip.SetDefault("by Professor Alex Tannis\nContains information on a strange ore found in Granite Caverns");
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI != Main.LocalPlayer.whoAmI)
				return false;

			SoundEngine.PlaySound(SoundID.MenuOpen);
			ModContent.GetInstance<SpiritMod>().BookUserInterface.SetState(new UIGraniteMaterialPageState());
			return true;
		}
	}

	class HeartScalePage : MaterialPage<UIHeartScalePageState>
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Notes on Heart Scales");
			// Tooltip.SetDefault("by Professor Alex Tannis\nContains information on a glimmering scale often found near Ardorfish");
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI != Main.LocalPlayer.whoAmI)
				return false;

			SoundEngine.PlaySound(SoundID.MenuOpen);
			ModContent.GetInstance<SpiritMod>().BookUserInterface.SetState(new UIHeartScalePageState());
			return true;
		}
	}

	class BismitePage : MaterialPage<UIBismitePageStsate>
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Notes on Bismite Crystals");
			// Tooltip.SetDefault("by Professor Alex Tannis\nContains information on a toxic ore fond around the caverns");
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI != Main.LocalPlayer.whoAmI)
				return false;

			SoundEngine.PlaySound(SoundID.MenuOpen);
			ModContent.GetInstance<SpiritMod>().BookUserInterface.SetState(new UIBismitePageStsate());
			return true;
		}
	}

	class GlowrootPage : MaterialPage<UIGlowrootPageState>
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Notes on Glowroot");
			// Tooltip.SetDefault("by Professor Alex Tannis\nContains information on a strange root found at the base of tall mushroom trees");
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI != Main.LocalPlayer.whoAmI)
				return false;

			SoundEngine.PlaySound(SoundID.MenuOpen);
			ModContent.GetInstance<SpiritMod>().BookUserInterface.SetState(new UIGlowrootPageState());
			return true;
		}
	}

	class FrigidFragmentPage : MaterialPage<UIFrigidFragmentPageState>
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Notes on Frigid Fragments");
			// Tooltip.SetDefault("by Professor Alex Tannis\nContains information on an icy crystal found in the frozen tundra");
		}

		public override bool? UseItem(Player player)
		{
			if (player.whoAmI != Main.LocalPlayer.whoAmI)
				return false;

			SoundEngine.PlaySound(SoundID.MenuOpen);
			ModContent.GetInstance<SpiritMod>().BookUserInterface.SetState(new UIFrigidFragmentPageState());
			return true;
		}
	}
}