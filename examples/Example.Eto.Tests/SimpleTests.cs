using Eto.Drawing;
using Eto.Forms;
using NUnit.Framework;
using Rhino.Geometry;
using Rhino.UI;

namespace Example.Tests;

// https://docs.nunit.org/articles/nunit/writing-tests/attributes/apartment.html?q=STA
[TestFixture, Apartment(ApartmentState.STA)]
public class SimpleTests : Rhino.Testing.Fixtures.RhinoTestFixture
{

    static SimpleTests()
    {
        new Application(Eto.Platform.Detect);
    }

    private static void TestForm(Form form, Action<Form> test)
    {
        form.Shown += async (s, e) =>
        {
            try
            {
                test(form);
            }
            catch (AssertionException ae)
            {

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                form.Close();
            }
        };

        form.Show();
    }

    [Test]
    public void TestAForm()
    {
        bool clicked = false;

        var button = new Button() { Text = "I'm a Test Button, and that's ok." };
        button.Click += (s, e) => { clicked = true; };

        Form form = new Form()
        {
            Size = new Size(200, 200),
            Content = button
        };

        TestForm(form, (f) =>
        {
            var button = form.FindChild<Button>();
            button.PerformClick();
            Assert.IsTrue(clicked);
        });
    }

}
