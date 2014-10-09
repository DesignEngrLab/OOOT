using System;
using System.Collections.Generic;
using StarMathLib;

namespace OptimizationToolbox
{
	public class GA: abstractOptMethod
	{
		public abstractGenerator neighborGenerator { get; private set; }
		/// <summary>
		/// Gets the selector.
		/// </summary>
		/// <value>The selector.</value>
		public abstractSelector selector { get; private set; }

		private readonly int popSize = 50;
		private readonly int numGenes = 2;
		private readonly double minGene = -150.0;
		private readonly double maxGene = 150.0;
		private  double mutateRate = 1.0;    
		private readonly double precision = 0.0001;             // controls mutation magnitude
		private readonly double tau = 0.40;                     // tournament selection factor
		private readonly int maxGeneration = 100;
		private static Random rnd = null;
		private readonly int randomorx=0;//0- random number


		public GA ()
		{
			RequiresObjectiveFunction = true;
			ConstraintsSolvedWithPenalties = true;
			InequalitiesConvertedToEqualities = false;
			RequiresSearchDirectionMethod = false;
			RequiresAnInitialPoint = true;
			RequiresConvergenceCriteria = true;
			RequiresFeasibleStartPoint = false;
			RequiresDiscreteSpaceDescriptor = false;
			RequiresLineSearchMethod = true;
		}

		public GA(double minG,double maxG,int no_of_var)
			: this()
		{
			this.minGene=minG;
			this.maxGene = maxG;
			this.numGenes = no_of_var;
		}

		public GA(double minG,double maxG,int no_of_var, int doInitial)
			: this()
		{
			this.minGene=minG;
			this.maxGene = maxG;
			this.numGenes = no_of_var;
			this.randomorx = doInitial; //if 1 then take x
		}

		private Individual[] Reproduce(Individual parent1, Individual parent2) // crossover and mutation
		{

			rnd=new Random(0);
			int cross = rnd.Next(0, numGenes - 1); // crossover point. 0 means 'between 0 and 1'.

			Individual child1 = new Individual(numGenes, minGene, maxGene, mutateRate, precision); // random chromosome
			Individual child2 = new Individual(numGenes, minGene, maxGene, mutateRate, precision); 

//			child1.fitness = calc_f (child1.chromosome);
//			child2.fitness = calc_f (child2.chromosome);

			for (int i = 0; i <= cross; ++i)
				child1.chromosome[i] = parent1.chromosome[i];
			for (int i = cross + 1; i < numGenes; ++i)
				child2.chromosome[i] = parent1.chromosome[i];
			for (int i = 0; i <= cross; ++i)
				child2.chromosome[i] = parent2.chromosome[i];
			for (int i = cross + 1; i < numGenes; ++i)
				child1.chromosome[i] = parent2.chromosome[i];

			child1.Mutate();
			child2.Mutate();

			//			child1.fitness = Problem.Fitness(child1.chromosome);
			//			child2.fitness = Problem.Fitness(child2.chromosome);

			child1.fitness = calc_f (child1.chromosome);
			child2.fitness = calc_f (child2.chromosome);

			Individual[] result = new Individual[2];
			result[0] = child1;
			result[1] = child2;

			return result;
		} // Reproduce



		protected override double run (out double[] xStar)
		{

			mutateRate = 1.0 / numGenes;

			Console.WriteLine("\nPopulation size = " + popSize);
			Console.WriteLine("Number genes = " + numGenes);
			Console.WriteLine("minGene value = " + minGene.ToString("F1"));
			Console.WriteLine("maxGene value = " + maxGene.ToString("F1"));
			Console.WriteLine("Mutation rate = " + mutateRate.ToString("F4"));
			Console.WriteLine("Mutation precision = " + precision.ToString("F4"));
			Console.WriteLine("Selection pressure tau = " + tau.ToString("F2"));
			Console.WriteLine("Maximum generations = " + maxGeneration);

			Evolver ev;
			if(randomorx==0)
			ev = new Evolver(popSize, numGenes, minGene, maxGene, mutateRate, precision, tau, maxGeneration); // assumes existence of a Problem.Fitness method
			else
				ev = new Evolver(popSize, numGenes, minGene, maxGene, mutateRate, precision, tau, maxGeneration,x);
			//foreach population individual, compute the fitness

			foreach (var x1 in ev.population) {

				x1.fitness = calc_f (x1.chromosome);

			}



			double bestFitness = ev.population[0].fitness;
			double[] bestChomosome = new double[numGenes];

			ev.population[0].chromosome.CopyTo(bestChomosome, 0);
			long gen = 0;
			while (gen < maxGeneration)  
			//while(notConverged (gen,numEvals,bestFitness,bestChomosome))
			{
				//Console.WriteLine ("iteration: "+gen);
				Individual[] parents = ev.Select(2);
				Individual[] children = Reproduce(parents[0], parents[1]); // crossover & mutation
				ev.Accept(children[0], children[1]);

				//ev.Immigrate();
				//these three lines below are taken from the other lines
				Individual immigrant = new Individual(numGenes, minGene, maxGene, mutateRate, precision);
				immigrant.fitness = calc_f (immigrant.chromosome);
				ev.population[popSize - 3] = immigrant; // replace third worst individual



				for (int i = popSize - 3; i < popSize; ++i)  {
					if (ev.population[i].fitness < bestFitness)  {
						bestFitness = ev.population[i].fitness;
						Console.WriteLine ("bestFitness GA: " + bestFitness);
						ev.population[i].chromosome.CopyTo(bestChomosome, 0);
					}
				}

				if (bestFitness < 0.5)
					break;
				++gen;
			}
			//double[] best = ev.Evolve();
			double[] best = bestChomosome;

			Console.WriteLine("\nBest (x,y) solution found:");
			for (int i = 0; i < best.Length; ++i)
				Console.Write(best[i].ToString("F4") + " ");

			//double fitness = Problem.Fitness(best);
			double fitness = calc_f (best);
			Console.WriteLine("\nFunction value at best solution = " + fitness.ToString("F4"));

			Console.WriteLine("\nEnd Evolutionary Optimization demo\n");
			//Console.ReadLine();

			xStar = best;
			return fitness;
		}
	}


}

