using SpiritMod.Projectiles.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Weapon.Thrown
{
	public class MimeBomb : ModItem
	{
		public override void SetStaticDefaults() => DisplayName.SetDefault("Mime Bomb");

		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.width = 16;
			Item.height = 16;
			Item.noUseGraphic = true;
			Item.DamageType = DamageClass.Ranged;
			Item.channel = true;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<MimeBombProj>();
			Item.useAnimation = 46;
			Item.useTime = 46;
			Item.consumable = true;
			Item.maxStack = 999;
			Item.shootSpeed = 5f;
			Item.damage = 40;
			Item.knockBack = 9.5f;
			Item.value = Item.sellPrice(0, 0, 3, 0);
			Item.rare = ItemRarityID.Green;
			Item.autoReuse = false;
			Item.consumable = true;
		}
	}
}