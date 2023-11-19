using SpiritMod.Projectiles.Arrow;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Ammo.Arrow
{
	public class SepulchreArrow : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 8;
			Item.height = 16;
			Item.value = 80;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(0, 0, 0, 30);
			Item.maxStack = Item.CommonMaxStack;
			Item.damage = 9;
			Item.knockBack = 2f;
			Item.ammo = AmmoID.Arrow;
			Item.DamageType = DamageClass.Ranged;
			Item.consumable = true;
			Item.shoot = ModContent.ProjectileType<AccursedArrow>();
			Item.shootSpeed = 3.7f;
		}
	}
}