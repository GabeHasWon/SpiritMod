using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.DuskingDrops
{
	public class DuskPendant : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dusk Pendant");
			Tooltip.SetDefault("13% increased magic and ranged critical strike chance at night");
			SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/BossLoot/DuskingDrops/DuskPendant_Glow");
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 28;
			Item.rare = ItemRarityID.LightRed;
			Item.value = 80000;
			Item.expert = true;
			Item.DamageType = DamageClass.Melee;
			Item.accessory = true;

			Item.knockBack = 9f;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			if (!Main.dayTime) {
				player.GetCritChance(DamageClass.Ranged) += 13;
				player.GetCritChance(DamageClass.Magic) += 13;
			}
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
			=> GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
	}
}
