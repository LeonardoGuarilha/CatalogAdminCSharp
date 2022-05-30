using System;
using System.Collections.Generic;
using Bogus;
using FC.Codeflix.Catalog.Domain.Exceptions;
using Xunit;
using FC.Codeflix.Catalog.Domain.Validation;
using FluentAssertions;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Validation;

public class DomainValidationTest
{
    private Faker Faker { get; set; } = new Faker();
    
    [Fact(DisplayName = nameof(NotNullOk))]
    [Trait("Domain", "DomainValidation - Validations")]
    public void NotNullOk()
    {
        var value = Faker.Commerce.ProductName();
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        
        Action action = () => DomainValidation.NotNull(value, fieldName);

        action.Should().NotThrow();
    }
    
    [Fact(DisplayName = nameof(NotNullThrowWhenNull))]
    [Trait("Domain", "DomainValidation - Validations")]
    public void NotNullThrowWhenNull()
    {
        string? value = null;
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");

        Action action = () => DomainValidation.NotNull(value, fieldName);

        action
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should not be null");
    }

    [Theory(DisplayName = nameof(NotNullOrEmptyThrowWhenEmpty))]
    [Trait("Domain", "DomainValidation - Validations")]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void NotNullOrEmptyThrowWhenEmpty(string? target)
    {
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        
        Action action = () => DomainValidation.NotNullOrEmpty(target, fieldName);
        
        action
            .Should()
            .Throw<EntityValidationException>($"{fieldName} should not be empty or null");
    }
    
    [Fact(DisplayName = nameof(NotNullOrEmptyOk))]
    [Trait("Domain", "DomainValidation - Validations")]
    public void NotNullOrEmptyOk()
    {
        var target = Faker.Commerce.ProductName();
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        
        Action action = () => DomainValidation.NotNullOrEmpty(target, fieldName);

        action
            .Should()
            .NotThrow();
    }

    [Theory(DisplayName = nameof(MinLenghtThrowWhenLess))]
    [Trait("Domain", "DomainValidation - Validations")]
    [MemberData(nameof(GetValuesSmallerThenMin), parameters: 10)]
    public void MinLenghtThrowWhenLess(string targer, int minLenght)
    {
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        
        Action action = () => DomainValidation.MinLenght(targer, minLenght, fieldName);

        action
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should be at least {minLenght} characters long");
    }

    public static IEnumerable<object[]> GetValuesSmallerThenMin(int numberOfTests = 5)
    {
                                    // Target, Value
        yield return new object[] { "123456", 10 };
        
        var faker = new Faker();
        
        for (int i = 0; i < (numberOfTests - 1); i++)
        {
            var example = faker.Commerce.ProductName();
            var minLenght = example.Length + (new Random().Next(1, 20));
            yield return new object[] { example, minLenght };
        }
    }
    
    [Theory(DisplayName = nameof(MinLenghtOk))]
    [Trait("Domain", "DomainValidation - Validations")]
    [MemberData(nameof(GetValuesGreaterThenMin), parameters: 10)]
    public void MinLenghtOk(string targer, int minLenght)
    {
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        Action action = () => DomainValidation.MinLenght(targer, minLenght, fieldName);

        action
            .Should()
            .NotThrow();
    }
    public static IEnumerable<object[]> GetValuesGreaterThenMin(int numberOfTests = 5)
    {
        yield return new object[] { "123456", 6 };
        
        var faker = new Faker();
        
        for (int i = 0; i < (numberOfTests - 1); i++)
        {
            var example = faker.Commerce.ProductName();
            var minLenght = example.Length - (new Random().Next(1, 5));
            yield return new object[] { example, minLenght };
        }
    }

    [Theory(DisplayName = nameof(MaxLenghtThrowWhenGreater))]
    [Trait("Domain", "DomainValidation - Validations")]
    [MemberData(nameof(GetValuesGreaterThenMax), parameters: 10)]
    public void MaxLenghtThrowWhenGreater(string targer, int maxLenght)
    {
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        
        Action action = () => DomainValidation.MaxLenght(targer, maxLenght, fieldName);

        action
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage($"{fieldName} should be less or equal {maxLenght} characters long");  
    }
    
    public static IEnumerable<object[]> GetValuesGreaterThenMax(int numberOfTests = 5)
    {
        yield return new object[] { "123456", 5 };
        
        var faker = new Faker();
        
        for (int i = 0; i < (numberOfTests - 1); i++)
        {
            var example = faker.Commerce.ProductName();
            var maxLenght = example.Length - (new Random().Next(1, 5));
            yield return new object[] { example, maxLenght };
        }
    }
    
    [Theory(DisplayName = nameof(MaxLenghtOk))]
    [Trait("Domain", "DomainValidation - Validations")]
    [MemberData(nameof(GetValuesLessThenMax), parameters: 10)]
    public void MaxLenghtOk(string targer, int maxLenght)
    {
        var fieldName = Faker.Commerce.ProductName().Replace(" ", "");
        
        Action action = () => DomainValidation.MaxLenght(targer, maxLenght, fieldName);

        action
            .Should()
            .NotThrow();  
    }
    
    public static IEnumerable<object[]> GetValuesLessThenMax(int numberOfTests = 5)
    {
        yield return new object[] { "123456", 6 };
        
        var faker = new Faker();
        
        for (int i = 0; i < (numberOfTests - 1); i++)
        {
            var example = faker.Commerce.ProductName();
            var maxLenght = example.Length + (new Random().Next(0, 5));
            yield return new object[] { example, maxLenght };
        }
    }
    
}