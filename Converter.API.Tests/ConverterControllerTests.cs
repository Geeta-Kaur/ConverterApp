using Xunit;
using AutoFixture;
using FakeItEasy;
using ConverterAPI.Controllers;
using ConverterAPI.Domain.Read;
using Microsoft.Extensions.Logging;
using ConverterAPI.Domain;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using ConverterAPI.Enums;
using ConverterAPI.Models;
using System.Net;

namespace Converter.API.Tests{

    public class ConverterControllerTests{

        //Fakes
        private readonly ICurrencyReader currencyService;
        private readonly ICountryReader countryService;
        private readonly ILogger<ConverterController> logger;
        //Dummy Data Generator
        private readonly Fixture fixture;
        
        //System under test
        private readonly ConverterController sut;
        public ConverterControllerTests()
        {
           logger = A.Fake<ILogger<ConverterController>>();
           currencyService = A.Fake<ICurrencyReader>();
           countryService = A.Fake<ICountryReader>();
          
           fixture = new Fixture();
        }             

        [Fact]
        public void When_GetCurrencies_then_Returns_CurrencyList()
        {
            //Arrange
            var currencies = new List<Currency> {new Currency("GBP","Pounds")};              
            var sut = new ConverterController(logger, currencyService,countryService);            
            
            A.CallTo(() => currencyService.GetCurrenciesAsync()).Returns(currencies);

            //Act
            var result = sut.GetCurrencies().GetAwaiter().GetResult();

            //Assert
            A.CallTo(() => currencyService.GetCurrenciesAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<ActionResult<List<Currency>>>(result);
            Assert.NotNull(result);           

        } 
       
        [Fact]
        public void When_GetCountries_Then_Returns_CountryList()
        {
            //Arrange            
            var countries = new List<Country> {new Country{ Id = 1, Name= "UK", CurrencyName = CurrencyType.GBP}};  

            var sut = new ConverterController(logger, currencyService, countryService);            
            
            A.CallTo(() => countryService.GetCountriesAsync()).Returns(countries);

            //Act
            var result = sut.GetCountries().GetAwaiter().GetResult();

            //Assert
            A.CallTo(() => countryService.GetCountriesAsync()).MustHaveHappenedOnceExactly();
            Assert.IsType<ActionResult<List<Country>>>(result);                 
            
        }

        [Fact]
        public void When_Convert_InValid_Params_Then_ThrowsNullException()
        {
            //Arrange
            ConvertInputModel input = new ConvertInputModel();
            input.ConvertFrom= CurrencyType.GBP;
            input.ConvertTo = CurrencyType.USD;
            
            var sut = new ConverterController(logger, currencyService, countryService);
           
            //Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Convert(input));            
        }

        [Theory]       
        [InlineData(-2)]
        [InlineData(0)]
        public void When_Convert_InValid_valueToConvert_Then_Response400BadRequestWithMessage(decimal testValue)
        {
            //Arrange
            ConvertInputModel input = new ConvertInputModel();
            input.ConvertFrom= CurrencyType.GBP;
            input.ConvertTo = CurrencyType.USD;
            input.ValueToConvert = testValue;
            var sut = new ConverterController(logger, currencyService, countryService);
            //Act
            var actualResult = sut.Convert(input).GetAwaiter().GetResult() as BadRequestObjectResult;
            //Assert
            Assert.Equal("Invalid data", actualResult.Value);
            
        }
        [Fact]
        public void When_Convert_Valid_Params_Then_Response200OK()
        {
            //Arrange
            ConvertInputModel input = new ConvertInputModel();
            input.ConvertFrom= CurrencyType.GBP;
            input.ConvertTo = CurrencyType.USD;
            input.ValueToConvert = 2;
            decimal testconvertRate = 1.60m * (input.ValueToConvert);            
            var sut = new ConverterController(logger, currencyService, countryService);
            //Act
            var testResult = sut.Convert(input).GetAwaiter().GetResult() as OkObjectResult;
            //Assert
            Assert.NotNull(testResult);
            Assert.Equal("200",testResult.StatusCode.ToString());
            Assert.Equal(testconvertRate, testResult.Value);
            
        }
        

    }
}
