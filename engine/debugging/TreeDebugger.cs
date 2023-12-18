using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ImGuiNET;
using OpenTKFPS.engine.singletons;
using OpenTKFPS.engine.structure;
using OpenTKFPS.engine.structure.actors;

namespace OpenTKFPS.engine.debugging
{
    public class TreeDebugger : DebugWindow
    {
        Scene sceneRef;

        public TreeDebugger(Scene s){
            sceneRef = s;
        }

        public override void Update()
        {
            if(ImGui.Begin("TreeDebugger")){
                ExposeHeirarchy(sceneRef.root);
            }
        }

        private void SelectForInspection(Actor selection){
            ActorInspector inspector = (ActorInspector)DebugWindowManager.GetDebugWindow(typeof(ActorInspector));
            if(inspector != null){
                inspector.selectedActor = selection;
            }
        }

        private void ExposeHeirarchy(Actor actor){
            List<Actor> kids = actor.GetChildren();

            if(kids.Count == 0){
                ImGui.Text(actor.name);
                ImGui.SameLine();
                if(ImGui.Button("select##" + actor.id)){
                    SelectForInspection(actor);
                }
            }
            else{
                bool tn = ImGui.TreeNode(actor.name + "##" + actor.id);
                ImGui.SameLine();
                if(ImGui.Button("select##" + actor.id)){
                    SelectForInspection(actor);
                }
                if(tn){
                    foreach(Actor kid in kids){
                        ExposeHeirarchy(kid);
                    }
                    ImGui.TreePop();
                }
            }
        }
    }
}