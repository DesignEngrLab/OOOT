﻿/*************************************************************************
 *     This file & class is part of the Object-Oriented Optimization
 *     Toolbox (or OOOT) Project
 *     Copyright 2010 Matthew Ira Campbell, PhD.
 *
 *     OOOT is free software: you can redistribute it and/or modify
 *     it under the terms of the MIT X11 License as published by
 *     the Free Software Foundation, either version 3 of the License, or
 *     (at your option) any later version.
 *  
 *     OOOT is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     MIT X11 License for more details.
 *  


 *     
 *     Please find further details and contact information on OOOT
 *     at http://designengrlab.github.io/OOOT/.
 *************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using StarMathLib;

namespace OptimizationToolbox
{
    public class SimulatedAnnealing : abstractOptMethod
    {
        public abstractSimulatedAnnealingCoolingSchedule scheduler { get; set; }
        public abstractGenerator neighborGenerator { get; set; }
        public abstractSelector selector { get; set; }

        #region Constructor

        public SimulatedAnnealing(optimize direction, Boolean ConstraintsSolvedWithPenalties = true)
        {
            RequiresObjectiveFunction = true;
            this.ConstraintsSolvedWithPenalties = ConstraintsSolvedWithPenalties;
            InequalitiesConvertedToEqualities = false;
            RequiresSearchDirectionMethod = false;
            RequiresLineSearchMethod = false;
            RequiresAnInitialPoint = true;
            RequiresConvergenceCriteria = true;
            RequiresFeasibleStartPoint = true;
            RequiresDiscreteSpaceDescriptor = true;

            selector = new MetropolisCriteria(direction);
        }

        #endregion

        public override void Add(object function)
        {
            if (function is abstractSimulatedAnnealingCoolingSchedule)
            {
                scheduler = (abstractSimulatedAnnealingCoolingSchedule)function;
                scheduler.SetOptimizationDetails(this);
            }
            else if (function is abstractGenerator)
                neighborGenerator = (abstractGenerator)function;
            else base.Add(function);
        }

        protected override double run(out double[] xStar)
        {
            var candidates = new List<ICandidate> { new Candidate(calc_f(x), x) };
            var temperature = scheduler.SetInitialTemperature();
            while (notConverged(k++, numEvals, candidates[0].objectives[0], candidates[0].x))
            {
                SearchIO.output(k + ": f = " + candidates[0].objectives[0], 4);
                SearchIO.output("     x = " + candidates[0].x.MakePrintString(), 4);
                var neighbors = neighborGenerator.GenerateCandidates(candidates[0].x);
                var feasibleAndEvaluatedNeighbors =
                    from neighbor in neighbors
                    where ConstraintsSolvedWithPenalties || feasible(neighbor)
                    let f = calc_f(neighbor)
                    select new Candidate(f, neighbor);
                candidates.AddRange(feasibleAndEvaluatedNeighbors.Cast<ICandidate>());

                temperature = scheduler.UpdateTemperature(temperature, candidates);
                SearchIO.output("temperture = " + temperature,3);
                selector.SelectCandidates(ref candidates, control: temperature);
            }
            xStar = candidates[0].x;
            return candidates[0].objectives[0];
        }
    }
}