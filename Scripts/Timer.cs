using Godot;

namespace SnakeGame
{
	public partial class Timer : Node
	{
		// Ajastimen aloitusaika.
		[Export] private double _time = 1;
		// Ajastimen aika tällä hetkellä.
		private double _timer = 0;
		// Onko ajastin käynnissä vai ei
		private bool _isRunning = false;
		private bool _isComplete = false;
		public bool IsRunning
		{
			get { return _isRunning; }
			private set { _isRunning = value; }
		}
		public bool IsComplete
		{
			get { return _isComplete; }
			private set { _isComplete = value; }
		}
		public override void _Process(double delta)
		{
			if (IsRunning && !IsComplete)
			{
				_timer -= delta;
				if (_timer <= 0)
				{
					// Ajastin päättyi
					_timer = 0;
					IsComplete = true;
					Stop();
					// GD.Print("Ajastin kului loppuun.");
				}
			}
		}
		public void SetTime(double time)
		{
			_time = time;
			_timer = time;
		}
		// Start
		public void Start()
		{
			IsRunning = true;
		}
		// Stop
		public void Stop()
		{
			IsRunning = false;
		}
		// Reset
		public void Reset(bool autoStart)
		{
			IsComplete = false;
			SetTime(_time);
			if (autoStart)
			{
				// Autostartin ollessa true, käynnistä ajastin heti.
				Start();
			}
			else
			{
				// Suorita varmuudeksi Stop, jos ajastin resetoidaan ennen, kuin se on kulunut loppuun.
				Stop();
			}
		}
	}
}
