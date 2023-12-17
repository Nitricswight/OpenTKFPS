using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using ImGuiNET;
using OpenTKFPS.engine.singletons;

namespace OpenTKFPS.engine.graphics
{
    public class StandardMaterial : Material
    {
        Color4 albedo;

        int albedoTexture = -1;

        public StandardMaterial(Color4 albedo, string? albedoTexturePath = null){
            this.albedo = albedo;
            if(albedoTexturePath != null){
                albedoTexture = TextureLoader.loadTexture(albedoTexturePath);
            }
            this.shaderProgram = MaterialLoader.LoadShader("assets/shaders/StandardShader");
        }

        public void SetTransformation(Matrix4 t){
            GL.UniformMatrix4(MaterialLoader.GetUniformLocation("assets/shaders/StandardShader", "transformation"), false, ref t);
        }

        public override void Use()
        {
            base.Use();
            GL.Uniform4(MaterialLoader.GetUniformLocation("assets/shaders/StandardShader", "albedo"), albedo);

            if(albedoTexture != -1){
                GL.Uniform1(MaterialLoader.GetUniformLocation("assets/shaders/StandardShader", "albedoTextureEnabled"), 1);
                GL.BindTexture(TextureTarget.Texture2D, albedoTexture);
            }
            else{
                GL.Uniform1(MaterialLoader.GetUniformLocation("assets/shaders/StandardShader", "albedoTextureEnabled"), 0);
            }

            GL.Uniform1(MaterialLoader.GetUniformLocation("assets/shaders/StandardShader", "time"), TimeManager.totalRunningTime);
        }

        public override void ExposeToInspector()
        {
            if(ImGui.TreeNode("Material : StandardMaterial")){
                OpenTKColorEdit4("albedo", ref albedo);

            }
        }

        public static StandardMaterial Default(){
            return new StandardMaterial(Color4.Pink, null);
        }
    }
}