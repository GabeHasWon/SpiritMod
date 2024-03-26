using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SummonsMisc.SoulDagger
{
	public class SoulDagger : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 70;
			Item.width = 74;
			Item.height = 74;
			Item.value = Item.sellPrice(0, 2, 0, 0);
			Item.rare = ItemRarityID.LightRed;
			Item.mana = 40;
			Item.knockBack = 3f;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<SoulDaggerProj>();
			Item.UseSound = SoundID.Item44;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			position = Main.MouseWorld;
			Projectile.NewProjectile(source, position, Main.rand.NextVector2Circular(3, 3), type, damage, knockback, player.whoAmI);
			return false;
		}
	}
}