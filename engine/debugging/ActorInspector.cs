using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImGuiNET;
using OpenTKFPS.engine.structure.actors;

namespace OpenTKFPS.engine.debugging
{
    public class ActorInspector : DebugWindow
    {
        public Actor? selectedActor = null;

        public override void Update()
        {
            if(ImGui.Begin("Actor Inspector")){
                if(selectedActor == null){
                    ImGui.Text("No Actor selected!");
                }
                else{
                    ImGui.Text("Selected: " + selectedActor.name);
                    selectedActor.ExposeToInspector();
                }
            }
            
        }
    }
}