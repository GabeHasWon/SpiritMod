using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace SpiritMod.World.Micropasses;

internal class BismiteCavernMicropass : Micropass
{
	public override string WorldGenName => "Bismite Caverns";

	public override int GetWorldGenIndexInsert(List<GenPass> tasks, ref bool afterIndex) => tasks.FindIndex(genpass => genpass.Name.Equals("Sunflowers"));

	public override void Run(GenerationProgress progress, GameConfiguration config)
	{
		List<Point16> points = new();
		float repeats = Main.maxTilesX / 4200f * 24;
		int tries = 0;

		for (int i = 0; i < repeats; ++i)
		{
			if (++tries > 2000) //Short circuit if needed
				return;

			int x = WorldGen.genRand.Next(Main.maxTilesX / 6, Main.maxTilesX / 6 * 5);
			int y = WorldGen.genRand.Next((int)Main.worldSurface + 50, Main.maxTilesY - 400);

			if (!Collision.SolidCollision(new Vector2(x - 2, y - 2) * 16, 16 * 4, 16 * 4)) //Get at least a moderately sized opening
				points.Add(new(x, y));
			else
				i--;
		}

		foreach (var item in points)
			PopulateCavern(item);
	}

	private void PopulateCavern(Point16 item)
	{

	}
}
