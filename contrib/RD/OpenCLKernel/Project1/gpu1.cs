using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace GPUEnumerator

{
        
    
public struct GPU
{
 public    string vendor;
   public string version;
    public string name;
    public bool avail;
    public int id;
    

}




   public class Enumeration
   {
       

       public List<GPU> Enumerate() {
            EnumerateGPUs e = new EnumerateGPUs();
                  List<GPU> g = e.SuckInGPUs();
           return g;

       }

   }


    
    }


