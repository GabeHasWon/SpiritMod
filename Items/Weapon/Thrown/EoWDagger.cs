using SpiritMod.Projectiles.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Weapon.Thrown
{
	public class EoWDagger : ModItem
	{
		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.width = 30;
			Item.height = 50;
			Item.noUseGraphic = true;
			Item.UseSound = SoundID.Item1;
			Item.DamageType = DamageClass.Ranged;
			Item.channel = true;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<EoWDaggerProj>();
			Item.useAnimation = 25;
			Item.consumable = true;
			Item.maxStack = Item.CommonMaxStack;
			Item.useTime = 25;
			Item.shootSpeed = 8.5f;
			Item.damage = 15;
			Item.knockBack = 3.5f;
			Item.value = Item.sellPrice(0, 0, 0, 50);
			Item.value = Item.buyPrice(0, 0, 0, 60);
			Item.rare = ItemRarityID.Green;
			Item.autoReuse = false;
			Item.consumable = true;
		}
	}
}
