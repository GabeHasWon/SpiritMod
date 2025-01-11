using Microsoft.Xna.Framework;
using SpiritMod.NPCs.AsteroidDebris;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SpiritMod.Tiles.Block;

public class Asteroid : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileBlockLight[Type] = true;
	
		HitSound = SoundID.Tink;
		MinPick = 40;

		AddMapEntry(new Color(99, 79, 49));
	}

	public override bool CanKillTile(int i, int j, ref bool blockDamaged) => Main.LocalPlayer.HeldItem.type != ItemID.ReaverShark;

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (!fail)
		{
			if (Main.rand.NextBool(3))
			{
				NPC npc = NPC.NewNPCDirect(new Terraria.DataStructures.EntitySource_TileBreak(i, j), (i * 16) + 8, (j * 16) + 8, ModContent.NPCType<AsteroidDebris>());
				npc.velocity = new Vector2(Main.rand.NextFloat(-1.0f, 1.0f) * 1.5f, Main.rand.NextFloat(-1.0f, 1.0f) * 1.5f);
			}

			if (Main.netMode != NetmodeID.Server)
			{
				int randomAmount = Main.rand.Next(3) + 1;

				for (int e = 0; e < randomAmount; e++)
					Gore.NewGore(new Terraria.DataStructures.EntitySource_TileBreak(i, j), new Vector2(i, j).ToWorldCoordinates(),
						new Vector2(Main.rand.NextFloat(-1.0f, 1.0f) * 3f, Main.rand.NextFloat(-1.0f, 1.0f) * 3f), Mod.Find<ModGore>("AsteroidDebrisSmall").Type);
			}
		}
	}

	//UNCOMMENT THIS IF YOU WANT CRYSTALS TO GROW ON REGULAR ASTEROIDS
	/*    public override void RandomUpdate(int i, int j)
            {
                if (!Framing.GetTileSafely(i, j - 1).active() && Main.rand.Next(50) == 0)
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            ReachGrassTile.PlaceObject(i, j - 1, mod.TileType("GlowShard1"));
                            NetMessage.SendObjectPlacment(-1, i, j - 1, mod.TileType("GlowShard1"), 0, 0, -1, -1);
                            break;
                        case 1:
                            ReachGrassTile.PlaceObject(i, j - 1, ModContent.TileType<GreenShard>());
                            NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<GreenShard>(), 0, 0, -1, -1);
                            break;
                        case 2:
                            ReachGrassTile.PlaceObject(i, j - 1, ModContent.TileType<PurpleShard>());
                            NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<PurpleShard>(), 0, 0, -1, -1);
                            break;
                    }
                }
            }*/
}