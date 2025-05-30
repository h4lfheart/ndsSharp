using OpenTK.Mathematics;

namespace ndsSharp.Viewer.Shared.Rendering.Rendering;

public class Camera
{
    public Vector3 Position;
    public Vector3 Direction;
    public Vector3 Up = Vector3.UnitY;

    public float Yaw = -90.0f;
    public float Pitch;
    public float FOV = 60f;
    public float Speed = 0.05f;
    public float Sensitivity = 0.5f;
    public float Near = 0.1f;
    public float Far = 25000f;
    public float AspectRatio = 16f / 9f;

    public Camera()
    {
        Position = new Vector3(0.741224229f, 1.47437632f, 1.39653087f);
        Direction = new Vector3(-0.430533648f, -0.398749083f, -0.809715986f);
    }

    public void CalculateDirection(float x, float y)
    {
        Yaw += x;
        Pitch -= y;

        UpdateDirection();
    }
    
    public void UpdateDirection()
    {
        Pitch = Math.Clamp(Pitch, -89f, 89f);

        var direction = Vector3.Zero;
        var yaw = MathHelper.DegreesToRadians(Yaw);
        var pitch = MathHelper.DegreesToRadians(Pitch);
        direction.X = MathF.Cos(yaw) * MathF.Cos(pitch);
        direction.Y = MathF.Sin(pitch);
        direction.Z = MathF.Sin(yaw) * MathF.Cos(pitch);
        Direction = Vector3.Normalize(direction);
    }


    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Position, Position + Direction, Up);
    }

    public Matrix4 GetProjectionMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOV), AspectRatio, Near, Far);
    }
}