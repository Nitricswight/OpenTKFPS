using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTKFPS.engine.singletons;
using OpenTKFPS.engine.structure.actors;

namespace OpenTKFPS.game.customActors
{
    public class FlyCam : CameraActor3D
    {
        const float SPEED = 1f;
        const float MOUSE_SENSITIVITY = 0.001f;

        float PITCH_CLAMP = MathHelper.DegreesToRadians(89);
        public FlyCam(Vector3 position, Vector3 scale, Vector3 rotation, float fov = 45f, float near = 0.1F, float far = 1000, string name = "FlyCam") : base(position, scale, rotation, fov, near, far, name)
        {
        }

        public override void Update(float deltaTime)
        {
            Vector3 m;

            m.X = (InputManager.IsKeyPressed(Keys.A)? 1f:0f) - (InputManager.IsKeyPressed(Keys.D)? 1f:0f);
            m.Y = (InputManager.IsKeyPressed(Keys.W)? 1f:0f) - (InputManager.IsKeyPressed(Keys.S)? 1f:0f);
            m.Z = (InputManager.IsKeyPressed(Keys.E)? 1f:0f) - (InputManager.IsKeyPressed(Keys.Q)? 1f:0f);

            if(m.Length > 0){
                m = m.Normalized() * SPEED * deltaTime;
            }
            

            

            if(InputManager.IsMouseButtonPressed(MouseButton.Right)){
                local_rotation.Y += InputManager.GetMouseDelta().X * MOUSE_SENSITIVITY;
                local_rotation.X = MathHelper.Clamp(local_rotation.X - InputManager.GetMouseDelta().Y * MOUSE_SENSITIVITY, -PITCH_CLAMP, PITCH_CLAMP);
                local_position += m.X * right;
                //Debug.WriteLine(right);
                local_position += m.Y * forward;
                //Debug.WriteLine(forward);
                local_position += m.Z * up;
                //Debug.WriteLine(m);
            }

            base.Update(deltaTime);

            
        }
    }
}