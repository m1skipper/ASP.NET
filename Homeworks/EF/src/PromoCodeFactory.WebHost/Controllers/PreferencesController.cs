using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Роли сотрудников
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController
                : ControllerBase
    {
        private readonly IRepository<Preference> _preferences;
        private readonly IMapper _mapper;

        public PreferencesController(IRepository<Preference> preferences, IMapper mapper)
        {
            _preferences = preferences;
            _mapper = mapper;
        }

        /// <summary>
        /// Получить все возможные предпочтения
        /// </summary>
        /// <returns>Класс предпочтения</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PreferenceResponse>>> GetPreferencesAsync()
        {
            var preferences = await _preferences.GetAllAsync();
            var preferencesResponse = _mapper.Map<IEnumerable<Preference>, List<PreferenceResponse>>(preferences);
            return Ok(preferencesResponse);
        }

        /// <summary>
        /// Получить полную информацию о предпочтении по id
        /// </summary>
        /// <param name="id">Идентификатор предпочтения</param>
        /// <returns>Класс предпочтения</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PreferenceResponse>> GetPreferenceAsync(Guid id)
        {
            var preference = await _preferences.GetByIdAsync(id);
            if (preference == null)
                return NotFound();
            var preferenceResponse = _mapper.Map<Preference, PreferenceResponse>(preference);
            return Ok(preferenceResponse);
        }

        /// <summary>
        /// Создать предпочтение
        /// </summary>
        /// <param name="request">Информация о предпочтении</param>
        /// <returns>Нет возвращаемого значения</returns>
        [HttpPost]
        public async Task<IActionResult> CreatePreferenceAsync(CreateOrEditPreferenceRequest request)
        {
            Preference c = _mapper.Map<CreateOrEditPreferenceRequest, Preference>(request);
            _preferences.Add(c);
            await _preferences.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Редактировать пердпочтение
        /// </summary>
        /// <param name="id">Идентификатор предпочтения</param>
        /// <param name="request">Отредактированные параметры</param>
        /// <returns>Нет возвращаемого значения</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditPreferencesAsync(Guid id, CreateOrEditPreferenceRequest request)
        {
            Preference c = await _preferences.GetByIdAsync(id);
            _mapper.Map(request, c);

            await _preferences.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Удалить предпочтение по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор предпочтения</param>
        /// <returns>Нет возвращаемого значения</returns>
        [HttpDelete]
        public async Task<IActionResult> DeletePreference(Guid id)
        {
            Preference c = await _preferences.GetByIdAsync(id);
            _preferences.Delete(c);
            await _preferences.SaveChangesAsync();
            return Ok();
        }
    }
}