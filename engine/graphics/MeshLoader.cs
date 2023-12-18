using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Assimp;
using Newtonsoft.Json.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTKFPS.engine.structure.actors;
using OpenTKFPS.engine.structure.actors.renderableActors;

namespace OpenTKFPS.engine.graphics
{
    public static class MeshLoader
    {
        private static List<int> buffers = new List<int>();
        private static List<int> arrays = new List<int>();

        public static void CleanUp(){
            GL.DeleteBuffers(buffers.Count, buffers.ToArray());
            GL.DeleteVertexArrays(arrays.Count, arrays.ToArray());
        }

        public static Mesh LoadMesh(float[] vertices, float[]? uvs = null, float[]? normals = null, uint[]? indices = null){
            int vao = GL.GenVertexArray();

            arrays.Add(vao);
            GL.BindVertexArray(vao);

            LoadToArrayAttribute(0, vertices, 3, false);
            int attributes = 0;
            if (uvs != null){
                LoadToArrayAttribute(1, uvs, 2, false);
                attributes = 1;
            }

            if (normals != null){
                LoadToArrayAttribute(2, normals, 3, true);
                attributes = 2;
            }
            
            Mesh.DRAW_TYPE type;
            int drawCount;
            if(indices == null){
                type = Mesh.DRAW_TYPE.ARRAY;
                drawCount = vertices.Length / 3;
            }
            else{
                int ebo = GL.GenBuffer();
                buffers.Add(ebo);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
                type = Mesh.DRAW_TYPE.ELEMENT;
                drawCount = indices.Length;
            }
            return new Mesh(drawCount, type, vao, attributes);
            
        }

