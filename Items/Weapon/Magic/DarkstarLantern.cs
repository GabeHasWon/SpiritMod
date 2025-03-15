using Microsoft.Xna.Framework;
using SpiritMod.Items.BossLoot.DuskingDrops;
using SpiritMod.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Weapon.Magic;

public class DarkstarLantern : ModItem
{
	public override void SetStaticDefaults() => Item.staff[Item.type] = true;

	public override void SetDefaults()
	{
		Item.damage = 55;
		Item.DamageType = DamageClass.Magic;
		Item.mana = 20;
		Item.width = 66;
		Item.height = 68;
		Item.useTime = 32;
		Item.useAnimation = 32;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.noMelee = true;
		Item.knockBack = 2;
		Item.crit = 10;
		Item.value = Item.sellPrice(0, 3, 0, 0);
		Item.rare = ItemRarityID.Yellow;
		Item.UseSound = SoundID.Item93;
		Item.autoReuse = false;
		Item.shoot = ModContent.ProjectileType<ShadowOrb>();
		Item.shootSpeed = 1f;
	}

	public override void AddRecipes() => CreateRecipe().AddIngredient(ModContent.ItemType<DuskStone>(), 10)
		.AddIngredient(ItemID.Ectoplasm, 14).AddTile(TileID.MythrilAnvil).Register();

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
	{
		Vector2 mouse = Main.MouseWorld;
		Projectile.NewProjectile(source, mouse.X, mouse.Y, 0f, 0f, type, damage, knockback, player.whoAmI);

		return false;
	}
}