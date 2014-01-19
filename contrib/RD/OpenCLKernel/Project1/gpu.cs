  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Drawing;
  using System.Drawing.Imaging;
  using System.IO;
  using System.Runtime.InteropServices;
  using OpenCL.Net;
using GPUEnumerator;



  public class EnumerateGPUs

    {


        public struct clState 
{
             public  Context cl_context;
            //cl_context OpenCL. context;
            public Kernel cl_kernel;
            public CommandQueue cl_command_queue;
            public Program cl_program;
            public Mem outputBuffer;
            public uint4 CLBuffer0;

            public uint4 padbuffer8;


            public IntPtr padbufsize;
            //	size_t padbufsize;
            public IntPtr cldata;
            //	void * cldata;
            public 	bool hasBitAlign;
            public 	bool hasOpenCL11plus;
            public 	bool hasOpenCL12plus;
            public 	bool goffset;
            public uint16 vwidth;
            //	uint16 vwidth;
            public IntPtr max_work_size;
            public IntPtr wsize;

} 


public struct dev_blk_ctx {
public OpenCL.Net.uint16 	 ctx_a;
  public   uint16  ctx_b;
   public uint16 ctx_c; 
  public  uint16 ctx_d;
   public uint16 ctx_e; uint16 ctx_f; uint16 ctx_g; uint16 ctx_h;

public	uint16 cty_a; uint16 cty_b; uint16 cty_c; uint16 cty_d;
public	uint16 cty_e; uint16 cty_f; uint16 cty_g; uint16 cty_h;
public	uint16 merkle; uint16 ntime; uint16 nbits; uint16 nonce;
public	uint16 fW0; uint16 fW1; uint16 fW2; uint16 fW3; uint16 fW15;
public	uint16 fW01r; uint16 fcty_e; uint16 fcty_e2;
public	uint16 W16; uint16 W17; uint16 W2;
public	uint16 PreVal4; uint16 T1;
	public uint16 C1addK5; uint16 D1A; uint16 W2A; uint16 W17_2;
public	uint16 PreVal4addT1; uint16 T1substate0;
public	uint16 PreVal4_2;
public	uint16 PreVal0;
public	uint16 PreW18;
public	uint16 PreW19;
public	uint16 PreW31;
public	uint16 PreW32;
public	uint16 B1addK6, PreVal0addK7, W16addK16, W17addK17;
public	uint16 zeroA, zeroB;
public	uint16 oneA, twoA, threeA, fourA, fiveA, sixA, sevenA;
} 

        private Context _context;
        private Device _device;

        private void CheckErr(ErrorCode err, string name)
        {
            if (err != ErrorCode.Success)
            {
                Console.WriteLine("ERROR: " + name + " (" + err.ToString() + ")");
            }
        }

        private void ContextNotify(string errInfo, byte[] data, IntPtr cb, IntPtr userData)
        {
            Console.WriteLine("OpenCL Notification: " + errInfo);
        }



        public List<GPUEnumerator.GPU> SuckInGPUs()
        {

            ErrorCode error;
            Platform[] platforms = Cl.GetPlatformIDs(out error);
            List<Device> devicesList = new List<Device>();
            List<GPU> GPUList = new List<GPU>();
            CheckErr(error, "Cl.GetPlatformIDs");
            int id = 0;
            foreach (Platform platform in platforms)
            {
                //ToDO:Log Platform Device Name;
                //		status = clGetPlatformInfo(platform, CL_PLATFORM_NAME, sizeof(pbuff), pbuff, NULL);
                //		status = clGetPlatformInfo(platform, CL_PLATFORM_VERSION, sizeof(pbuff), pbuff, NULL);
                //		status = clGetDeviceIDs(platform, CL_DEVICE_TYPE_GPU, 0, NULL, &numDevices);
                //ToDO: Saved the compiled version so it can be built the next time as a binary:
                //ToDO:  Enable certain cards, fan control, scheduling

                string platformName = Cl.GetPlatformInfo(platform, PlatformInfo.Name, out error).ToString();
                Console.WriteLine("Platform: " + platformName);
                CheckErr(error, "Cl.GetPlatformInfo");

                //We will be looking only for GPU devices
                foreach (OpenCL.Net.Device device in Cl.GetDeviceIDs(platform, DeviceType.Gpu, out error))
                {
                    CheckErr(error, "Cl.GetDeviceIDs");
                    Console.WriteLine("Device: " + device.ToString());
                    string vendor = Cl.GetDeviceInfo(device, OpenCL.Net.DeviceInfo.Vendor, out error).ToString();
                    string version = Cl.GetDeviceInfo(device, OpenCL.Net.DeviceInfo.Version, out error).ToString();

                    string name = Cl.GetDeviceInfo(device, OpenCL.Net.DeviceInfo.Name, out error).ToString();

                    string avail = Cl.GetDeviceInfo(device, OpenCL.Net.DeviceInfo.Available, out error).ToString();
                    GPU g = new GPU();
                    g.avail = false;
                    if (avail == "") g.avail = true;
                    g.id = id;
                    g.name = name;
                    g.version = version;
                    g.vendor = vendor;

                    id++;

                    GPUList.Add(g);
               }
            }



            return GPUList;


           
        }






    }
















