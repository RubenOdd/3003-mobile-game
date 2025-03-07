using System.Collections.Generic;
using Godot;
using System;

namespace SnakeGame
{
	// Dataluokka
	public class Score
	{
		public string Name { get; set; }
		public int Value { get; set; }
	}

	public class HighScore
	{
		private List<Score> _scores = null;

		private List<Score> LoadScore()
		{
			if (!Godot.FileAccess.FileExists(Config.HighScoreFile))
			{
				// Highscoretiedostoa ei ole kirjoitettu.
				return new List<Score>();
			}

			using (Godot.FileAccess file = Godot.FileAccess.Open(Config.HighScoreFile, Godot.FileAccess.ModeFlags.Read))
			{
				List<Score> scores = new List<Score>();

				while (!file.EofReached())
				{
					// Lue uusi rivi tiedostosta, kunnes tiedoston loppu saavutetaan.
					string line = file.GetLine();
					string[] parts = line.Split(',');

					if (parts.Length == 2)
					{
						// Tiedosto on (oletettavasti) oikeassa formaatissa.
						if (int.TryParse(parts[0], out int score))
						{
							// Pisteiden tulkinta onnistui
							scores.Add(new Score()
							{
								Value = score,
								Name = parts[1]
							});
						}
						else
						{
							GD.PrintErr("Can't read score");
						}
					}
					else if (!string.IsNullOrEmpty(line))
					{
						GD.PrintErr("Tiedoston formaatti on virheellinen");
					}
				}
				return scores;
			}
		}

		public List<Score> GetScores()
		{
			if (_scores == null)
			{
				_scores = LoadScore();
			}

			return _scores;
		}

		public bool AddScore(string name, int score)
		{
			if (_scores == null)
			{
				_scores = LoadScore();
			}

			_scores.Add(new Score()
			{
				Name = name,
				Value = score
			});

			_scores.Sort((a, b) => b.Value.CompareTo(a.Value));

			while (_scores.Count > 10)
			{
				// Poista pienin pistemäärä kunnes listalla on max. 10 arvoa.
				_scores.RemoveAt(_scores.Count - 1);
			}

			try
			{
				using (Godot.FileAccess file = Godot.FileAccess.Open(Config.HighScoreFile, FileAccess.ModeFlags.Write))
				{
					foreach (Score scoreObject in _scores)
					{
						file.StoreLine($"{scoreObject.Value},{scoreObject.Name}");
					}
				}

				return true;
			}
			catch (Exception ex)
			{
				// Gotta catch 'em all
				GD.PrintErr(ex);
				return false;
			}
		}
	}
}