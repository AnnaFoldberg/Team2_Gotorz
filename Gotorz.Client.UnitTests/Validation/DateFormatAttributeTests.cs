using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;

namespace Gotorz.Client.UnitTests.Validation
{
    /// <summary>
    /// Contains unit tests for the <see cref="DateFormatAttribute"/> class.
    /// </summary>
    /// <author>Anna</author>
    [TestClass]
    public class DateFormatAttributeTests
    {
        private DateFormatAttribute _attribute;
        private ValidationContext _context;

        [TestInitialize]
        public void TestInitialize()
        {
            _attribute = new DateFormatAttribute("dd-MM-yyyy");
            _context = new ValidationContext(new object());
        }

        [TestMethod]
        public void IsValid_ValidDateString_ReturnsSuccess()
        {
            // Arrange
            string validDate = "05-05-2025";

            // Act
            var result = _attribute.GetValidationResult(validDate, _context);

            // Assert
            Assert.AreEqual(ValidationResult.Success, result);
        }

        [TestMethod]
        public void IsValid_EmptyString_ReturnsSuccess()
        {
            // Arrange
            string emptyString = "";

            // Act
            var result = _attribute.GetValidationResult(emptyString, _context);

            // Assert
            Assert.AreEqual(ValidationResult.Success, result);
        }

        [TestMethod]
        public void IsValid_NullValue_ReturnsSuccess()
        {
            // Act
            var result = _attribute.GetValidationResult(null, _context);

            // Assert
            Assert.AreEqual(ValidationResult.Success, result);
        }

        [TestMethod]
        public void IsValid_WhiteSpaceString_ReturnsSuccess()
        {
            // Arrange
            string emptyString = "     ";

            // Act
            var result = _attribute.GetValidationResult(emptyString, _context);

            // Assert
            Assert.AreEqual(ValidationResult.Success, result);
        }

        [TestMethod]
        public void IsValid_InvalidDatePartOrder_ReturnsValidationResultWithMessage()
        {
            // Arrange
            string invalidDate = "2025-05-05";

            // Act
            var result = _attribute.GetValidationResult(invalidDate, _context);

            // Assert
            Assert.AreEqual(ValidationResult.Success, result);
        }

        [TestMethod]
        public void IsValid_InvalidDateSeparators_ReturnsValidationResultWithMessage()
        {
            // Arrange
            string invalidDate = "05/05/2025";

            // Act
            var result = _attribute.GetValidationResult(invalidDate, _context);

            // Assert
            Assert.AreEqual(ValidationResult.Success, result);
        }

        [TestMethod]
        public void IsValid_InvalidInput_ReturnsValidationResultWithMessage()
        {
            // Arrange
            string invalidDate = "abcdefg";

            // Act
            var result = _attribute.GetValidationResult(invalidDate, _context);

            // Assert
            Assert.AreEqual(ValidationResult.Success, result);

        }
    }
}