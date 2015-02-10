using System.Net.Http;
using NUnit.Framework;
using Should;

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

            content.ShouldNotBeEmpty();
        }

        [Test]
        public async void Basic_Web_Call_Succeeds()
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

            testObject.TestValueObject.ShouldNotBeNull();

            testObject.TestValueString.ShouldNotBeNull();
            testObject.TestValueString.ShouldNotBeEmpty();

            result.Status.ShouldEqual(NodeResultStatus.Succeeded);
            testNode.Status.ShouldEqual(NodeRunStatus.Completed);
        } 
    }
}