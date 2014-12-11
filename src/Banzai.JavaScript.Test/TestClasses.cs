namespace Banzai.JavaScript.Test
{

    public class TestObjectA
    {
        public string TestValueString { get; set; }

        public int TestValueInt { get; set; }

        public object TestValueObject { get; set; }
    }

    public class TestObjectASub : TestObjectA
    {
        public decimal TestValueDecimal;
    }

    public class TestObjectB
    {
        public bool TestValueBool { get; set; }

    }
}