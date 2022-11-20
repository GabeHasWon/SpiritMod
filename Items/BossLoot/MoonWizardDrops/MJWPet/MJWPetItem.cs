using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.MoonWizardDrops.MJWPet
{
	internal class MJWPetItem : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Curious Lightbulb");
			Tooltip.SetDefault("Summons a Moon Jelly Lightbulb");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.ShadowOrb);
			Item.shoot = ModContent.ProjectileType<MJWPetProjectile>();
			Item.buffType = ModContent.BuffType<Buffs.Pet.MJWPetBuff>();
			Item.UseSound = SoundID.NPCDeath6; 
			Item.rare = ItemRarityID.Master;
			Item.master = true;
		}

		public override void UseStyle(Player player, Rectangle heldItemFrame)
		{
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
				player.AddBuff(Item.buffType, 3600, true);
		}

		public override bool CanUseItem(Player player) => player.miscEquips[1].IsAir;

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.46f, .07f, .52f);
			Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, texture, rotation, scale);
		}
	}
}
