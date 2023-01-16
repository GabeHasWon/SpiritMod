using SpiritMod.Projectiles.Flail;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace SpiritMod.Items.Sets.FlailsMisc.ClatterMace
{
	public class ClatterMace : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Clattering Mace");
			Tooltip.SetDefault("Has a chance to lower enemy defense on hit");
			ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
		}

		public override void SetDefaults()
		{
			Item.width = 38;
			Item.height = 34;
			Item.value = Item.sellPrice(0, 0, 60, 0);
			Item.rare = ItemRarityID.Green;
			Item.damage = 10;
			Item.knockBack = 5.5f;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = Item.useAnimation = 30;
			Item.DamageType = DamageClass.Melee;
			Item.noMelee = true;
			Item.channel = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<ClatterMaceProj>();
			Item.shootSpeed = 12.5f;
			Item.UseSound = SoundID.Item1;
		}
	}
}