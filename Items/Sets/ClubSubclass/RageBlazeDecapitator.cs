using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace SpiritMod.Items.Sets.ClubSubclass
{
    public class RageBlazeDecapitator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Unstable Adze");
            Tooltip.SetDefault("Charges rapidly\nCharged strikes release multiple shards upon damaging enemies");
            SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/Sets/ClubSubclass/RageBlazeDecapitator_Glow");
        }

        public override void SetDefaults()
        {
            Item.channel = true;
            Item.damage = 25;
            Item.width = 60;
            Item.height = 60;
            Item.useTime = 320;
            Item.useAnimation = 320;
            Item.crit = 8;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.knockBack = 10;
            Item.useTurn = true;
            Item.value = Item.sellPrice(0, 0, 90, 0);
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<Projectiles.Clubs.EnergizedAxeProj>();
            Item.shootSpeed = 6f;
            Item.noUseGraphic = true;
        }
		public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

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