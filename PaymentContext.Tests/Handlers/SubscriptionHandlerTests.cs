using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Handlers;
using PaymentContext.Tests.Mocks;

namespace PaymentContext.Tests.Handlers
{
    [TestClass]
    public class SubscriptionHandlerTests
    {
        [TestMethod]
        public void DocumentExists()
        {
            var handler = new SubscriptionHandler(new FakeStudentRepository(), new FakeEmailService());
            var command = new CreateBoletoSubscriptionCommand();

            command.FirstName = "Jao";
            command.LastName = "claudio";
            command.Document = "12345678901";
            command.Email = "teste@teste.com";
            command.BarCode = "12312312";
            command.BoletoNumber = "12123123";
            command.PaymentNumber = "123123123123";
            command.PaidDate = DateTime.Now;
            command.ExpireDate = DateTime.Now.AddMonths(1);
            command.Total = 60;
            command.TotalPaid = 60;
            command.Payer = "Test Company";
            command.PayerDocument = "12312312312312";
            command.PayerDocumentType = EDocumentType.CPF;
            command.PayerEmail = "test@test.com";
            command.Street = "a";
            command.Number = "1";
            command.Neighborhood = "bh";
            command.State = "mg";
            command.Country = "br";
            command.ZipCode = "31456789";

            handler.Handle(command);
            Assert.AreEqual(false, handler.Valid);

        }
    }
}