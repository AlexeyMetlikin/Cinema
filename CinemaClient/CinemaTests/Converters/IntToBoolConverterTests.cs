using Cinema.Converters;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Globalization;

namespace CinemaTests.Converters
{
    [TestFixture]
    public class IntToBoolConverterTests
    {
        [Test]
        public void Can_Get_True_If_Value_Non_zero()
        {
            //Arrange
            var convertFrom = 1;

            IntToBoolConverter converter = new IntToBoolConverter();

            //Act
            var result = converter.Convert(convertFrom, typeof(bool), null, CultureInfo.CurrentCulture);

            //Assert
            Assert.AreEqual(result, true);
        }

        [Test]
        public void Can_Get_False_If_Value_Zero()
        {
            //Arrange
            var convertFrom = 0;

            IntToBoolConverter converter = new IntToBoolConverter();

            //Act
            var result = converter.Convert(convertFrom, typeof(bool), null, CultureInfo.CurrentCulture);

            //Assert
            Assert.AreEqual(result, false);
        }

        [Test]
        public void Cannot_Get_Bool_From_Null_Value()
        {
            //Arrange
            IntToBoolConverter converter = new IntToBoolConverter();

            //Act
            ActualValueDelegate<object> testDelegate = () => converter.Convert(null, typeof(bool), null, CultureInfo.CurrentCulture);

            //Assert
            Assert.That(testDelegate, Throws.TypeOf<NullReferenceException>());
        }

        [Test]
        public void Cannot_Get_Bool_From_Invalid_Type_Value()
        {
            //Arrange
            var convertFrom = "string";

            IntToBoolConverter converter = new IntToBoolConverter();

            //Act
            ActualValueDelegate<object> testDelegate = () => converter.Convert(convertFrom, typeof(bool), null, CultureInfo.CurrentCulture);

            //Assert
            Assert.That(testDelegate, Throws.TypeOf<InvalidCastException>());
        }
    }
}
