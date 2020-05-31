using Flunt.Notifications;
using PaymentContext.Shared.Commands;
using PaymentContext.Domain.Commands;
using PaymentContext.Shared.Handlers;
using PaymentContext.Domain.Repositories;
using Flunt.Validations;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Entities;
using System;
using PaymentContext.Domain.Services;

namespace PaymentContext.Domain.Handlers
{
    public class SubscriptionHandler : 
        Notifiable,
        IHandler<CreateBoletoSubscriptionCommand>,
        IHandler<CreatePaypalSubscriptionCommand>
    {

        private readonly IStudentRepository _repository;
        private readonly IEmailService _emailService;

        public SubscriptionHandler(IStudentRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        public ICommandResult Handle(CreateBoletoSubscriptionCommand command)
        {   
            command.Validate();
            if(command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(false, "Não foi possível realizar sua assinatura");
            }
            // Verificar se documento já estpa cadastrado
            if(_repository.DocumentExists(command.Document))
                AddNotification("Document", "CPF já utilizado");

            // Verificar se E-mail já está cadastrado
            if (_repository.EmailExists(command.Email))
                AddNotification("Email", "CPF já utilizado");

            // GErar VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.State, command.Country, command.ZipCode);

            // Gerar Entidades
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new BoletoPayment(
                command.BarCode, command.BoletoNumber, command.PaidDate, command.ExpireDate,
                command.Total, command.TotalPaid, command.Payer, 
                new Document(command.PayerDocument, command.PayerDocumentType),
                address, email
            );

            //Relacionamentos
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            // Agrupar Validações
            AddNotifications(name, document, email, address, student, subscription, payment);

            // Salvar informações
            _repository.CreateSubscription(student);

            // Enviar e-mail de boas vindas
            _emailService.Send(student.Name.ToString(), student.Email.Address, "welcome", "ass ok");
            //Retornar informacoes

            return new CommandResult(true, "Assinatura realizada com sucesso");

        }

        public ICommandResult Handle(CreatePaypalSubscriptionCommand command)
        {
            // command.Validate();
            // if (command.Invalid)
            // {
            //     AddNotifications(command);
            //     return new CommandResult(false, "Não foi possível realizar sua assinatura");
            // }

            // Verificar se documento já estpa cadastrado
            if (_repository.DocumentExists(command.Document))
                AddNotification("Document", "CPF já utilizado");

            // Verificar se E-mail já está cadastrado
            if (_repository.EmailExists(command.Email))
                AddNotification("Email", "CPF já utilizado");

            // GErar VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.State, command.Country, command.ZipCode);

            // Gerar Entidades
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new PayPalPayment(
                command.TransactionCode, command.PaidDate, command.ExpireDate,
                command.Total, command.TotalPaid, command.Payer,
                new Document(command.PayerDocument, command.PayerDocumentType),
                address, email
            );

            //Relacionamentos
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            // Agrupar Validações
            AddNotifications(name, document, email, address, student, subscription, payment);

            if (Invalid)
                return new CommandResult(false, "ass false");

            // Salvar informações
            _repository.CreateSubscription(student);

            // Enviar e-mail de boas vindas
            _emailService.Send(student.Name.ToString(), student.Email.Address, "welcome", "ass ok");
            //Retornar informacoes

            return new CommandResult(true, "Assinatura realizada com sucesso");
        }
    }
}