using Godot;

namespace RicochetShooter;

public partial class Bullet : Area2D {

	[Export]
	public float Speed { get; set; } = 100f;

	public Vector2 Direction { get; set; } = Vector2.Zero;

	public Timer Lifetime { get; set; }

    public override void _Ready() {
		Lifetime = GetNode<Timer>("Lifetime");
		Lifetime.Timeout += OnLifetimeTimeout;
    }

    public override void _PhysicsProcess(double deltaSeconds) {
        Position += Direction * Speed * (float)deltaSeconds;
    }

    private void OnLifetimeTimeout() {
		GD.Print("ree");
		// QueueFree();
	}

}
