using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs.Pet;
using SpiritMod.Projectiles.Pet;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Pets
{
	[Sacrifice(1)]
	public class PowerRing : ModItem
	{
		public override void SetStaticDefaults() => SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");

		public override void SetDefaults()
		{
			Item.useStyle = ItemUseStyleID.Swing;
			Item.shoot = ModContent.ProjectileType<LanternPet>();
			Item.width = 16;
			Item.height = 30;
			Item.useAnimation = 20;
			Item.useTime = 20;
			Item.rare = ItemRarityID.Orange;
			Item.noMelee = true;
			Item.UseSound = SoundID.Item93;
			Item.value = Item.sellPrice(0, 3, 50, 0);
			Item.buffType = ModContent.BuffType<LanternBuff>();
		}

		public override void UseStyle(Player player, Rectangle heldItemFrame)
		{
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
				player.AddBuff(Item.buffType, 3600, true);
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
			=> GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);

		public override bool CanUseItem(Player player) => player.miscEquips[1].IsAir;
	}
}