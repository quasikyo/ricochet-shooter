extends CharacterBody2D

const SPEED := 300.0
const JUMP_VELOCITY := -400.0

var bullet_scene := preload('res://bullet.tscn') as PackedScene
var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")

var grapple_speed := 300
var is_grappling := false
var step: float = 0
var g1: Vector2
var g2: Vector2
var g3: Vector2


func _process(_deltaSeconds: float) -> void:
	if position.y > 1000:
		position = Vector2(480, 360)

	if Input.is_action_pressed('primary_action'):
		var bullet := bullet_scene.instantiate() as Area2D
		var shooting_direction := (get_global_mouse_position() - position).normalized()
		bullet.position = self.global_position
		bullet.direction = shooting_direction
		bullet.rotation_degrees = rad_to_deg(shooting_direction.angle()) - 90
		%Projectiles.add_child(bullet)

	if Input.is_action_just_pressed('secondary_action'):
		var queryParameters := PhysicsPointQueryParameters2D.new()
		queryParameters.collide_with_areas = true
		queryParameters.collide_with_bodies = false
		queryParameters.position = get_global_mouse_position()

		var anchors = get_world_2d().direct_space_state.intersect_point(queryParameters, 1)
		if anchors.size() == 0:
			return

		velocity.y = 0
		step = 0
		is_grappling = true
		var anchor := anchors[0].collider as Area2D

		anchor.look_at(global_position)
		var distance_from_anchor := position.distance_to(anchor.position)

		g1 = position

		var x2 := anchor.position.x
		var y2 := anchor.position.y + distance_from_anchor
		g2 = Vector2(x2, y2)

		var x3 := anchor.position.x + (anchor.position.x - position.x)
		g3 = Vector2(x3, position.y)
	elif Input.is_action_just_released('secondary_action'):
		is_grappling = false


func _physics_process(deltaSeconds: float) -> void:
	if not is_on_floor() and not is_grappling:
		velocity.y += gravity * deltaSeconds

	if Input.is_action_just_pressed('jump') and is_on_floor():
		velocity.y = JUMP_VELOCITY

	if is_grappling and step <= 1:
		position = quadratic_bezier(g1, g2, g3, step)
		step += 1 * deltaSeconds

	var direction := Input.get_axis("left", "right")
	if !is_grappling && direction:
	# if direction:
		velocity.x = direction * SPEED
	else:
		# deceleration
		velocity.x = move_toward(velocity.x, 0, SPEED)
	move_and_slide()


func quadratic_bezier(p0: Vector2, p1: Vector2, p2: Vector2, t: float) -> Vector2:
	var q0 = p0.lerp(p1, t)
	var q1 = p1.lerp(p2, t)
	var r = q0.lerp(q1, t)
	return r

