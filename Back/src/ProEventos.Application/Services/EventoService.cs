using ProEventos.Application.ViewModels;
using ProEventos.Infra.Data;
using ProEventos.Domain.Entities;
using AutoMapper;
using ProEventos.Infra.Data.Models;

namespace ProEventos.Application.Services
{
    public class EventoService : IEventoService
    {
        private readonly IEventoRepository eventoRepository;
        private readonly IMapper mapper;

        public EventoService(IEventoRepository eventoRepository, IMapper mapper)
        {
            this.eventoRepository = eventoRepository;
            this.mapper = mapper;
        }
        public async Task<EventoViewModel> AddEvento(int userId, EventoViewModel model)
        {
            try
            {
                var evento = mapper.Map<Evento>(model);
                evento.UserId = userId;
                eventoRepository.Add<Evento>(evento);
                if (await eventoRepository.SaveChangeAsync())
                {
                    var evento_ins = await eventoRepository.GetEventoByIdAsync(userId, evento.Id, false);
                    return mapper.Map<EventoViewModel>(evento_ins);
                }
                return null;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<EventoViewModel> UpdateEvento(int userId, int eventoId, EventoViewModel model)
        {
            try
            {
                var evento = await eventoRepository.GetEventoByIdAsync(userId, eventoId, false);
                if (evento == null)
                    return null;

                model.Id = evento.Id;
                model.UserId = userId;

                var evento_upd = mapper.Map<Evento>(model);
                eventoRepository.Update<Evento>(evento_upd);

                if (await eventoRepository.SaveChangeAsync())
                {
                    var evento_atz = await eventoRepository.GetEventoByIdAsync(userId, evento_upd.Id, false);
                    return mapper.Map<EventoViewModel>(evento_atz);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(message: ex.InnerException?.Message);
            }
        }
        public async Task<bool> DeleteEvento(int userId, int eventoId)
        {
            try
            {
                var evento = await eventoRepository.GetEventoByIdAsync(userId, eventoId, false);
                if (evento == null)
                    throw new Exception("Evento para delete não encontrado.");

                eventoRepository.Delete(evento);
                return await eventoRepository.SaveChangeAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<PageList<EventoViewModel>> GetAllEventosAsync(int userId, PageParms pageParms, bool includePalestrantes = false)
        {
            try
            {
                var eventos = await eventoRepository.GetAllEventosAsync(userId, pageParms, includePalestrantes);
                var result = mapper.Map<PageList<EventoViewModel>>(eventos);
                result.CurrentPage = eventos.CurrentPage;
                result.TotalPages = eventos.TotalPages;
                result.PageSize = eventos.PageSize;
                result.TotalCount = eventos.TotalCount;                
                return result;
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<EventoViewModel> GetEventoByIdAsync(int userId, int eventoId, bool includePalestrantes = false)
        {
            try
            {
                var evento = await eventoRepository.GetEventoByIdAsync(userId, eventoId, includePalestrantes);
                return mapper.Map<EventoViewModel>(evento);
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}