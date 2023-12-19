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

        public static void LoadByteArrayAttribute(int index, byte[] data, int length, bool normalised, int stride, int offset, int elementsPerItem){
            int vbo = GL.GenBuffer();
            buffers.Add(vbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, length, data, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(index, elementsPerItem, VertexAttribPointerType.Byte, normalised, stride, offset);
        }



    }

    public class Mesh{
        public int drawCount;
        public int elementOffset;
        public enum DRAW_TYPE{
            ARRAY,
            ELEMENT
        }

        public DRAW_TYPE drawType;

        public int vaoID;

        public int vertexAttributes;

        public Mesh(int drawCount, DRAW_TYPE drawType, int vaoID, int vertexAttributes, int elementOffset = 0)
        {
            this.drawCount = drawCount;
            this.drawType = drawType;
            this.vaoID = vaoID;
            this.vertexAttributes = vertexAttributes;
            this.elementOffset = elementOffset;
        }
    }
}