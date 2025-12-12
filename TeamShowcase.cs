using Godot;
using System;

public partial class TeamShowcase : Control
{
	[Export] public GridContainer TeamContainer;
	[Export] public Button PlayAgainButton;

	public override void _Ready()
	{
		// Get team from the autoload singleton
		var teamData = GetNode("/root/TeamData") as TeamData;
		if (teamData == null || TeamContainer == null) return;

		// Populate sprites
		foreach (string mon in teamData.TeamList)
		{
			string iconPath = $"res://prefabs/Pokemon/{mon}.png";
			Texture2D tex = GD.Load<Texture2D>(iconPath);

			var rect = new TextureRect
			{
				Texture = tex,
				StretchMode = TextureRect.StretchModeEnum.KeepAspect,
				CustomMinimumSize = new Vector2(128, 128)
			};

			TeamContainer.AddChild(rect);
		}

		if (PlayAgainButton != null)
		{
			PlayAgainButton.Pressed += () =>
			{
				GD.Print("Play Again button pressed!");  // debug
				var td = GetNode("/root/TeamData") as TeamData;
				td.TeamList.Clear();
				GetTree().ChangeSceneToFile("res://levels/plinko_level.tscn");
			};
		}
		else
		{
			GD.PrintErr("PlayAgainButton export not assigned!");
		}
	}
}
