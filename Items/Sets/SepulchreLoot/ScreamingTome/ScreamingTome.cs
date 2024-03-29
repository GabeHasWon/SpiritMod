using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SepulchreLoot.ScreamingTome
{
    public class ScreamingTome : ModItem
    {
		public override void SetDefaults()
        {
            Item.damage = 20;
            Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.DamageType = DamageClass.Magic;
            Item.width = 36;
            Item.height = 40;
            Item.useTime = Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.HiddenAnimation;
            Item.shoot = ModContent.ProjectileType<ScreamingSkull>();
            Item.shootSpeed = 18f;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item104;
            Item.value = Item.buyPrice(0, 1, 40, 0);
            Item.useTurn = false;
            Item.mana = 8;
            Item.channel = true;
        }

		public override bool CanUseItem(Player player)
		{
			int skulls = Main.projectile.Where(x => x.active && x.type == Item.shoot && x.ai[1] == 0).Count();
			return skulls < 4;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
        {
            Projectile.NewProjectile(source, player.Center - (Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.0f, 50.0f)), Vector2.Zero, type, damage, knockback, player.whoAmI, player.whoAmI);

			int heldType = ModContent.ProjectileType<ScreamingTomeProj>();

			if (player.ownedProjectileCounts[heldType] < 1)
				Projectile.NewProjectile(source, player.MountedCenter, Vector2.Zero, heldType, damage, knockback, player.whoAmI);

			return false;
        }

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
			=> GlowmaskUtils.DrawItemGlowMaskWorld(Main.spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
	}

	public class ScreamingTomeProj : ModProjectile
	{
		public Player Owner => Main.player[Projectile.owner];

		public override LocalizedText DisplayName => Language.GetText("Mods.SpiritMod.Items.ScreamingTome.DisplayName");

		public override void SetStaticDefaults() => Main.projFrames[Type] = 5;

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
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, new Vector2(Owner.direction * 20, -4 * Owner.gravDir), .1f);

			Owner.itemRotation = MathHelper.WrapAngle(Projectile.velocity.ToRotation() + (Projectile.direction < 0 ? MathHelper.Pi : 0));
			Owner.heldProj = Projectile.whoAmI;

			Projectile.direction = Projectile.spriteDirection = Owner.direction;
			Projectile.rotation = 0f;
			Projectile.Center = Owner.MountedCenter + Projectile.velocity;

			float rotation = Projectile.velocity.ToRotation() - (1.57f - (.785f * Projectile.direction));
			Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, rotation);
			Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, rotation);

			if (++Projectile.frameCounter >= 4)
			{
				Projectile.frameCounter = 0;
				Projectile.frame = ++Projectile.frame % Main.projFrames[Type];
			}

			if (Owner.channel)
				Projectile.timeLeft = 2;
			else
				Owner.reuseDelay = Owner.itemTimeMax;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Rectangle drawFrame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame, 0, -2);
			SpriteEffects effects = (Projectile.spriteDirection == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), drawFrame, Projectile.GetAlpha(lightColor), Projectile.rotation, drawFrame.Size() / 2, Projectile.scale, effects, 0);
			Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), drawFrame, Projectile.GetAlpha(Color.White), Projectile.rotation, drawFrame.Size() / 2, Projectile.scale, effects, 0);

			return false;
		}

		public override bool? CanDamage() => false;

		public override bool? CanCutTiles() => false;

		public override bool ShouldUpdatePosition() => false;
	}
}
