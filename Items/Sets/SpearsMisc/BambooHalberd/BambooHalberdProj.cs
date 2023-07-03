using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SpiritMod.Items.Sets.SpearsMisc.BambooHalberd
{
	public class BambooHalberdProj : ModProjectile
	{
		private int Counter
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		private int CounterMax => Main.player[Projectile.owner].itemTimeMax - 2;

		private readonly int lungeLength = 60;

		public override void SetStaticDefaults() => DisplayName.SetDefault("Bamboo Halberd");

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(10);
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			Projectile.rotation = Projectile.velocity.ToRotation();

			if (!player.frozen)
				Counter++;
			if (Counter < CounterMax && player.active && !player.dead)
				Projectile.timeLeft = 2;

			int halfTime = (int)(CounterMax * .5f);
			Vector2 TargetVector(float magnitude) => (Vector2.UnitX * magnitude).RotatedBy(Projectile.rotation);

			if (Counter > halfTime)
				Projectile.velocity = Vector2.Lerp(TargetVector(lungeLength), TargetVector(30), (float)Counter / CounterMax);
			else
				Projectile.velocity = Vector2.Lerp(TargetVector(10), TargetVector(lungeLength), (float)Counter / halfTime);

			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, -1.57f + Projectile.rotation);
			player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, -1.57f + Projectile.rotation);

			player.itemRotation = MathHelper.WrapAngle(player.AngleTo(Projectile.Center) - player.fullRotation - ((player.direction < 0) ? MathHelper.Pi : 0));
			player.heldProj = Projectile.whoAmI;
			player.ChangeDir(Projectile.direction);
			Projectile.Center = player.MountedCenter + Projectile.velocity;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			SpriteEffects effects = (Projectile.spriteDirection < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Vector2 origin = new Vector2((effects == SpriteEffects.FlipHorizontally) ? Projectile.width / 2 : texture.Width - (Projectile.width / 2), texture.Height / 2);

			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, effects, 0);
			return false;
		}
	}
}