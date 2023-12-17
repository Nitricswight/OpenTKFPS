using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ImGuiNET;
using OpenTK.Mathematics;

namespace OpenTKFPS.engine.structure.actors
{
    public class Actor3D : Actor
    {
        public Vector3 local_position;

        public Vector3 local_scale;

        public Vector3 local_rotation;

        public Matrix4 global_matrix {get; private set;}

        public Vector3 forward;

        public Vector3 right;

        public Vector3 up;

        public Actor3D(Vector3 position, Vector3 scale, Vector3 rotation, string name = "Actor3D") : base(name){
            local_position = position;
            local_scale = scale;
            local_rotation = rotation;
        }
        public override void Update(float deltaTime)
        {
            //Debug.WriteLine("updating node 3d");
            Matrix4 trans = Matrix4.CreateTranslation(local_position);
            Matrix4 rot = Matrix4.CreateRotationX(local_rotation.X) * Matrix4.CreateRotationY(local_rotation.Y) * Matrix4.CreateRotationZ(local_rotation.Z);
            Matrix4 sca = Matrix4.CreateScale(local_scale);
            Matrix4 local_matrix = sca * rot * trans;
            //Debug.WriteLine(global_matrix.ToString());

            if(parent is Actor3D){
                //Debug.WriteLine("child matrix");
                global_matrix = local_matrix * ((Actor3D)parent).global_matrix;
            }
            else{
                global_matrix = local_matrix;
            }

            forward.X =  MathF.Cos(local_rotation.X) * MathF.Sin(local_rotation.Y);
            forward.Y = -MathF.Sin(local_rotation.X);
            forward.Z =  MathF.Cos(local_rotation.X) * MathF.Cos(local_rotation.Y);

            right.X =  MathF.Cos(local_rotation.Y);
            right.Y =  0;
            right.Z = -MathF.Sin(local_rotation.Y);

            up = Vector3.Cross(forward, right);
            base.Update(deltaTime);
        }

        public override void ExposeToInspector()
        {
            base.ExposeToInspector();
            System.Numerics.Vector3 editPosition = new System.Numerics.Vector3(local_position.X, local_position.Y, local_position.Z);

            ImGui.DragFloat3("position", ref editPosition, 0.1f);

            local_position = new Vector3(editPosition.X, editPosition.Y, editPosition.Z);

            System.Numerics.Vector3 editScale = new System.Numerics.Vector3(local_scale.X, local_scale.Y, local_scale.Z);

            ImGui.DragFloat3("scale", ref editScale, 0.1f);

            local_scale = new Vector3(editScale.X, editScale.Y, editScale.Z);


            System.Numerics.Vector3 editRotation = new System.Numerics.Vector3(MathHelper.RadiansToDegrees(local_rotation.X), MathHelper.RadiansToDegrees(local_rotation.Y), MathHelper.RadiansToDegrees(local_rotation.Z));

            ImGui.DragFloat3("rotation", ref editRotation,1f);

            local_rotation = new Vector3(MathHelper.DegreesToRadians(editRotation.X), MathHelper.DegreesToRadians(editRotation.Y), MathHelper.DegreesToRadians(editRotation.Z));

            ImGui.Text("forward vector: " + forward.ToString());
            ImGui.Text("right vector: " + right.ToString());
            ImGui.Text("up vector: " + up.ToString());

        }
    }
}