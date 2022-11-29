using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace SpiritMod.Items.Books
{
	public abstract class Book : ModItem
	{
		public abstract string BookText { get; }

		public override void SetDefaults()
		{
			Item.noMelee = true;
			Item.useTurn = true;
			Item.rare = ItemRarityID.Green;
			Item.width = 54;
			Item.height = 50;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.autoReuse = false;
			Item.noUseGraphic = false;
		}

		public override Vector2? HoldoutOffset() => new Vector2(-10, 0);
		public override bool CanUseItem(Player player) => !(ModContent.GetInstance<SpiritMod>().BookUserInterface.CurrentState is UI.UIBookState currentBookState) || currentBookState.title != Item.Name;

		public override bool? UseItem(Player player)
		{
			if (player != Main.LocalPlayer)
				return false;

			SoundEngine.PlaySound(SoundID.MenuOpen);
			ModContent.GetInstance<SpiritMod>().BookUserInterface.SetState(new UI.UIBookState(Item.Name, Item.ToolTip.GetLine(0), BookText));
			return true;
		}
	}
}