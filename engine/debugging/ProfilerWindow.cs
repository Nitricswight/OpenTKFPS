using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImGuiNET;
using OpenTKFPS.engine.singletons;

namespace OpenTKFPS.engine.debugging
{
    public class ProfilerWindow : DebugWindow
    {
        List<float> values = new List<float>() {0.0f};

        float peak = 0;
        float trough = 100000;

        bool msPerFrame = false;

        public ProfilerWindow(bool msPerFrame){
            this.msPerFrame = msPerFrame;
        }

        private void ToggleMode(){
            msPerFrame = !msPerFrame;
            peak = 0;
            trough = 100000;
            values.Clear();
            values.Add(0);
        }

        public override void Update()
        {
            if(msPerFrame){
                float ms = TimeManager.lastUpdateDelta * 1000.0f;

                if(ms > peak && TimeManager.totalRunningTime > 2.0f){
                    peak = ms;
                }

                if(ms < trough && TimeManager.totalRunningTime > 2.0f){
                    trough = ms;
                }

                values.Add(ms);
                if(values.Count > 120){
                    values.RemoveAt(0);
                }
                

                float[] f = values.ToArray();

                if(ImGui.Begin("profiler")){
                    if(ImGui.Button("Mode: " + (msPerFrame? "ms per frame" : "frames per second"))){
                        ToggleMode();
                    }
                    ImGui.PlotLines("MS per frame : " + values[values.Count - 1], ref f[0], f.Length);
                    ImGui.Text("Peak MS: " + peak.ToString());
                    ImGui.Text("Lowest MS: " + trough.ToString());
                    
                }

                
            }
            else{
                int FPS = (int)TimeManager.FPS;

                if(FPS > peak && TimeManager.totalRunningTime > 2.0f){
                    peak = FPS;
                }

                if(FPS < trough && TimeManager.totalRunningTime > 2.0f){
                    trough = FPS;
                }

                values.Add(FPS);
                if(values.Count > 120){
                    values.RemoveAt(0);
                }
                

                float[] f = values.ToArray();

                if(ImGui.Begin("profiler")){
                    if(ImGui.Button("Mode: " + (msPerFrame? "ms per frame" : "frames per second"))){
                        ToggleMode();
                    }
                    ImGui.PlotLines("FPS : " + values[values.Count - 1], ref f[0], f.Length);
                    ImGui.Text("Peak FPS: " + peak.ToString());
                    ImGui.Text("Lowest FPS: " + trough.ToString());
                    
                }
            }

            
        }
    }
}