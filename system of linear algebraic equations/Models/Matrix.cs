using System.Collections;
using System.Numerics;

namespace system_of_linear_algebraic_equations.Models
{
	public class Matrix<T> : IMatrix<T>
		where T : INumber<T>, ISignedNumber<T>
	{
		public List<List<T>> Data { get; set; }

		public int Rank
		{
			get
			{
				var _data = ToUpperTriangle(Data);

				int result = 0;
				foreach(var row in _data)
				{
					result += isNotNullRow(row) ? 1 : 0;
				}

				return result;
			}
		}

		public int RowsCount => Data.Count();

		public int ColumnsCount => Data[0].Count();

		public Matrix()
		{
			Data = new List<List<T>>();
		}
		public Matrix(IEnumerable<IEnumerable<T>> matrix)
		{
			ValidateInputArguments(matrix);

			Data = matrix.Select(item => item.ToList()).ToList();
		}

		private static void ValidateInputArguments(IEnumerable<IEnumerable<T>> matrix)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException("Input matrix must be not null.");
			}

			var size = matrix.ElementAt(0).Count();
			foreach (var line in matrix)
			{
				if (line.Count() != size)
				{
					throw new ArgumentException("Input matrix must be not stepped.");
				}
			}
		}
		private static bool isNotNullRow(IEnumerable<T> row)
		{
			foreach(var elem in row)
			{
				if(elem != T.Zero)
				{
					return true;
				}
			}

			return false;
		}

		private static Matrix<T> ToUpperTriangleUnchecked(IEnumerable<IEnumerable<T>> matrix)
		{
			var _data = matrix.Select(item => item.ToList()).ToList();

			for (int i = 1; i < _data.Count(); ++i)
			{
				for (int k = 0; k < i; ++k)
				{
					if (_data[k][k] != T.Zero)
					{
						T multiplier = _data.ElementAt(i).ElementAt(k) / _data.ElementAt(k).ElementAt(k);
						Parallel.For(0, _data[0].Count, iter =>
						{
							_data[i][iter] -= _data[k][iter] * multiplier;
						});
					}
				}
			}
			return new Matrix<T>(_data);
		}
		private static Matrix<T> ToLowerTriangleUnchecked(IEnumerable<IEnumerable<T>> matrix)
		{
			var _data = matrix.Select(item => item.ToList()).ToList();

			for (int i = _data.Count - 2; i >= 0; --i)
			{
				for (int k = _data.Count - 1; k > i; --k)
				{
					if (_data[k][k] != T.Zero)
					{
						T multiplier = _data[i][k] / _data[k][k];
						Parallel.For(0, _data[0].Count, iter =>
						{
							_data[i][iter] -= _data[k][iter] * multiplier;
						});
					}
				}
			}

			return new Matrix<T>(_data);
		}
		public static Matrix<T> ToUpperTriangle(IEnumerable<IEnumerable<T>> matrix)
		{
			ValidateInputArguments(matrix);

			return ToUpperTriangleUnchecked(matrix);
		}
		public static Matrix<T> ToLowerTriangle(IEnumerable<IEnumerable<T>> matrix)
		{
			ValidateInputArguments(matrix);

			return ToLowerTriangleUnchecked(matrix);
		}
		public Matrix<T> ToUpperTriangle()
		{
			return ToUpperTriangleUnchecked(Data);
		}
		public Matrix<T> ToLowerTriangle()
		{
			return ToLowerTriangleUnchecked(Data);
		}

		public void AddColumn(IEnumerable<T> column)
		{
			if (column.Count() != Data.Count())
			{
				throw new ArgumentException("The size of the input column must match the size of the matrix columns.");
			}

			for (int i = 0; i < Data.Count(); ++i)
			{
				Data[i].Add(column.ElementAt(i));
			}
		}
		public void AddRow(IEnumerable<T> row)
		{
			if (Data.Count() != 0 && row.Count() != Data[0].Count())
			{
				throw new ArgumentException("The size of the input row must match the size of the matrix rows.");
			}

			Data.Add(row.ToList());
		}
		public void AddColumns(IEnumerable<IEnumerable<T>> matrix)
		{
			ValidateInputArguments(matrix);

			if(Data.Count != matrix.Count())
			{
				throw new ArgumentException("The count of input matrix rows must match the count of current.");
			}

			for(int i = 0; i < Data.Count; ++i)
			{
				Data[i].AddRange(matrix.ElementAt(i));
			}
		}
		public void AddRows(IEnumerable<IEnumerable<T>> matrix)
		{
			ValidateInputArguments(matrix);

			if(Data[0].Count != matrix.ElementAt(0).Count())
			{
				throw new ArgumentException("The count of input matrix columns must match the count of current.");
			}

			for(int i = 0; i < matrix.Count(); ++i)
			{
				Data.Add(matrix.ElementAt(i).ToList());
			}
		}
		public void Add(params T[] row)
		{
			AddRow(row);
		}
		public void Add(params IEnumerable<T>[] rows)
		{
			foreach (var row in rows)
			{
				AddRow(row);
			}
		}

		private static IEnumerable<T> GetMainDiagonalUnchecked(IEnumerable<IEnumerable<T>> matrix)
		{
			for(int i = 0; i < matrix.Count(); ++i)
			{
				yield return matrix.ElementAt(i).ElementAt(i);
			}
		}
		public static IEnumerable<T> GetMainDiagonal(IEnumerable<IEnumerable<T>> matrix)
		{
			ValidateInputArguments(matrix);

			return GetMainDiagonalUnchecked(matrix);
		}
		public IEnumerable<T> GetMainDiagonal()
		{
			return GetMainDiagonalUnchecked(Data);
		}

		public List<T> this[int index]
		{
			get => Data[index];
			set => Data[index] = value;
		}
		public T this[int row, int column]
		{
			get => Data[row][column];
			set => Data[row][column] = value;
		}
		public IEnumerator<List<T>> GetEnumerator() => Data.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => Data.GetEnumerator();
	}
}