        private static void LoadToArrayAttribute(int index, float[] data, int elementsPerItem, bool normalised){
            int vbo = GL.GenBuffer();
            buffers.Add(vbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(index, elementsPerItem, VertexAttribPointerType.Float, normalised, elementsPerItem * sizeof(float), 0);
        }

        public static Actor3D LoadAssimpFile(string filePath){
            AssimpContext importer = new AssimpContext();
            Scene file = importer.ImportFile(filePath);

            if(importer.IsImportFormatSupported("." + filePath.Split(".")[1])){
                Debug.WriteLine("format supported");
            }

            List<Mesh> meshes = new List<Mesh>();
            List<Material> materials = new List<Material>();
            materials.Add(StandardMaterial.Default());
            foreach(Assimp.Mesh mesh in file.Meshes){
                float[] vertices = new float[mesh.Vertices.Count * 3];
                float[] uvs = new float[mesh.TextureCoordinateChannels[0].Count * 2];
                float[] normals = new float[mesh.Normals.Count * 3];
                uint[] indices = mesh.GetUnsignedIndices();

                for(int i = 0; i < indices.Length; i++){
                    vertices[(3 * i) + 0] = mesh.Vertices[i].X;
                    vertices[(3 * i) + 1] = mesh.Vertices[i].Y;
                    vertices[(3 * i) + 2] = mesh.Vertices[i].Z;

                    if(mesh.TextureCoordinateChannelCount > 0){
                        uvs[(2 * i) + 0] = mesh.TextureCoordinateChannels[0][i].X;
                        uvs[(2 * i) + 1] = mesh.TextureCoordinateChannels[0][i].Y;
                    }
                    
                    if(mesh.HasNormals){
                        normals[(3 * i) + 0] = mesh.Normals[i].X;
                        normals[(3 * i) + 1] = mesh.Normals[i].Y;
                        normals[(3 * i) + 2] = mesh.Normals[i].Z;
                    }
                    
                }

                meshes.Add(LoadMesh(vertices, uvs, normals, indices));
            }

            //TODO: add material loading here

            Debug.WriteLine("expected meshes: " + file.MeshCount.ToString() + "\nloaded meshes: " + meshes.Count.ToString());

            Debug.WriteLine("found nodes: " + file.RootNode.ChildCount.ToString());

            return LoadAssimpNode(Assimp.Matrix4x4.FromEulerAnglesXYZ(new Vector3D(0,0,0)), meshes, materials, file.RootNode);
        }

        private static Actor3D LoadAssimpNode(Assimp.Matrix4x4 currentMatrix, List<Mesh> meshes, List<Material> materials, Node root){
            
            Actor3D result;
            Assimp.Matrix4x4 mat = root.Transform * currentMatrix;
            Assimp.Vector3D position;
            Assimp.Quaternion rotation;
            Assimp.Vector3D scale;

            mat.Decompose(out scale, out rotation, out position);

            Vector3 pos = new Vector3(position.X, position.Y, position.Z);
            Vector3 sca = new Vector3(scale.X, scale.Y, scale.Z);
            Assimp.Matrix3x3 rotm = rotation.GetMatrix();
            Vector3 rot = new Vector3(
                MathF.Atan2(rotm.C2, rotm.C3),
                MathF.Atan2(-rotm.C1, MathF.Sqrt((rotm.C2 * rotm.C2) + (rotm.C3 * rotm.C3))),
                MathF.Atan2(rotm.B1, rotm.A1)
            );


            if(root.MeshCount == 1){
                Debug.WriteLine("making Mesh Actor: " + root.Name);
                result = new MeshActor3D(meshes[root.MeshIndices[0]], materials[0], pos, rot, sca, root.Name);
            }
            else{
                Debug.WriteLine("making Node Actor: " + root.Name);
                result = new Actor3D(pos,sca,rot, root.Name);

                for(int i = 0; i < root.MeshCount; i++){
                    Debug.WriteLine("making surface " + i.ToString());
                    result.AddChild(new MeshActor3D(meshes[root.MeshIndices[i]], materials[0], Vector3.Zero, Vector3.Zero, Vector3.One, "surface" + i.ToString()));
                }


            }

            Debug.WriteLine("node " + root.Name + " has " + root.ChildCount.ToString() + " children.");

            for(int j = 0; j < root.ChildCount; j++){
                result.AddChild(LoadAssimpNode(mat, meshes, materials, root.Children[j]));
            }
            

            return result;
        }

        public static void MyGLTFParse(string filePath){

            

            JObject file = JObject.Parse(new StreamReader(filePath).ReadToEnd());

            

            Debug.WriteLine("meshes: " + file.GetValue("meshes").Count().ToString());
            Debug.WriteLine("nodes: " + file.GetValue("nodes").Count().ToString());
            Debug.WriteLine("materials: " + file.GetValue("materials").Count().ToString());
            Debug.WriteLine("buffers: " + file.GetValue("buffers").Count().ToString());
            Debug.WriteLine("bufferViews: " + file.GetValue("bufferViews").Count().ToString());
            Debug.WriteLine("accessors: " + file.GetValue("accessors").Count().ToString());
            // Debug.WriteLine("nodes: " + file.RootElement.GetProperty("nodes").GetArrayLength());
            // Debug.WriteLine("buffers: " + file.RootElement.GetProperty("buffers").GetArrayLength());
            // Debug.WriteLine("buffer views: " + file.RootElement.GetProperty("bufferViews").GetArrayLength());
            // Debug.WriteLine("accessors: " + file.RootElement.GetProperty("accessors").GetArrayLength());
            // Debug.WriteLine("materials: " + file.RootElement.GetProperty("materials").GetArrayLength());
            
        }
    }

    public class Mesh{
        public int drawCount;
        public enum DRAW_TYPE{
            ARRAY,
            ELEMENT
        }

        public DRAW_TYPE drawType;

        public int vaoID;

        public int vertexAttributes;

        public Mesh(int drawCount, DRAW_TYPE drawType, int vaoID, int vertexAttributes)
        {
            this.drawCount = drawCount;
            this.drawType = drawType;
            this.vaoID = vaoID;
            this.vertexAttributes = vertexAttributes;
        }
    }
}