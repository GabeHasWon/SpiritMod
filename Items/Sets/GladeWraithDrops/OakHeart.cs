using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GladeWraithDrops
{
	public class OakHeart : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Oak Heart");
			Tooltip.SetDefault("Hitting foes in quick succession causes poisonous spores to rain down\nMore effective at close range");
		}

		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.width = 9;
			Item.height = 15;
			Item.noUseGraphic = true;
			Item.UseSound = SoundID.Item1;
			Item.DamageType = DamageClass.Melee;
			Item.channel = true;
			Item.noMelee = true;
			Item.maxStack = 1;
			Item.shoot = ModContent.ProjectileType<Projectiles.Thrown.OakHeart>();
			Item.shootSpeed = 12f;
			Item.useAnimation = 32;
			Item.useTime = 32;
			Item.damage = 12;
			Item.knockBack = 1.5f;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
			Item.consumable = false;
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 20f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
				position += muzzleOffset;

			velocity = velocity.RotatedByRandom(player.GetModPlayer<MyPlayer>().oakHeartStacks / 12);
		}

		public override float UseSpeedMultiplier(Player player) => 1f + ((float)((int)player.GetModPlayer<MyPlayer>().oakHeartStacks / (float)player.GetModPlayer<MyPlayer>().oakHeartStacksMax) * .5f);
	}
}