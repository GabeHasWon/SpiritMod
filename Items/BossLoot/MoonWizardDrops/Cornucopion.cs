using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.MoonWizardDrops
{
    public class Cornucopion : ModItem
    {
        public override void SetStaticDefaults() => SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");

		public override void SetDefaults()
        {
            Item.damage = 24;
            Item.knockBack = 8;
            Item.noMelee = true;
			Item.noUseGraphic = true;
            Item.useTurn = true;
            Item.channel = true;
            Item.rare = ItemRarityID.Pink;
            Item.width = 18;
            Item.height = 18;
            Item.useTime = 20;
			Item.UseSound = new Terraria.Audio.SoundStyle("SpiritMod/Sounds/MoonWizardHorn");
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.expert = true;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<CornucopionProj>();
            Item.shootSpeed = 1f;
            Item.value = 10000;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Lighting.AddLight(Item.position, 0.08f, .28f, .38f);
			Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, texture, rotation, scale);
		}

		public override bool CanUseItem(Player player) => player.ZoneOverworldHeight || player.ZoneSkyHeight;
	}
}
