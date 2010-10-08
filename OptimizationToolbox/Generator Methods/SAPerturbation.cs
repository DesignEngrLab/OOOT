using System;

using System.Collections.Generic;

namespace OptimizationToolbox
{
    public class SAPerturbation : abstractGenerator
    {
        public SAPerturbation(DesignSpaceDescription discreteSpaceDescriptor)
            : base(discreteSpaceDescriptor)
        {
        }

        public override void generateCandidates(ref List<KeyValuePair<double, double[]>> candidates, int number = -1)
        {
            throw new NotImplementedException();
        }
    }
}


// somehow want to get Hustin moveset moved about