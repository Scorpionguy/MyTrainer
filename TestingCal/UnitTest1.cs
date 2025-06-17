using Xunit;

namespace TestingCal
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void CalculateCalories_ValidInput_ReturnsCorrectResult()
		{
			// Arrange
			var service = new WorkoutService();
			int reps = 10;
			double weight = 50;

			// Act
			var result = service.CalculateCalories(reps, weight);

			// Assert
			Assert.AreEqual(50.0, result);
		}
	}
	public class WorkoutService
	{
		public double CalculateCalories(int reps, double weight)
		{
			return reps * weight * 0.1;
		}
	}
}
