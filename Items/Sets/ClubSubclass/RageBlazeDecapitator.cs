using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.ClubSubclass
{
    public class RageBlazeDecapitator : ClubItem
    {
		internal override int ChargeTime => 40;
		internal override Vector2 Size => new(96, 96);
		internal override float Acceleration => 28f;
		internal override int MinDamage => 60;
		internal override int MaxDamage => 175;
		internal override float MinKnockback => 6f;
		internal override float MaxKnockback => 10f;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Unstable Adze");
            Tooltip.SetDefault("Charges rapidly\nCharged strikes create an energy shockwave on impact");
            SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/Sets/ClubSubclass/RageBlazeDecapitator_Glow");
        }

        public override void Defaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.crit = 6;
            Item.value = Item.sellPrice(0, 0, 90, 0);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<Projectiles.Clubs.EnergizedAxeProj>();
        }

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.06f, .16f, .22f);
			Texture2D texture = ModContent.Request<Texture2D>("SpiritMod/Items/Sets/ClubSubclass/RageBlazeDecapitator_Glow").Value;
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, texture, rotation, scale);
		}

		public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<GraniteSet.GraniteChunk>(), 18);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}