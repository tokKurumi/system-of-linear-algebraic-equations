using System;
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

			extended = new Matrix<T>(systemMatrix);
			extended.AddColumn(freeVector);
		}

		public Matrix<T> SystemMatrix { get; private set; }
		public IEnumerable<T> FreeVector { get; private set; }
		private Matrix<T> extended;
		private static IEnumerable<T> DevideRow(IEnumerable<T> row, T divider)
		{
			return row.Select(elem => elem / divider).ToList();
		}

		public bool IsSolvable()
		{
			return SystemMatrix.Rank == extended.Rank;
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
			extended = extended.ToUpperTriangle();
			extended = extended.ToLowerTriangle();

			Parallel.For(0, extended.Count(), i =>
			{
				extended[i][^1] /= extended[i][i];
				extended[i][i] = T.One;
			});

			return extended.Select(x => x.Last());
		}
		public List<InfiniteSolution> SolveInfinite()
		{
			extended = extended.ToUpperTriangle();
			extended = extended.ToLowerTriangle();

			var notValueableList = extended.GetMainDiagonal().Select((elem, index) => new MatrixCell(index, elem)).Where(cell => cell.Сoefficient == T.Zero).Select(cell => cell.Index).ToArray();

			Parallel.For(0, extended.RowsCount, i =>
			{
				extended[i] = DevideRow(extended[i], extended[i, i]).ToList();
				extended[i][i] = T.One;
			});

			var result = new List<InfiniteSolution>();
			for (int i = 0; i < extended.RowsCount; ++i)
			{
				if(notValueableList.Contains(i))
				{
					continue;
				}

				var infSolution = new InfiniteSolution(i, extended[i][^1]);
				foreach (var notValueableIndex in notValueableList)
				{
					infSolution.Add(new MatrixCell(notValueableIndex, extended[i, notValueableIndex] * T.NegativeOne));
				}

				result.Add(infSolution);
			}

			return result;
		}
	}
}