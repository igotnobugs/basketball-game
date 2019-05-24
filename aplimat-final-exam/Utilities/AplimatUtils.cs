using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aplimat_final_exam.Models;
using System.Windows.Input;

namespace aplimat_final_exam.Utilities
{
    public class AplimatUtils : CubeMesh
    {
        public static float Constrain(float val, float min, float max)
        {
            if (val <= min) return min;
            else if (val >= max) return max;
            else return val;
        }
    }
}
