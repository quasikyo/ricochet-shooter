using Godot;
using System;

namespace RicochetShooter;

public partial class Hook : CharacterBody2D {

	public enum HookState {
		Inactive,
		Flying,
		Attached
	}

	public float Speed { get; set; } = 50f;
	public Vector2 Direction { get; set; } = Vector2.Zero;
	public HookState State { get; set; } = HookState.Inactive;
	public Vector2 AttachedPosition { get; set; } = Vector2.Zero;


	private void Shoot(Vector2 direction) {
		Position = Vector2.Zero;
		Direction = direction.Normalized();
		State = HookState.Flying;
	}

	private void Release() {
		State = HookState.Inactive;
		Position = Vector2.Zero;
		AttachedPosition = Vector2.Zero;
	}

    public override void _Ready() {
		GetParent<Player>().ShootHook += Shoot;
		GetParent<Player>().ReleaseHook += Release;
    }

    public override void _PhysicsProcess(double delta) {
		if (!AttachedPosition.IsZeroApprox()) {
			GlobalPosition = AttachedPosition;
		}

		if (State != HookState.Flying) {
			return;
		}

		KinematicCollision2D collision = MoveAndCollide(Direction * Speed);
		if (collision == null) {
			return;
		}
		AttachedPosition = collision.GetPosition();
		State = HookState.Attached;
	}
}
