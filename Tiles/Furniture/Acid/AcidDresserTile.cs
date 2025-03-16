using Microsoft.Xna.Framework;
using SpiritMod.Items.Placeable.Furniture.Acid;
using SpiritMod.Tiles.BaseTile;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Furniture.Acid;

public class AcidDresserTile : BaseDresser
{
	public override ModItem Item => ModContent.GetInstance<AcidDresser>();

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		base.KillMultiTile(i, j, frameX, frameY);
		SoundEngine.PlaySound(SoundID.NPCHit4, new Vector2(i, j) * 16);
	}
}