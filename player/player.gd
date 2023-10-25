extends CharacterBody2D


signal shoot_hook(direction: Vector2)
signal release_hook

@onready var hook := $Hook as Hook
var gravity = ProjectSettings.get_setting('physics/2d/default_gravity')
var moveSpeed := 300
var jumpForce := -300
var hook_force = 100

var bullet_scene := preload('res://projectiles/bullet/bullet.tscn') as PackedScene


func _input(event: InputEvent) -> void:
	if event.is_action_pressed('primary_action'):
		var bullet := bullet_scene.instantiate() as Bullet
		bullet.position = position
		bullet.direction = (get_global_mouse_position() - position).normalized()
		bullet.rotation_degrees = rad_to_deg(bullet.direction.angle()) - 90
		%Projectiles.add_child(bullet)

	if event.is_action_pressed('secondary_action'):
		shoot_hook.emit(get_global_mouse_position() - position)
	elif event.is_action_released('secondary_action'):
		release_hook.emit()

	if event.is_action_pressed('special_action'):
		for bullet in ($ParryZone as Area2D).get_overlapping_areas():
			bullet.direction = (get_global_mouse_position() - position).normalized()
			bullet.rotation_degrees = rad_to_deg(bullet.direction.angle()) - 90


func _physics_process(deltaSeconds: float) -> void:
	velocity.x = Input.get_axis('left', 'right') * moveSpeed

	var is_grounded = is_on_floor()
	if !is_grounded:
		velocity.y += gravity * deltaSeconds
	elif Input.is_action_just_pressed('jump'):
		velocity.y = jumpForce

	if hook.hook_state == Hook.HookState.Attached:
		var pull_direction: Vector2 = (hook.attached_position - position).normalized()
		# var pull_velocity: Vector2 = hook.direction.normalized() * hook_force
		var pull_velocity: Vector2 = pull_direction * hook_force
		velocity += pull_velocity

	move_and_slide()
