[System.AttributeUsage(
    System.AttributeTargets.Class |
    System.AttributeTargets.Method |
    System.AttributeTargets.Constructor
)]
public class IgnoreAttribute : System.Attribute
{
    public IgnoreAttribute() { }
    public IgnoreAttribute(int intParam) { }

    public int namedIntParam { get; set; }
}
namespace CatLib
{
    public class Cat
    {
        [Ignore]
        public Cat(string name) { }
        [Ignore(666)]
        public void Meow() { }
    }
}
namespace DogLib
{
    public class Dog
    {
        [Ignore(666, namedIntParam = 999)]
        public void Woof() { }
    }
}