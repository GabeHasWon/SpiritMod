using Microsoft.Xna.Framework;
using SpiritMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.TideDrops
{
	public class PumpBubbleGun : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Bubble Blaster");
			// Tooltip.SetDefault("Hold for a longer blast\nConsumes 20 mana each second");
		}

		public override void SetDefaults()
		{
			Item.channel = true;
			Item.damage = 30;
			Item.DamageType = DamageClass.Magic;
			Item.width = 24;
			Item.height = 24;
			Item.useTime = 24;
			Item.useAnimation = 24;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 3;
			Item.useTurn = false;
			Item.value = Item.sellPrice(0, 1, 42, 0);
			Item.rare = ItemRarityID.Orange;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<BubblePumpProj>();
			Item.shootSpeed = 6f;
		}

		public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

		public override bool CanUseItem(Player player) => player.statMana >= 20;
	}
}