using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTKFPS.engine.singletons;

namespace OpenTKFPS.engine.structure.actors
{
    public class CameraActor3D : Actor3D
    {
        public float fov;
        public float near;
        public float far;

        public bool isCurrent = false;
        public CameraActor3D(Vector3 position, Vector3 scale, Vector3 rotation, float fov = 60f, float near = 0.1f, float far = 1000f, string name = "Camera3D") : base(position, scale, rotation, name)
        {
            this.fov = fov;
            this.near = near;
            this.far = far;
        }

        public void setCurrent(){
            Renderer.SetCurrentCamera(this);
        }
    }
}