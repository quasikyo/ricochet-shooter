using Godot;
using System;

namespace RicochetShooter;

public partial class Hook : CharacterBody2D {

	public enum HookState {
		Inactive,
		Flying,
		Attached
	}

	public RigidPlayer Player { get; set; }

	[Export]
	public float HookForce { get; set; } = 2500f;
	[Export]
	public float Speed { get; set; } = 2000f;
	public Vector2 Direction { get; set; } = Vector2.Zero;
	public HookState State { get; set; } = HookState.Inactive;
	public Marker2D AttachPoint { get; set; }
	public Node2D AttachedObject { get; set; }

	public override void _Ready() {
		Player = GetParent<RigidPlayer>();
		Player.ShootHook += Shoot;
		Player.ReleaseHook += Release;
		Visible = false;
    }

    public override void _PhysicsProcess(double deltaSeconds) {
		if (State == HookState.Inactive) {
			return;
		}

		if (State == HookState.Attached && AttachPoint != null) {
			GlobalPosition = AttachPoint.GlobalPosition;

			Vector2 fromPlayerToAttached = (GlobalPosition - Player.GlobalPosition).Normalized();
			float massSum = Player.Mass;
			float otherMass = massSum;
			if (AttachedObject is RigidBody2D) {
				RigidBody2D rigidbodyReference = AttachedObject as RigidBody2D;
				otherMass = rigidbodyReference.Mass;
				massSum += otherMass;

				float forceTowardPlayer = HookForce * (Player.Mass / massSum);
				rigidbodyReference.ApplyForce(fromPlayerToAttached * -1 * forceTowardPlayer);
			}
			float forceTowardAttached = HookForce * (otherMass / massSum);
			Player.ApplyCentralForce(fromPlayerToAttached * forceTowardAttached);
		}

		if (State == HookState.Flying) {
			KinematicCollision2D collision = MoveAndCollide(Direction * Speed * (float)deltaSeconds);
			if (collision == null) {
				return;
			}
			State = HookState.Attached;

			AttachedObject = collision.GetCollider() as Node2D;
			AttachPoint = new Marker2D();
			AttachedObject.AddChild(AttachPoint);
			AttachPoint.GlobalPosition = collision.GetPosition();

			SetCollisions(false);
		}
	}

	private void Shoot(Vector2 direction) {
		Position = Vector2.Zero;
		Direction = direction.Normalized();
		State = HookState.Flying;
		Visible = true;
	}

	private void Release() {
		State = HookState.Inactive;
		Position = Vector2.Zero;
		// AttachedPosition = Vector2.Zero;
		AttachPoint?.QueueFree();
		AttachPoint = null;
		AttachedObject = null;
		Visible = false;

		SetCollisions(true);
	}

	private void SetCollisions(bool isEnabled) {
		SetCollisionLayerValue(3, isEnabled);
		SetCollisionMaskValue(2, isEnabled);
		SetCollisionMaskValue(4, isEnabled);
		SetCollisionMaskValue(6, isEnabled);
	}

}
