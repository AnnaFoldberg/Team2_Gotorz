using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;

namespace Gotorz.Client.UnitTests.Validation
{
    [TestClass]
    public class DateFormatAttributeTests
    {
        private DateFormatAttribute _attribute;
        private ValidationContext _context;

        [TestInitialize]
        public void TestInitialize()
        {
            _attribute = new DateFormatAttribute("yyyy-MM-dd");
            _context = new ValidationContext(new object());
        }

        [TestMethod]
        public void IsValid_ValidDateString_ReturnsSuccess()
        {

        }

        [TestMethod]
        public void IsValid_EmptyString_ReturnsSuccess()
        {

        }

        [TestMethod]
        public void IsValid_NullValue_ReturnsSuccess()
        {

        }

        [TestMethod]
        public void IsValid_WhiteSpaceString_ReturnsSuccess()
        {

        }

        [TestMethod]
        public void IsValid_InvalidDatePartOrder_ReturnsValidationResultWithMessage()
        {
            
        }

        [TestMethod]
        public void IsValid_InvalidDateSeparators_ReturnsValidationResultWithMessage()
        {
            
        }

        [TestMethod]
        public void IsValid_InvalidString_ReturnsValidationResultWithMessage()
        {
            
        }
    }
}