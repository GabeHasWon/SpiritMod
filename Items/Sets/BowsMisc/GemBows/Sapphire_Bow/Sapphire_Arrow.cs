using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace SpiritMod.Items.Sets.BowsMisc.GemBows.Sapphire_Bow
{
	public class Sapphire_Arrow : GemArrow
	{
		public Sapphire_Arrow() : base(Color.Blue, DustID.GemSapphire) { }
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sapphire Arrow");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; 
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

		protected override void SafeSetDefaults() => Projectile.aiStyle = -1;

		static readonly int gravityTimer = 30;
		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi / 2;

			if (++Projectile.ai[0] <= gravityTimer && Main.myPlayer == Projectile.owner) { //check for a timer, if the projectile's owner is the client
				//messy looking way of making the projectile always maintain the same total velocity, takes the product of a vector2 lerp to home in on cursor, safe normalizes it with a default value of the normalized projectile velocity, then multiplies by projectile velocity length
				Projectile.velocity = Projectile.velocity.Length() * Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.MouseWorld) * Projectile.velocity.Length(), 0.08f).SafeNormalize(Vector2.Normalize(Projectile.velocity));

				Projectile.netUpdate = true; //netupdate needs to be called due to changes in velocity dependant on mouse position on only one client
			}
			else if(Projectile.ai[0] > gravityTimer) //otherwise, add gravity, but still dependant on a timer
				Projectile.velocity.Y += 0.25f;
		}
	}
}
