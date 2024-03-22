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
    public async Task<ResPostConcilliationDTO> PostConcilliation(string fileUrl, int paymentProviderId)
    {
        // Concilliation? concilliation = await _concilliationRepository
        //     .RetrieveConcilliationByFileUrl(fileUrl);

        // if (concilliation != null) throw new ConflictOnCreation("Concilliation already exists.");

        Concilliation newConcilliation = new Concilliation(fileUrl, paymentProviderId);
        await _concilliationRepository.CreateConcilliation(newConcilliation);
        string queueName = "concilliations";
        _messagePublisher.Publish(newConcilliation.Id, queueName);
        ResPostConcilliationDTO response = new ResPostConcilliationDTO(newConcilliation);
        
        return response;
    }
}