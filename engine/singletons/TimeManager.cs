using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OpenTKFPS.engine.singletons
{
    public static class TimeManager
    {
        public static float FPS;

        public static float lastUpdateDelta;

        public static float totalRunningTime;

        private static int FPSlimit = -1;

        public static void Update(float delta){
            totalRunningTime += delta;
            lastUpdateDelta = delta;
            FPS = (1.0f / lastUpdateDelta);

            if(FPSlimit != -1){
                int ms = (int)(1000f / FPSlimit - delta);
                
                Debug.WriteLine("milliseconds to sleep: " + ms.ToString());

                Thread.Sleep(ms > 0? ms : 0);
            }
        }

        public static void SetFPSLimit(int limit){
            FPSlimit = limit;
        }

        public static void UnlimitFPS(){
            FPSlimit = -1;
        }
    }
}