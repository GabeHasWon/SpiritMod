using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Accessory.ShieldCore
{
	public class ShieldCore : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 28;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetSpiritPlayer().shieldCore = true;

			if (player.whoAmI == Main.myPlayer)
			{
				int shieldCount = 2;
				int type = ModContent.ProjectileType<InterstellarShield>();

				if (player.ownedProjectileCounts[type] < shieldCount)
					for (int i = 0; i < shieldCount; i++)
						Projectile.NewProjectileDirect(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<InterstellarShield>(), 0, 0, player.whoAmI, i * (360 / shieldCount), -(InterstellarShield.cooldownTime * InterstellarShield.rechargeRate));
			}
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) =>
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
	}
}
