using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using SpiritMod.Projectiles.BaseProj;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Weapon.Magic.ShadowbreakWand
{
	public class ShadowflameStoneStaff : ModItem
	{
		public override void SetStaticDefaults() => SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");

		public override void SetDefaults()
		{
			Item.width = 44;
			Item.height = 46;
			Item.value = Item.buyPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.Green;
			Item.damage = 16;
			Item.knockBack = 5;
			Item.useStyle = ItemUseStyleID.HiddenAnimation;
			Item.useTime = Item.useAnimation = 24;
			Item.mana = 10;
			Item.DamageType = DamageClass.Magic;
			Item.channel = true;
			Item.UseSound = SoundID.Item8;
			Item.autoReuse = false;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<ShadowbreakOrb>();
			Item.shootSpeed = 10f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			int heldType = ModContent.ProjectileType<ShadowflameStoneStaff_Held>();
			if (player.ownedProjectileCounts[heldType] < 1)
				Projectile.NewProjectile(source, position, velocity, heldType, 0, 0, player.whoAmI);

			return true;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) =>
			GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);
	}

	internal class ShadowflameStoneStaff_Held : BaseHeldProj
	{
		public ref float Counter => ref Projectile.ai[0];

		public override string Texture => "SpiritMod/Items/Weapon/Magic/ShadowbreakWand/ShadowflameStoneStaff";

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
			float spinEnd = ShadowbreakOrb.CounterMax * .25f;

			if (Counter == 0)
				Projectile.scale = 0;
			if (Counter == (int)spinEnd)
				Projectile.localAI[0] = 1f;

			Projectile.rotation = (-0.785f + (MathHelper.Max(1f - ((float)Counter / spinEnd), 0) * 4) - (Projectile.localAI[0] * .2f)) * Owner.direction;
			Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, -1.57f * Owner.direction);

			Counter = MathHelper.Min(Counter + 1, ShadowbreakOrb.CounterMax);
			if ((Counter + 1) == ShadowbreakOrb.CounterMax)
			{
				Projectile.localAI[0] = 1f;

				Vector2 tipPos = Projectile.Center + (new Vector2((Projectile.width - 10) * Owner.direction, -(Projectile.height - 10)) / 2).RotatedBy(Projectile.rotation);
				ParticleHandler.SpawnParticle(new PulseCircle(Owner, Color.Magenta, 40, 12, PulseCircle.MovementType.Inwards, tipPos));
			}

			Projectile.localAI[0] = MathHelper.Max(Projectile.localAI[0] - .1f, 0);
			Projectile.scale = MathHelper.Min(Projectile.scale + .1f, 1);

			ChannelKillCheck();
		}

		public override bool? CanDamage() => false;

		public override bool ShouldUpdatePosition() => false;

		public override Vector2 HoldoutOffset() => new(6 * Owner.direction, 0);

		public override bool AutoAimCursor() => false;

		public override bool PreDraw(ref Color lightColor)
		{
			Projectile.QuickDraw();
			Projectile.QuickDrawGlow();

			if (Counter >= ShadowbreakOrb.CounterMax)
			{
				Texture2D twinkle = TextureAssets.Projectile[79].Value;
				float scale = Projectile.scale * Projectile.localAI[0];
				Vector2 tipPos = Projectile.Center + (new Vector2((Projectile.width - 10) * Owner.direction, -(Projectile.height - 10)) / 2).RotatedBy(Projectile.rotation);

				Main.EntitySpriteDraw(twinkle, tipPos - Main.screenPosition, null, Color.LightPink with { A = 0 }, .785f, twinkle.Size() / 2, scale, SpriteEffects.None, 0);
			}

			#region portalFX
			int numFrames = 7;
			int frame = (int)Counter / 3;
			if (frame >= numFrames)
				return false;

			Player owner = Main.player[Projectile.owner];

			Texture2D portalTex = ModContent.Request<Texture2D>(Texture + "_Portal").Value;
			Rectangle drawFrame = portalTex.Frame(1, numFrames, 0, frame, 0, -2);
			Vector2 dirUnit = Vector2.Normalize(Projectile.velocity);

			Main.EntitySpriteDraw(portalTex, owner.Center + (dirUnit * 25) - Main.screenPosition, drawFrame, Color.White, dirUnit.ToRotation(), drawFrame.Size() / 2, 1, (owner.direction == -1) ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);
			#endregion
			return false;
		}
	}
}
