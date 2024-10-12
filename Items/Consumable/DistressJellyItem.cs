using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable
{
	[Sacrifice(3)]
    public class DistressJellyItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = Item.height = 16;
            Item.rare = ItemRarityID.Green;
			Item.maxStack = Item.CommonMaxStack;
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
				Main.NewText(Language.GetTextValue("Mods.SpiritMod.Events.JellyDeluge.OnStart"), 61, 255, 142);
			else if (Main.netMode == NetmodeID.Server)
				ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Mods.SpiritMod.Events.JellyDeluge.OnStart"), new Color(61, 255, 142));

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				MyWorld.jellySky = true;

				if (Main.netMode == NetmodeID.Server)
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