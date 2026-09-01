using Impresiones.Domain.Exceptions;
using Impresiones.Domain.ValueObjects;

namespace Impresiones.Domain.Tests.ValueObjects;

public class PhoneNumberTests
{
    [Fact]
    public void Constructor_AcceptsExactlyNineDigits()
    {
        var phoneNumber = new PhoneNumber("123456789");

        Assert.Equal("123456789", phoneNumber.Value);
        Assert.Equal("123456789", phoneNumber.ToString());
    }

    [Fact]
    public void Constructor_PreservesLeadingZeroes()
    {
        var phoneNumber = new PhoneNumber("001234567");

        Assert.Equal("001234567", phoneNumber.Value);
    }

    [Fact]
    public void Equals_UsesValueEquality()
    {
        Assert.Equal(new PhoneNumber("123456789"), new PhoneNumber("123456789"));
        Assert.NotEqual(new PhoneNumber("123456789"), new PhoneNumber("987654321"));
    }

    [Fact]
    public void GetHashCode_ReturnsSameHashForEqualValues()
    {
        var first = new PhoneNumber("123456789");
        var second = new PhoneNumber("123456789");

        Assert.NotSame(first, second);
        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void Constructor_RejectsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new PhoneNumber(null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345678")]
    [InlineData("1234567890")]
    [InlineData("123456789012345")]
    [InlineData("12345678A")]
    [InlineData("123 56789")]
    [InlineData("+51123456")]
    [InlineData("123-56789")]
    [InlineData("(1234567)")]
    public void Constructor_RejectsInvalidValues(string value)
    {
        Assert.Throws<DomainRuleException>(() => new PhoneNumber(value));
    }

    [Fact]
    public void Constructor_RejectsNineUnicodeDigitsOutsideAsciiRange()
    {
        const string unicodeDigits = "\u0661\u0662\u0663\u0664\u0665\u0666\u0667\u0668\u0669";

        Assert.Equal(PhoneNumber.RequiredLength, unicodeDigits.Length);
        Assert.All(unicodeDigits, character => Assert.True(char.IsDigit(character)));
        Assert.DoesNotContain(unicodeDigits, character => character is >= '0' and <= '9');
        Assert.Throws<DomainRuleException>(() => new PhoneNumber(unicodeDigits));
    }
}
