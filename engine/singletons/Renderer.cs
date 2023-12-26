using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTKFPS.engine.graphics;
using OpenTKFPS.engine.structure;
using OpenTKFPS.engine.structure.actors;
using OpenTKFPS.engine.structure.actors.renderableActors;

namespace OpenTKFPS.engine.singletons
{
    public static class Renderer
    {
        private static Matrix4 projection;
        private static Matrix4 view;

        private static float aspect;

        public static CameraActor3D currentCamera {get; private set;}

        public static bool wireframeMode = false;

        private static Mesh screenQuad = MeshLoader.LoadMesh(
            new float[]{
                -1.0f,-1.0f,0.0f,
                1.0f,1.0f,0.0f,
                -1.0f,1.0f,0.0f,
                1.0f,-1.0f,0.0f
            },
            new float[]{
                0.0f,0.0f,
                1.0f,1.0f,
                0.0f,1.0f,
                1.0f,0.0f
            },
            null,
            new uint[]{
                0,1,2,
                0,3,1
            }
        );

        private static ScreenQuadMaterial screenQuadMat = new ScreenQuadMaterial();

        public static Vector2i windowSize;

        public static void SetWindowSize(Vector2i size){
            windowSize = size;
            aspect = (float)size.X / (float)size.Y;
        }



        public static void DrawMeshActor3D(MeshActor3D meshActor){
            Material material = meshActor.material;
            Mesh mesh = meshActor.mesh;
            material.Use();
            Matrix4 global_mat = meshActor.global_matrix;
            GL.UniformMatrix4(MaterialLoader.GetUniformLocation("assets/shaders/StandardShader", "transformation"), false, ref global_mat);
            GL.UniformMatrix4(MaterialLoader.GetUniformLocation("assets/shaders/StandardShader", "projection"), false, ref projection);
            GL.UniformMatrix4(MaterialLoader.GetUniformLocation("assets/shaders/StandardShader", "view"), false, ref view);
            //Debug.WriteLine(mesh.vertexAttributes);
            GL.BindVertexArray(mesh.vaoID);
            for(int i = 0; i < mesh.vertexAttributes + 1; i++){
                GL.EnableVertexAttribArray(i);
            }
            
            
            if(mesh.drawType == Mesh.DRAW_TYPE.ARRAY){
                GL.DrawArrays(PrimitiveType.Triangles, 0, mesh.drawCount);
            }
            else{
                GL.DrawElements(PrimitiveType.Triangles, mesh.drawCount, DrawElementsType.UnsignedInt, mesh.elementOffset);
            }
        }

        public static void UpdateMatrices(){
            if(currentCamera == null){
                Debug.WriteLine("NO CAMERA!!");
                projection = Matrix4.Identity;
                view = Matrix4.Identity;
            }
            else{
                projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(currentCamera.fov), aspect, currentCamera.near, currentCamera.far);
                view = Matrix4.LookAt(currentCamera.local_position, currentCamera.local_position + currentCamera.forward, Vector3.UnitY);
            }

            //Debug.WriteLine(projection);
            //Debug.WriteLine(view);
        }

        public static void SetCurrentCamera(CameraActor3D newCam){
            if(currentCamera != null){
                currentCamera.isCurrent = false;
            }

            currentCamera = newCam;
            newCam.isCurrent = true;
        }

        public static void DrawScreenQuad(ViewportActor viewport){
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            screenQuadMat.Use();
            GL.BindVertexArray(screenQuad.vaoID);
            GL.BindTexture(TextureTarget.Texture2D, viewport.colourTexture);

            Vector2 finalScale = Vector2.One;
            if(viewport.keepAspect){
                float scaleX = (float)windowSize.X / (float)viewport.width;
                float scaleY = (float)windowSize.Y / (float)viewport.height;

                

                if(scaleX > scaleY){
                    finalScale.X = ((float)viewport.width * scaleY) / (float)windowSize.X;
                    finalScale.Y = ((float)viewport.height * scaleY) / (float)windowSize.Y;
                }
                else{
                    finalScale.Y = ((float)viewport.height * scaleX) / (float)windowSize.Y;
                    finalScale.X = ((float)viewport.width * scaleX) / (float)windowSize.X;
                }

            }

            //Debug.WriteLine(finalScale.ToString());

            GL.Uniform2(GL.GetUniformLocation(screenQuadMat.shaderProgram, "scale"), finalScale);
            
            
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.DrawElements(PrimitiveType.Triangles, screenQuad.drawCount, DrawElementsType.UnsignedInt, 0);
            GL.Enable(EnableCap.DepthTest);
            if(!wireframeMode){
                GL.Enable(EnableCap.CullFace);
            }

            GL.PolygonMode(MaterialFace.FrontAndBack, wireframeMode? PolygonMode.Line : PolygonMode.Fill);
        }
    }
}