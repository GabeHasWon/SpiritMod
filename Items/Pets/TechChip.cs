using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs.Pet;
using SpiritMod.Projectiles.Pet;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Pets
{
	public class TechChip : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stardrive Chip");
			Tooltip.SetDefault("Summons a Star Spider to run alongside you\n'It's inscribed in an Astral language'");
			SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/Pets/TechChip_Glow");
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.shoot = ModContent.ProjectileType<CogSpiderPet>();
			Item.buffType = ModContent.BuffType<CogSpiderPetBuff>();
			Item.UseSound = SoundID.Item93;
		}

		public override void UseStyle(Player player, Rectangle heldItemFrame)
		{
			if (player.whoAmI == Main.myPlayer && player.itemTime == 0) {
				player.AddBuff(Item.buffType, 3600, true);
			}
		}

		public override bool CanUseItem(Player player) => player.miscEquips[0].IsAir;

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.08f, .28f, .38f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}
	}
}