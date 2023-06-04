using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable
{
    public class DistressJellyItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Distress Jelly");
            Tooltip.SetDefault("'It needs help!'\nUse at nighttime to summon the Jelly Deluge");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 16;
            Item.rare = ItemRarityID.Green;
            Item.maxStack = 99;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = Item.useAnimation = 20;
            Item.noMelee = true;
            Item.consumable = true;
            Item.autoReuse = false;
            Item.UseSound = SoundID.Item43;
        }

        public override bool CanUseItem(Player player) => !MyWorld.jellySky && !Main.dayTime && (player.ZoneSkyHeight || player.ZoneOverworldHeight);

        public override bool? UseItem(Player player)
        {
			if (Main.netMode == NetmodeID.SinglePlayer)
				Main.NewText("Strange jellyfish are raining from the sky!", 61, 255, 142);
			else if (Main.netMode == NetmodeID.Server)
				ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral("Strange jellyfish are raining from the sky!"), new Color(61, 255, 142));

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				MyWorld.jellySky = true;

				if (Main.netMode != NetmodeID.SinglePlayer)
					NetMessage.SendData(MessageID.WorldData);
			}
            return true;
        }

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.46f, .07f, .64f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}
	}
}