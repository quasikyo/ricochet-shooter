using Godot;
using System;

public partial class Turret : StaticBody2D {

	private Area2D DetectionZone { get; set; }
	private Timer ShootTimer { get; set; }
	private Timer TargetAcquisitionTimer { get; set; }
	private Node2D Target { get; set; }
	private Vector2 TargetPosition { get; set; } = Vector2.Zero;

	private PackedScene RigidBulletScene { get; set; } = GD.Load<PackedScene>("res://projectiles/bullet/rigid_bullet.tscn");

	[Export]
	private float ShootForce { get; set; } = 1000;

	public override void _Ready() {
		DetectionZone = GetNode<Area2D>("DetectionZone");
		DetectionZone.BodyEntered += TrackTarget;
		DetectionZone.BodyExited += CeaseOperation;

		ShootTimer = GetNode<Timer>("ShootTimer");
		ShootTimer.Timeout += ShootAtTarget;
		TargetAcquisitionTimer = GetNode<Timer>("TargetAcquisitionTimer");
		TargetAcquisitionTimer.Timeout += AcquireTargetPosition;
	}

    public override void _Process(double deltaSeconds) {
		if (!TargetPosition.IsZeroApprox()) {
        	LookAt(TargetPosition);
		} else if (Target != null) {
			LookAt(Target.GlobalPosition);
		} else {
			Rotation = 0;
		}
    }

    private void AcquireTargetPosition() {
		TargetPosition = Target.GlobalPosition;
		ShootTimer.Start();
	}

	private void ShootAtTarget() {
		if (TargetPosition.IsZeroApprox()) {
			return;
		}

		Vector2 toTarget = (TargetPosition - GlobalPosition).Normalized();
		RigidBullet bullet = RigidBulletScene.Instantiate<RigidBullet>();
		bullet.Position = Position + (toTarget * 100);
		bullet.RotationDegrees = Mathf.RadToDeg(toTarget.Angle()) - 90;

		bullet.ApplyCentralImpulse(toTarget * ShootForce);
		GetNode("%Projectiles").AddChild(bullet);
		TargetPosition = Vector2.Zero;
	}

    private void TrackTarget(Node2D body) {
		Target = body;
		TargetAcquisitionTimer.Start();
	}

	private void CeaseOperation(Node2D body) {
		Target = null;
		TargetPosition = Vector2.Zero;
		TargetAcquisitionTimer.Stop();
	}

}
