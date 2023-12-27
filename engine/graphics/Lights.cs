using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTKFPS.engine.singletons;
using OpenTKFPS.engine.structure.actors;

namespace OpenTKFPS.engine.graphics
{
    public class Light : Actor3D
    {
        public Color4 colour;
        public float intensity;

        public Light(Color4 colour, float intensity, Vector3 position, Vector3 rotation, Vector3 scale, string name = "Light") : base(position, scale, rotation, name)
        {
            this.colour = colour;
            this.intensity = intensity;
        }

        public override void Begin()
        {
            base.Begin();
            Renderer.AddLight(this);
        }

        public virtual void LoadData(Material mat, int index){
            
        }

        public override void End()
        {
            base.End();
            Renderer.RemoveLight(this);
        }
    }

    public class DirectionalLight : Light
    {
        public DirectionalLight(Color4 colour, float intensity, Vector3 position, Vector3 rotation, Vector3 scale, string name = "DirectionalLight") : base(colour, intensity, position, rotation, scale, name)
        {
        }

        public override void LoadData(Material mat, int index)
        {
            mat.SetUniformColour("lightData.directionalLight.colour", colour);
            mat.SetUniformVec3("lightData.directionalLight.dir", -forward);
            mat.SetUniformScalar("lightData.directionalLight.intensity", intensity);
            base.LoadData(mat, index);
        }

        public override void ExposeToInspector()
        {
            base.ExposeToInspector();
            ImGui.SliderFloat("intensity" , ref intensity, 0f, 10f);

            System.Numerics.Vector4 editColour = new System.Numerics.Vector4(colour.R, colour.G, colour.B, colour.A);

            ImGui.ColorEdit4("colour", ref editColour);

            colour = new Color4(editColour.X, editColour.Y, editColour.Z, editColour.W);

        }
    }

    public class PointLight : Light{
        public float range;
        public float falloff;

        public PointLight(float range, float falloff, Color4 colour, float intensity, Vector3 position, Vector3 rotation, Vector3 scale, string name = "PointLight") : base(colour, intensity, position, rotation, scale, name)
        {
            this.range = range;
            this.falloff = falloff;
        }

        public override void LoadData(Material mat, int index)
        {
            string path = "lightData.pointLights[" + index + "]";

            mat.SetUniformColour(path + ".colour", colour);
            mat.SetUniformVec3(path + ".pos", global_matrix.ExtractTranslation());
            mat.SetUniformScalar(path + ".intensity", intensity);

            mat.SetUniformScalar(path + ".range", range);
            mat.SetUniformScalar(path + ".falloff", falloff);
            
            base.LoadData(mat, index);
        }

        public override void ExposeToInspector()
        {
            base.ExposeToInspector();
            ImGui.SliderFloat("range", ref range, 0f, 100f);
            ImGui.SliderFloat("falloff", ref falloff, 0f, 1f);
            ImGui.SliderFloat("intensity" , ref intensity, 0f, 10f);

            System.Numerics.Vector4 editColour = new System.Numerics.Vector4(colour.R, colour.G, colour.B, colour.A);

            ImGui.ColorEdit4("colour", ref editColour);

            colour = new Color4(editColour.X, editColour.Y, editColour.Z, editColour.W);
        }
    }
}