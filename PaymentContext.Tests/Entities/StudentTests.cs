using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.ValueObjects;

namespace PaymentContext.Tests.Entities
{
    [TestClass]
    public class StudentTests
    { 
        private readonly Name _name;
        private readonly Document _document;
        private readonly Address _address;
        private readonly Email _email;
        private readonly Student _student;
        private readonly Subscription _subscription;

        public StudentTests()
        {
            _name = new Name("Joao0", "Martins");
            _document = new Document("08741452002", EDocumentType.CPF);
            _email = new Email("joao@email.com");
            _address = new Address("Rua A", "Minas", "BH", "MG", "BR", "31615000");
            _student = new Student(_name, _document, _email);
            _subscription = new Subscription(null);
        }

        [TestMethod]
        public void ActivedSubscription()
        {
            var payment = new PayPalPayment("1231asdasAS", DateTime.Now, DateTime.Now.AddDays(5), 10, 10, "Test Payment", _document, _address, _email);

            _subscription.AddPayment(payment);

            _student.AddSubscription(_subscription);
            _student.AddSubscription(_subscription);

            Assert.IsTrue(_student.Invalid);
        }

        [TestMethod]
        public void SubscriptionHasNoPayment()
        {
            _student.AddSubscription(_subscription);
            Assert.IsTrue(_student.Invalid);
        }

        

        [TestMethod]
        public void AddSubscription()
        {
            var payment = new PayPalPayment("1231asdasAS", DateTime.Now, DateTime.Now.AddDays(5), 10, 10, "Test Payment", _document, _address, _email);

            _subscription.AddPayment(payment);

            _student.AddSubscription(_subscription);

            Assert.IsTrue(_student.Valid);
        }
    }
}