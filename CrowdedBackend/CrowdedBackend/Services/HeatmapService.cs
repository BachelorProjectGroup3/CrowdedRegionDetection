namespace CrowdedBackend.Services;

using Python.Runtime;

public class HeatmapService
{
    public static String runHeatmapScript()
    {
        Runtime.PythonDLL = @"HeatmapScript\Python\Python311\python311.dll";
        PythonEngine.Initialize();
        using (Py.GIL()) // Only single thread the interpreter
        {
            var heatmapScript = Py.Import("heatmap");
            var originalRaspPositions = new List<List<int>>();
            originalRaspPositions.Add(new List<int> {8, 2});
            originalRaspPositions.Add(new List<int> {6, 5});
            originalRaspPositions.Add(new List<int> {10, 10});
            PyObject[] raspPositions = [];

            foreach (var position in originalRaspPositions)
            {
                PyObject raspPosition = position.ToPython();
                raspPositions.Append(raspPosition);
            }
            
            PyObject[] peoplePositions = [];

            // var result = heatmapScript.InvokeMethod("generateHeatmap", new PyObject[] { raspPositions, peoplePositions });
        }
        return "I have been called";
    }
}