using Cinema.Converters;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System;
using System.Globalization;
using System.Windows.Media;

namespace CinemaTests.Converters
{
    [TestFixture]
    public class StringToTimeFormatConverterTests
    {
        [Test]
        public void Can_Get_Time_String_From_Valid_String()
        {
            //Arrange
            var convertFrom = "12:15:00";
            string[] time = convertFrom.Split(':');

            StringToTimeFormatConverter converter = new StringToTimeFormatConverter();

            //Act
            var result = converter.Convert(convertFrom, typeof(Brush), null, CultureInfo.CurrentCulture);

            //Assert
            Assert.AreEqual(result, string.Format("Начало показа: {0}:{1}", time[0], time[1]));
        }

        [Test]
        public void Cannot_Get_String_From_Invalid_String_Value()
        {
            //Arrange
            string convertFrom = "It's no time";

            StringToTimeFormatConverter converter = new StringToTimeFormatConverter();

            //Act
            ActualValueDelegate<object> testDelegate = () => converter.Convert(convertFrom, typeof(Brush), null, CultureInfo.CurrentCulture);

            //Assert
            Assert.That(testDelegate, Throws.TypeOf<IndexOutOfRangeException>());
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(NullReferenceException))]
        public void Cannot_Get_String_From_Null_Value()
        {
            //Arrange
            StringToTimeFormatConverter converter = new StringToTimeFormatConverter();

            //Act
            ActualValueDelegate<object> testDelegate = () => converter.Convert(null, typeof(Brush), null, CultureInfo.CurrentCulture);

            //Assert
            Assert.That(testDelegate, Throws.TypeOf<NullReferenceException>());
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedException(typeof(InvalidCastException))]
        public void Cannot_Get_Brush_From_Invalid_Type_Value()
        {
            //Arrange
            var convertFrom = 123;

            BoolToBrushConverter converter = new BoolToBrushConverter();

            //Act
            ActualValueDelegate<object> testDelegate = () => converter.Convert(convertFrom, typeof(Brush), null, CultureInfo.CurrentCulture);

            //Assert
            Assert.That(testDelegate, Throws.TypeOf<InvalidCastException>());
        }
    }
}
