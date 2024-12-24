using System;
using DriverBase;
using DriverBase_Platform;
using Octopus;
using Nodify.Calculator;
public class TC_GENERATED_OMER : AbsTesterDriver
{
static Octolog Log = new Octolog();
public void Setup(){
                SetupBase();
                Log.Set_Author("Mehmet Erkuş");
                Log.Set_ExecutedBy("Mehmet Erkuş");
                Log.Set_UutVersion("SRS_LLR_DISCRET_IN(Baseline 7.0), SRS_DISCRETE_IN(Baseline 5.0), DD_DISCRETE_IN(Baseline 7.0)");
                if (SuiteConfig.configId == ConfigId.MANUAL)
                {
                            Defs.TIME_OUT = 1_000_000; 
                }
}
 public static class Defs{
            public static int TIME_OUT = 1_000; // 1 second
            public static byte True = 1;
            public static byte False = 0;
}
public void TCF_OFI()
{
Setup();
}
}
