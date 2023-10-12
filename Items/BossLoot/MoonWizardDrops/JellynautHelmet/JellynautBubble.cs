using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;

namespace SpiritMod.Items.BossLoot.MoonWizardDrops.JellynautHelmet
{
	[AutoloadEquip(EquipType.Head)]
	public class JellynautBubble : ModItem
	{
		public override void SetStaticDefaults()
			=> SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/BossLoot/MoonWizardDrops/JellynautHelmet/JellynautBubble_Head_Glow");

		public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
			=> glowMaskColor = Color.White;

        public override void SetDefaults()
		{
			Item.width = 22;
			Item.height = 18;
			Item.value = Item.sellPrice(0, 2, 30, 0);
			Item.rare = ItemRarityID.Green;
			Item.defense = 2;
		}

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Lighting.AddLight(Item.position, 0.08f, .4f, .28f);
			Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, texture, rotation, scale);
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
			=> body.type >= ItemID.AmethystRobe && body.type <= ItemID.DiamondRobe || body.type == ItemID.GypsyRobe || body.type == ItemID.AmberRobe;

		public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Generic) += 10;
            player.statManaMax2 += 20;
        }

        public override void UpdateArmorSet(Player player)
        {
            string tapDir = Language.GetTextValue(Main.ReversedUpDownArmorSetBonuses ? "Key.UP" : "Key.DOWN");
            player.setBonus = Language.GetTextValue("Mods.SpiritMod.SetBonuses.JellynautBubble", tapDir);
			player.GetSpiritPlayer().jellynautHelm = true;
        }
	}
}
