using Godot;

namespace RicochetShooter;

public partial class Bullet : CharacterBody2D {

	private float _speed = 100f;
	[Export]
	public float Speed {
		get => _speed;
		set {
			_speed = value;
			Velocity = Direction * _speed;
		}
	}

	private Vector2 _direction = Vector2.Zero;
	public Vector2 Direction {
		get => _direction;
		set {
			_direction = value.IsNormalized() ? value : value.Normalized();
			Velocity = _direction * Speed;
			RotationDegrees = Mathf.RadToDeg(_direction.Angle()) - 90;
		}
	}

	private WeightComponent weightComponent;

    public override void _Ready() {
		GetNode<Timer>("Lifetime").Timeout += OnLifetimeTimeout;
		weightComponent = GetNode<WeightComponent>("WeightComponent");
		Velocity = Direction * Speed;
    }

    public override void _PhysicsProcess(double deltaSeconds) {
        // Position += Direction * Speed * (float)deltaSeconds;
		KinematicCollision2D collision = MoveAndCollide(Velocity * (float)deltaSeconds * weightComponent.WeightMultiplier);
		if (collision == null) {
			return;
		}

		Velocity = Velocity.Bounce(collision.GetNormal());
		Direction = Velocity.Normalized();
    }

    private void OnLifetimeTimeout() {
		// QueueFree();
	}

}
