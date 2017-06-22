using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Banzai.JavaScript.Test
{
    [TestFixture, Explicit]
    public class WhenCallingAWebService
    {
        [Test]
        public void Can_Get_Content_From_Google()
        {
            var client = new HttpClient();
            var result = client.GetAsync("http://www.google.com/").Result;
            var content = result.Content.ReadAsStringAsync().Result;

            content.Should().NotBeEmpty();
        }

        [Test]
        public async Task Basic_Web_Call_Succeeds()
        {
            string script = @"
                var client = new httpClient();
                var getResult = client.GetAsync('http://www.google.com/').Result;
                context.Subject.TestValueObject = getResult;
                context.Subject.TestValueString = getResult.Content.ReadAsStringAsync().Result;
                ";

            var testNode = new JavaScriptNode<TestObjectA>
            {
                ExecutedScript = script
            };

            testNode.AddScriptType("httpClient", typeof(HttpClient));

            var testObject = new TestObjectA();

            var context = new ExecutionContext<TestObjectA>(testObject);

            var result = await testNode.ExecuteAsync(context);

            testObject = result.GetSubjectAs<TestObjectA>();

            testObject.TestValueObject.Should().NotBeNull();

            testObject.TestValueString.Should().NotBeNull();
            testObject.TestValueString.Should().NotBeEmpty();

            result.Status.Should().Be(NodeResultStatus.Succeeded);
            testNode.Status.Should().Be(NodeRunStatus.Completed);
        } 
    }
}