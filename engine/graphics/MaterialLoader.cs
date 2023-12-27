using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenTKFPS.engine.graphics
{
    public class Material{
        public bool lit;
        public int shaderProgram;

        public virtual void Use(){
            GL.UseProgram(shaderProgram);
        }

        public virtual void ExposeToInspector(){
        }

        public virtual void CacheUniformLocations(){

        }

        public void OpenTKColorEdit4(string label, ref Color4 col){
            System.Numerics.Vector4 editAlbedo = new System.Numerics.Vector4(col.R, col.G, col.B, col.A);

            ImGui.ColorEdit4("albedo", ref editAlbedo);

            col = new Color4(editAlbedo.X, editAlbedo.Y, editAlbedo.Z, editAlbedo.W);
        }

        public Material(bool lit){
            this.lit = lit;
        }

        public void SetUniformVec3(string name, Vector3 value){
            GL.Uniform3(GL.GetUniformLocation(shaderProgram, name), ref value);
        }

        public void SetUniformVec4(string name, Vector4 value){
            GL.Uniform4(GL.GetUniformLocation(shaderProgram, name), ref value);
        }

        public void SetUniformColour(string name, Color4 value){
            GL.Uniform4(GL.GetUniformLocation(shaderProgram, name), value);
        }

        public void SetUniformScalar(string name, int value){
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, name), value);
        }

        public void SetUniformScalar(string name, float value){
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, name), value);
        }

        public void SetUniformMatrix(string name, Matrix4 value, bool transpose){
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, name), transpose, ref value);
        }
    }

    public static class MaterialLoader
    {
        private static Dictionary<string, int> cachedShaders = new Dictionary<string, int>();

        public static void CleanUp(){
            int[] p = cachedShaders.Values.ToArray();
            foreach(int i in p){
                GL.DeleteProgram(i);
            }
        }

        public static int LoadShader(string path){
            if(cachedShaders.ContainsKey(path)){
                return cachedShaders[path];
            }

            string vertexShaderSource;
            using(StreamReader reader = new StreamReader(path + ".vert")){
                vertexShaderSource = reader.ReadToEnd();
            }

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);

            GL.ShaderSource(vertexShader, vertexShaderSource);

            GL.CompileShader(vertexShader);

            string infoLog = GL.GetShaderInfoLog(vertexShader);
            if(infoLog != String.Empty){
                Debug.WriteLine("VERTEX_SHADER::COMPILATION_FAIL::" + infoLog);
            }

            string fragmentShaderSource;
            using(StreamReader reader = new StreamReader(path + ".frag")){
                fragmentShaderSource = reader.ReadToEnd();
            }

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(fragmentShader, fragmentShaderSource);

            GL.CompileShader(fragmentShader);

            infoLog = GL.GetShaderInfoLog(fragmentShader);
            if(infoLog != String.Empty){
                Debug.WriteLine("FRAGMENT_SHADER::COMPILATION_FAIL::" + infoLog);
            }

            int program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);

            infoLog = GL.GetProgramInfoLog(program);
            if(infoLog != string.Empty){
                Debug.WriteLine("SHADER_PROGRAM::LINK_FAIL::" + infoLog);
            }

            cachedShaders.Add(path , program);

            return program;
        }
    }
}