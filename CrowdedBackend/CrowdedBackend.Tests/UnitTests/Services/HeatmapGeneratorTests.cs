using Xunit;
using SkiaSharp;

namespace CrowdedBackend.Tests.UnitTests.Services
{
    public class HeatmapGeneratorTests
    {
        /*
         * This test checks if the Generate method produces a valid Base64 string that represents an image.
         * We want to ensure that the method outputs something meaningful 
         */
        [Fact]
        public void Generate_ReturnsValidBase64String()
        {
            // Arrange
            var venueName = "testVenue";
            var people = new List<(float x, float y)> { (1, 1), (5, 5), (10, 10) };
            var raspberries = new List<(float x, float y)> { (0, 0), (6, 6) };

            // Create dummy background file
            var testImagePath = Path.Combine(Directory.GetCurrentDirectory(), "Services/HeatmapScript", venueName + ".png");
            Directory.CreateDirectory(Path.GetDirectoryName(testImagePath));
            using (var bmp = new SKBitmap(800, 800))
            using (var fs = File.OpenWrite(testImagePath))
            {
                bmp.Encode(fs, SKEncodedImageFormat.Png, 100);
            }

            // Act
            var base64 = HeatmapGenerator.Generate(venueName, raspberries, people);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(base64));
            var imageBytes = Convert.FromBase64String(base64);
            Assert.True(imageBytes.Length > 100); // Arbitrary sanity check
        }
        /*
         * This test checks if the Generate method produces a valid Base64 string that represents an image
         */
        [Fact]
        public void Compute2DKDE_ReturnsNonZeroDensity()
        {
            var points = new List<(float x, float y)> { (6, 6) };

            var density = typeof(HeatmapGenerator)
                .GetMethod("Compute2DKDE", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .Invoke(null, new object[] { points }) as double[,];

            Assert.NotNull(density);
            Assert.Contains(
                density.Cast<double>(),
                val => val > 0
            );
        }

        /*
         * This test ensures that even if the background image doesn't exist
         * (i.e., a file is missing), the Generate method still returns a valid image 
         */
        [Fact]
        public void Generate_WithMissingBackground_StillReturnsImage()
        {
            var base64 = HeatmapGenerator.Generate("nonexistentVenue", new(), new());
            Assert.False(string.IsNullOrWhiteSpace(base64));
        }
    }
}
