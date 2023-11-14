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

	public float HookForce { get; set; } = 2500f;
	public float Speed { get; set; } = 2000f;
	public Vector2 Direction { get; set; } = Vector2.Zero;
	public HookState State { get; set; } = HookState.Inactive;
	public Vector2 AttachedPosition { get; set; } = Vector2.Zero;
	public PhysicsBody2D AttachedObject { get; set; } = null;

	public override void _Ready() {
		Player = GetParent<RigidPlayer>();
		Player.ShootHook += Shoot;
		Player.ReleaseHook += Release;
		Visible = false;
    }

    public override void _PhysicsProcess(double deltaSeconds) {
		if (!AttachedPosition.IsZeroApprox()) {
			GlobalPosition = AttachedPosition;

			Vector2 fromPlayerToAttached = (GlobalPosition - Player.GlobalPosition).Normalized();
			float massSum = Player.Mass;
			float otherMass = massSum;
			if (AttachedObject is RigidBody2D) {
				RigidBody2D rigidbodyReference = AttachedObject as RigidBody2D;
				otherMass = rigidbodyReference.Mass;
				massSum += otherMass;

				rigidbodyReference.ApplyForce(fromPlayerToAttached * -1 * HookForce * (Player.Mass / massSum));
			}
			Player.ApplyForce(fromPlayerToAttached * HookForce * (otherMass / massSum));
		}

		if (State == HookState.Inactive) {
			return;
		}

		KinematicCollision2D collision = MoveAndCollide(Direction * Speed * (float)deltaSeconds);
		if (collision == null) {
			return;
		}

		AttachedPosition = collision.GetPosition();
		AttachedObject = collision.GetCollider() as PhysicsBody2D;
		if (State != HookState.Attached) {
			State = HookState.Attached;
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
		AttachedPosition = Vector2.Zero;
		AttachedObject = null;
		Visible = false;
	}

}
