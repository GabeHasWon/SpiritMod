using SpiritMod.Projectiles.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Weapon.Thrown
{
	public class BoCShuriken : ModItem
	{
		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.width = 14;
			Item.height = 50;
			Item.noUseGraphic = true;
			Item.UseSound = SoundID.Item1;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<BrainProj>();
			Item.useAnimation = 24;
			Item.consumable = true;
			Item.maxStack = Item.CommonMaxStack;
			Item.useTime = 24;
			Item.shootSpeed = 15f;
			Item.damage = 17;
			Item.knockBack = 3.7f;
			Item.value = Item.sellPrice(0, 0, 0, 50);
			Item.value = Item.buyPrice(0, 0, 0, 60);
			Item.rare = ItemRarityID.Green;
			Item.autoReuse = false;
			Item.consumable = true;
		}
	}
}
