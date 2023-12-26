using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using glTFLoader;
using glTFLoader.Schema;
using Newtonsoft.Json;
using OpenTKFPS.engine.structure.actors;
using OpenTK.Mathematics;
using OpenTKFPS.engine.structure.actors.renderableActors;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Specialized;
using Assimp.Configs;

namespace OpenTKFPS.engine.graphics
{

    public static class SceneLoader
    {
        #region myImplementation

        public static Actor3D LoadGLTFScene(string filePath){
            Gltf file = Interface.LoadModel(filePath);

            List<byte[]> buffers = LoadBuffers(file);
            
            Debug.WriteLine("nodes: " + file.Nodes.Length.ToString());
            Debug.WriteLine("meshes: " + file.Meshes.Length.ToString());
            Debug.WriteLine("materials: " + file.Materials.Length.ToString());
            Debug.WriteLine("accessors: " + file.Accessors.Length.ToString());
            Debug.WriteLine("bufferViews: " + file.BufferViews.Length.ToString());
            Debug.WriteLine("buffers: " + file.Buffers.Length.ToString());

            Actor3D root = new Actor3D(Vector3.Zero, Vector3.One, Vector3.Zero, file.Scenes[0].Name);

            foreach(int nodeIndex in file.Scenes[0].Nodes){
                root.AddChild(LoadGLTFNode(file, file.Nodes[nodeIndex], buffers));
            }

            return root;
        }

        private static object convertVector(float[] inps){
            if(inps.Length == 2){
                return new Vector2(inps[0], inps[1]);
            }
            else if(inps.Length == 3){
                return new Vector3(inps[0],inps[1],inps[2]);
            }
            else if (inps.Length == 4){
                Quaternion q = new Quaternion(inps[0],inps[1],inps[2],inps[3]);
                return q.ToEulerAngles();
            }
            else{
                return null;
            }
        }

        private static Actor3D LoadGLTFNode(Gltf data, Node currentNode, List<byte[]> buffers){
            Actor3D result;
            if(currentNode.Mesh.HasValue){

                glTFLoader.Schema.Mesh mesh = data.Meshes[currentNode.Mesh.Value];
                if(mesh.Primitives.Length > 1){
                    result = new Actor3D((Vector3)convertVector(currentNode.Translation), (Vector3)convertVector(currentNode.Scale), (Vector3)convertVector(currentNode.Rotation), currentNode.Name);

                    foreach(MeshPrimitive primitive in mesh.Primitives){
                        result.AddChild(new MeshActor3D(LoadGLTFMesh(data, primitive, buffers), StandardMaterial.Default(),(Vector3)convertVector(currentNode.Translation), (Vector3)convertVector(currentNode.Rotation),(Vector3)convertVector(currentNode.Scale), currentNode.Name));
                    }


                }
                else{
                    result = new MeshActor3D(LoadGLTFMesh(data, mesh.Primitives[0], buffers), StandardMaterial.Default(),(Vector3)convertVector(currentNode.Translation), (Vector3)convertVector(currentNode.Rotation),(Vector3)convertVector(currentNode.Scale), currentNode.Name);
                }

                if(currentNode.Children != null){
                    foreach(int childIndex in currentNode.Children){
                        result.AddChild(LoadGLTFNode(data, data.Nodes[childIndex], buffers));
                    }
                }

                

                return result;
            }
            else{
                //Debug.WriteLine(currentNode.Rotation.Length);
                result = new Actor3D((Vector3)convertVector(currentNode.Translation), (Vector3)convertVector(currentNode.Scale), (Vector3)convertVector(currentNode.Rotation));

                if(currentNode.Children != null){
                    foreach(int childIndex in currentNode.Children){
                        result.AddChild(LoadGLTFNode(data, data.Nodes[childIndex], buffers));
                    }
                }

                return result;
            }

            
        }

        private static List<byte[]> LoadBuffers(Gltf data){
            List<byte[]> result = new List<byte[]>();

            foreach(glTFLoader.Schema.Buffer buffer in data.Buffers){
                byte[] bytes = Convert.FromBase64String(buffer.Uri.Split(";base64,")[1]);
                result.Add(bytes);
            }

            return result;
        }

