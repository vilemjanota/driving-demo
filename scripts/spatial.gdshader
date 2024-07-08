shader_type spatial;

// Define uniforms for start and end colors, and the speed of change
uniform vec3 start_color;
uniform vec3 end_color;
uniform float speed = 2.0;

void fragment() {
    // Calculate a time factor that oscillates between 0 and 1
    float time_factor = sin(TIME * speed * 0.5) * 0.5 + 0.5;
    
    // Interpolate between start_color and end_color based on time_factor
    vec3 current_color = mix(start_color, end_color, time_factor);
    
    // Apply the interpolated color as the albedo
    ALBEDO = current_color;
}