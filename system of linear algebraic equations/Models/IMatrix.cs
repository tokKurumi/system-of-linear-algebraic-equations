using System.Numerics;

namespace system_of_linear_algebraic_equations.Models
{
	public interface IMatrix<T> : IEnumerable<List<T>>
		where T : INumber<T>, ISignedNumber<T>
	{
		List<List<T>> Data { get; set; }
		int Rank { get; }
		List<T> this[int index] { get; set; }
	}
}
