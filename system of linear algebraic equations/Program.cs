//slae
//testNone
//testSingle
//testInfinite1
//testInfinite2
//variant11

//saidel
//slae

#define slae
#define testInfinite2

using system_of_linear_algebraic_equations.Models;

public class Program
{
#if slae
	public static void Main()
	{
		Console.ForegroundColor = ConsoleColor.Cyan;
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
#if testInfinite1
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
#if testInfinite2
	//INFINITE SOLUTIONS
	var systemMatrix = new Matrix<double>
		{
			{ 1, 3, -2, -2 }
		};
		var freeVector = new double[] { -3 };
#endif
#if variant11
		//VARIANT 11
		var systemMatrix = new Matrix<double>
		{
			{ 1, -2, 16, 0 },
			{ 10, -1, 0, 1 },
			{ 0, 12, 1, -1 },
			{ 0, 2, 0, 16 }
		};
		var freeVector = new double[] { 31, 0, -28, 29 };
#endif

		var slae = new SLAE<double>(systemMatrix, freeVector);
		foreach(var row in slae.Extended)
		{
			foreach(var elem in row)
			{
				Console.Write($"{elem}\t");
			}
			Console.WriteLine();
		}

		Console.ForegroundColor = ConsoleColor.Magenta;
		switch (slae.NumSolutions())
		{
			case SolutionsCount.None:
			{
				Console.WriteLine("There are no any solutions.");
				break;
			}

			case SolutionsCount.Single:
			{
				Console.WriteLine("Found single solution.");

				Console.ForegroundColor = ConsoleColor.Yellow;
				foreach(var solution in slae.SolveSingle())
				{
					Console.Write($"{solution}\t");
				}
				break;
			}

			case SolutionsCount.Infinity:
			{
				Console.WriteLine("Found an infinite count of solutions.");

				Console.ForegroundColor = ConsoleColor.Yellow;
				foreach (var solution in slae.SolveInfinite())
				{
					Console.WriteLine(solution);
				}
				break;
			}
		}

		Console.ForegroundColor = ConsoleColor.White;
		Console.WriteLine();
	}
#endif
#if seidel
	public static void Main()
	{
#if variant11
		//VARIANT 11
		var systemMatrix = new Matrix<double>
		{
			{ 1, -2, 16, 0 },
			{ 10, -1, 0, 1 },
			{ 0, 12, 1, -1 },
			{ 0, 2, 0, 16 }
		};
		var freeVector = new double[] { 31, 0, -28, 29 };
#endif

		var seidel = new Seidel<double>(systemMatrix, freeVector);
		foreach(var item in seidel.Solve)
		{
			Console.WriteLine(item);
		}
	}
#endif
}