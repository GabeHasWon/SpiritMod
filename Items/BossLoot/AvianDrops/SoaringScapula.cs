using SpiritMod.Projectiles.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.AvianDrops
{
	public class SoaringScapula : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soaring Scapula");
			Tooltip.SetDefault("Pulls enemies towards the ground, causing them to take additional damage on collision");
		}

		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 20;
			Item.value = Item.sellPrice(0, 0, 40, 0);
			Item.rare = ItemRarityID.Green;
			Item.maxStack = 1;
			Item.damage = 32;
			Item.knockBack = 3;
			Item.useStyle = ItemUseStyleID.Rapier;
			Item.useTime = Item.useAnimation = 19;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.consumable = false;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<Scapula>();
			Item.shootSpeed = 11;
			Item.UseSound = SoundID.Item1;
		}
	}
}