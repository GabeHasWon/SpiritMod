using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.InfernonDrops;

public class SevenSins : ModItem
{
	private int _charger;

	public override void SetStaticDefaults() => SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");

	public override void SetDefaults()
	{
		Item.damage = 44;
		Item.noMelee = true;
		Item.DamageType = DamageClass.Ranged;
		Item.width = 20;
		Item.height = 38;
		Item.useTime = 22;
		Item.useAnimation = 22;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.shoot = ProjectileID.Shuriken;
		Item.useAmmo = AmmoID.Arrow;
		Item.knockBack = 1;
		Item.rare = ItemRarityID.Pink;
		Item.UseSound = SoundID.Item5;
		Item.value = Item.buyPrice(0, 5, 0, 0);
		Item.value = Item.sellPrice(0, 2, 0, 0);
		Item.autoReuse = true;
		Item.shootSpeed = 13f;
	}

	public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
	{
		if (++_charger >= 5)
		{
			for (int i = 0; i < 5; i++)
			{
				Vector2 vel = velocity.RotatedByRandom(0.5f);
				var proj = Projectile.NewProjectileDirect(source, position, vel, ProjectileID.GreekFire3, 50, knockback, player.whoAmI, 0f, 0f);

				proj.hostile = false;
				proj.friendly = true;
				proj.penetrate = 2;
				proj.netUpdate = true;
			}

			_charger = 0;
		}

		return true;
	}
}