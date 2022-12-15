using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpiritMod.Items.Sets.ClubSubclass
{
    public class NautilusClub : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nautilobber");
            Tooltip.SetDefault("Generates a cascade of bubbles at the collision zone");
			SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/Sets/ClubSubclass/NautilusClub_Glow");
		}

        public override void SetDefaults()
        {
            Item.channel = true;
            Item.damage = 24;
            Item.width = 66;
            Item.height = 66;
            Item.useTime = 320;
            Item.useAnimation = 320;
            Item.crit = 6;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.knockBack = 11;
            Item.useTurn = true;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<Projectiles.Clubs.NautilusClubProj>();
            Item.shootSpeed = 6f;
            Item.noUseGraphic = true;
        }
		public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.06f, .16f, .22f);
			Texture2D texture = ModContent.Request<Texture2D>("SpiritMod/Items/Sets/ClubSubclass/NautilusClub_Glow").Value;
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, texture, rotation, scale);
		}
	}
}