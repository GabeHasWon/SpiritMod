using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using SpiritMod.Projectiles.Bullet;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace SpiritMod.Items.BossLoot.StarplateDrops
{
	public class OrionPistol : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Orion's Quickdraw");
			Tooltip.SetDefault("Converts regular bullets into Orion Bullets\nOrion Bullets leave lingering stars in their wake\n'Historically accurate'");
			SpiritGlowmask.AddGlowMask(Item.type, Texture + "_Glow");
		}

		public override void SetDefaults()
		{
			Item.damage = 28;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 56;
			Item.height = 26;
			Item.useTime = 23;
			Item.useAnimation = 23;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 0;
			Item.useTurn = false;
			Item.value = Item.sellPrice(0, 2, 0, 0);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item67 with { Volume = 0.5f, PitchVariance = 1.0f };
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<OrionBullet>();
			Item.shootSpeed = 6f;
			Item.useAmmo = AmmoID.Bullet;
		}

		public override void UseItemFrame(Player player)
		{
			int offset = (int)MathHelper.Clamp(player.itemAnimation - 18, 0, Item.useAnimation);
			player.itemLocation -= new Vector2(offset * player.direction, 0).RotatedBy(player.itemRotation);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y - 1)) * 45f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
				position += muzzleOffset;

			if (!Main.dedServ)
				ParticleHandler.SpawnParticle(new OrionPistolFlash(position, 1, velocity.ToRotation()));

			if (type == ProjectileID.Bullet)
				type = ModContent.ProjectileType<OrionBullet>();

			Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
			return false;
		}

		public override Vector2? HoldoutOffset() => new Vector2(-10, 0);
		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI) => GlowmaskUtils.DrawItemGlowMaskWorld(spriteBatch, Item, ModContent.Request<Texture2D>(Texture + "_Glow").Value, rotation, scale);

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.FlintlockPistol, 1);
			recipe.AddIngredient(ModContent.ItemType<CosmiliteShard>(), 16);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}

	internal class OrionPistolFlash : Particle
	{
		private const int _numFrames = 4;
		private int _frame;
		private readonly int _direction;
		private const int _displayTime = 8;

		public OrionPistolFlash(Vector2 position, float scale, float rotation)
		{
			Position = position;
			Scale = scale;
			_direction = Main.rand.NextBool() ? -1 : 1;
			Rotation = (_direction < 0) ? (rotation - MathHelper.Pi) : rotation;
		}

		public override void Update()
		{
			_frame = (int)(_numFrames * TimeActive / _displayTime);
			Lighting.AddLight(Position, Color.LightBlue.ToVector3() / 3);
			if (TimeActive > _displayTime)
				Kill();
		}

		public override bool UseCustomDraw => true;

		public override void CustomDraw(SpriteBatch spriteBatch)
		{
			Texture2D tex = ParticleHandler.GetTexture(Type);
			var DrawFrame = new Rectangle(0, _frame * tex.Height / _numFrames, tex.Width, tex.Height / _numFrames);
			spriteBatch.Draw(tex, Position - Main.screenPosition, DrawFrame, Color.White, Rotation, DrawFrame.Size() / 2, Scale, (_direction > 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		}
	}
}