using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.DuskingDrops
{
	public class UmbraStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Umbra Staff");
			Tooltip.SetDefault("Shoots out homing Shadow Balls");
			SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/BossLoot/DuskingDrops/UmbraStaff_Glow");
		}

		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 36;
			Item.value = Item.buyPrice(0, 7, 0, 0);
			Item.rare = ItemRarityID.Pink;
			Item.damage = 44;
			Item.knockBack = 3;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.staff[Item.type] = true;
			Item.useTime = 24;
			Item.useAnimation = 24;
			Item.mana = 6;
			Item.DamageType = DamageClass.Magic;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.shoot = ModContent.ProjectileType<ShadowBall_Friendly>();
			Item.shootSpeed = 10f;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
			=> GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
	}
}
