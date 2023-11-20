using SpiritMod.Buffs.Pet;
using SpiritMod.Projectiles.DonatorItems;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace SpiritMod.Items.Pets
{
	[Sacrifice(1)]
	public class CagedMoonlight : ModItem
	{
		public override void SetStaticDefaults() => SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.sellPrice(0, 3, 50, 0);
			Item.buffType = ModContent.BuffType<FaeriePetBuff>();
			Item.shoot = ModContent.ProjectileType<FaeriePet>();
		}

		public override void UseStyle(Player player, Rectangle heldItemFrame)
		{
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
				player.AddBuff(Item.buffType, 3600, true);
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.08f, .38f, .28f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}

		public override bool CanUseItem(Player player) => player.miscEquips[0].IsAir;
	}
}