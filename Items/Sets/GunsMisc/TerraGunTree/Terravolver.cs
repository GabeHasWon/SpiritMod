using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Particles;
using SpiritMod.Projectiles.Bullet;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.GunsMisc.TerraGunTree
{
	public class Terravolver : ModItem
	{
		private int charger;

		/* public override void SetStaticDefaults() => Tooltip.SetDefault("Shoots elemental bullets and bombs that inflict powerful burns\n" +
			"Right click while holding for a shotgun blast\n" +
			"33% chance to not consume ammo\n" +
			"'Nature goes out with a bang'"); */

		public override void SetDefaults()
		{
			Item.damage = 43;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 58;
			Item.height = 32;
			Item.useTime = Item.useAnimation = 8;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = .3f;
			Item.useTurn = false;
			Item.value = Item.sellPrice(0, 3, 0, 0);
			Item.rare = ItemRarityID.Yellow;
			Item.UseSound = SoundID.Item92;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<TerraBullet>();
			Item.shootSpeed = 14f;
			Item.useAmmo = AmmoID.Bullet;
		}

		public override Vector2? HoldoutOffset() => new Vector2(-10, -4);

		public override bool AltFunctionUse(Player player) => true;

		public override void UseItemFrame(Player player)
		{
			if (!player.IsUsingAlt())
				return;

			int offset = (int)MathHelper.Clamp(player.itemAnimation - 10, 0, Item.useAnimation);
			player.itemLocation -= new Vector2(offset * player.direction, 0).RotatedBy(player.itemRotation);
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
			if (!player.IsUsingAlt())
				velocity = velocity.RotatedByRandom(.0785f);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y - 1)) * 58f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
				ParticleHandler.SpawnParticle(new TerravolverFlash(player, position, velocity.ToRotation()));
			}

			if (player.IsUsingAlt()) //Fire a shotgun blast
			{
				if (type == ProjectileID.Bullet) type = ModContent.ProjectileType<TerraBullet1>();
				player.itemTime = player.itemAnimation = 29;

				for (int i = 0; i <= 3; i++)
					Projectile.NewProjectile(source, position, velocity.RotatedByRandom(.3f), type, damage, 8f, player.whoAmI);
			}
			else //Periodically fire bombs in addition to normal fire
			{
				if (type == ProjectileID.Bullet) type = ModContent.ProjectileType<TerraBullet>();

				if (++charger >= 7)
				{
					Projectile.NewProjectile(source, position, velocity.RotatedByRandom(.1f), ModContent.ProjectileType<TerraBomb>(), (int)(damage * 1.33f), knockback, player.whoAmI);
					charger = 0;
				}
				Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
			}
			return false;
		}

		public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextFloat() > (1 / 3f);

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ModContent.ItemType<TrueShadowShot>(), 1);
			recipe.AddIngredient(ModContent.ItemType<TrueHolyBurst>(), 1);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();

			Recipe recipe1 = CreateRecipe(1);
			recipe1.AddIngredient(ModContent.ItemType<TrueCrimbine>(), 1);
			recipe1.AddIngredient(ModContent.ItemType<TrueHolyBurst>(), 1);
			recipe1.AddTile(TileID.MythrilAnvil);
			recipe1.Register();
		}
	}

	internal class TerravolverFlash : Particle
	{
		private readonly Entity entity;
		private const int _numFrames = 4;
		private int _frame;
		private readonly int _direction;
		private const int _displayTime = 8;

		public TerravolverFlash(Entity attachedEntity, Vector2 position, float rotation)
		{
			entity = attachedEntity;
			Position = position;
			if (attachedEntity != null)
				_offset = Position - entity.Center;

			_direction = Main.rand.NextBool() ? -1 : 1;
			Scale = 1;
			Rotation = (_direction < 0) ? (rotation - MathHelper.Pi) : rotation;
		}

		private Vector2 _offset = Vector2.Zero;
		public override void Update()
		{
			if (entity != null)
			{
				if (!entity.active)
				{
					Kill();
					return;
				}
				Position = entity.Center + _offset;
			}

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