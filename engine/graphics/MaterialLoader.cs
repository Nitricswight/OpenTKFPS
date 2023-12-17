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
        public int shaderProgram;

        public virtual void Use(){
            GL.UseProgram(shaderProgram);
        }

        public virtual void ExposeToInspector(){
        }

        public void OpenTKColorEdit4(string label, ref Color4 col){
            System.Numerics.Vector4 editAlbedo = new System.Numerics.Vector4(col.R, col.G, col.B, col.A);

            ImGui.ColorEdit4("albedo", ref editAlbedo);

            col = new Color4(editAlbedo.X, editAlbedo.Y, editAlbedo.Z, editAlbedo.W);
        }
    }

    public static class MaterialLoader
    {
        private static Dictionary<string, int> programCache = new Dictionary<string, int>();

        private static Dictionary<string, int> uniformLocationCache = new Dictionary<string, int>();

        public static int GetUniformLocation(string shaderName, string uniformName){
            if(uniformLocationCache.ContainsKey(shaderName + "_" + uniformName)){
                return uniformLocationCache[shaderName + "_" + uniformName];
            }
            else{
                int location = GL.GetUniformLocation(programCache[shaderName], uniformName);
                uniformLocationCache.Add(shaderName + "_" + uniformName, location);
                return location;
            }
        }

        public static void CleanUp(){
            int[] programs = programCache.Values.ToArray();
            for(int i = 0; i < programs.Length; i++){
                GL.DeleteProgram(programs[i]);
            }
        }

        public static int LoadShader(string path){
            if(programCache.ContainsKey(path)){
                return programCache[path];
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

            programCache.Add(path , program);

            return program;
        }
    }
}