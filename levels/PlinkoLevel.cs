using Godot;
using System;
using System.Collections.Generic;

public partial class PlinkoLevel : Node2D
{
	public int Score = 0;

	[Export] public Label ScoreValue;
	[Export] public Label RespawnMessage;
	[Export] public Label EncounterLabel;
	[Export] public PackedScene PlayerScene;
	[Export] public Node2D PlayerDisks;
	[Export] public Camera2D GameCamera;
	[Export] public Player FirstPlayer;
	[Export] public Node2D EncounterSpawns;
	[Export] public AudioStreamPlayer CatchSound;
	[Export] public HBoxContainer TeamPanel;   // UI container for team display
	[Export] public Label TeamTitle;           // Label above team panel

	private List<string> team = new List<string>();

	private AudioStream normalCatch;
	private AudioStream mewtwoCatch;

	private bool EnableRespawn = false;
	private bool InEncounter = false;
	private Vector2 InitialPosition;

	public override void _Ready()
	{
		RespawnMessage.Hide();
		InitialPosition = FirstPlayer.Position;

		normalCatch = GD.Load<AudioStream>("res://sounds/catch.mp3");
		mewtwoCatch = GD.Load<AudioStream>("res://sounds/catchmewtwo.mp3");
		
		team.Clear();
		RenderTeam();

		EnableRespawn = false;
		InEncounter = false;
	}

	public override void _Process(double delta)
	{
		ScoreValue.Text = $"Score: {Score}";
		RespawnMessage.Visible = EnableRespawn;

		if (Input.IsActionJustPressed("drop_disk") && EnableRespawn && !InEncounter)
		{
			SpawnNewPlayer();
			EnableRespawn = false;
		}
	}

	private void SpawnNewPlayer()
	{
		Player newPlayer = PlayerScene.Instantiate<Player>();
		newPlayer.Position = InitialPosition;
		newPlayer.GameCamera = GameCamera;
		newPlayer.AddToGroup("pokeball");   // ensure ball is in group
		PlayerDisks.AddChild(newPlayer);
	}

	private Node2D SpawnPokemon(string scenePath, Vector2 position)
	{
		var ps = GD.Load<PackedScene>(scenePath);
		var node = ps.Instantiate<Node2D>();
		node.Position = position;
		AddChild(node);
		return node;
	}

	public void IncreaseScore(int scoreIncrease)
	{
		Score += scoreIncrease;
	}

	public void TriggerEncounter(int tier)
	{
		if (InEncounter)
			return;

		InEncounter = true;

		string[] weakPaths = {
			"res://prefabs/Pokemon/Bidoof.tscn",
            "res://prefabs/Pokemon/Caterpie.tscn"
		};
		string[] mediumPaths = {
			"res://prefabs/Pokemon/Shuckle.tscn",
            "res://prefabs/Pokemon/Electabuzz.tscn"
		};
		string[] strongPaths = {
			"res://prefabs/Pokemon/Scyther.tscn",
            "res://prefabs/Pokemon/Haunter.tscn"
		};
		string[] strongestPaths = {
            "res://prefabs/Pokemon/Mewtwo.tscn"
		};

		string[] pool = strongestPaths;
		if (tier == 1) pool = weakPaths;
		else if (tier == 2) pool = mediumPaths;
		else if (tier == 3) pool = strongPaths;

		string scenePath = pool[GD.Randi() % pool.Length];
		var spawned = SpawnPokemon(scenePath, new Vector2(InitialPosition.X, InitialPosition.Y + 600));

		string name = System.IO.Path.GetFileNameWithoutExtension(scenePath);
		GD.Print($"You caught {name}!");
		EncounterLabel.Text = $"You caught {name}!";

		if (CatchSound != null)
		{
			CatchSound.Stream = (name == "Mewtwo") ? mewtwoCatch : normalCatch;
			CatchSound.Play();
		}

		if (TryAddToTeam(name))
			RenderTeam();

		var timer = new Timer { WaitTime = 4.0, OneShot = true };
		AddChild(timer);
		timer.Timeout += () =>
		{
			spawned.QueueFree();

			if (TeamIsFull())
{
	EnableRespawn = false;
	RespawnMessage.Text = "Team complete!";
	RespawnMessage.Show();

	// Copy team list into global singleton
	var teamData = GetNode("/root/TeamData") as TeamData;
	if (teamData != null)
	{
		teamData.TeamList.Clear();
		teamData.TeamList.AddRange(team);
	}

	// Optional: clean remaining balls
	foreach (Node child in PlayerDisks.GetChildren())
		child.CallDeferred("QueueFree");

	// Switch to showcase scene
	GetTree().ChangeSceneToFile("res://TeamShowcase.tscn");
}
else
{
	EnableRespawn = true;
}

InEncounter = false;

		};
		timer.Start();
	}

	// --- Team helpers ---
	private bool TeamIsFull() => team.Count >= 6;

	private bool TryAddToTeam(string name)
	{
		if (TeamIsFull()) return false;
		team.Add(name);
		return true;
	}

	private void ClearTeamPanel()
	{
		if (TeamPanel == null) return;
		foreach (Node child in TeamPanel.GetChildren())
			child.QueueFree();
	}

	private void RenderTeam()
	{
		if (TeamTitle != null)
			TeamTitle.Text = $"Your Team ({team.Count}/6)";

		ClearTeamPanel();
		foreach (var mon in team)
		{
			var label = new Label { Text = mon };
			TeamPanel.AddChild(label);
		}
	}
}
