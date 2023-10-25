extends Camera2D


const DEAD_ZONE = 300


func _input(event: InputEvent) -> void:
	if event is InputEventMouseMotion:
		var _target = event.position - get_viewport().size * 0.5
		if _target.length() < DEAD_ZONE:
			self.position = Vector2.ZERO
		else:
			# _target.normalized() is the direction in which to move
			# _target.length() - DEAD_ZONE is the distance the mouse is outside of the dead zone
			# 0.5 is an arbitrary scalar
			self.position = _target.normalized() * (_target.length() - DEAD_ZONE) * 0.5
