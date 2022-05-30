using System.Collections.Generic;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.CreateCategory;

public class CreateCategoryTestDataGenerator
{
    public static IEnumerable<object[]> GetInvalidInputs(int times = 12)
    {
        var invalidInputsList = new List<object[]>();
        var fixture = new CreateCategoryTestFixture();
        var totalInvalidCases = 4;

        for (int i = 0; i < times; i++)
        {
            switch (i % totalInvalidCases)
            {
                case 0:
                    // nome não pode ser menor que 3 caracteres
                    invalidInputsList.Add(new object[]
                    {
                        fixture.GetInvalidInputShortName(), // Esse é o input do método ThrowWhenCantInstantiateAggregate
                        "Name should be at least 3 characters long" // Esse é a exceptionMessage do método ThrowWhenCantInstantiateAggregate
                    }); 
                    break;
                
                case 1:
                    // nome não pode ser maior que 255 caracteres
                    invalidInputsList.Add(new object[]
                    {
                        fixture.GetInvalidInputTooLongName(),
                        "Name should be less or equal 255 characters long"
                    });
                    break;
                
                case 2:
                    // descrição pode ser null                  
                    invalidInputsList.Add(new object[]
                    {
                        fixture.GetInvalidInputDescriptionNull(),
                        "Description should not be null"
                    });
                    break;
                
                case 3:
                    // descrição ser maior que 10_000 caracteres
                    invalidInputsList.Add(new object[]
                    {
                        fixture.GetInvalidInputTooLongDescription(),
                        "Description should be less or equal 10000 characters long"
                    });
                    break;
                
                default:
                    break;

            }
        }

        return invalidInputsList;
    } 
}