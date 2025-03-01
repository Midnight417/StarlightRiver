﻿using StarlightRiver.Content.Configs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Terraria.UI.Chat;

namespace StarlightRiver.Core.Systems.KeywordSystem
{
	public struct Keyword
	{
		public readonly string keyword;
		public readonly string message;
		public readonly string colorHex;

		public Keyword(string keyword, string message, Color color)
		{
			this.keyword = keyword;
			this.message = message;
			colorHex = BitConverter.ToString(new byte[] { color.R, color.G, color.B }).Replace("-", "");
		}
	}

	internal class KeywordScanner : GlobalItem
	{
		public static List<Keyword> keywords = new();

		public List<Keyword> thisKeywords = new();

		public override bool InstancePerEntity => true;

		public override void Load() //temporary debug behavior
		{
			if (Main.dedServ)
				return;

			ReLogic.Graphics.DynamicSpriteFont font = Terraria.GameContent.FontAssets.MouseText.Value;

			Stream stream = Mod.GetFileStream("Keywords.txt");

			var reader = new StreamReader(stream);
			string[] lines = reader.ReadToEnd().Split('\n');

			stream.Close();

			foreach (string line in lines)
			{
				string[] split = line.Split(" | ");
				keywords.Add(new Keyword(split[0], Helpers.Helper.WrapString(split[1], 200, font, 1), new Color(int.Parse(split[2]), int.Parse(split[3]), int.Parse(split[4]))));
			}
		}

		public override void Unload()
		{
			keywords = null;
		}

		private string BuildKeyword(Keyword word)
		{
			GUIConfig config = ModContent.GetInstance<GUIConfig>();

			string color;

			if (config.KeywordStyle == KeywordStyle.Colors || config.KeywordStyle == KeywordStyle.Both)
				color = word.colorHex;
			else
				color = "AAAAAA";

			if (config.KeywordStyle == KeywordStyle.Brackets || config.KeywordStyle == KeywordStyle.Both)
				return "[c/" + color + ":{" + word.keyword + "}]";
			else
				return "[c/" + color + ":" + word.keyword + "]";
		}

		public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
		{
			if (Main.LocalPlayer.controlUp && thisKeywords.Count > 0)
			{
				float width;
				float height = -16;
				Vector2 pos;

				ReLogic.Graphics.DynamicSpriteFont font = Terraria.GameContent.FontAssets.MouseText.Value;

				if (Main.MouseScreen.X < Main.screenWidth / 2)
				{
					string widest = lines.OrderBy(n => ChatManager.GetStringSize(font, n.Text, Vector2.One).X).Last().Text;
					width = ChatManager.GetStringSize(font, widest, Vector2.One).X;

					pos = new Vector2(x, y) + new Vector2(width + 30, 0);
				}
				else
				{
					Keyword widest = thisKeywords.OrderBy(n => ChatManager.GetStringSize(font, n.message, Vector2.One).X).Last();
					width = ChatManager.GetStringSize(font, widest.message, Vector2.One).X + 20;

					pos = new Vector2(x, y) - new Vector2(width + 30, 0);
				}

				Keyword widest2 = thisKeywords.OrderBy(n => ChatManager.GetStringSize(font, n.message, Vector2.One).X).Last();
				width = ChatManager.GetStringSize(font, widest2.message, Vector2.One).X + 20;

				foreach (Keyword keyword in thisKeywords)
				{
					height += ChatManager.GetStringSize(font, "{Dummy}\n" + keyword.message, Vector2.One).Y + 16;
				}

				Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)pos.X - 10, (int)pos.Y - 10, (int)width + 20, (int)height + 20), new Color(25, 20, 55) * 0.925f);

				foreach (Keyword keyword in thisKeywords)
				{
					Utils.DrawBorderString(Main.spriteBatch, BuildKeyword(keyword) + ":\n[c/AAAAAA: " + keyword.message.Replace("\n", "]\n [c/AAAAAA:") + "]", pos, Color.White);
					pos.Y += ChatManager.GetStringSize(font, "{Dummy}\n" + keyword.message, Vector2.One).Y + 16;
				}
			}

			return true;
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
		{
			foreach (TooltipLine line in tooltips)
			{
				if (line.Name == "ItemName")
					continue;

				line.Text = ScanLine(line.Text);
			}

			if (thisKeywords.Count > 0 && !Main.LocalPlayer.controlUp)
				tooltips.Add(new TooltipLine(Mod, "KeywordInfo", "[c/AAAAAA:Press UP for more info]"));
		}

		public string ScanLine(string input)
		{
			string[] strings = input.Split(' ');

			for (int k = 0; k < strings.Length; k++)
			{
				string word = strings[k];

				if (word.Length < 1)
					continue;

				if (word[0] == '\\')
				{
					strings[k] = word[1..];
					continue;
				}

				if (keywords.Any(n => string.Equals(n.keyword, word, StringComparison.OrdinalIgnoreCase)))
				{
					Keyword keyword = keywords.FirstOrDefault(n => string.Equals(n.keyword, word, StringComparison.OrdinalIgnoreCase));

					if (!thisKeywords.Contains(keyword))
						thisKeywords.Add(keyword);

					strings[k] = BuildKeyword(keyword);
					continue;
				}

				string suffix = word[^1..];

				if (suffix == ":" || suffix == "," || suffix == "." || suffix == ";") //Common punctuation that shouldnt invalidate a keyword.
				{
					if (keywords.Any(n => string.Equals(n.keyword, word[..^1], StringComparison.OrdinalIgnoreCase)))
					{
						Keyword keyword = keywords.FirstOrDefault(n => string.Equals(n.keyword, word[..^1], StringComparison.OrdinalIgnoreCase));

						if (!thisKeywords.Contains(keyword))
							thisKeywords.Add(keyword);

						strings[k] = BuildKeyword(keyword) + suffix;
						continue;
					}
				}
			}

			return string.Join(' ', strings);
		}
	}
}
