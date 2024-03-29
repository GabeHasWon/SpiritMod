using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.OpalFrog
{
	[Sacrifice(1)]
	public class OpalFrogItem : ModItem
	{
		public override void SetDefaults()
		{
			Item.Size = new Vector2(40, 34);
			Item.value = Item.sellPrice(gold: 8);
			Item.useStyle = ItemUseStyleID.Swing;
			Item.rare = ItemRarityID.LightRed;
			Item.createTile = ModContent.TileType<OpalFrog_Tile>();
			Item.maxStack = 1;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			OpalFrogPlayer modPlayer = player.GetModPlayer<OpalFrogPlayer>();
			modPlayer.HookStat += 0.25f;
			if (!hideVisual)
				modPlayer.AutoUnhook = true;
		}
	}
}