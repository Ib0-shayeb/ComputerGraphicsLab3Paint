using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ComputerGraphicsLab3Paint
{
    public static class CanvasStorage
    {
        public static void Save(CanvasData canvas, string filePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true // needed if you have public fields instead of properties
            };
            var json = JsonSerializer.Serialize(canvas, options);
            File.WriteAllText(filePath, json);
        }

        public static CanvasData Load(string filePath)
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true
            };
            var json = File.ReadAllText(filePath);
            var canvasData = JsonSerializer.Deserialize<CanvasData>(json, options);
            return canvasData;
        }
    }
}