﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.ModLoader;

namespace SpiritMod.Utilities.Wiki
{
	public class WikiWriter : ModSystem
	{
		public override bool IsLoadingEnabled(Mod mod) =>
#if DEBUG
			false;
#else
			false;
#endif

		public static string Path => Environment.CurrentDirectory + "/Wiki/";

		public override void OnWorldLoad() => WriteWiki();

		public static void WriteWiki()
		{
			if (!Directory.Exists(Path))
				Directory.CreateDirectory(Path);

			ItemWikiWriter.WriteItems();
			NPCWikiWriter.WriteNPCs();
		}

		public static IEnumerable<Type> GetAllOf<T>() => SpiritMod.Instance.GetType().Assembly.GetTypes().Where(x => typeof(T).IsAssignableFrom(x) && !x.IsAbstract);
	}
}
