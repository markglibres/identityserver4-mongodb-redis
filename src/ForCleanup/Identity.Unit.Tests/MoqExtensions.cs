using AutoFixture;
using AutoFixture.Kernel;
using Moq;

namespace Identity.Unit.Tests
{
    public static class MoqExtensions
    {
        public static Mock<T> FreezeMoq<T>(this IFixture fixture)
            where T : class
        {
            var td = new Mock<T>();
            fixture.Inject(td.Object);
            return td;
        }
        
        public static T TryCreate<T>( this IFixture fixture)
        {
            try
            {
                var result = (T)new SpecimenContext( fixture ).Resolve( typeof(T) );
                return result;
            }
            catch ( ObjectCreationException )
            {
                return default;
            }
        }
    }
}