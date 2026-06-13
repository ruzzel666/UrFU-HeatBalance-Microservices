/**
 * Мок-данные ответа от Java-микросервиса сушильного барабана.
 * Структура адаптирована под типичный расчёт теплового баланса барабанной сушилки.
 */

export const drumDryerMockResponse = {
  /* ===== Расчёт горения топлива ===== */
  fuelCombustion: {
    aerodynamics: {
      relativeConsumption: { value: 4.215, unit: '', symbol: 'Z', label: 'Относительный расход' },
      exitEnthalpy: { value: 685.32, unit: 'кДж/м³', symbol: 'i₀', label: 'Энтальпия газов на выходе из топки' },
      exitTemperature: { value: 475.60, unit: '°С', symbol: 't₀', label: 'Температура газов на выходе из топки' },
      flueGasEnthalpy: { value: 2885.10, unit: 'кДж/м³', symbol: 'Iф', label: 'Теплосодержание топочных газов' },
      airAmountToReduce: { value: 3.54, unit: 'м³/м³', symbol: 'Хв', label: 'Кол-во воздуха для снижения энтальпии' },
    },

    oxygenAndAir: {
      oxygenVolume: { value: 2.074, unit: 'м³/м³', symbol: 'Vo₂', label: 'Объём кислорода' },
      airVolumeTheoretical: { value: 9.872, unit: 'м³/м³', symbol: 'Lo', label: 'Теоретический объём воздуха' },
      airVolumeActual: { value: 11.352, unit: 'м³/м³', symbol: 'Lα', label: 'Действительный объём воздуха' },
    },

    combustionProductsAlpha1: {
      co2: { value: 1.067, unit: 'м³/м³', symbol: 'V°CO₂', label: 'CO₂' },
      h2o: { value: 2.034, unit: 'м³/м³', symbol: 'V°H₂O', label: 'H₂O' },
      n2: { value: 7.827, unit: 'м³/м³', symbol: 'V°N₂', label: 'N₂' },
      total: { value: 10.929, unit: 'м³/м³', symbol: 'V°', label: 'Общий объём' },
    },

    combustionProductsAlpha115: {
      co2: { value: 1.067, unit: 'м³/м³', symbol: 'VαCO₂', label: 'CO₂' },
      h2o: { value: 2.060, unit: 'м³/м³', symbol: 'VαH₂O', label: 'H₂O' },
      n2: { value: 8.997, unit: 'м³/м³', symbol: 'VαN₂', label: 'N₂' },
      o2excess: { value: 0.311, unit: 'м³/м³', symbol: 'VО₂изб', label: 'O₂ избыточный' },
      total: { value: 12.435, unit: 'м³/м³', symbol: 'Vα', label: 'Общий объём' },
    },

    compositionAlpha1: {
      co2: { value: 9.77, unit: '%', symbol: 'V°CO₂', label: 'CO₂' },
      h2o: { value: 18.61, unit: '%', symbol: 'V°H₂O', label: 'H₂O' },
      n2: { value: 71.62, unit: '%', symbol: 'V°N₂', label: 'N₂' },
    },

    compositionAlpha115: {
      co2: { value: 8.58, unit: '%', symbol: 'VαCO₂', label: 'CO₂' },
      h2o: { value: 16.56, unit: '%', symbol: 'VαH₂O', label: 'H₂O' },
      n2: { value: 72.35, unit: '%', symbol: 'VαN₂', label: 'N₂' },
      o2excess: { value: 2.50, unit: '%', symbol: 'VО₂изб', label: 'O₂ избыточный' },
    },

    combustionHeat: {
      lowerHeatValue: { value: 37222.40, unit: 'кДж/м³', symbol: 'Qₙ', label: 'Низшая теплота сгорания' },
      theoreticalCombustionTemp: { value: 1848.34, unit: '°С', symbol: 'tтα', label: 'Теоретическая температура горения' },
      balanceCombustionTemp: { value: 1825.53, unit: '°С', symbol: 'tбα', label: 'Балансовая температура горения' },
    },
  },

  /* ===== Тепловой баланс (без рециркуляции) ===== */
  heatBalanceNoRecirculation: {
    items: [
      { key: 'Q1', label: 'Расход теплоты на испарение влаги (Q₁)', value: 5280000.00, percent: 18.42, unit: 'кДж' },
      { key: 'Q2', label: 'Потери теплоты с отходящими газами (Q₂)', value: 7150320.50, percent: 24.94, unit: 'кДж' },
      { key: 'Q3', label: 'Потери вследствие хим. недожога (Q₃)', value: 573268.14, percent: 2.00, unit: 'кДж' },
      { key: 'Q5_furnace', label: 'Потери теплоты топкой (Q₅ топ)', value: 5732681.40, percent: 20.00, unit: 'кДж' },
      { key: 'Q5_walls', label: 'Потери через стенки барабана (Q₅ ст)', value: 7465780.00, percent: 26.04, unit: 'кДж' },
      { key: 'Q6', label: 'Аккумулированное тепло корпусом (Q₆)', value: 2461356.96, percent: 8.59, unit: 'кДж' },
    ],
    totalHeat: { value: 28663407.00, unit: 'кДж', label: 'Сумма Q' },
  },

  /* ===== Тепловой баланс (с рециркуляцией) ===== */
  heatBalanceWithRecirculation: {
    items: [
      { key: 'Q1p', label: 'Расход теплоты на испарение влаги (Q₁\')', value: 5280000.00, percent: 19.85, unit: 'кДж' },
      { key: 'Q2p', label: 'Потери теплоты с отходящими газами (Q₂\')', value: 6230150.30, percent: 23.42, unit: 'кДж' },
      { key: 'Q3p', label: 'Потери вследствие хим. недожога (Q₃\')', value: 531906.80, percent: 2.00, unit: 'кДж' },
      { key: 'Q5_furnace_p', label: 'Потери теплоты топкой (Q₅\' топ)', value: 5319068.00, percent: 20.00, unit: 'кДж' },
      { key: 'Q5_walls_p', label: 'Потери через стенки барабана (Q₅\' ст)', value: 6830420.00, percent: 25.67, unit: 'кДж' },
      { key: 'Q6p', label: 'Аккумулированное тепло корпусом (Q₆\')', value: 2411994.90, percent: 9.06, unit: 'кДж' },
    ],
    totalHeat: { value: 26603540.00, unit: 'кДж', label: 'Сумма Q\'' },
  },

  /* ===== Ключевые показатели ===== */
  summary: {
    efficiency: { value: 18.42, unit: '%', symbol: 'η', label: 'КПД барабана' },
    fuelConsumption: { value: 0.0268, unit: 'м³/с', symbol: 'В', label: 'Расход топлива' },
    totalFuelHeat: { value: 28663407.00, unit: 'кДж', symbol: 'Qxh', label: 'Теплота топлива' },
    heatPerKgMoisture: { value: 38217.88, unit: 'кДж/кг', symbol: 'qисп', label: 'Расход теплоты на 1 кг влаги' },
    removedMoisture: { value: 750.00, unit: 'кг', symbol: 'Gвод', label: 'Количество удалённой влаги' },
    drumVolume: { value: 28.27, unit: 'м³', symbol: 'Vб', label: 'Объём барабана' },
    productivity: { value: 0.347, unit: 'кг/с', symbol: 'П', label: 'Производительность по сухому материалу' },
  },
};

/**
 * Имитация запроса к API с задержкой.
 * @param {object} _inputData - данные формы (не используются в моке)
 * @returns {Promise<object>} мок-ответ
 */
export function fetchDrumDryerResult(_inputData) {
  return new Promise((resolve) => {
    setTimeout(() => {
      resolve(drumDryerMockResponse);
    }, 1200 + Math.random() * 800);
  });
}
