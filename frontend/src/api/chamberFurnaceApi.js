import axiosInstance from './axiosInstance';
import { fetchChamberFurnaceResult as fetchMock } from '../mocks/chamberFurnaceMock';

/**
 * Переключатель мок/реальный API.
 * true = используются мок-данные (backend ещё не готов).
 * false = реальные запросы к Java-микросервису.
 */
const USE_MOCKS = import.meta.env.VITE_USE_MOCKS !== 'false';

/**
 * Расчёт теплового баланса камерной сушильной печи.
 *
 * @param {object} inputData - данные формы
 * @param {object} [options] - доп. опции
 * @param {string} [options.accept='application/json'] - формат ответа (json/xml)
 * @returns {Promise<object>} результаты расчёта
 */
export async function calculateChamberFurnace(inputData, options = {}) {
  if (USE_MOCKS) {
    console.log('[API Mock] calculateChamberFurnace', inputData);
    return fetchMock(inputData);
  }

  const accept = options.accept || 'application/json';

  const response = await axiosInstance.post('/chamber-furnace/calculate', inputData, {
    headers: { Accept: accept },
  });

  return response.data;
}

/**
 * Получение значений по умолчанию для формы (если backend их предоставляет).
 *
 * @returns {Promise<object>} значения по умолчанию
 */
export async function getChamberFurnaceDefaults() {
  if (USE_MOCKS) {
    return null; // используем локальные defaults
  }

  const response = await axiosInstance.get('/chamber-furnace/defaults');
  return response.data;
}
