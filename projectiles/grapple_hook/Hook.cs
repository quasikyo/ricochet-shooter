using Godot;
using System;

namespace RicochetShooter;

public partial class Hook : CharacterBody2D {

	public enum HookState {
		Inactive,
		Flying,
		Attached
	}

	public float Speed { get; set; } = 2000f;
	public Vector2 Direction { get; set; } = Vector2.Zero;
	public HookState State { get; set; } = HookState.Inactive;
	public Vector2 AttachedPosition { get; set; } = Vector2.Zero;

	public float WeightMultiplier = 1.0f;

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
		Visible = false;
	}

    public override void _Ready() {
		GetParent<Player>().ShootHook += Shoot;
		GetParent<Player>().ReleaseHook += Release;
		Visible = false;
    }

    public override void _PhysicsProcess(double deltaSeconds) {
		if (!AttachedPosition.IsZeroApprox()) {
			GlobalPosition = AttachedPosition;
		}

		if (State == HookState.Inactive) {
			return;
		}

		KinematicCollision2D collision = MoveAndCollide(Direction * Speed * (float)deltaSeconds);
		if (collision == null) {
			return;
		}

		AttachedPosition = collision.GetPosition();
		Node2D collisionObject = collision.GetCollider() as Node2D;
		if (State != HookState.Attached) {
			State = HookState.Attached;
			GetNode<WeightComponent>("../WeightComponent").SetWeightMultipliers(collisionObject.GetNodeOrNull<WeightComponent>("WeightComponent"));
		}
		collisionObject.Set("Direction", (GetParent<Player>().GlobalPosition - AttachedPosition).Normalized());
	}
}
