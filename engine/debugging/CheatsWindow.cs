using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTKFPS.engine.singletons;

namespace OpenTKFPS.engine.debugging
{
    public class CheatsWindow : DebugWindow
    {

        public override void Update()
        {
            if(ImGui.Begin("Cheats")){
                if(ImGui.Button("toggle wireframe: " + (Renderer.wireframeMode? "On" : "Off"))){

                    Renderer.wireframeMode = !Renderer.wireframeMode;
                }

                if(ImGui.TreeNode("FPS limit test")){
                    if(ImGui.Button("limit to 120")){
                        TimeManager.SetFPSLimit(120);
                    }

                    if(ImGui.Button("limit to 60")){
                        TimeManager.SetFPSLimit(60);
                    }

                    if(ImGui.Button("unlimit FPS")){
                        TimeManager.UnlimitFPS();
                    }
                }
            }
        }
    }
}