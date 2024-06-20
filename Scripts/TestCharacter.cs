using System;
using Godot;

namespace BottomlessPit.Scripts;

public partial class TestCharacter : CharacterBody2D {
	public const float GroundFriction = 20f;
	public const float AirFriction = 1f;

	[Export]
	public FastNoiseLite _noise;

	[Export]
	public AnimatedSprite2D _animatedSprite;

	public float _time;

	public bool _isInSlipstream;
	public int _slipstreamCounter;

	public Vector2 _gravity = new (0f, 250f); // Low gravity.

	public override void _Ready() {
		var random = new Random();

		_noise.Seed = random.Next();
	}

	public override void _PhysicsProcess(double delta) {
		var inputVector = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		_time += (float)delta;

		if (_isInSlipstream) {
			_MoveInSlipstream(inputVector, delta);
		}
		else {
			_MoveFalling(inputVector, delta);
		}
	}

	public void _MoveFalling(Vector2 inputVector, double delta) {
		var drift = new Vector2(
			_noise.GetNoise2D(_time, 0),
			0f
		);

		var targetVelocity = 25f * drift + 50f * inputVector;
		var velocityChange = new Vector2 {
			X = (targetVelocity.X - Velocity.X) * AirFriction * (float)delta,
			Y = (targetVelocity.Y - Velocity.Y) * AirFriction * (float)delta
		};

		Velocity += velocityChange;

		_animatedSprite.Animation = "Falling";

		MoveAndSlide();
	}

	public void _MoveInSlipstream(Vector2 inputVector, double delta) {
		var isOnFloor = IsOnFloor();
		if (!isOnFloor) {
			Velocity += _gravity * (float)delta;

			MoveAndSlide();

			return;
		}

		var targetVelocity = 75f * inputVector.X;
		var velocityChange = (targetVelocity - Velocity.X) * GroundFriction * (float)delta;

		Velocity = new Vector2(
			Velocity.X + velocityChange,
			0f
		);

		var isZeroX = Mathf.IsEqualApprox(Velocity.X, 0f, 0.01f);
		if (isZeroX) {
			_animatedSprite.Animation = "Stand";
		}
		else {
			_animatedSprite.Animation = "Walk";
			_animatedSprite.FlipH = Velocity.X < 0;
		}

		if (Input.IsActionJustPressed("ui_accept")) {
			Velocity = new Vector2(Velocity.X, -150f);

			_animatedSprite.Animation = "Jump";
		}

		MoveAndSlide();
	}

	public void _CheckSlipstream() {
		_isInSlipstream = _slipstreamCounter > 0;

		GD.Print($"Is in slipstream: {_isInSlipstream}");
	}

	public void EnterSlipstream() {
		_slipstreamCounter++;
		_CheckSlipstream();
	}

	public void ExitSlipstream() {
		_slipstreamCounter--;
		_CheckSlipstream();
	}
}