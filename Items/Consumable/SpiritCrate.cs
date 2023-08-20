using SpiritMod.Items.Material;
using SpiritMod.Items.Sets.SpiritSet;
using SpiritMod.Items.Sets.SeraphSet;
using SpiritMod.Items.Sets.RunicSet;
using SpiritMod.Items.Sets.SpiritBiomeDrops;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SpiritMod.NPCs;

using SpiritCrateTile = SpiritMod.Tiles.Furniture.SpiritCrate;

namespace SpiritMod.Items.Consumable
{
	[Sacrifice(5)]
	public class SpiritCrate : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Spirit Crate");
			// Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
		}

		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.rare = ItemRarityID.Pink;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.createTile = ModContent.TileType<SpiritCrateTile>();
			Item.maxStack = 999;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.consumable = true;
		}

		public override bool CanRightClick() => true;

		public override void ModifyItemLoot(ItemLoot itemLoot)
		{
			itemLoot.Add(DropRules.LootPoolDrop.SameStack(5, 9, 1, 1, ModContent.ItemType<SpiritOre>(), ModContent.ItemType<Rune>(),
				ModContent.ItemType<MoonStone>()));
			itemLoot.AddCommon<Books.Book_SpiritArt>(10);
			itemLoot.Add(DropRules.LootPoolDrop.SameStack(40, 80, 1, 6, 1, ModContent.ItemType<StarCutter>(), ModContent.ItemType<GhostJellyBomb>()));
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) => GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
	}
}
