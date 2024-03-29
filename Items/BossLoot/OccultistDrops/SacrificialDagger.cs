using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace SpiritMod.Items.BossLoot.OccultistDrops
{
	public class SacrificialDagger : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 18;
			Item.height = 44;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.sellPrice(0, 0, 80, 0);
			Item.damage = 15;
			Item.knockBack = 2;
			Item.mana = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = Item.useAnimation = 35;
			Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<Projectiles.Summon.SacrificialDagger.SacrificialDaggerProj>();
			Item.shootSpeed = 14;
			Item.UseSound = SoundID.Item1;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.46f, .07f, .52f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}
	}
}