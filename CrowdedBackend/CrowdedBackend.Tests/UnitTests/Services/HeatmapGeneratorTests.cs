using Xunit;
using SkiaSharp;

namespace CrowdedBackend.Tests.UnitTests.Services
{
    public class HeatmapGeneratorTests
    {

        /// <summary>
        ///     This test checks if the Generate method produces a valid Base64 string that represents an image.
        ///     We want to ensure that the method outputs something meaningful 
        /// </summary>
        /// <remark>
        ///     Expected to create an image by checking if the imageBytes.lengths is over 100
        /// </remark>
        [Fact]
        public void Generate_ReturnsValidBase64String()
        {
            var venueName = "testVenue";
            var people = new List<(float x, float y)> { (1, 1), (5, 5), (10, 10) };
            var raspberries = new List<(float x, float y)> { (0, 0), (6, 6) };

            var testImagePath = Path.Combine(Directory.GetCurrentDirectory(), "Services/HeatmapScript", venueName + ".png");
            Directory.CreateDirectory(Path.GetDirectoryName(testImagePath));
            using (var bmp = new SKBitmap(800, 800))
            using (var fs = File.OpenWrite(testImagePath))
            {
                bmp.Encode(fs, SKEncodedImageFormat.Png, 100);
            }

            var base64 = HeatmapGenerator.Generate(venueName, raspberries, people);

            Assert.False(string.IsNullOrWhiteSpace(base64));
            var imageBytes = Convert.FromBase64String(base64);
            Assert.True(imageBytes.Length > 100); // Arbitrary sanity check
        }

        /// <summary>
        ///     testing to generate a valid heatmap with skiasharp
        /// </summary>
        /// <remark>
        ///     Expected to have a density over 0
        /// </remark>
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

        /// <summary>
        ///     testing to generate a heatmap can be generated even though no background image is found 
        /// </summary>
        /// <remark>
        ///     Ensures that the output is not null, empty, or whitespace
        /// </remark>
        [Fact]
        public void Generate_WithMissingBackground_StillReturnsImage()
        {
            var base64 = HeatmapGenerator.Generate("nonexistentVenue", new(), new());
            Assert.False(string.IsNullOrWhiteSpace(base64));
        }
    }
}
