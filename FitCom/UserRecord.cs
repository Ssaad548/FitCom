using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCom
{
    public class UserRecord
    {
        public string Name { get; set; }
        public string Goal { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public double BMIResult { get; set; }
        public string BMIStatus { get; set; }
        public List<string> ExerciseIds { get; set; } // Add a list to store exercise IDs
    }
}
