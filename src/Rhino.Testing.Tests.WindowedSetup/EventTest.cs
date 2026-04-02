using System;
using Rhino.Testing.Fixtures;

namespace Rhino.Testing.Tests.WindowedSetup
{
    internal class EventTest : RhinoTestFixture
    {
        [Test]
        public void NewDocumentTest()
        {
            bool documentEventFired = false;

            void LocalScopeNewDocument(object? sender, DocumentEventArgs e)
            {
                documentEventFired = true;
            }

            RhinoDoc.NewDocument += LocalScopeNewDocument;
            using (var doc = RhinoDoc.Create(null))
            {
                Assert.That(documentEventFired, Is.True);
                Assert.That(doc, Is.Not.Null);
            }
            RhinoDoc.NewDocument -= LocalScopeNewDocument;
        }
    }
}
