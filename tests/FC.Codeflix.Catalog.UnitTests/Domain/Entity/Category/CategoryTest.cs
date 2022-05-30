using System;
using System.Collections.Generic;
using System.Linq;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity; // Alias

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Category;

[Collection(nameof(CategoryTestFixture))]
public class CategoryTest
{

    private readonly CategoryTestFixture _categoryTestFixture;

    public CategoryTest(CategoryTestFixture categoryTestFixture)
        => _categoryTestFixture = categoryTestFixture;
    

    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "Category - Aggregates")]
    public void Instantiate()
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();
        var datetimeBefore = DateTime.Now;
        
        // Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description);
        var datetimeAfter = DateTime.Now.AddSeconds(1);

        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        (category.CreatedAt >= datetimeBefore).Should().BeTrue();
        (category.CreatedAt <= datetimeAfter).Should().BeTrue();
        (category.IsActive).Should().BeTrue();
    }
    
    // Passo valores para o teste
    [Theory(DisplayName = nameof(InstantiateWithIsActive))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData(true)] // São os dados para o teste
    [InlineData(false)]
    public void InstantiateWithIsActive(bool isActive)
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();
        var datetimeBefore = DateTime.Now;
        
        // Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, isActive);
        var datetimeAfter = DateTime.Now.AddSeconds(1);

        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be(validCategory.Name);
        category.Description.Should().Be(validCategory.Description);
        category.Id.Should().NotBeEmpty();
        category.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
        (category.CreatedAt >= datetimeBefore).Should().BeTrue();
        (category.CreatedAt <= datetimeAfter).Should().BeTrue();
        (category.IsActive).Should().Be(isActive);
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void InstantiateErrorWhenNameIsEmpty(string? name)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        
        Action action = () => new DomainEntity.Category(name!, validCategory.Name);
        
        action
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }
    
    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsNull))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsNull()
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        
        Action action = () => new DomainEntity.Category(validCategory.Name, null!);
        
        action
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should not be null");
    }
    
    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [MemberData(nameof(GetNamesWithLessThan3Characters), parameters: 10)]
    public void InstantiateErrorWhenNameIsLessThan3Characters(string invalidName)
    {
        var validCategory = _categoryTestFixture.GetValidCategory();
        Action action = () => new DomainEntity.Category(invalidName, validCategory.Description);
        
        action
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be at least 3 characters long");
    }

    public static IEnumerable<object[]> GetNamesWithLessThan3Characters(int numberOfTests = 6)
    {
        // Crio a fixture porque o método é static
        var fixture = new CategoryTestFixture();

        for (int i = 0; i < numberOfTests; i++)
        {
            var isOdd = i % 2 == 1;
            // Com o yeld ele já gera o valor e retorna. Com isso eu não preciso ter uma lista, pois o meu 
            // retorno é um IEumerable<object[]>
            yield return new object[]
            {
                // se for impar, retorna 1 caractere e se for par, retorna 2 caracteres
                fixture.GetValidCategoryName().Substring(0, isOdd ? 1 : 2)
            };
        }
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThan255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenNameIsGreaterThan255Characters()
    {
        var invalidName = String.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());
        var validCategory = _categoryTestFixture.GetValidCategory();
        
        Action action = () => new DomainEntity.Category(invalidName, validCategory.Description);
        
        action
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal 255 characters long");

    }
    
    [Fact(DisplayName = nameof(InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void InstantiateErrorWhenDescriptionIsGreaterThan10_000Characters()
    {
        var invalidDescription = String.Join(null, Enumerable.Range(1, 10001).Select(_ => "a").ToArray());
        var validCategory = _categoryTestFixture.GetValidCategory();
        
        Action action = () => new DomainEntity.Category(validCategory.Name, invalidDescription);
        
        action
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal 10000 characters long");

    }
    
    [Fact(DisplayName = nameof(ActivateCategory))]
    [Trait("Domain", "Category - Aggregates")]
    public void ActivateCategory()
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();

        // Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description, false);
        category.Activate();
        
        // Assert
        category.IsActive.Should().BeTrue();
    }
    
    [Fact(DisplayName = nameof(DeactivateCategory))]
    [Trait("Domain", "Category - Aggregates")]
    public void DeactivateCategory()
    {
        // Arrange
        var validCategory = _categoryTestFixture.GetValidCategory();

        // Act
        var category = new DomainEntity.Category(validCategory.Name, validCategory.Description);
        category.Deactivate();
        
        // Assert
        category.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = nameof(UpdateCategory))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateCategory()
    {
        var category = _categoryTestFixture.GetValidCategory();
        var categoryWithNewValues = _categoryTestFixture.GetValidCategory();

        category.Update(categoryWithNewValues.Name, categoryWithNewValues.Description);
        
        category.Name.Should().Be(categoryWithNewValues.Name);
        category.Description.Should().Be(categoryWithNewValues.Description);
    }
    
    [Fact(DisplayName = nameof(UpdateCategoryOnlyName))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateCategoryOnlyName()
    {
        var category = _categoryTestFixture.GetValidCategory();
        var newName = _categoryTestFixture.GetValidCategoryName();
        var currentDescription = category.Description;

        category.Update(newName);
        
        category.Name.Should().Be(newName);
        category.Description.Should().Be(currentDescription);
    }
    
    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void UpdateErrorWhenNameIsEmpty(string? name)
    {
        var category = _categoryTestFixture.GetValidCategory();
        Action action = () => category.Update(name!);
        
        action
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }
    
    [Theory(DisplayName = nameof(UpdateErrorWhenNameIsLessThan3Characters))]
    [Trait("Domain", "Category - Aggregates")]
    [InlineData("a")]
    [InlineData("ab")]
    [InlineData("1")]
    [InlineData("12")]
    public void UpdateErrorWhenNameIsLessThan3Characters(string invalidName)
    {
        var category = _categoryTestFixture.GetValidCategory();
        Action action = () => category.Update(invalidName);
        
        action
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be at least 3 characters long");
    }
    
    [Fact(DisplayName = nameof(UpdateErrorWhenNameIsGreaterThan255Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenNameIsGreaterThan255Characters()
    {
        var category = _categoryTestFixture.GetValidCategory();
        var invalidName = _categoryTestFixture.Faker.Lorem.Letter(256); //String.Join(null, Enumerable.Range(1, 256).Select(_ => "a").ToArray());
        
        Action action = () => category.Update(invalidName);
        
        action
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be less or equal 255 characters long");
    }
    
    [Fact(DisplayName = nameof(UpdateErrorWhenDescriptionIsGreaterThan10_000Characters))]
    [Trait("Domain", "Category - Aggregates")]
    public void UpdateErrorWhenDescriptionIsGreaterThan10_000Characters()
    {
        var category = _categoryTestFixture.GetValidCategory();
        var invalidDescription = _categoryTestFixture.Faker.Commerce.ProductDescription(); //String.Join(null, Enumerable.Range(1, 10001).Select(_ => "a").ToArray());

        while (invalidDescription.Length <= 10000)
            invalidDescription = $"{invalidDescription} {_categoryTestFixture.Faker.Commerce.ProductDescription()}";
        
        
        Action action = () => category.Update("Category New Name", invalidDescription);
        
        action
            .Should()
            .Throw<EntityValidationException>()
            .WithMessage("Description should be less or equal 10000 characters long");
    }
    
}