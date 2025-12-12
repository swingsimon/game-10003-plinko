using Godot;
using System;

public partial class Bucket : Area2D
{
	[Export] public int BucketScore = 0;          // optional scoring
	[Export] public int EncounterTier = 1;        // 1 = weak, 2 = medium, 3 = strong, 4 = strongest
	[Export] public PlinkoLevel PlinkoLevelNode;

	private Label scoreLabel;

	public override void _Ready()
	{
		scoreLabel = GetNode<Label>("%ScoreLabel");
		scoreLabel.Text = $"Tier {EncounterTier}";
		BodyEntered += Bucket_BodyEntered;
	}

	private void Bucket_BodyEntered(Node2D body)
	{
		GD.Print($"Bucket_BodyEntered fired with {body.Name}");

		// Only react to Player balls
		if (body.IsInGroup("pokeball"))
		{
			// Prevent double-trigger: remove group so this ball can’t fire again
			body.RemoveFromGroup("pokeball");

			// Trigger encounter in PlinkoLevel
			PlinkoLevelNode.TriggerEncounter(EncounterTier);

			// Free the ball root node
			body.CallDeferred("QueueFree");
			GD.Print("Pokéball landed, freeing Player root");
		}
	}
}
