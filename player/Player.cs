using Godot;
using System;

namespace RicochetShooter;

public partial class Player : CharacterBody2D {

	public Hook hook;
	public float HookForce { get; set; } = 100f;

	[Signal]
	public delegate void ShootHookEventHandler(Vector2 direction);
	[Signal]
	public delegate void ReleaseHookEventHandler();

	public float Speed { get; set; }= 300.0f;
	public float JumpForce { get; set; } = -400.0f;
	public float MoveSpeed { get; set; } = 300;

	private PackedScene BulletScene { get; set; } = GD.Load<PackedScene>("res://projectiles/bullet/bullet.tscn");

	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _Ready() {
		hook = GetNode<Hook>("Hook");
    }

    public override void _Input(InputEvent @event) {
        if (@event.IsActionPressed("primary_action")) {
			Bullet bullet = BulletScene.Instantiate<Bullet>();
			bullet.Position = Position;
			bullet.Direction = (GetGlobalMousePosition() - Position).Normalized();
			bullet.RotationDegrees = Mathf.RadToDeg(bullet.Direction.Angle()) - 90;
			GetNode("%Projectiles").AddChild(bullet);
		}

		if (@event.IsActionPressed("secondary_action")) {
			EmitSignal(SignalName.ShootHook, GetGlobalMousePosition() - Position);
		} else if (@event.IsActionReleased("secondary_action")) {
			EmitSignal(SignalName.ReleaseHook);
		}

		if (@event.IsActionPressed("special_action")) {
			foreach (Bullet bullet in GetNode<Area2D>("ParryZone").GetOverlappingAreas()) {
				bullet.Direction = (GetGlobalMousePosition() - Position).Normalized();
				bullet.RotationDegrees = Mathf.RadToDeg(bullet.Direction.Angle()) - 90;
			}
		}
    }

    public override void _PhysicsProcess(double deltaSeconds) {
		Vector2 velocity = Velocity;
		velocity.X = Input.GetAxis("left", "right") * MoveSpeed;

		bool isGrounded = IsOnFloor();
		if (!isGrounded) {
			velocity.Y += gravity * (float)deltaSeconds;
		} else if (Input.IsActionJustPressed("jump")) {
			velocity.Y = JumpForce;
		}

		if (hook.State == Hook.HookState.Attached) {
			Vector2 pullDirection = (hook.AttachedPosition - Position).Normalized();
			// Vector2 pullVelocity = hook.Direction.Normalized() * HookForce;
			Vector2 pullVelocity = pullDirection * HookForce;
			velocity += pullVelocity;
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}