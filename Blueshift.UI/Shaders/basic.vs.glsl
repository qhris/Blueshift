#version 330 core
layout (location = 0) in vec3 aPosition;

out vec2 TexCoord0;

void main()
{
    // Map vertex position to texture coordinate [-1,1] => [0,1]
    TexCoord0 = vec2(aPosition.x, -aPosition.y) * 0.5 + vec2(0.5);
    gl_Position = vec4(aPosition, 1.0);
}
