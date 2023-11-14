using Microsoft.Xna.Framework;
using SpiritMod.Items.Placeable.Furniture.Acid;
using SpiritMod.Tiles.BaseTile;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Furniture.Acid;

public class AcidDoorOpen : BaseDoorOpen
{
	public override int Item => ModContent.ItemType<AcidDoor>();

	public override int CloseID => ModContent.TileType<AcidDoorClosed>();

	public override void StaticDefaults(LocalizedText defaultName) => AddMapEntry(new Color(100, 122, 111), defaultName);

	public override void KillMultiTile(int i, int j, int frameX, int frameY) => SoundEngine.PlaySound(SoundID.NPCHit4, new Vector2(i, j) * 16);
}

public class AcidDoorClosed : BaseDoorClosed
{
	public override int Item => ModContent.ItemType<AcidDoor>();

	public override int OpenID => ModContent.TileType<AcidDoorOpen>();

	public override void StaticDefaults(LocalizedText defaultName) => AddMapEntry(new Color(100, 122, 111), defaultName);

	public override void KillMultiTile(int i, int j, int frameX, int frameY) => SoundEngine.PlaySound(SoundID.NPCHit4, new Vector2(i, j) * 16);
}