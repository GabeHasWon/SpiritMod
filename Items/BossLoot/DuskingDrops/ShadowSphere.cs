using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles.Magic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.DuskingDrops
{
	public class ShadowSphere : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shadow Sphere");
			Tooltip.SetDefault("Summons a slow shadow sphere that shoots out Crystal Shadows at foes");
			SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/BossLoot/DuskingDrops/ShadowSphere_Glow");
		}

		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 36;
			Item.value = Item.buyPrice(0, 4, 0, 0);
			Item.rare = ItemRarityID.Pink;
			Item.damage = 39;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.UseSound = SoundID.Item20;
			Item.staff[Item.type] = true;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.mana = 10;
			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<ShadowCircleRune>();
			Item.shootSpeed = 0f;
			Item.sentry = true; 
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			//remove any other owned SpiritBow projectiles, just like any other sentry minion
			for (int i = 0; i < Main.projectile.Length; i++)
			{
				Projectile p = Main.projectile[i];
				if (p.active && p.type == Item.shoot && p.owner == player.whoAmI)
					p.active = false;
			}
			player.UpdateMaxTurrets();
			position = Main.MouseWorld;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
			=> GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
	}
}
