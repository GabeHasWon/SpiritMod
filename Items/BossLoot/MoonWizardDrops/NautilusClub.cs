using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Sets.ClubSubclass;

namespace SpiritMod.Items.BossLoot.MoonWizardDrops
{
    public class NautilusClub : ClubItem
    {
		internal override int MinDamage => 34;
		internal override int MaxDamage => 80;
		internal override float MinKnockback => 4f;
		internal override float MaxKnockback => 8f;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nautilobber");
            Tooltip.SetDefault("Generates a cascade of bubbles at the collision zone");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
		}

        public override void Defaults()
        {
            Item.width = 66;
            Item.height = 66;
            Item.crit = 6;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<Projectiles.Clubs.NautilusClubProj>();
        }

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.06f, .16f, .22f);
			Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, texture, rotation, scale);
		}
	}
}