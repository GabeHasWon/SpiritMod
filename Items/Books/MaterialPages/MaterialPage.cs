using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.UI;
using SpiritMod.Items.Books.UI;

namespace SpiritMod.Items.Books.MaterialPages
{
	[Sacrifice(1)]
	public abstract class MaterialPage<T> : ModItem where T : UIState
	{
		protected virtual bool CheckTitle => false;
		protected bool IsOpen => ModContent.GetInstance<SpiritMod>().BookUserInterface.CurrentState is T currentBookState && (!CheckTitle || (currentBookState as UIBookState).title == Item.Name);

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
		public override bool CanUseItem(Player player) => player.whoAmI == Main.myPlayer && !IsOpen;
	}
}