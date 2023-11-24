using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.FrigidSet.Frostbite
{
	public class HowlingScepter : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 7;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Magic;
			Item.width = 64;
			Item.height = 64;
			Item.useTime = 30;
			Item.mana = 4;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0;
			Item.value = Item.sellPrice(0, 0, 5, 0);
			Item.rare = ItemRarityID.Blue;
			Item.noUseGraphic = true;
			Item.autoReuse = true;
			Item.channel = true;
			Item.UseSound = SoundID.Item20;
			Item.shoot = ModContent.ProjectileType<FrostbiteProj>();
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) => position = Main.MouseWorld;

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			int heldType = ModContent.ProjectileType<HowlingScepterProj>();
			if (player.ownedProjectileCounts[heldType] < 1)
				Projectile.NewProjectile(source, player.MountedCenter, Vector2.Zero, heldType, damage, knockback, player.whoAmI);

			return true;
		}

		public override void AddRecipes()
		{
			Recipe modRecipe = CreateRecipe();
			modRecipe.AddIngredient(ModContent.ItemType<FrigidFragment>(), 9);
			modRecipe.AddTile(TileID.Anvils);
			modRecipe.Register();
		}
	}

	public class HowlingScepterProj : ModProjectile
	{
		public Player Owner => Main.player[Projectile.owner];

		public override LocalizedText DisplayName => Language.GetText("Mods.SpiritMod.Items.HowlingScepter.DisplayName");

		public override void SetStaticDefaults() => Main.projFrames[Type] = 4;

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(20);
			Projectile.penetrate = -1;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
		}

		public override void OnSpawn(IEntitySource source)
		{
			if (!Main.dedServ)
				SoundEngine.PlaySound(new SoundStyle("SpiritMod/Sounds/PageFlip") with { PitchVariance = 0.3f, Volume = 0.65f }, Owner.Center);
		}

		public override void AI()
		{
			Owner.itemRotation = MathHelper.WrapAngle(Projectile.velocity.ToRotation() + (Projectile.direction < 0 ? MathHelper.Pi : 0));
			Owner.heldProj = Projectile.whoAmI;

			Projectile.direction = Projectile.spriteDirection = Owner.direction;
			Projectile.rotation = .4f * Owner.direction;
			Projectile.Center = Owner.MountedCenter + new Vector2(Owner.direction * 14, 4);

			float rotation = Projectile.rotation - (1.57f * Projectile.direction);
			Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, rotation);
			Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, rotation);

			if (Main.rand.NextBool(10))
			{
				Dust dust = Dust.NewDustDirect(Projectile.position - new Vector2(0, Projectile.height / 2), Projectile.width, Projectile.height, DustID.GemSapphire);
				dust.noGravity = true;
				dust.velocity = new Vector2(0, -Main.rand.NextFloat(2f));
			}
			if (++Projectile.frameCounter >= 4)
			{
				Projectile.frameCounter = 0;
				if (Projectile.frame < (Main.projFrames[Type] - 1))
					Projectile.frame++;
			}

			if (Owner.channel)
				Projectile.timeLeft = 2;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle drawFrame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame, 0, -2);
			SpriteEffects effects = (Projectile.spriteDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), drawFrame, Projectile.GetAlpha(lightColor), Projectile.rotation, drawFrame.Size() / 2, Projectile.scale, effects, 0);
			return false;
		}

		public override bool? CanDamage() => false;

		public override bool? CanCutTiles() => false;
	}
}
