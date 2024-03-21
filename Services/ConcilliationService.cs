using System.Net;
using System.Text.RegularExpressions;
using APIx.RequestDTOs;
using APIx.Exceptions;
using APIx.Models;
using APIx.Repositories;
using APIx.ResponseDTOs;
using APIx.Helpers.RabbitMQ;

namespace APIx.Services;

public partial class ConcilliationService(UsersRepository usersRepository,
    IRabbitManager messagePublisher, AccountsRepository accountsRepository,
    KeysRepository keysRepository, PaymentsRepository paymentsRepository)
{
    private readonly UsersRepository _usersRepository = usersRepository;
    private readonly IRabbitManager _messagePublisher = messagePublisher;
    private readonly AccountsRepository _accountsRepository = accountsRepository;
    private readonly KeysRepository _keysRepository = keysRepository;
    private readonly PaymentsRepository _paymentsRepository = paymentsRepository;
    public async Task<string> PostConcilliation(string fileUrl, int paymentProviderId)
    {
        //criar concilliation no DB
        //mandar concilliation para o rabbit
        //retornar concilliation
        
        await Task.Delay(100);
        return "aoba";
    }
}