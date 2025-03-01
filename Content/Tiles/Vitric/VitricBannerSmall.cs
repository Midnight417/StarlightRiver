﻿using StarlightRiver.Content.Physics;
using StarlightRiver.Core.Systems.DummyTileSystem;
using System;
using Terraria.ID;

using static Terraria.ModLoader.ModContent;

namespace StarlightRiver.Content.Tiles.Vitric
{
	class VitricBannerSmall : DummyTile
	{
		public override string Texture => AssetDirectory.VitricTile + Name;

		public override int DummyType => ProjectileType<VitricBannerSmallDummy>();

		public override void SetStaticDefaults()
		{
			this.QuickSetFurniture(1, 1, DustType<Dusts.Air>(), SoundID.Tink, false, new Color(120, 100, 100));
		}
	}

	internal class VitricBannerSmallDummy : Dummy
	{
		public VitricBannerSmallDummy() : base(TileType<VitricBannerSmall>(), 16, 16) { }

		private VerletChain Chain;

		public override void SafeSetDefaults()
		{
			Chain = new VerletChain(8, false, Projectile.Center, 8)
			{
				constraintRepetitions = 2,//defaults to 2, raising this lowers stretching at the cost of performance
				drag = 2f,//This number defaults to 1, Is very sensitive
				forceGravity = new Vector2(0f, 0.3f),//gravity x/y
				scale = 1.1f,
				parent = Projectile
			};
		}

		public override void Update()
		{
			Chain.UpdateChain(Projectile.Center);

			Chain.IterateRope(WindForce);
			Projectile.ai[0] += 0.005f;
		}

		private void WindForce(int index)//wind
		{
			int offset = (int)(Projectile.position.X / 16 + Projectile.position.Y / 16);

			float sin = (float)Math.Sin(StarlightWorld.visualTimer + offset - index / 3f);

			float cos = (float)Math.Cos(Projectile.ai[0]);
			float sin2 = (float)Math.Sin(StarlightWorld.visualTimer + offset + cos);

			var pos = new Vector2(Chain.ropeSegments[index].posNow.X + 0.2f + sin2 * 0.2f, Chain.ropeSegments[index].posNow.Y + sin * 0.3f);

			Color color = new Color(150, 10, 35).MultiplyRGB(Color.White * (1 - sin * 0.2f)).MultiplyRGB(Lighting.GetColor((int)pos.X / 16, (int)pos.Y / 16));

			Chain.ropeSegments[index].posNow = pos;
			Chain.ropeSegments[index].color = color;
		}

		public override void Kill(int timeLeft)
		{
			VerletChainSystem.toDraw.Remove(Chain);
		}
	}

	class VitricBannerSmallItem : QuickTileItem
	{
		public VitricBannerSmallItem() : base("Short Flowing Banner", "", "VitricBannerSmall", 2, AssetDirectory.VitricTile, false) { }
	}
}
