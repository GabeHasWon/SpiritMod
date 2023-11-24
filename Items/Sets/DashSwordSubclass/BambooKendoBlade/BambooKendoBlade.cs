using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Material;
using SpiritMod.Items.Sets.DashSwordSubclass.AnimeSword;
using SpiritMod.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.DashSwordSubclass.BambooKendoBlade
{
	public class BambooKendoBlade : DashSwordItem
	{
		public override void SetDefaults()
		{
			Item.damage = 7;
			Item.crit = 2;
			Item.knockBack = 3;
			Item.useTime = Item.useAnimation = 20;
			Item.DamageType = DamageClass.Melee;
			Item.width = Item.height = 46;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(silver: 18);
			Item.rare = ItemRarityID.White;
			Item.UseSound = SoundID.Item1;
			Item.shoot = ModContent.ProjectileType<BambooKendoBladeProj>();
			Item.shootSpeed = 1f;
			Item.autoReuse = true;
			Item.useTurn = true;
		}

		public override void HoldItem(Player player)
		{
			if (!player.ItemAnimationActive)
			{
				player.GetModPlayer<DashSwordPlayer>().holdingSword = true;
				player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, -1.1f * player.direction);
			}
		}
		
		public override bool AltFunctionUse(Player player) => player.GetModPlayer<DashSwordPlayer>().hasDashCharge;

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				Item.noUseGraphic = true;
				Item.noMelee = true;
			}
			else
			{
				Item.noUseGraphic = false;
				Item.noMelee = false;
				return true;
			}
			return base.CanUseItem(player);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.altFunctionUse == 2)
				Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
			return false;
		}

		public override void DrawHeld(PlayerDrawSet info)
		{
			Texture2D texture = TextureAssets.Item[Type].Value;

			Vector2 drawPos = info.drawPlayer.Center + new Vector2(0, 6 * info.drawPlayer.gravDir + info.drawPlayer.gfxOffY);
			float rotation = (.5f * info.drawPlayer.direction) + MathHelper.Pi;

			info.DrawDataCache.Add(new DrawData(texture,
				drawPos - Main.screenPosition,
				null,
				Lighting.GetColor((int)info.drawPlayer.Center.X / 16, (int)info.drawPlayer.Center.Y / 16),
				rotation,
				texture.Size() / 2,
				1,
				info.playerEffect,
				0
			));
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<StrippedBamboo>(), 20);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}

	public class BambooKendoBladeProj : DashSwordProjectile
	{
		public override int WaitTime => 30;

		public override string Texture => "SpiritMod/Items/Sets/DashSwordSubclass/BambooKendoBlade/BambooKendoBlade";

		public override LocalizedText DisplayName => Language.GetText("Mods.SpiritMod.Items.BambooKendoBlade.DisplayName");

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(38);
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = 1;
		}

		public override void AbstractAI()
		{
			if (Counter < DashDuration)
			{
				if (Counter == ChargeupTime)
					SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, Projectile.Center);
			}
			else
			{
				if (Counter == (DashDuration + 1) && startPos.Distance(endPos) > 50)
				{
					for (int i = 0; i < 5; i++)
					{
						Vector2 position = Vector2.Lerp(startPos, endPos, -.2f);
						Vector2 randomOff = Main.rand.NextVector2Unit() * Main.rand.NextFloat() * 34f;

						ParticleHandler.SpawnParticle(new ImpactLine(position + randomOff, Vector2.Normalize(endPos - startPos) * Main.rand.NextFloat() * 4f, Color.Lerp(Color.FloralWhite, Color.Yellow, Main.rand.NextFloat()) * .5f, new Vector2(1f, (startPos.Distance(endPos) + 50) / 72f), 12));
						Dust.NewDustPerfect(startPos + randomOff, DustID.Smoke, Vector2.Normalize(endPos - startPos) * 50 * Main.rand.NextFloat(0.05f, 0.23f), 100, Color.White with { A = 0 }, Main.rand.NextFloat(1.0f, 2.0f)).noGravity = true;
					}
				}
			}
			if (Counter >= (ChargeupTime + DashDuration + StrikeDelay))
				Projectile.localAI[0]++;

			float startTime = WaitTime * .75f;
			float restingRotation = .5f + (Owner.direction == 1 ? MathHelper.Pi : 0);
			float addRotation = (Projectile.localAI[0] > startTime) ? (Projectile.localAI[0] - startTime) / (WaitTime - startTime) * (Math.Abs(Projectile.velocity.ToRotation() - restingRotation) / MathHelper.PiOver2) : 0;

			Projectile.rotation = ((.785f + addRotation) * Owner.direction) + Projectile.velocity.ToRotation() + ((Owner.direction == -1) ? MathHelper.Pi : 0);
			Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Owner.AngleTo(Projectile.Center) - 1.57f);

			Owner.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			base.OnHitNPC(target, hit, damageDone);
			SpiritMod.primitives.CreateTrail(new AnimePrimTrailTwo(target));
		}

		public override Vector2 HoldoutOffset() => Vector2.Normalize(Projectile.velocity) * (25 - (Projectile.localAI[0] / WaitTime * 25f));

		public override bool AutoAimCursor() => false;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2, Projectile.scale, effects, 0);
			
			return false;
		}
	}
}