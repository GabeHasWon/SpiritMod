using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.BloodcourtSet.Headsplitter
{
	public class Headsplitter : ModItem
	{
		private float cachedCharge;

		private static bool Empowered(Player player) => player.HasBuff(ModContent.BuffType<Empowered>());

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Headsplitter");
			Tooltip.SetDefault("Strikes inflict 'Surging Anguish', slowly depleting enemy life\n" +
				"Right click grants empowerement for a short time, dealing bonus damage to anguished foes");
		}

		public override void SetDefaults()
		{
			Item.damage = 21;
			Item.DamageType = DamageClass.Melee;
			Item.width = 34;
			Item.height = 40;
			Item.useTime = Item.useAnimation = 24;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = Item.sellPrice(0, 0, 20, 0);
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<HeadsplitterProj>();
			Item.shootSpeed = 6f;
			Item.noUseGraphic = true;
			Item.noMelee = true;
			Item.autoReuse = true;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, .42f, .02f, .13f);
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		}

		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			if (Main.LocalPlayer.HeldItem != Item)
				return;

			var modPlayer = Main.LocalPlayer.GetModPlayer<HeadsplitterPlayer>();
			Texture2D bar = ModContent.Request<Texture2D>(Texture + "_Bar").Value;
			Vector2 drawPos = position + ((frame.Size() / 2) + (Vector2.UnitY * 30)) * scale;

			for (int i = 0; i < 3; i++)
			{
				Rectangle barFrame = bar.Frame(1, 3, 0, i, 0, -2);
				Rectangle barDrawFrame = barFrame with
				{
					Width = i switch
					{
						0 => bar.Width,
						1 => (int)(bar.Width * ((float)cachedCharge / modPlayer.chargeMax)),
						_ => (int)(bar.Width * ((float)modPlayer.charge / modPlayer.chargeMax))
					}
				};

				Color color = Color.White;
				if (i == 2 && modPlayer.charge >= modPlayer.chargeMax)
					color = Color.White * (float)Math.Sin((float)Main.timeForVisualEffects / 30f);

				spriteBatch.Draw(bar, drawPos, barDrawFrame, color, 0, barFrame.Size() / 2, scale, SpriteEffects.None, 0);
			}
		}

		public override bool AltFunctionUse(Player player) => !Empowered(player) && player.GetModPlayer<HeadsplitterPlayer>().charge > (player.GetModPlayer<HeadsplitterPlayer>().chargeMax * .25f);

		public override bool CanUseItem(Player player)
		{
			var modPlayer = player.GetModPlayer<HeadsplitterPlayer>();

			if (player.altFunctionUse == 2)
			{
				ParticleHandler.SpawnParticle(new PulseCircle(player.Center, Color.Magenta, 100, 12, PulseCircle.MovementType.InwardsQuadratic));
				for (int i = 0; i < 30; i++)
				{
					Vector2 dustPos = player.Center + new Vector2(player.width * 2 * Main.rand.NextFloat(-1.0f, 1.0f), player.height / 2 * player.gravDir);
					Dust dust = Dust.NewDustPerfect(dustPos, DustID.LavaMoss, Vector2.Zero, 0, Color.White, Main.rand.NextFloat(0.5f, 1.5f));
					dust.velocity = Vector2.UnitY * Main.rand.NextFloat() * -5f * player.gravDir;
					dust.noGravity = true;
					dust.shader = GameShaders.Armor.GetSecondaryShader(93, Main.LocalPlayer);
				}

				player.AddBuff(ModContent.BuffType<Empowered>(), (int)modPlayer.charge);
				SoundEngine.PlaySound(SoundID.NPCDeath39 with { PitchVariance = 0.2f }, player.position);
			}

			if (Empowered(player))
				Item.useTime = Item.useAnimation = 38;
			else
				Item.useTime = Item.useAnimation = 24;

			return player.altFunctionUse != 2 || modPlayer.charge >= modPlayer.chargeMax;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			var modPlayer = player.GetModPlayer<HeadsplitterPlayer>();

			int state = Empowered(player) ? ((modPlayer.charge >= modPlayer.chargeMax) ? 2 : 1) : 0;
			velocity = Vector2.UnitX * ((state == 2) ? -player.direction : player.GetModPlayer<HeadsplitterPlayer>().SwingDirection);
			modPlayer.SwingDirection *= -1;

			Projectile.NewProjectile(source, position, velocity, type, damage * (state + 1), Math.Min(11, knockback * (state + 1)), player.whoAmI, state);

			return false;
		}

		public override void HoldItem(Player player)
		{
			var modPlayer = player.GetModPlayer<HeadsplitterPlayer>();

			if (!Empowered(player))
				modPlayer.charge = Math.Min(modPlayer.chargeMax, modPlayer.charge + .1f);
			cachedCharge = MathHelper.Lerp(cachedCharge, modPlayer.charge, .05f);

			if (modPlayer.charge >= modPlayer.chargeMax && Main.rand.NextBool(5))
			{
				Vector2 dustPos = player.Center + new Vector2(player.width * 2 * Main.rand.NextFloat(-1.0f, 1.0f), player.height / 2 * player.gravDir);
				Dust dust = Dust.NewDustPerfect(dustPos, DustID.LavaMoss, Vector2.Zero, 0, Color.White, Main.rand.NextFloat(0.5f, 1.5f));
				dust.velocity = Vector2.UnitY * Main.rand.NextFloat() * -5f * player.gravDir;
				dust.noGravity = true;
				dust.shader = GameShaders.Armor.GetSecondaryShader(93, Main.LocalPlayer);
			}
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<DreamstrideEssence>(), 8);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}

	internal class HeadsplitterPlayer : ModPlayer
	{
		public float charge;
		public readonly int chargeMax = 250;

		public int SwingDirection { get; set; } = 1;

		public override void PostUpdate()
		{
			if (Player.HeldItem.type != ModContent.ItemType<Headsplitter>()) //Reset when held item changes
				SwingDirection = 1;
			if (Player.HasBuff(ModContent.BuffType<Empowered>()))
				charge = Math.Max(0, charge - 1);
		}
	}
}