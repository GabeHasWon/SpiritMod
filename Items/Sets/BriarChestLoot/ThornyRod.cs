using Microsoft.Xna.Framework;
using SpiritMod.Tiles.Block;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.BriarChestLoot
{
	public class ThornyRod : ModItem
	{
		// public override void SetStaticDefaults() => Tooltip.SetDefault("Creates briar grass on dirt");

		public override void SetDefaults()
		{
			Item.damage = 5;
			Item.DamageType = DamageClass.Melee;
			Item.knockBack = 2;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 25;
			Item.useTime = 20;
			Item.autoReuse = true;
			Item.maxStack = 1;
			Item.width = Item.height = 36;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(silver: 30);
		}

		public override bool CanUseItem(Player player)
		{
			Point tilePos = Main.SmartCursorIsUsed ? new Point(Main.SmartCursorX, Main.SmartCursorY) : (Main.MouseWorld / 16).ToPoint();
			Tile tile = Framing.GetTileSafely(tilePos);

			if (tile.TileType == TileID.Dirt)
				tile.TileType = (ushort)ModContent.TileType<BriarGrass>();
			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendTileSquare(-1, tilePos.X, tilePos.Y);

			return true;
		}

		public override void MeleeEffects(Player player, Rectangle hitbox) => Dust.NewDustDirect(hitbox.TopLeft(), hitbox.Width, hitbox.Height, DustID.Plantera_Green, player.direction).noGravity = true;
	}
}