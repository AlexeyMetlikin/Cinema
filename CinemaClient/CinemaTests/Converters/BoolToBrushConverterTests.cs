using Cinema.Converters;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Globalization;
using System.Windows.Media;

namespace CinemaTests.Converters
{
    [TestFixture]
    public class BoolToBrushConverterTests
    {
        [Test]
        public void Can_Get_Green_Brush_From_True_Value()
        {
            //Arrange
            var convertFrom = true;

            BoolToBrushConverter converter = new BoolToBrushConverter();

            //Act
            var result = converter.Convert(convertFrom, typeof(Brush), null, CultureInfo.CurrentCulture);

            //Assert
            Assert.AreEqual(result, Brushes.GreenYellow);
        }

        [Test]
        public void Can_Get_White_Brush_From_True_Value()
        {
            //Arrange
            var convertFrom = false;

            BoolToBrushConverter converter = new BoolToBrushConverter();

            //Act
            var result = converter.Convert(convertFrom, typeof(Brush), null, CultureInfo.CurrentCulture);

            //Assert
            Assert.AreEqual(result, Brushes.White);
        }

        [Test]
        public void Cannot_Get_Brush_From_Null_Value()
        {
            //Arrange
            BoolToBrushConverter converter = new BoolToBrushConverter();

            //Act
            ActualValueDelegate<object> testDelegate = () => converter.Convert(null, typeof(Brush), null, CultureInfo.CurrentCulture);

            //Assert
            Assert.That(testDelegate, Throws.TypeOf<NullReferenceException>());
        }

        [Test]
        public void Cannot_Get_Brush_From_Invalid_Type_Value()
        {
            //Arrange
            var convertFrom = "string";

            BoolToBrushConverter converter = new BoolToBrushConverter();

            //Act
            ActualValueDelegate<object> testDelegate = () => converter.Convert(convertFrom, typeof(Brush), null, CultureInfo.CurrentCulture);

            //Assert
            Assert.That(testDelegate, Throws.TypeOf<InvalidCastException>());
        }
    }
}
