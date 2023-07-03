using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Items.Material;
using SpiritMod.Items.Weapon.Swung.AnimeSword;
using SpiritMod.Particles;
using SpiritMod.Projectiles.BaseProj;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SwordsMisc.BambooKendoBlade
{
	public class BambooKendoBlade : ModItem
	{
		public override void SetStaticDefaults() => Tooltip.SetDefault("Right click to dash forward");

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
			Item.shoot = ModContent.ProjectileType<BambooKendoBlade_Held>();
			Item.shootSpeed = 1f;
			Item.autoReuse = true;
			Item.useTurn = true;
		}

		public override void HoldItem(Player player)
		{
			if (!player.ItemAnimationActive)
				player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, -1.1f * player.direction);
		}
		
		public override bool AltFunctionUse(Player player) => true;

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
			}

			return base.CanUseItem(player);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.altFunctionUse == 2)
				Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
			return false;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<StrippedBamboo>(), 20);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}

	internal class BambooKendoBlade_Held : BaseHeldProj
	{
		public ref float Counter => ref Projectile.ai[0];
		private readonly int chargeDuration = 2;
		private readonly int waitTime = 20;
		private readonly int animDelay = 30;

		Vector2 startPos = Vector2.Zero;
		Vector2 endPos = Vector2.Zero;

		private int firstHitWhoAmI;

		public override string Texture => "SpiritMod/Items/Sets/SwordsMisc/BambooKendoBlade/BambooKendoBlade";

		public override void SetStaticDefaults() => DisplayName.SetDefault("Bamboo Kendo Blade");

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(38);
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
		}

		public override void AbstractAI()
		{
			if (Counter < chargeDuration)
			{
				if (Counter == 0)
				{
					startPos = Owner.Center;
					SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, Projectile.Center);
				}

				Owner.GetModPlayer<MyPlayer>().AnimeSword = true;
				Owner.velocity = Vector2.Normalize(Projectile.velocity) * 16 * 8;
			}
			else
			{
				if (Counter == chargeDuration)
				{
					Owner.GetModPlayer<MyPlayer>().AnimeSword = false;
					Owner.velocity *= 0.001f;
					endPos = Owner.Center;

					if (startPos.Distance(endPos) > 50)
					{
						for (int i = 0; i < 5; i++)
						{
							Vector2 position = Vector2.Lerp(startPos, endPos, -.2f);
							Vector2 randomOff = Main.rand.NextVector2Unit() * Main.rand.NextFloat() * 34f;

							ParticleHandler.SpawnParticle(new ImpactLine(position + randomOff, Vector2.Normalize(endPos - startPos) * Main.rand.NextFloat() * 3f, Color.Lerp(Color.FloralWhite, Color.Yellow, Main.rand.NextFloat()) * .5f, new Vector2(1f, (startPos.Distance(endPos) + 40) / 72f), 12));

							Dust.NewDustPerfect(startPos + randomOff, DustID.Smoke, Vector2.Normalize(endPos - startPos) * 50 * Main.rand.NextFloat(0.05f, 0.23f), 100, Color.White with { A = 0 }, Main.rand.NextFloat(1.0f, 2.0f)).noGravity = true;
						}
					}

					int distance = 1000;
					float collisionPoint = 0;

					foreach (NPC npc in Main.npc)
					{
						if ((npc.CanBeChasedBy(Projectile) || npc.type == NPCID.TargetDummy) && npc.active && Collision.CheckAABBvLineCollision(npc.Hitbox.TopLeft(), npc.Hitbox.Size(), startPos, endPos, 50, ref collisionPoint))
						{
							if (startPos.Distance(npc.Center) < distance)
							{
								distance = (int)startPos.Distance(npc.Center);
								firstHitWhoAmI = npc.whoAmI;
							}
						}
					}
				}
				if (Counter >= (chargeDuration + waitTime + animDelay))
					Projectile.Kill();
			}
			if (++Counter >= (chargeDuration + waitTime))
				Projectile.localAI[0]++;

			float startTime = animDelay * .75f;
			float restingRotation = .5f + (Owner.direction == 1 ? MathHelper.Pi : 0);
			float addRotation = (Projectile.localAI[0] > startTime) ? (Projectile.localAI[0] - startTime) / (animDelay - startTime) * (Math.Abs(Projectile.velocity.ToRotation() - restingRotation) / MathHelper.PiOver2) : 0;

			Projectile.rotation = ((.785f + addRotation) * Owner.direction) + Projectile.velocity.ToRotation() + ((Owner.direction == -1) ? MathHelper.Pi : 0);
			Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Owner.AngleTo(Projectile.Center) - 1.57f);

			Owner.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			if (Counter == (chargeDuration + waitTime) && firstHitWhoAmI > -1 && targetHitbox == Main.npc[firstHitWhoAmI].Hitbox)
			{
				if (Main.npc[firstHitWhoAmI].active && Main.netMode != NetmodeID.Server)
					SpiritMod.primitives.CreateTrail(new AnimePrimTrailTwo(Main.npc[firstHitWhoAmI]));
				return true;
			}
			return false;
		}

		public override bool ShouldUpdatePosition() => false;

		public override Vector2 HoldoutOffset() => Vector2.Normalize(Projectile.velocity) * (25 - (Projectile.localAI[0] / animDelay * 25f));

		public override bool AutoAimCursor() => false;

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			SpriteEffects effects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2, Projectile.scale, effects, 0);
			
			return false;
		}
	}

	public class BambooKendoBladeLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.HeldItem);

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			if (drawInfo.drawPlayer.HeldItem.type == ModContent.ItemType<BambooKendoBlade>() && !drawInfo.drawPlayer.ItemAnimationActive)
				DrawItem(drawInfo);
		}

		public static void DrawItem(PlayerDrawSet drawInfo)
		{
			if (drawInfo.shadow != 0f || drawInfo.drawPlayer.frozen || drawInfo.drawPlayer.dead)
				return;

			Texture2D texture = TextureAssets.Item[ModContent.ItemType<BambooKendoBlade>()].Value;
			Vector2 drawPos = drawInfo.drawPlayer.Center + new Vector2(0, 6 * drawInfo.drawPlayer.gravDir - drawInfo.drawPlayer.gfxOffY);
			float rotation = (.5f * drawInfo.drawPlayer.direction) + MathHelper.Pi;

			drawInfo.DrawDataCache.Add(new DrawData(texture,
				drawPos - Main.screenPosition,
				null,
				Lighting.GetColor((int)drawInfo.drawPlayer.Center.X / 16, (int)drawInfo.drawPlayer.Center.Y / 16),
				rotation,
				texture.Size() / 2,
				1,
				drawInfo.playerEffect,
				0
			));
		}
	}
}