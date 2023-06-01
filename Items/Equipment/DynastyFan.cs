using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace SpiritMod.Items.Equipment
{
	public class DynastyFan : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dynasty Fan");
			Tooltip.SetDefault("Launch yourself in any direction with a gust of wind");

			Item.staff[Item.type] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 44;
			Item.height = 48;
			Item.useTime = 100;
			Item.useAnimation = 100;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.value = Item.buyPrice(gold: 5);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item20;
			Item.autoReuse = false;
			Item.shoot = ProjectileID.WoodenArrowFriendly;
			Item.shootSpeed = 12f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.HasBuff(BuffID.Featherfall))
				velocity *= 0.75f;

			player.velocity = -velocity;
			player.AddBuff(ModContent.BuffType<Buffs.DynastyFanBuff>(), 120);

			for (int i = 0; i < 3; i++)
			{
				Gore gore = Gore.NewGoreDirect(source, player.Center, player.velocity * 4, 825 + i);
				gore.timeLeft = Main.rand.Next(30, 90);
			}

			return false;
		}
	}
}