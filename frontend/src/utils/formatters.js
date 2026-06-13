/**
 * Утилиты форматирования для отображения числовых значений.
 */

/**
 * Форматирование числа с разделителями тысяч и фиксированной точностью.
 * @param {number} num - число для форматирования
 * @param {number} [decimals=2] - количество знаков после запятой
 * @returns {string} отформатированная строка
 */
export function formatNumber(num, decimals = 2) {
  if (num == null || isNaN(num)) return '—';
  return num.toLocaleString('ru-RU', {
    minimumFractionDigits: decimals,
    maximumFractionDigits: decimals,
  });
}

/**
 * Форматирование значения с единицей измерения.
 * @param {number} num - число
 * @param {string} unit - единица измерения
 * @param {number} [decimals=2] - точность
 * @returns {string} например: "10 000,00 кДж"
 */
export function formatWithUnit(num, unit, decimals = 2) {
  const formatted = formatNumber(num, decimals);
  return unit ? `${formatted} ${unit}` : formatted;
}

/**
 * Форматирование процентного значения.
 * @param {number} num - число (уже в процентах)
 * @param {number} [decimals=2] - точность
 * @returns {string} например: "13,78%"
 */
export function formatPercent(num, decimals = 2) {
  if (num == null || isNaN(num)) return '—';
  return `${num.toFixed(decimals).replace('.', ',')}%`;
}

/**
 * Форматирование времени из секунд в часы:минуты.
 * @param {number} seconds - время в секундах
 * @returns {string} например: "8 ч 00 мин"
 */
export function formatTime(seconds) {
  if (seconds == null || isNaN(seconds)) return '—';
  const hours = Math.floor(seconds / 3600);
  const minutes = Math.floor((seconds % 3600) / 60);
  if (hours > 0) {
    return `${hours} ч ${minutes.toString().padStart(2, '0')} мин`;
  }
  return `${minutes} мин`;
}
