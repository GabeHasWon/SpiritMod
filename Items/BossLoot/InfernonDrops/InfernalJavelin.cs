using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.InfernonDrops;

public class InfernalJavelin : ModItem
{
	public override void SetDefaults()
	{
		Item.width = Item.height = 46;
		Item.rare = ItemRarityID.Pink;
		Item.value = Terraria.Item.sellPrice(0, 3, 70, 0);
		Item.damage = 42;
		Item.knockBack = 6;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = Item.useAnimation = 25;
		Item.DamageType = DamageClass.Melee;
		Item.noMelee = true;
		Item.autoReuse = true;
		Item.noUseGraphic = true;
		Item.shoot = ModContent.ProjectileType<Projectiles.Thrown.InfernalJavelin>();
		Item.shootSpeed = 14;
		Item.UseSound = SoundID.Item1;
	}

	public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		=> GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
}