﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using StarlightRiver.Core;

namespace StarlightRiver.Content.Bosses.GlassBoss
{
	class BrickGode : ILoadable
	{
		public float Priority => 1;

		public void Load()
		{
			for(int k = 1; k <= 19; k++)
				StarlightRiver.Instance.AddGore(AssetDirectory.GlassBoss + "Gore/Cluster" + k);

			StarlightRiver.Instance.AddGore(AssetDirectory.GlassBoss + "TempleHole");
		}

		public void Unload() { }
	}
}
