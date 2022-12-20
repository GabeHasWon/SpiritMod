using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace SpiritMod.Dusts
{
    public class FrostBreath : ModDust
    {
        public override void OnSpawn(Dust dust)
		{
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

			dust.noGravity = true;
			dust.frame = new Rectangle(0, texture.Height / 3 * Main.rand.Next(3), texture.Width, texture.Height / 3);
			dust.noLight = true;

			dust.position -= dust.frame.Size() / 2; //Center the dust on spawn
		}

		public override Color? GetAlpha(Dust dust, Color lightColor) => dust.color;
		public override bool Update(Dust dust)
        {
            dust.color = Lighting.GetColor((int)(dust.position.X / 16), (int)(dust.position.Y / 16)).MultiplyRGB(new Color(156, 217, 255)) * 0.36f;
            dust.scale *= 0.992f;
            dust.velocity *= 0.97f;
            dust.rotation += 0.05f;

			Vector2 rotationOffset = (new Vector2(dust.scale) / MathHelper.PiOver2).RotatedBy(-dust.rotation);

			dust.position += dust.velocity * 0.1f + new Vector2(rotationOffset.X, -rotationOffset.Y);

			if (dust.scale <= 0.2f) dust.active = false;
            return false;
        }
    }
}