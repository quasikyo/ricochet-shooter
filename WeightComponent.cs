using Godot;

public partial class WeightComponent : Node2D {
	[Export]
	public float Weight { get; set; } = 100f;

	public float WeightMultiplier { get; set; } = 1f;

	public void SetWeightMultipliers(WeightComponent other) {
		other ??= new WeightComponent {
			Weight = float.MaxValue
		};

		float weightSum = Weight + other.Weight;
		WeightMultiplier = other.Weight / weightSum;
		other.WeightMultiplier = Weight / weightSum;
	}
}
