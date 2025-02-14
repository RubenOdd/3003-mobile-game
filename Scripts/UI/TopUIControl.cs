using Godot;

namespace SnakeGame.UI
{
	public partial class TopUIControl : Node
	{
		[Export] private Label _scoreLabel = null;
		[Export] private BaseButton _restartButton = null;

		public override void _Ready()
		{
			// Ala kuuntelemaan napin Pressed eventiä. Event laukeaa, kun
			// nappia painetaan.
			if (_restartButton != null)
			{
				// Väitetään suoritettava metodi eventille. HUOM! Metodia
				// ei suoriteta tässä kohdassa. Ainoastaan viittaus metodiin välitetään
				// eventille.
				_restartButton.Pressed += OnRestartPressed;
			}
		}

		private void OnRestartPressed()
		{
			Level.Current.ResetGame();
		}

		public void SetScore(int score)
		{
			_scoreLabel.Text = $"Pisteet: {score}";
		}
	}
}