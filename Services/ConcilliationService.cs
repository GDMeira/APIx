using System.Net;
using System.Text.RegularExpressions;
using APIx.RequestDTOs;
using APIx.Exceptions;
using APIx.Models;
using APIx.Repositories;
using APIx.ResponseDTOs;
using APIx.Helpers.RabbitMQ;

namespace APIx.Services;

public partial class ConcilliationService(ConcilliationRepository concilliationRepository,
    IRabbitManager messagePublisher)
{
    private readonly ConcilliationRepository _concilliationRepository = concilliationRepository;
    private readonly IRabbitManager _messagePublisher = messagePublisher;
    public async Task<ResPostConcilliationDTO> PostConcilliation(ReqPostConcilliationDTO req, int paymentProviderId)
    {
        Concilliation newConcilliation = new Concilliation(req.GetFile(), req.GetPostback(), req.GetDate(), paymentProviderId);
        Concilliation? concilliationDB = await _concilliationRepository
            .RetrieveConcilliationByDateAndProvider(newConcilliation);

        if (concilliationDB != null) throw new ConflictOnCreationException("Concilliation already exists.");

        Console.WriteLine("Concilliation date: " + newConcilliation.Date.ToString());
        await _concilliationRepository.CreateConcilliation(newConcilliation);
        string queueName = "concilliations";
        _messagePublisher.Publish(newConcilliation.Id, queueName);
        ResPostConcilliationDTO response = new ResPostConcilliationDTO(newConcilliation);
        
        return response;
    }
}