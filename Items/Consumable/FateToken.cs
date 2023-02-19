using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpiritMod.Buffs;
using SpiritMod.Mechanics.CooldownItem;
using SpiritMod.Particles;
using SpiritMod.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Items.Consumable
{
	public class FateToken : ModItem, ICooldownItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fate Token");
			Tooltip.SetDefault("Protects the user from fatal damage for 1 minute\n2 minute cooldown");
		}

		public override void SetDefaults()
		{
			Item.width = 36;
			Item.height = 36;
			Item.maxStack = 999;
			Item.rare = ItemRarityID.Red;
			Item.value = Item.buyPrice(3, 0, 0, 0);
			Item.useAnimation = 45;
			Item.useTime = 45;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.noUseGraphic = true;
			Item.UseSound = SoundID.Item44;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 2f;
			Item.consumable = true;
		}

		public override bool? UseItem(Player player)
		{
			player.AddBuff(ModContent.BuffType<FateBlessing>(), 3600);
			CooldownGItem.GetCooldown(Type, player, 7200); //Apply a cooldown for 2 minutes

			Rectangle rect = player.getRect();
			for (int i = 0; i < 20; i++)
			{
				Color color = Color.Lerp(Main.DiscoColor, Color.White, Main.rand.NextFloat(1.0f));
				Dust dust = Dust.NewDustDirect(rect.BottomLeft(), rect.Width, 0, DustID.FireworksRGB, 0, -Main.rand.NextFloat(0.5f, 3f), 80, color, Main.rand.NextFloat(0.5f, 1.0f));
				dust.noGravity = true;
			}
			return true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			velocity = (velocity * Main.rand.NextFloat(0.6f, 1.0f)).RotatedByRandom(.12f);
			velocity.Y -= 5f;

			if (!Main.dedServ)
				ParticleHandler.SpawnParticle(new FateTokenParticle(player.Center, velocity, 1f, 0f));
			return false;
		}

		public override bool CanUseItem(Player player) => CooldownGItem.GetCooldown(Type, player) == 0;
	}

	public class FateTokenParticle : Particle
	{
		private const int _numFrames = 4;
		private int _frame;

		private const int _displayTime = 30;
		private const int frameDur = 4;

		public FateTokenParticle(Vector2 position, Vector2 velocity, float scale, float rotation)
		{
			Position = position;
			Velocity = velocity;
			Scale = scale;
			Rotation = rotation;
		}

		public override void Update()
		{
			Position += Velocity;
			Velocity.Y += 0.3f;
			Rotation += Velocity.X / 20;

			_frame = (int)(_numFrames * TimeActive / frameDur % _numFrames);

			Lighting.AddLight(Position, Color.Goldenrod.ToVector3() / 2);
			if (Main.rand.NextBool(2))
			{
				Color color = Color.Lerp(Main.DiscoColor, Color.White, Main.rand.NextFloat(1.0f));
				Dust dust = Dust.NewDustPerfect(Position + Origin, DustID.FireworksRGB, Main.rand.NextVector2Unit(), 80, color, Main.rand.NextFloat(0.5f, 1.0f));
				dust.noGravity = true;
			}

			if (TimeActive > _displayTime)
				Kill();
		}

		public override bool UseCustomDraw => true;

		public override bool UseAdditiveBlend => false;

		public override void CustomDraw(SpriteBatch spriteBatch)
		{
			Texture2D tex = ParticleHandler.GetTexture(Type);
			var DrawFrame = new Rectangle(0, _frame * tex.Height / _numFrames, tex.Width, (tex.Height / _numFrames) - 2);

			spriteBatch.Draw(tex, Position - Main.screenPosition, DrawFrame, Color.White, Rotation, DrawFrame.Size() / 2, Scale, SpriteEffects.None, 0);

			int threshold = _displayTime - 8;
			if (TimeActive >= threshold)
			{
				float scalar = TimeActive - threshold;

				Texture2D star = ModContent.Request<Texture2D>("SpiritMod/Effects/Masks/Star").Value;
				DrawAberration.DrawChromaticAberration(Vector2.UnitX.RotatedBy(Rotation), 1.5f, delegate (Vector2 offset, Color colorMod)
				{
					spriteBatch.Draw(star, Position + offset - Main.screenPosition, null, colorMod with { A = 0 }, scalar / 5, star.Size() / 2, scalar / 14, SpriteEffects.None, 0);
				});
			}
		}
	}
}
