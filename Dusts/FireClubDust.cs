using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Dusts
{
    public class FireClubDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = false;
            dust.noLight = false;
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) => new Color(220, 230, 220, 100);

		public override bool Update(Dust dust)
        {
            dust.position += dust.velocity;
            dust.velocity.Y += 0.2f;

			Tile tile = Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16];
			if (tile.HasTile && tile.BlockType == BlockType.Solid && Main.tileSolid[tile.TileType])
			{
                dust.velocity *= -0.5f;
            }

            dust.rotation = dust.velocity.ToRotation();
            dust.scale *= 0.99f;
            if (dust.scale < 0.2f)
            {
                dust.active = false;
            }
            return false;
        }
    }

}