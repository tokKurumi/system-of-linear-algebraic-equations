//testNone
//testSingle
//testInfinite
#define testInfinite

using system_of_linear_algebraic_equations.Models;

public class Program
{
	public static void Main()
	{
#if testNone
			//NO SOLUTIONS
			var systemMatrix = new Matrix<double>
			{
				{ 4, -3, 2, -1 },
				{ 3, -2, 1, -3 },
				{ 5, -3, 1, -8 }	
			};
			var freeVector = new double[] { 8, 7, 1 };
#endif
#if testSingle
			//SINGLE SOLUTION
			var systemMatrix = new Matrix<double>
			{
				{ 1, 2, 3 },
				{ 1, -1, 1 },
				{ 1, 3, -1 },
			};
			var freeVector = new double[] { 2, 0, -2 };
		
#endif
#if testInfinite
			//INFINITE SOLUTIONS
			var systemMatrix = new Matrix<double>
			{
				{ 1, 3, -2, -2 },
				{ -1, -2, 1, 2 },
				{ -2, -1, 3, 1 },
				{ -3, -2, 3, 3 }
			};
			var freeVector = new double[] { -3, 2, -2, -1 };
#endif

		var slae = new SLAE<double>(systemMatrix, freeVector);
		switch(slae.NumSolutions())
		{
			case SolutionsCount.None:
			{
				Console.WriteLine("There are no any solutions.");
				break;
			}

			case SolutionsCount.Single:
			{
				Console.WriteLine("Found single solution.");
				foreach(var solution in slae.SolveSingle())
				{
					Console.Write($"{solution}\t");
				}
				break;
			}

			case SolutionsCount.Infinity:
			{
				Console.WriteLine("Found an infinite count of solutions.");
				foreach (var solution in slae.SolveInfinite())
				{
					Console.WriteLine(solution);
				}
				break;
			}
		}
	}
}