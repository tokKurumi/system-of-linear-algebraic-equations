using System.Collections;
using System.Numerics;
using System.Text;

namespace system_of_linear_algebraic_equations.Models
{
	public enum SolutionsCount : ushort
	{
		None,
		Single,
		Infinity
	}

	public class SLAE<T>
		where T : INumber<T>, ISignedNumber<T>
	{
		public struct MatrixCell
		{
			public int Index;
			public T Сoefficient;

			public override string ToString()
			{
				if(Сoefficient != T.Zero)
				{
					return $"{(T.IsPositive(Сoefficient) ? "+" : "-")} {T.Abs(Сoefficient)}*x{Index} ";
				}
				return "";
			}

			public MatrixCell(int index, T сoefficient)
			{
				Index = index;
				Сoefficient = сoefficient;
			}
		}
		public class InfiniteSolution : IEnumerable<MatrixCell>
		{
			public InfiniteSolution(int index, T freeValue)
			{
				FreeValue = freeValue;
				Index = index;
				Cells = new List<MatrixCell>();
			}

			public InfiniteSolution(List<SLAE<T>.MatrixCell> cells, T freeValue)
			{
				Cells = cells;
				FreeValue = freeValue;
			}

			public void Add(MatrixCell cell)
			{
				Cells.Add(cell);
			}

			public List<MatrixCell> Cells { get; private set; }
			public T FreeValue { get; private set; }
			public int Index { get; private set; }

			public override string ToString()
			{
				var sb = new StringBuilder($"x{Index} = ");
				foreach(var cell in Cells)
				{
					sb.Append(cell);
				}

				return sb.Append($"{(T.IsPositive(FreeValue) ? "+" : "-")} {T.Abs(FreeValue)}").ToString();
			}

			public IEnumerator GetEnumerator() => Cells.GetEnumerator();
			IEnumerator<SLAE<T>.MatrixCell> IEnumerable<SLAE<T>.MatrixCell>.GetEnumerator() => Cells.GetEnumerator();
		}

		public SLAE(Matrix<T> systemMatrix, IEnumerable<T> freeVector)
		{
			SystemMatrix = systemMatrix;
			FreeVector = freeVector;

			Extended = new Matrix<T>(systemMatrix);
			Extended.AddColumn(freeVector);
		}

		public Matrix<T> SystemMatrix { get; private set; }
		public IEnumerable<T> FreeVector { get; private set; }
		public Matrix<T> Extended;
		private static IEnumerable<T> DevideRow(IEnumerable<T> row, T divider)
		{
			return row.Select(elem => elem / divider).ToList();
		}

		public bool IsSolvable()
		{
			return SystemMatrix.Rank == Extended.Rank;
		}
		public SolutionsCount NumSolutions()
		{
			if(!IsSolvable())
			{
				return SolutionsCount.None;
			}

			if(SystemMatrix.Rank == SystemMatrix.ColumnsCount)
			{
				return SolutionsCount.Single;
			}

			return SolutionsCount.Infinity;
		}

		public IEnumerable<T> SolveSingle()
		{
			Extended = Extended.ToUpperTriangle();
			Extended = Extended.ToLowerTriangle();

			Parallel.For(0, Extended.Count(), i =>
			{
				Extended[i][^1] /= Extended[i][i];
				Extended[i][i] = T.One;
			});

			return Extended.Select(x => x.Last());
		}
		public IEnumerable<InfiniteSolution> SolveInfinite()
		{
			Extended = Extended.ToUpperTriangle();
			Extended = Extended.ToLowerTriangle();

			var notValueableList =
				Enumerable.Range(0, Extended.ColumnsCount - 1)
				.Except(Extended.GetMainDiagonal()
				.Select((elem, index) => new MatrixCell(index, elem))
				.Where(cell => cell.Сoefficient != T.Zero)
				.Select(cell => cell.Index)).ToArray();

			Parallel.For(0, Extended.RowsCount, i =>
			{
				Extended[i] = DevideRow(Extended[i], Extended[i, i]).ToList();
				Extended[i, i] = T.One;
			});

			var result = new List<InfiniteSolution>();
			for (int i = 0; i < Extended.RowsCount; ++i)
			{
				if(notValueableList.Contains(i))
				{
					continue;
				}

				var infSolution = new InfiniteSolution(i, Extended[i][^1]);
				var test = notValueableList.GetEnumerator();

				foreach (var notValueableIndex in notValueableList)
				{
					infSolution.Add(new MatrixCell(notValueableIndex, Extended[i, notValueableIndex] * T.NegativeOne));
				}

				result.Add(infSolution);
			}

			return result;
		}
	}

	public class Seidel<T>
		where T : INumber<T>, ISignedNumber<T>
	{
		public Seidel(Matrix<T> systemMatrix, IEnumerable<T> freeVector)
		{
			SystemMatrix = systemMatrix;
			FreeVector = freeVector;
			Approximation = FreeVector.Zip(SystemMatrix.GetMainDiagonal()).Select(elem => elem.First / elem.Second).ToList();
		}

		public Matrix<T> SystemMatrix { get; private set; }
		public IEnumerable<T> FreeVector { get; private set; }
		public List<T> Approximation { get; private set; }

		private T RowSumWithoutCur(int indexOfRow)
		{
			return SystemMatrix[indexOfRow]
				.Zip(Approximation)
				.Select(elem => elem.First * elem.Second)
				.Where((elemValue, elemIndex) => elemIndex != indexOfRow)
				.Aggregate(T.Zero, (accumulated, current) => accumulated + current);
		}
		public IEnumerable<T> Solve(T eps)
		{
			T currentEps = T.Zero;
			do
			{
				var oldValue = new List<T>(Approximation);

				for (int i = 0; i < Approximation.Count; ++i)
				{
					Approximation[i] = (FreeVector.ElementAt(i) - RowSumWithoutCur(i)) / SystemMatrix[i, i];
				}

				currentEps = oldValue.Zip(Approximation).Select(elem => (elem.Second - elem.First) / elem.Second).Max()!;
			} while (currentEps > eps);

			return Approximation;
		}
	}
}