extends Area2D


@export var speed := 100
var direction := Vector2.ZERO

@onready var lifetime := $Lifetime as Timer


func _ready():
	lifetime.timeout.connect(_on_lifetime_timeout)


func _process(deltaSeconds: float) -> void:
	position += direction * speed * deltaSeconds


func _on_lifetime_timeout():
	print('ree')
	# queue_free()
