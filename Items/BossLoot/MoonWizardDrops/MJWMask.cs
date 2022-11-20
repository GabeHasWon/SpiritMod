using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.MoonWizardDrops
{
	[AutoloadEquip(EquipType.Head)]
	public class MJWMask : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Moon Jelly Wizard Mask");
			SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/BossLoot/MoonWizardDrops/MJWMask_Head_Glow");
		}

		public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 20;
			Item.value = 3000;
			Item.rare = ItemRarityID.Blue;
			Item.vanity = true;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.08f, .4f, .28f);
			Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, texture, rotation, scale);
		}
		public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) => glowMaskColor = Color.White;
	}
}
