﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StarlightRiver.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace StarlightRiver.Content.Items.BaseTypes
{
	public class RelicParticleDrawer : ModSystem
    {
		private static ParticleSystem.Update UpdateRelic => UpdateRelicBody;
		public static ParticleSystem RelicSystem;

		public override void Load()
		{
			RelicSystem = new ParticleSystem(AssetDirectory.Keys + "GlowHarshAlpha", UpdateRelic);
		}

		private static void UpdateRelicBody(Particle particle)
		{
			float sin = particle.StoredPosition.Y; //abusing storedposition cause theres no other way to pass in special data
			float fadeTime = particle.StoredPosition.X;

			particle.StoredPosition.Y += 0.05f;

			particle.Velocity.Y = -0.2f;
			particle.Velocity.X = 0.7f * (float)Math.Cos(sin);

			if (particle.Timer > 180)
				particle.Alpha += 0.05f;
			else if (particle.Timer < fadeTime)
				particle.Alpha -= 0.025f;

			particle.Alpha = MathHelper.Clamp(particle.Alpha, 0, 1);
			particle.Position += particle.Velocity;
			particle.Timer--;
		}

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
			RelicSystem.DrawParticles(spriteBatch);
        }
    }
	class RelicItem : GlobalItem
    {
        public bool isRelic = false;

        public bool doubled = false;

        public override bool InstancePerEntity => true;

		public Color RelicColor(int offset) => Color.Lerp(Color.Yellow, Color.LimeGreen, 0.5f + (float)(Math.Sin(Main.GameUpdateCount / 20f + offset)) / 2f);
        public Color RelicColorBad(int offset) => Color.Lerp(Color.Yellow, Color.OrangeRed, 0.5f + (float)(Math.Sin(Main.GameUpdateCount / 20f + offset)) / 2f);

		public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color ItemColor, Vector2 origin, float scale)
		{
			if (!isRelic)
				return;

			//spriteBatch.End();
			//spriteBatch.Begin(default, BlendState.Additive, default, default, default, default, Main.UIScaleMatrix);
			float particleScale = Main.rand.NextFloat(0.3f, 0.45f) * scale;

			float sin = Main.rand.NextFloat(6.28f);
			Vector2 pos = new Vector2(position.X + (frame.Width * (0.5f * (1 + (float)Math.Sin(sin)))) - (80 * particleScale), position.Y + (frame.Height * Main.rand.NextFloat(0.8f,1f)) - (80 * particleScale));

			Color color = Color.Lerp(Color.Gold, Color.White, Main.rand.NextFloat(0.2f));
			color.A = 0;
			if (Main.rand.NextBool(18))
				RelicParticleDrawer.RelicSystem.AddParticle(new Particle(pos, new Vector2(0, 0), 0, particleScale, color, 200, new Vector2(Main.rand.Next(100,130), sin), default, 0));

			//spriteBatch.End();
			//spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
		}

		public override GlobalItem Clone(Item item, Item itemClone)
		{
			return item.TryGetGlobalItem<RelicItem>(out var gi) ? gi : base.Clone(item, itemClone);
		}

		public override bool? PrefixChance(Item item, int pre, UnifiedRandom rand)
        {
            if (item.GetGlobalItem<RelicItem>().isRelic)
            {
                if (pre == -3)
                    return false;

                if (pre == -1)
                    return true;
            }

            return base.PrefixChance(item, pre, rand);
        }

        public override int ChoosePrefix(Item item, UnifiedRandom rand)
        {
            if (item.GetGlobalItem<RelicItem>().isRelic)
            {
                int result = base.ChoosePrefix(item, rand);
                return result != 0 ? result : ChoosePrefix(item, rand);
            }

            return base.ChoosePrefix(item, rand);
        }

        public override void UpdateAccessory(Item item, Player Player, bool hideVisual) //re-add vanilla prefixes to double power. This is bad, but its not IL atleast :)
        {
			if (!item.GetGlobalItem<RelicItem>().isRelic)
			{ 
				base.UpdateAccessory(item, Player, hideVisual);
				return;
				}

			if (item.prefix == 62)
			{
				Player.statDefense++;
			}
			if (item.prefix == 63)
			{
				Player.statDefense += 2;
			}
			if (item.prefix == 64)
			{
				Player.statDefense += 3;
			}
			if (item.prefix == 65)
			{
				Player.statDefense += 4;
			}
			if (item.prefix == 66)
			{
				Player.statManaMax2 += 20;
			}
			if (item.prefix == 67)
			{
				Player.GetCritChance(DamageClass.Melee) += 2;
				Player.GetCritChance(DamageClass.Ranged) += 2;
				Player.GetCritChance(DamageClass.Magic) += 2;
				Player.GetCritChance(DamageClass.Throwing) += 2;
			}
			if (item.prefix == 68)
			{
				Player.GetCritChance(DamageClass.Melee) += 4;
				Player.GetCritChance(DamageClass.Ranged) += 4;
				Player.GetCritChance(DamageClass.Magic) += 4;
				Player.GetCritChance(DamageClass.Throwing) += 4;
			}
			if (item.prefix == 69)
			{
				Player.GetDamage(DamageClass.Generic) += 0.01f; 
			}
			if (item.prefix == 70)
			{
				Player.GetDamage(DamageClass.Generic) += 0.02f;
			}
			if (item.prefix == 71)
			{
				Player.GetDamage(DamageClass.Generic) += 0.03f;
			}
			if (item.prefix == 72)
			{
				Player.GetDamage(DamageClass.Generic) += 0.04f;
			}
			if (item.prefix == 73)
			{
				Player.moveSpeed += 0.01f;
			}
			if (item.prefix == 74)
			{
				Player.moveSpeed += 0.02f;
			}
			if (item.prefix == 75)
			{
				Player.moveSpeed += 0.03f;
			}
			if (item.prefix == 76)
			{
				Player.moveSpeed += 0.04f;
			}
			if (item.prefix == 77)
			{
				Player.GetAttackSpeed(DamageClass.Melee) += 0.01f;
			}
			if (item.prefix == 78)
			{
				Player.GetAttackSpeed(DamageClass.Melee) += 0.02f;
			}
			if (item.prefix == 79)
			{
				Player.GetAttackSpeed(DamageClass.Melee) += 0.03f;
			}
			if (item.prefix == 80)
			{
				Player.GetAttackSpeed(DamageClass.Melee) += 0.04f;
			}
		}

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.GetGlobalItem<RelicItem>().isRelic)
            {
                for(int k = 0; k < tooltips.Count; k++)
                {
                    var line = tooltips[k];

                    if (line.Name == "ItemName")
                    {
                        line.Text = line.Text.Insert(0, "Twice ");
                        line.OverrideColor = RelicColor(k);
                    }

					if (line.IsModifier)
					{
						line.OverrideColor = RelicColor(k);

						if (item.accessory)
							line.Text = DoubleIntValues(line.Text);
					}

                    if (line.IsModifierBad)
                        line.OverrideColor = RelicColorBad(k);
                }

                var newLine = new TooltipLine(Mod, "relicLine", "Cannot be reforged");
                newLine.OverrideColor = new Color(255, 180, 100);
                tooltips.Add(newLine);
            }
        }

		public string DoubleIntValues(string input)
        {
			for (int k = 0; k < input.Length; k++)
			{
				int result = 0;
				if (int.TryParse(input[k].ToString(), out result))
				{
					input = input.Remove(k, 1);
					input = input.Insert(k, (result * 2).ToString());
				}
			}

			return input;
        }

        public override void SaveData(Item item, TagCompound tag)
        {
			if(item.GetGlobalItem<RelicItem>().isRelic)
				tag["isRelic"] = true;
		}

        public override void LoadData(Item item, TagCompound tag)
        {
			if (tag.ContainsKey("isRelic"))
				item.GetGlobalItem<RelicItem>().isRelic = tag.GetBool("isRelic");
		}
	}
}
