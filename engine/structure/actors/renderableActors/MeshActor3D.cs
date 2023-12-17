using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTKFPS.engine.graphics;
using OpenTKFPS.engine.singletons;

namespace OpenTKFPS.engine.structure.actors.renderableActors
{
    public class MeshActor3D : Actor3D
    {
        public Mesh mesh;
        public Material material;

        public MeshActor3D(Mesh mesh, Material mat, Vector3 position, Vector3 rotation, Vector3 scale, string name = "MeshActor3D") : base(position, scale, rotation, name){
            this.mesh = mesh;
            this.material = mat;
        }

        public override void Render(float deltaTime)
        {
            Renderer.DrawMeshActor3D(this);

            base.Render(deltaTime);
        }

        public override void ExposeToInspector()
        {
            base.ExposeToInspector();
            ImGui.Text("draw type: " + mesh.drawType.ToString());
            ImGui.Text("draw count: " + mesh.drawCount.ToString());
            ImGui.Text("enabled vertex attributes: " + (mesh.vertexAttributes + 1).ToString());

            material.ExposeToInspector();
        }
    }
}