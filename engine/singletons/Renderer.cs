using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTKFPS.engine.graphics;
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

        public static void DrawMeshActor3D(MeshActor3D meshActor){
            Material material = meshActor.material;
            Mesh mesh = meshActor.mesh;
            material.Use();
            Matrix4 global_mat = meshActor.global_matrix;
            GL.UniformMatrix4(MaterialLoader.GetUniformLocation("assets/shaders/StandardShader", "transformation"), false, ref global_mat);
            GL.UniformMatrix4(MaterialLoader.GetUniformLocation("assets/shaders/StandardShader", "projection"), false, ref projection);
            GL.UniformMatrix4(MaterialLoader.GetUniformLocation("assets/shaders/StandardShader", "view"), false, ref view);
            for(int i = 0; i < mesh.vertexAttributes + 1; i++){
                GL.EnableVertexAttribArray(i);
            }
            GL.BindVertexArray(mesh.vaoID);
            
            if(mesh.drawType == Mesh.DRAW_TYPE.ARRAY){
                GL.DrawArrays(PrimitiveType.Triangles, 0, mesh.drawCount);
            }
            else{
                GL.DrawElements(PrimitiveType.Triangles, mesh.drawCount, DrawElementsType.UnsignedInt, 0);
            }
        }

        public static void SetAspect(float _aspect){
            aspect = _aspect;
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
    }
}