using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.StarplateDrops
{
	[Sacrifice(1)]
	public class StarMap : ModItem
	{
		private static int CooldownTime => Main.expertMode ? 1200 : 600;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Astral Map");
			// Tooltip.SetDefault("Teleports you to the cursor location\n{N} second cooldown");

			SpiritGlowmask.AddGlowMask(Item.type, "SpiritMod/Items/BossLoot/StarplateDrops/StarMap_Glow");
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			var line = tooltips.FirstOrDefault(x => x.Name == "Tooltip1");
			
			if (line is not null)
				line.Text = line.Text.Replace("{N}", (CooldownTime / 60).ToString());
		}

		public override void SetDefaults()
		{
			Item.damage = 0;
			Item.noMelee = true;
			Item.channel = true;
			Item.rare = ItemRarityID.Pink;
			Item.width = 42;
			Item.height = 58;
			Item.useTime = Item.useAnimation = 12;
			Item.UseSound = SoundID.Item8;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.expert = true;
			Item.autoReuse = false;
			Item.shootSpeed = 0f;
			Item.noUseGraphic = true;
		}

		public override bool CanUseItem(Player player) => !player.HasBuff(ModContent.BuffType<Buffs.AstralMapCooldown>());

		public override bool? UseItem(Player player)
		{
			AstralTeleport(player);
			return true;
		}

		private static void AstralTeleport(Player player)
		{
			if (player.whoAmI == Main.myPlayer)
			{
				bool inTempleEarly = (Framing.GetTileSafely((Main.MouseWorld / 16).ToPoint()).WallType == WallID.LihzahrdBrickUnsafe) && !NPC.downedGolemBoss;
				if (!Collision.SolidCollision(Main.MouseWorld, player.width, player.height) && !inTempleEarly)
				{
					RunTeleport(player, Main.MouseWorld);
					player.AddBuff(ModContent.BuffType<Buffs.AstralMapCooldown>(), CooldownTime);
				}
			}
		}

		private static void RunTeleport(Player player, Vector2 pos)
		{
			player.Teleport(pos, 2, 0);
			player.velocity = Vector2.Zero;
			SoundEngine.PlaySound(SoundID.Item6, player.Center);
			DustHelper.DrawStar(player.Center, DustID.GoldCoin, pointAmount: 4, mainSize: 1.7425f, dustDensity: 6, dustSize: .65f, pointDepthMult: 3.6f, noGravity: true);

			if (Main.netMode != NetmodeID.SinglePlayer)
				NetMessage.SendData(MessageID.SyncPlayer, number: player.whoAmI);
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.2f, .142f, .032f);

			Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			Texture2D outline = ModContent.Request<Texture2D>(Texture + "_Outline", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			float Timer = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3) / 2 + 0.5f;

			void DrawTex(Texture2D tex, float opacity, Vector2? offset = null) => spriteBatch.Draw(tex, Item.Center + (offset ?? Vector2.Zero) - Main.screenPosition, null, Color.White * opacity, rotation, tex.Size() / 2, scale, SpriteEffects.None, 0);

			for (int i = 0; i < 6; i++)
			{
				Vector2 drawPos = Vector2.UnitX.RotatedBy(i / 6f * MathHelper.TwoPi) * Timer * 6;
				DrawTex(glow, (1 - Timer) / 2, drawPos);
				DrawTex(outline, (1 - Timer) / 2, drawPos + (Vector2.UnitY * 2));
			}

			DrawTex(glow, (Timer / 5) + 0.5f);
			DrawTex(outline, (Timer / 5) + 0.5f, Vector2.UnitY * 2);
		}
	}
}
