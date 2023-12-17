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

        int peak = 0;
        int trough = 1000;
        public override void Update()
        {
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
                ImGui.PlotLines("FPS : " + values[values.Count - 1], ref f[0], f.Length);
                ImGui.Text("Peak FPS: " + peak.ToString());
                ImGui.Text("Lowest FPS: " + trough.ToString());
            }
        }
    }
}