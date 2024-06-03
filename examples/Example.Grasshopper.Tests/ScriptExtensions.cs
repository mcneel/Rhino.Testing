using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using NUnit.Framework;
using Rhino.Commands;

namespace Example.Tests
{
    internal static class ScriptExtensions
    {

        internal static GH_Document LoadDocument(string path)
        {
            var docs = Grasshopper.Instances.DocumentServer;
            var doc = docs.AddDocument(path, true);

            Assert.That(doc, Is.Not.Null, $"Document {path} not found");

            return doc;
        }

        internal static void Solve(GH_Document doc)
        {
            doc.NewSolution(false);
        }

        internal static void SetValueOfSlider(GH_Document doc, string name, decimal value)
        {
            var sliders = doc.ActiveObjects().OfType<GH_NumberSlider>();
            var slider = sliders.FirstOrDefault(s => s.NickName == name);
            Assert.That(slider, Is.Not.Null, $"Slider with name {name} not found");

            Assert.True(slider.TrySetSliderValue(value));
            slider.CollectData();
        }

        internal static bool GetAssertion(GH_Document doc)
        {
            var params_ = doc.ActiveObjects().OfType<GH_Param<GH_Boolean>>().Where(ao => ao.NickName.ToUpperInvariant().Equals("AT"));

            bool result = true;

            foreach (var param in params_)
            {
                var results = param.VolatileData.AllData(true).Cast<bool>();
                result &= results.All(t => t);
            }

            return result;
        }

    }
}