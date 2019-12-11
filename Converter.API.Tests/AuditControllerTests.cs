using System;
using System.Collections.Generic;
using ConverterAPI.Controllers;
using ConverterAPI.Domain;
using ConverterAPI.Domain.Read;
using ConverterAPI.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Converter.API.Tests{
    public class AuditControllerTests
    {
        [Fact]
        public void When_Convert_InValid_Params_Then_ReturnsNullException()
        {
            //Arrange
                    
            var auditReader = A.Fake<IAuditReader>();
            
            var sut = new AuditController(auditReader);           
            //Assert
            var result = sut.GetAuditsByDate(null) as BadRequestObjectResult;
            Assert.NotNull(result); 
            Assert.Equal("400", result.StatusCode.ToString());           
        }

        [Fact]
        public void When_Convert_Valid_Params_Then_Response200OK()
        {
            //Arrange
            SearchInputModel input = new SearchInputModel{DateFrom = DateTime.Now , DateTo = DateTime.Now.AddMinutes(60)};
            var auditReader = A.Fake<IAuditReader>();
            List<Audit> audits = new List<Audit>{new Audit("Rates checked from GBP to USD", DateTime.Parse("2019-12-10T21:52:58.2858813+00:00")),
                                                new Audit( "Rates checked from GBP to USD", DateTime.Parse("2019-12-10T21:58:08.7340439+00:00"))};
            
            A.CallTo(() => auditReader.GetAuditsAsync(input.DateFrom,input.DateTo)).Returns(audits); 
            var sut = new AuditController(auditReader);
            //Act
            var testResult = sut.GetAuditsByDate(input) as OkObjectResult;;
            //Assert
            Assert.NotNull(testResult);
            Assert.Equal("200",testResult.StatusCode.ToString());            
            
        }
        


    }

}