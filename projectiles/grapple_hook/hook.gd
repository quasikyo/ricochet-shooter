extends CharacterBody2D
class_name Hook


var speed = 50
enum HookState { Inactive, Flying, Attached }
var hook_state := HookState.Inactive
var direction := Vector2.ZERO
var attached_position := Vector2.ZERO


func _ready():
	get_parent().shoot_hook.connect(shoot)
	get_parent().release_hook.connect(release)


func shoot(shoot_direction: Vector2) -> void:
	position = Vector2.ZERO
	direction = shoot_direction.normalized()
	hook_state = HookState.Flying


func release() -> void:
	hook_state = HookState.Inactive
	attached_position = Vector2.ZERO
	position = Vector2.ZERO
	return


func _physics_process(_deltaSeconds: float) -> void:
	if attached_position:
		global_position = attached_position
	if hook_state != HookState.Flying:
		return

	var collision := move_and_collide(direction * speed)
	if !collision:
		return
	attached_position = collision.get_position()

	hook_state = HookState.Attached