        private static Mesh LoadGLTFMesh(Gltf data, MeshPrimitive primitive , List<byte[]> buffers){
            int vaoID = GL.GenVertexArray();

            GL.BindVertexArray(vaoID);

            LoadGLTFAccessor(0, 3, false, data.Accessors[primitive.Attributes["POSITION"]], data, buffers);
            LoadGLTFAccessor(1, 2, false, data.Accessors[primitive.Attributes["TEXCOORD_0"]], data, buffers);
            LoadGLTFAccessor(2, 3, false, data.Accessors[primitive.Attributes["NORMAL"]], data, buffers);


            BufferView eboBV = data.BufferViews[data.Accessors[primitive.Indices.Value].BufferView.Value];


            int ebo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

            GL.BufferData(BufferTarget.ElementArrayBuffer, eboBV.ByteLength, buffers[eboBV.Buffer], BufferUsageHint.StaticDraw);

            return new Mesh(data.Accessors[primitive.Indices.Value].Count, Mesh.DRAW_TYPE.ELEMENT, vaoID, 2, eboBV.ByteOffset);

            //TODO: Figure out why a vertex has four elements???????
            //TODO: Get the binary to count right, then we're sorted

        }

        private static void LoadGLTFAccessor(int index, int elementsPerItem, bool normalised, Accessor accessor, Gltf data , List<byte[]> buffers){
            BufferView bv = data.BufferViews[accessor.BufferView.Value];

            MeshLoader.LoadByteArrayAttribute(index, buffers[bv.Buffer], bv.ByteLength, normalised, bv.ByteStride.GetValueOrDefault(0), bv.ByteOffset, elementsPerItem);
        }

        #endregion


        #region assimpImplementation

        public static Actor3D AssimpLoadScene(string filePath){
            Assimp.AssimpContext loader = new Assimp.AssimpContext();

            Assimp.Scene scene = loader.ImportFile(filePath);

            List<Mesh> meshes = new List<Mesh>();

            Debug.WriteLine("Loading scene: " + filePath);
            Debug.WriteLine("Meshes: " + scene.MeshCount);
            Debug.WriteLine("Materials: " + scene.Materials.Count);

            foreach(Assimp.Mesh m in scene.Meshes){
                float[] vertices = new float[m.Vertices.Count * 3];
                float[] uvs = new float[m.TextureCoordinateChannels[0].Count * 2];
                float[] normals = new float[m.Normals.Count * 3];

                uint[] indices = m.GetUnsignedIndices();

                for(int i = 0; i < indices.Length; i++){
                    vertices[i * 3 + 0] = m.Vertices[i].X;
                    vertices[i * 3 + 1] = m.Vertices[i].Y;
                    vertices[i * 3 + 2] = m.Vertices[i].Z;
                    
                    if(m.HasNormals){
                        normals[i * 3 + 0] = m.Normals[i].X;
                        normals[i * 3 + 1] = m.Normals[i].Y;
                        normals[i * 3 + 2] = m.Normals[i].Z;
                    }
                    
                    if(m.HasTextureCoords(0)){
                        uvs[i * 2 + 0] = m.TextureCoordinateChannels[0][i].X;
                        uvs[i * 2 + 1] = m.TextureCoordinateChannels[0][i].Y;
                    }
                    
                }

                meshes.Add(MeshLoader.LoadMesh(vertices, m.HasTextureCoords(0)? uvs : null, m.HasNormals? normals : null, indices));
            }

            if(meshes.Count == 0){
                Debug.WriteLine("ASSIMP LOAD FAILED::NO MESHES FOUND");
                return null;
            }

            return AssimpLoadNode(scene.RootNode, meshes);
            

            
        }

        private static Actor3D AssimpLoadNode(Assimp.Node node, List<Mesh> meshes){

            Debug.WriteLine("Beginning load of node: " + node.Name);

            Debug.WriteLine("Surfaces: " + node.MeshCount);
            Actor3D result;

            Assimp.Vector3D p , s;
            Assimp.Quaternion r;

            node.Transform.Decompose(out s, out r, out p);

            Vector3 pos = new Vector3(p.X, p.Y, p.Z);
            Vector3 sca = new Vector3(s.X, s.Y, s.Z);
            Quaternion quat = new Quaternion(r.X, r.Y, r.Z, r.W);
            Vector3 rot = quat.ToEulerAngles();

            
            

            if(node.MeshCount == 1){
                result = new MeshActor3D(meshes[node.MeshIndices[0]], StandardMaterial.Default(), pos, rot, sca, node.Name);
            }
            else{
                result = new Actor3D(pos, sca, rot, node.Name);

                //add all separate meshes as children

                for(int j = 0; j < node.MeshCount; j++){
                    result.AddChild(new MeshActor3D(meshes[node.MeshIndices[j]], StandardMaterial.Default(), Vector3.Zero, Vector3.Zero, Vector3.One, "Surface " + j.ToString()));
                }


            }

            //add all children recursively
            Debug.WriteLine("children: " + node.ChildCount);

            foreach(Assimp.Node child in node.Children){
                if(child.ChildCount != 0 || child.MeshCount != 0){
                    result.AddChild(AssimpLoadNode(child, meshes));
                }
                else{
                    Debug.WriteLine("skipping...");
                }
                
            }

            return result;
        }

        #endregion
    }
}