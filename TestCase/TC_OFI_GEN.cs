using System;
using DriverBase;
using DriverBase_Platform;
using Octopus;
public class TC_GENERATED_OMER : AbsTesterDriver
{
static Octolog Log = new Octolog();
public void Setup(){
                SetupBase();
                Log.Set_Author("Ömer Faruk İlhan");
                Log.Set_ExecutedBy("Ömer Faruk İlhan");
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
           var msgo = new DriverBase_Platform.FuncMsg.phy_T1042_private.findPhy();
           msgo.chd.phyId = 1;
           msgo.chd.DriverReturn = 1;
           uut.SendMsg(msgo);
           var msgi = uut.GetMsg<DriverBase_Platform.FuncMsg.phy_T1042_private.findPhy>();
}
}
