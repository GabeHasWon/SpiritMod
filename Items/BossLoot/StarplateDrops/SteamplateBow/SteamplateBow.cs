using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using SpiritMod.Projectiles.Arrow;
using SpiritMod.Projectiles.BaseProj;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.BossLoot.StarplateDrops.SteamplateBow
{
	public class SteamplateBow : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Starcharger");
			// Tooltip.SetDefault("Left-click to shoot Positive Arrows\nRight-click to shoot Negative Arrows\nOppositely charged arrows explode upon touching each other");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
		}

		public override void SetDefaults()
		{
			Item.damage = 28;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 28;
			Item.height = 36;
			Item.useTime = Item.useAnimation = 26;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ProjectileID.WoodenArrowFriendly;
			Item.useAmmo = AmmoID.Arrow;
			Item.knockBack = 1;
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item5;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.autoReuse = true;
			Item.shootSpeed = 12f;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) => GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
		
		public override Vector2? HoldoutOffset() => new Vector2(-4, 0);
		
		public override bool AltFunctionUse(Player player) => true;
		
		public override void HoldItem(Player player) => player.GetModPlayer<SteamplateBowPlayer>().active = true;

		public override bool CanUseItem(Player player)
		{
			if (player.IsUsingAlt())
			{
				Item.noUseGraphic = false;
				Item.UseSound = SoundID.Item5;
				Item.useTime = Item.useAnimation = 26;
			}
			else
			{
				Item.noUseGraphic = true;
				Item.UseSound = null;
				Item.useTime = Item.useAnimation = 36;
			}
			return true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			SteamplateBowPlayer modPlayer = player.GetModPlayer<SteamplateBowPlayer>();
			if (player.IsUsingAlt())
			{
				type = ModContent.ProjectileType<NegativeArrow>();
				modPlayer.negative = true;

				Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
			}
			else
			{
				//type = ModContent.ProjectileType<PositiveArrow>();
				modPlayer.negative = false;

				int heldType = ModContent.ProjectileType<SteamplateBowProj>();
				if (player.ownedProjectileCounts[heldType] < 1)
					Projectile.NewProjectile(source, position, velocity, heldType, (int)(damage * .3f), knockback, player.whoAmI);
			}
			return false;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<CosmiliteShard>(), 17)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}

	public class SteamplateBowProj : BaseHeldProj
	{
		private ref float AiTimer => ref Projectile.ai[0];

		private int AiTimerMax => Owner.itemTimeMax;

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Starcharger");
			Main.projFrames[Type] = 6;
		}

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(26);
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
		}

		public override bool? CanDamage() => false;

		public override bool? CanCutTiles() => false;

		public override void AbstractAI()
		{
			const int delayTime = 12;

			if ((++AiTimer > AiTimerMax) || Owner.dead || !Owner.active)
			{
				if (AiTimer > (AiTimerMax + delayTime))
				{
					Projectile.Kill();
					Owner.itemTime = Owner.itemAnimation = 0;
				}
			}
			else
			{
				const int numShots = 3;
				Projectile.frame = (int)(AiTimer / AiTimerMax * Main.projFrames[Type] * numShots) % Main.projFrames[Type];

				if (AiTimer % (AiTimerMax / numShots) == 0)
				{
					Vector2 pos = Projectile.Center + (Projectile.velocity * 14f);

					SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);
					ParticleHandler.SpawnParticle(new StarParticle(pos, Vector2.Zero, Color.Cyan, .25f, 10, 0));
					for (int i = 0; i < 5; i++)
						Dust.NewDustPerfect(pos, DustID.Electric, (Projectile.velocity * Main.rand.NextFloat(.5f, 3f)).RotatedByRandom(1f), 0, default, Main.rand.NextFloat(.5f, 1f)).noGravity = true;

					if (Owner.whoAmI == Main.myPlayer)
						Projectile.NewProjectile(Entity.GetSource_FromAI(), pos, Projectile.velocity * Owner.HeldItem.shootSpeed, ModContent.ProjectileType<PositiveArrow>(), Projectile.damage, Projectile.knockBack, Owner.whoAmI);
				} //Fire
			}

			Player.CompositeArmStretchAmount amount = Projectile.frame switch
			{
				1 => Player.CompositeArmStretchAmount.Full,
				2 => Player.CompositeArmStretchAmount.ThreeQuarters,
				3 => Player.CompositeArmStretchAmount.Quarter,
				_ => Player.CompositeArmStretchAmount.None
			};

			Owner.SetCompositeArmFront(true, amount, Projectile.rotation - (1.57f * Projectile.direction));
			Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (1f * Projectile.direction));
		}

		public override Vector2 HoldoutOffset() => (Vector2.UnitX * 6f).RotatedBy(Projectile.velocity.ToRotation());

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			SpriteEffects effects = (Projectile.spriteDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame, 0, -2);

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() / 2, Projectile.scale, effects, 0);
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, frame.Size() / 2, Projectile.scale, effects, 0);

			return false;
		}
	}

	public class SteamplateBowPlayer : ModPlayer
	{
		public bool active = false;
		public bool negative = false;
		private bool negativeCached = false;

		public int counter;
		public readonly int counterMax = 10;

		public override void ResetEffects()
		{
			if (negative != negativeCached)
			{
				counter = counterMax;
				negativeCached = negative;
			}
			if (counter > 0)
				counter--;
			active = false;
		}
	}
}