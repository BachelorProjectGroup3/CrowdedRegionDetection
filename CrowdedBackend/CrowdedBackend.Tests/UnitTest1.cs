namespace CrowdedBackend.Tests
{
    using Xunit;

    public class UnitTest1
    {
        [Fact] // <-- This mean it is a test method
        public void Add_TwoNumbers_ReturnsSum()
        {
            // Arrange
            int a = 2;
            int b = 3;

            // Act
            int result = a + b;

            // Assert
            Assert.Equal(5, result);
        }
    }
}