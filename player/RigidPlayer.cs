using Godot;
using System.Collections.Generic;
using System.Linq;

namespace RicochetShooter;

public partial class RigidPlayer : RigidBody2D {

	public Hook hook;

	[Signal]
	public delegate void ShootHookEventHandler(Vector2 direction);
	[Signal]
	public delegate void ReleaseHookEventHandler();

	[Export]
	public float Speed { get; set; }= 300.0f;
	[Export]
	public float JumpForce { get; set; } = 400.0f;
	[Export]
	public float MoveSpeed { get; set; } = 1000f;
	[Export]
	public float ParryForce { get; set; } = 25f;

	[Export]
	public float HorizontalInput { get; set; } = 0f;
	public Vector2 horizontalForce = Vector2.Zero;

	private PackedScene RigidBulletScene { get; set; } = GD.Load<PackedScene>("res://projectiles/bullet/rigid_bullet.tscn");

	public override void _Ready() {
		hook = GetNode<Hook>("Hook");
    }

    public override void _Input(InputEvent @event) {
		Vector2 toMouse = (GetGlobalMousePosition() - Position).Normalized();

		if (@event.IsActionPressed("jump")) {
			ApplyCentralImpulse(Vector2.Up * JumpForce);
		}

		if (@event.IsActionPressed("primary_action")) {
			RigidBullet bullet = RigidBulletScene.Instantiate<RigidBullet>();
			bullet.ApplyCentralImpulse(toMouse * 1500);
			bullet.Position = Position + (toMouse * 25);
			bullet.RotationDegrees = Mathf.RadToDeg(toMouse.Angle()) - 90;
			GetNode("%Projectiles").AddChild(bullet);
		}

		if (@event.IsActionPressed("secondary_action")) {
			EmitSignal(SignalName.ShootHook, GetGlobalMousePosition() - Position);
		} else if (@event.IsActionReleased("secondary_action")) {
			EmitSignal(SignalName.ReleaseHook);
		}

		if (@event.IsActionPressed("special_action")) {
			IEnumerable<RigidBody2D> parriedProjectiles = GetNode<Area2D>("ParryZone").GetOverlappingBodies().Cast<RigidBody2D>();
			foreach (RigidBody2D projectile in parriedProjectiles) {
				projectile.ApplyCentralImpulse(toMouse * ParryForce);
				// projectile.RotationDegrees = Mathf.RadToDeg(toMouse.Angle()) - 90;
			}
		}
    }

    public override void _PhysicsProcess(double deltaSeconds) {
		HorizontalInput = Input.GetAxis("left", "right");
		horizontalForce.X = HorizontalInput * MoveSpeed;
		ApplyCentralForce(horizontalForce);
    }

    // public override void _IntegrateForces(PhysicsDirectBodyState2D state) {}

}
