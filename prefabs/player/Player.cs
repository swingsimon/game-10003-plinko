using Godot;
using System;

public partial class Player : RigidBody2D
{
	[Export] public Camera2D GameCamera;
	public bool Dead = false;

	public override void _Ready()
	{
		Freeze = true;
	}

	public override void _Process(double delta)
	{
		if (Freeze)
		{
			Vector2 newPosition = Position;
			newPosition.X = GetViewport().GetMousePosition().X;
			Position = newPosition;

			if (Input.IsActionJustPressed("drop_disk"))
				Freeze = false;
		}

		//if (!Dead)
		//{
		//	Vector2 newCamPosition = GameCamera.Position;
		//	newCamPosition.Y = Position.Y;
		//	GameCamera.Position = newCamPosition;
		//}
	}
}
 
