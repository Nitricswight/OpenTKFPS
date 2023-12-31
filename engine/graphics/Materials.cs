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
    public class ScreenQuadMaterial : Material{
        public ScreenQuadMaterial(): base(false){
            this.shaderProgram = MaterialLoader.LoadShader("assets/shaders/ScreenQuadShader");
        }

        public override void Use()
        {
            base.Use();
        }
    }

    public class StandardMaterial : Material
    {
        Color4 albedo;

        int albedoTexture = -1;

        public StandardMaterial(Color4 albedo, string? albedoTexturePath = null) : base(true){
            this.albedo = albedo;
            if(albedoTexturePath != null){
                albedoTexture = TextureLoader.loadTexture(albedoTexturePath);
            }
            this.shaderProgram = MaterialLoader.LoadShader("assets/shaders/StandardShader");
        }

        public override void Use()
        {
            base.Use();
            SetUniformColour("albedo", albedo);

            if(albedoTexture != -1){
                SetUniformScalar("albedoTextureEnabled", 1);
                GL.BindTexture(TextureTarget.Texture2D, albedoTexture);
            }
            else{
                SetUniformScalar("albedoTextureEnabled", 0);
            }

            SetUniformScalar("time", TimeManager.totalRunningTime);
        }

        public override void ExposeToInspector()
        {
            if(ImGui.TreeNode("Material : StandardMaterial")){
                OpenTKColorEdit4("albedo", ref albedo);

            }
        }

        public static StandardMaterial Default(){
            return new StandardMaterial(Color4.White, "assets/textures/noTex.png");
        }
    }
}