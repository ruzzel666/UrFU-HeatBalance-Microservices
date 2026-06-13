import axiosInstance from './axiosInstance';
import { fetchDrumDryerResult as fetchMock } from '../mocks/drumDryerMock';

/**
 * Переключатель мок/реальный API.
 */
const USE_MOCKS = import.meta.env.VITE_USE_MOCKS !== 'false';

/**
 * Расчёт теплотехнических параметров сушильного барабана.
 *
 * @param {object} inputData - данные формы
 * @param {object} [options] - доп. опции
 * @param {string} [options.accept='application/json'] - формат ответа
 * @returns {Promise<object>} результаты расчёта
 */
export async function calculateDrumDryer(inputData, options = {}) {
  if (USE_MOCKS) {
    console.log('[API Mock] calculateDrumDryer', inputData);
    return fetchMock(inputData);
  }

  const accept = options.accept || 'application/json';

  const response = await axiosInstance.post('/drum-dryer/calculate', inputData, {
    headers: { Accept: accept },
  });

  return response.data;
}
